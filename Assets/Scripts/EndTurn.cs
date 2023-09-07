using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndTurn : MonoBehaviour
{
    public static EndTurn instance;

    public static bool playerTurn = false;
    public List<Controller> playerCharacters;

    private void Start()
    {
        instance = this;
        StartPlayerTurn();
    }

    public void ButtonPressed()
    {
        if (!EndTurn.playerTurn) return;

        InputManager.inputAvailable = false;

        Invoke("ResetInput", 0.5f);

        if (!playerTurn) return;
        playerTurn = false;

        StartEnemyTurns();
    }

    public void StartPlayerTurn()
    {
        foreach (var item in playerCharacters)
        {
            item.StartTurn();
        }

        playerTurn = true;
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
}
