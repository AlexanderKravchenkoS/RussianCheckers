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
                    if (mouseDownPosition.x < 0 || mouseDownPosition.x > 7 || mouseDownPosition.y < 0 || mouseDownPosition.y > 7 || originPosition == mouseDownPosition)
                    {
                        selectedChecker.transform.position = new Vector3(originPosition.x, 0, originPosition.y);
                        selectedChecker = null;
                        return;
                    }
                    selectedChecker.Data.position = mouseDownPosition;
                    selectedChecker.transform.position = new Vector3(mouseDownPosition.x, 0, mouseDownPosition.y);
                    selectedChecker = null;
                    isWhiteTurn = !isWhiteTurn;
                }
            }
        }
    }
    private void AddChecker(CheckerData data)
    {
        GameObject checkerPrefab = data.isWhite ? WhitePrefab : BlackPrefab;
        var figureGameObject = Instantiate(checkerPrefab, new Vector3(data.position.x, 0, data.position.y), checkerPrefab.transform.rotation);
        figureGameObject.GetComponent<Checker>().Data = data;
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