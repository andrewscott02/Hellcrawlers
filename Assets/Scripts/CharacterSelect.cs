using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelect : MonoBehaviour
{
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
        SelectCharacter(0);
    }

    public void SelectCharacter(int index)
    {
        InputManager.inputAvailable = false;

        selectedCharacter = index;

        foreach (var item in availableCharacters)
        {
            item.Unselect();
            item.StopMovement();
        }

        availableCharacters[selectedCharacter].controlled = true;
        actionSelect.ActionList();

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
