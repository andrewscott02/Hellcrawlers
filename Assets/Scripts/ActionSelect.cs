using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ActionSelect : MonoBehaviour
{
    CharacterSelect characterSelect;
    Action[] actions;
    public TextMeshProUGUI[] buttonTexts;

    private void Start()
    {
        characterSelect = GameObject.FindAnyObjectByType<CharacterSelect>();
    }

    public void ActionList()
    {
        actions = characterSelect.GetSelectedCharacter().actions;

        for (int i = 0; i < actions.Length; i++)
        {
            buttonTexts[i].text = actions[i].actionName;
        }
    }

    public void SelectAction(int index)
    {
        if (!EndTurn.playerTurn) return;

        InputManager.inputAvailable = false;

        characterSelect.GetSelectedCharacter().PrepareAction(actions[index]);

        Invoke("ResetInput", 0.5f);
    }

    public void ShowAction(int index)
    {
        ActionUI.instance.ShowActionInfo(actions[index]);
    }

    public void HideAction()
    {
        ActionUI.instance.HideActionInfo();
    }

    void ResetInput()
    {
        InputManager.inputAvailable = true;
    }
}