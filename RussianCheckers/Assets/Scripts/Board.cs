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
    private Checker selectedChecker;
    private bool isWhiteTurn;
    private int whiteCheckers = 12;
    private int blackCheckers = 12;

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
                if (checker != null && checker.Data.isWhite == isWhiteTurn)
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
                    }if (originPosition == mouseDownPosition || !IsCorrectMove(mouseDownPosition))
                    {
                        selectedChecker.transform.position = new Vector3(originPosition.x, 0, originPosition.y);
                        selectedChecker = null;
                        return;
                    }
                    selectedChecker.Data.position = mouseDownPosition;
                    selectedChecker.transform.position = new Vector3(mouseDownPosition.x, 0, mouseDownPosition.y);
                    if ((selectedChecker.transform.position.z == 7 && selectedChecker.Data.isWhite) || (selectedChecker.transform.position.z == 0 && !selectedChecker.Data.isWhite))
                    {
                        selectedChecker.MakeKing();
                    }
                    WinCheck();
                    selectedChecker = null;
                    isWhiteTurn = !isWhiteTurn;
                }
            }
        }
    }
    private bool IsCorrectMove(Vector2Int mouseDownPosition)
    {
        Vector2Int vector2Int = (mouseDownPosition - selectedChecker.Data.position);
        if (Mathf.Abs(vector2Int.x) == Mathf.Abs(vector2Int.y))
        {
            Checker[] checkers = FindObjectsOfType<Checker>().ToArray();
            List<Checker> checkersOnLine = new List<Checker>();
            int deltaX = mouseDownPosition.x > selectedChecker.Data.position.x ? 1 : -1;
            int deltaZ = mouseDownPosition.y > selectedChecker.Data.position.y ? 1 : -1;
            int z = selectedChecker.Data.position.y + deltaZ;
            for (int x = selectedChecker.Data.position.x + deltaX; x != mouseDownPosition.x; x += deltaX)
            {
                if (checkers.Where(checker => checker.Data.position.x == x && checker.Data.position.y == z).FirstOrDefault() != null)
                {
                    checkersOnLine.Add(checkers.Where(checker => checker.Data.position.x == x && checker.Data.position.y == z).First());
                }
                z += deltaZ;
            }
            Checker checker = checkersOnLine
                        .Where(checker => checker.transform.position == new Vector3(mouseDownPosition.x, 0, mouseDownPosition.y))
                        .FirstOrDefault();
            if (checker != null)
            {
                return false;
            }
            if (!selectedChecker.Data.isKing)
            {
                int colourMult = selectedChecker.Data.isWhite ? 1 : -1;
                if (vector2Int.y == colourMult)
                {
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
                        {
                            selectedChecker.MakeKing();
                        }
                        if (enemy.Data.isWhite == true)
                        {
                            whiteCheckers--;
                        }
                        else
                        {
                            blackCheckers--;
                        }
                        Destroy(enemy.gameObject);
                        return true;
                    }
                }
            }
            else
            {
                if (checkersOnLine.Where(checker => checker.Data.isWhite == selectedChecker.Data.isWhite).ToArray().Length == 0)
                {
                    Checker[] enemies = checkersOnLine.Where(checker => checker.Data.isWhite != selectedChecker.Data.isWhite).ToArray();
                    if (enemies.Length == 0)
                    {
                        return true;
                    }
                    if (enemies.Length == 1)
                    {
                        if (enemies[0].Data.isWhite == true)
                        {
                            whiteCheckers--;
                        }
                        else
                        {
                            blackCheckers--;
                        }
                        Destroy(enemies[0].gameObject);
                        return true;
                    }
                }
            }
        }
        return false;
    }
    private void AddChecker(CheckerData data)
    {
        GameObject checkerPrefab = data.isWhite ? WhitePrefab : BlackPrefab;
        var figureGameObject = Instantiate(checkerPrefab, new Vector3(data.position.x, 0, data.position.y), checkerPrefab.transform.rotation);
        figureGameObject.GetComponent<Checker>().Data = data;
        if (data.isWhite)
        {
            whiteCheckers++;
        }
        else
        {
            blackCheckers++;
        }
    }
    public void SaveBoard()
    {
        BoardState boardState;
        boardState.checkersData = FindObjectsOfType<Checker>().Select(checker => checker.Data).ToArray();
        boardState.isWhiteTurn = isWhiteTurn;
        string path = Path.Combine(Application.dataPath, "previousGame.json");
        using (StreamWriter streamWriter = new StreamWriter(path))
        {
            string json = JsonUtility.ToJson(boardState);
            streamWriter.Write(json);
        }
    }
    public void GenerateBoard(bool isNewGame = false)
    {
        List<GameObject> checkers = FindObjectsOfType<Checker>().Select(x => x.transform.gameObject).ToList();
        foreach(GameObject checker in checkers)
        {
            Destroy(checker);
        }
        whiteCheckers = blackCheckers = 0;
        string path = isNewGame ? "newGame.json" : "previousGame.json";
        string fullPath = Path.Combine(Application.dataPath, path);
        using (StreamReader reader = new StreamReader(fullPath))
        {
            string json = reader.ReadToEnd();
            BoardState boardState = JsonUtility.FromJson<BoardState>(json);
            isWhiteTurn = boardState.isWhiteTurn;
            for (int i = 0; i < boardState.checkersData.Length; i++)
                AddChecker(boardState.checkersData[i]);
        }
    }
    private void WinCheck()
    {
        if (blackCheckers == 0)
        {
            Debug.Log("White WIN");
        }
        if (whiteCheckers == 0)
        {
            Debug.Log("Black WIN");
        }
    }
}

[Serializable]
public struct BoardState
{
    public CheckerData[] checkersData;
    public bool isWhiteTurn;
    public BoardState(CheckerData[] checkers, bool isWhiteTurn)
    {
        this.checkersData = checkers;
        this.isWhiteTurn = isWhiteTurn;
    }
}