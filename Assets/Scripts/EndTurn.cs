using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndTurn : MonoBehaviour
{
    bool playerTurn = false;
    public List<Controller> playerCharacters;

    public void ButtonPressed()
    {
        if (!playerTurn) return;

        playerTurn = false;

        //TODO: Start enemy turn
        StartPlayerTurn(); //TODO: Remove, call elsewhere
    }

    public void StartPlayerTurn()
    {
        foreach (var item in playerCharacters)
        {
            item.StartTurn();
        }

        playerTurn = true;
    }
}
