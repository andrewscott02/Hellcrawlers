using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

public class CharacterSelect : MonoBehaviour
{
    public static CharacterSelect instance;
    public CinemachineVirtualCamera vCam;
    ActionSelect actionSelect;
    public int selectedCharacter { get; private set; }
    public Controller[] availableCharacters;

    private void Start()
    {
        instance = this;
        actionSelect = GameObject.FindAnyObjectByType<ActionSelect>();
        Invoke("Setup", 0.05f);
    }

    void Setup()
    {
        foreach (var item in availableCharacters)
        {
            item.StartTurn();
        }

        SelectCharacter(0);
    }

    public void SelectCharacter(int index)
    {
        if (!EndTurn.playerTurn) return;

        InputManager.inputAvailable = false;
        Invoke("ResetInput", 0.5f);

        if (availableCharacters[index] == null) return;

        selectedCharacter = index;

        foreach (var item in availableCharacters)
        {
            if (item != null)
            {
                item.Unselect();
                item.StopMovement();
            }
        }

        availableCharacters[selectedCharacter].controlled = true;
        actionSelect.ActionList();
        ActionPointsUI.instance.DisplayAP(availableCharacters[selectedCharacter].actionsLeft);
        ActionPointsUI.instance.DisplayMovement(availableCharacters[selectedCharacter].movementLeft/ availableCharacters[selectedCharacter].maxMovement);

        vCam.Follow = availableCharacters[selectedCharacter].transform;

        
    }

    void ResetInput()
    {
        InputManager.inputAvailable = true;
    }

    public Controller GetSelectedCharacter()
    {
        return availableCharacters[selectedCharacter];
    }

    public void CharacterDied(Controller character)
    {
        if (character is AIController) return;

        int characterCount = 0;
        bool allDead = true;

        for (int i = 0; i < availableCharacters.Length; i++)
        {
            if (character == availableCharacters[i])
            {
                characterCount = i;
            }
            else if (availableCharacters[i] != null)
            {
                //Character is alive
                allDead = false;
            }
        }

        availableCharacters[characterCount] = null;

        if (allDead)
        {
            Debug.Log("All characters are dead");
            LoseGame();
        }
        else
        {
            foreach (var item in GameObject.FindObjectsOfType<AIController>())
            {
                item.AssessAction();
            }

            if (characterCount == selectedCharacter)
            {
                for (int i = 0; i < availableCharacters.Length; i++)
                {
                    if (character != null)
                    {
                        SelectCharacter(i);
                        break;
                    }
                }
            }
        }
    }

    public E_Scenes gameOverScene;

    void LoseGame()
    {
        //TODO: End game
        SceneManager.LoadScene(gameOverScene.ToString());
    }
}
