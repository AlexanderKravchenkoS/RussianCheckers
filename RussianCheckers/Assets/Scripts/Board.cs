using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System;

public class Board : MonoBehaviour
{
    [SerializeField] GameObject WhitePrefab;
    [SerializeField] GameObject BlackPrefab;
    List<Checker> allCheckers = new List<Checker>();
    List<Checker> beatCheckers = new List<Checker>();
    private Checker selectedChecker;
    private bool isWhiteTurn;
    private bool wasBeat;
    private int onlyKingTurns;
    void Start()
    {
        transform.position = new Vector3(3.5f, -0.14f, 3.5f);
    }
    void Update()
    {
        Turn();
    }
    private void Turn()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, LayerMask.GetMask("Board")))
        {
            Vector2Int mouseDownPosition = new Vector2Int((int)(hit.point.x + 0.5), (int)(hit.point.z + 0.5));
            if (Input.GetMouseButtonDown(0))
            {
                var checker = hit.transform.gameObject.GetComponent<Checker>();
                if (checker != null && checker.Data.isWhite == isWhiteTurn && beatCheckers.Count == 0) 
                    selectedChecker = checker;
                else if (checker != null && checker.Data.isWhite == isWhiteTurn && beatCheckers.Contains(checker))
                    selectedChecker = checker;
            }
            if (selectedChecker != null)
            {
                Vector3 vector3 = new Vector3(hit.point.x, 0, hit.point.z);
                selectedChecker.transform.position = vector3 + Vector3.up;
                if (Input.GetMouseButtonUp(0))
                {
                    Vector2Int originPosition = selectedChecker.Data.position;
                    if (mouseDownPosition.x < 0 || mouseDownPosition.x > 7 || mouseDownPosition.y < 0 || mouseDownPosition.y > 7)
                    {
                        selectedChecker.transform.position = new Vector3(originPosition.x, 0, originPosition.y);
                        selectedChecker = null;
                        return;
                    }
                    if (originPosition == mouseDownPosition || !IsCorrectMove(mouseDownPosition))
                    {
                        selectedChecker.transform.position = new Vector3(originPosition.x, 0, originPosition.y);
                        selectedChecker = null;
                        return;
                    }
                    selectedChecker.Data.position = mouseDownPosition;
                    selectedChecker.transform.position = new Vector3(mouseDownPosition.x, 0, mouseDownPosition.y);
                    if ((selectedChecker.transform.position.z == 7 && selectedChecker.Data.isWhite) || (selectedChecker.transform.position.z == 0 && !selectedChecker.Data.isWhite))
                        selectedChecker.MakeKing();
                    if (wasBeat)
                    {
                        beatCheckers = new List<Checker>();
                        beatCheckers.AddRange(BeatMoves(selectedChecker));
                    }
                    if (beatCheckers.Count == 0)
                    {
                        selectedChecker = null;
                        isWhiteTurn = !isWhiteTurn;
                        beatCheckers = new List<Checker>();
                        beatCheckers.AddRange(BeatMoves());
                        WinAndDrawCheck();
                    }
                }
            }
        }
    }
    private bool IsCorrectMove(Vector2Int mouseDownPosition)
    {
        Vector2Int vector2Int = (mouseDownPosition - selectedChecker.Data.position);
        if (Mathf.Abs(vector2Int.x) == Mathf.Abs(vector2Int.y))
        {
            List<Checker> checkersOnLine = new List<Checker>();
            int deltaX = mouseDownPosition.x > selectedChecker.Data.position.x ? 1 : -1;
            int deltaZ = mouseDownPosition.y > selectedChecker.Data.position.y ? 1 : -1;
            int z = selectedChecker.Data.position.y + deltaZ;
            for (int x = selectedChecker.Data.position.x + deltaX; x != mouseDownPosition.x + deltaX; x += deltaX)
            {
                if (allCheckers.Where(checker => checker.Data.position.x == x && checker.Data.position.y == z).FirstOrDefault() != null)
                    checkersOnLine.Add(allCheckers.Where(checker => checker.Data.position.x == x && checker.Data.position.y == z).First());
                z += deltaZ;
            }
            Checker checker = checkersOnLine.Where(checker => checker.Data.position == new Vector2Int(mouseDownPosition.x, mouseDownPosition.y))
                                            .FirstOrDefault();
            if (checker != null)
                return false;
            if (!selectedChecker.Data.isKing)
            {
                int colourMult = selectedChecker.Data.isWhite ? 1 : -1;
                if (vector2Int.y == colourMult && beatCheckers.Count == 0)
                {
                    onlyKingTurns = 0;
                    wasBeat = false;
                    return true;
                }
                if (Mathf.Abs(vector2Int.x) == 2)
                {
                    Vector2Int enemyPos = (mouseDownPosition + selectedChecker.Data.position) / 2;
                    Checker enemy = checkersOnLine
                        .Where(checker => 
                            checker.transform.position == new Vector3(enemyPos.x, 0, enemyPos.y) && checker.Data.isWhite != selectedChecker.Data.isWhite)
                        .FirstOrDefault();
                    if (enemy != null)
                    {
                        if (enemy.Data.isKing)
                            selectedChecker.MakeKing();
                        allCheckers.Remove(enemy);
                        wasBeat = true;
                        Destroy(enemy.gameObject);
                        onlyKingTurns = 0;
                        return true;
                    }
                }
            }
            else
            {
                if (checkersOnLine.Where(checker => checker.Data.isWhite == selectedChecker.Data.isWhite).ToArray().Length == 0)
                {
                    Checker[] enemies = checkersOnLine.Where(checker => checker.Data.isWhite != selectedChecker.Data.isWhite).ToArray();
                    if (enemies.Length == 0 && beatCheckers.Count == 0)
                    {
                        onlyKingTurns++;
                        wasBeat = false;
                        return true;
                    }
                    if (enemies.Length == 1)
                    {
                        onlyKingTurns = 0;
                        wasBeat = true;
                        allCheckers.Remove(enemies[0]);
                        Destroy(enemies[0].gameObject);
                        return true;
                    }
                }
            }
        }
        return false;
    }
    private List<Checker> BeatMoves(Checker checkerForOneMoreTurn = null)
    {
        List<Checker> beatCheckers = new List<Checker>();
        List<Checker> checkers;
        Vector2Int[] steps = new Vector2Int[]{
            new Vector2Int(1,1),
            new Vector2Int(1,-1),
            new Vector2Int(-1,-1),
            new Vector2Int(-1,1)
        };
        Vector2 max = new Vector2(7, 7);
        if (checkerForOneMoreTurn == null)
            checkers = allCheckers;
        else
            checkers = new List<Checker>() { checkerForOneMoreTurn };
        foreach (Checker checker in allCheckers.Where(checker => checker.Data.isWhite == isWhiteTurn))
        {
            if (checker.Data.isKing)
            {
                bool isFound = false;
                foreach (Vector2Int step in steps)
                {
                    if (!isFound)
                    {
                        int counter = 1;
                        while (((checker.Data.position + step * (counter + 1)).x >= 0 && (checker.Data.position + step * (counter + 1)).x <= 7)
                            && ((checker.Data.position + step * (counter + 1)).y >= 0 && (checker.Data.position + step * (counter + 1)).y <= 7))
                        {
                            if (allCheckers.Where(check => check.Data.position == (checker.Data.position + step * counter) && check.Data.isWhite != checker.Data.isWhite).FirstOrDefault() != null)
                            {
                                if (allCheckers.Where(check => check.Data.position == (checker.Data.position + (step * (counter + 1)))).FirstOrDefault() == null)
                                {
                                    beatCheckers.Add(checker);
                                    isFound = true;
                                    break;
                                }
                                else
                                    break;
                            }
                            counter++;
                        }
                    }
                }
            }
            else
            {
                foreach (Vector2Int step in steps)
                {
                    if (((checker.Data.position + step * 2).x >= 0 && (checker.Data.position + step * 2).x <= 7)
                            && ((checker.Data.position + step * 2).y >= 0 && (checker.Data.position + step * 2).y <= 7))
                    {
                        if (allCheckers.Where(check => check.Data.position == (checker.Data.position + step) && check.Data.isWhite != checker.Data.isWhite).Count() != 0
                            && allCheckers.Where(check => check.Data.position == (checker.Data.position + step * 2)).Count() == 0)
                        {
                            beatCheckers.Add(checker);
                            break;
                        }
                    }
                }
            }
        }
        return beatCheckers;
    }
    public void SaveBoard()
    {
        BoardState boardState;
        boardState.checkersData = allCheckers.Select(checker => checker.Data).ToArray();
        boardState.isWhiteTurn = isWhiteTurn;
        boardState.onlyKingTurns = onlyKingTurns;
        string path = Path.Combine(Application.dataPath, "previousGame.json");
        using (StreamWriter streamWriter = new StreamWriter(path))
        {
            string json = JsonUtility.ToJson(boardState);
            streamWriter.Write(json);
        }
    }
    public void GenerateBoard(bool isNewGame = false)
    {
        foreach (var checker in allCheckers.Where(checker => checker != null))
            Destroy(checker.gameObject);
        allCheckers = new List<Checker>();
        string path = isNewGame ? "newGame.json" : "previousGame.json";
        string fullPath = Path.Combine(Application.dataPath, path);
        using (StreamReader reader = new StreamReader(fullPath))
        {
            string json = reader.ReadToEnd();
            BoardState boardState = JsonUtility.FromJson<BoardState>(json);
            isWhiteTurn = boardState.isWhiteTurn;
            onlyKingTurns = boardState.onlyKingTurns;
            for (int i = 0; i < boardState.checkersData.Length; i++)
                AddChecker(boardState.checkersData[i]);
        }
        beatCheckers = BeatMoves();
    }
    private void AddChecker(CheckerData data)
    {
        GameObject checkerPrefab = data.isWhite ? WhitePrefab : BlackPrefab;
        var figureGameObject = Instantiate(checkerPrefab, new Vector3(data.position.x, 0, data.position.y), checkerPrefab.transform.rotation);
        if (data.isKing)
            figureGameObject.GetComponent<Checker>().MakeKing();
        figureGameObject.GetComponent<Checker>().Data = data;
        allCheckers.Add(figureGameObject.GetComponent<Checker>());
    }
    private void WinAndDrawCheck()
    {
        if (allCheckers.Where(checker => checker.Data.isWhite).Count() == 0)
            Debug.Log("Black WINS!!!");
        if (allCheckers.Where(checker => !checker.Data.isWhite).Count() == 0)
            Debug.Log("White WINS!!!");
        if (onlyKingTurns == 15)
            Debug.Log("DRAW");
    }
}
[Serializable]
public struct BoardState
{
    public CheckerData[] checkersData;
    public bool isWhiteTurn;
    public int onlyKingTurns;
    public BoardState(CheckerData[] checkers, bool isWhiteTurn, int onlyKingTurns)
    {
        this.checkersData = checkers;
        this.isWhiteTurn = isWhiteTurn;
        this.onlyKingTurns = onlyKingTurns;
    }
}