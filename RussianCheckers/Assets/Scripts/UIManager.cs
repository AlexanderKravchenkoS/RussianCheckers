using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] Board Board;
    [SerializeField] Canvas mainMenu;
    [SerializeField] Canvas pauseMenu;
    [SerializeField] Canvas endGameMenu;
    [SerializeField] TextMeshProUGUI winText;
    private void Awake()
    {
        Board.isRunning = false;
        mainMenu.gameObject.SetActive(true);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            if (Board.isRunning == true)
            {
                Board.isRunning = false;
                pauseMenu.gameObject.SetActive(true);
            }
    }
    public void Resume()
    {
        pauseMenu.gameObject.SetActive(false);
        Board.isRunning = true;
    }
    public void NewGame()
    {
        mainMenu.gameObject.SetActive(false);
        pauseMenu.gameObject.SetActive(false);
        endGameMenu.gameObject.SetActive(false);
        Board.GenerateBoard(true);
        Board.isRunning = true;
    }
    public void LoadGame()
    {
        mainMenu.gameObject.SetActive(false);
        pauseMenu.gameObject.SetActive(false);
        endGameMenu.gameObject.SetActive(false);
        Board.GenerateBoard(false);
        Board.isRunning = true;
    }
    public void SaveGame()
    {
        pauseMenu.gameObject.SetActive(false);
        Board.SaveBoard();
        Board.isRunning = true;
    }
    public void Exit()
    {
        Application.Quit(0);
    }
    public void EndGame(string str)
    {
        Board.isRunning = false;
        endGameMenu.gameObject.SetActive(true);
        winText.text = str;
    }
}