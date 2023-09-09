using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EndTurn : MonoBehaviour
{
    public static EndTurn instance;

    public static bool playerTurn = false;

    bool[] finishedTurn;

    private void Start()
    {
        instance = this;
        finishedTurn = new bool[CharacterSelect.instance.availableCharacters.Length];
        for (int i = 0; i < finishedTurn.Length; i++)
        {
            finishedTurn[i] = false;
        }
        StartPlayerTurn();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ButtonPressed();
        }
    }

    public void ButtonPressed()
    {
        if (!EndTurn.playerTurn) return;

        InputManager.inputAvailable = false;

        finishedTurn[CharacterSelect.instance.selectedCharacter] = true;

        bool endAll = true;

        for(int i = 0; i < finishedTurn.Length; i++)
        {
            if (!finishedTurn[i] && CharacterSelect.instance.availableCharacters[i] != null)
            {
                endAll = false;
                CharacterSelect.instance.SelectCharacter(i);
            }
        }

        if (endAll)
            EndAllTurns();

        Invoke("ResetInput", 0.5f);
    }

    private void EndAllTurns()
    {
        if (!playerTurn) return;
        playerTurn = false;

        StartEnemyTurns();
    }

    public void StartPlayerTurn()
    {
        foreach (var item in CharacterSelect.instance.availableCharacters)
        {
            if (item != null)
                item.StartTurn();
        }

        for (int i = 0; i < finishedTurn.Length; i++)
        {
            finishedTurn[i] = false;
        }

        playerTurn = true;
        turnText.text = "Player Turn";
    }

    int enemyCount = 0;

    void StartEnemyTurns()
    {
        AIController[] enemies = GameObject.FindObjectsOfType<AIController>();

        enemyCount = enemies.Length;

        foreach(var item in enemies)
        {
            item.StartTurn();
        }

        turnText.text = "Enemy Turn";
    }

    public void EndEnmyTurn()
    {
        enemyCount--;

        if (enemyCount <= 0)
        {
            StartPlayerTurn();
        }
    }

    void ResetInput()
    {
        InputManager.inputAvailable = true;
    }

    public TextMeshProUGUI turnText;
}
