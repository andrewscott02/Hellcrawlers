using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CharacterSelect : MonoBehaviour
{
    public CinemachineVirtualCamera vCam;
    ActionSelect actionSelect;
    int selectedCharacter;
    public Controller[] availableCharacters;

    private void Start()
    {
        actionSelect = GameObject.FindAnyObjectByType<ActionSelect>();
        Invoke("Setup", 0.05f);
    }

    void Setup()
    {
        foreach (var item in availableCharacters)
            item.StartTurn();

        SelectCharacter(0);
    }

    public void SelectCharacter(int index)
    {
        if (!EndTurn.playerTurn) return;

        InputManager.inputAvailable = false;

        selectedCharacter = index;

        foreach (var item in availableCharacters)
        {
            item.Unselect();
            item.StopMovement();
        }

        availableCharacters[selectedCharacter].controlled = true;
        actionSelect.ActionList();
        ActionPointsUI.instance.DisplayAP(availableCharacters[selectedCharacter].actionsLeft);
        ActionPointsUI.instance.DisplayMovement(availableCharacters[selectedCharacter].movementLeft/ availableCharacters[selectedCharacter].maxMovement);

        vCam.Follow = availableCharacters[selectedCharacter].transform;

        Invoke("ResetInput", 0.5f);
    }

    void ResetInput()
    {
        InputManager.inputAvailable = true;
    }

    public Controller GetSelectedCharacter()
    {
        return availableCharacters[selectedCharacter];
    }
}
