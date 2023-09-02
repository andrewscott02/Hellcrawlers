using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelect : MonoBehaviour
{
    int selectedCharacter;
    public Controller[] availableCharacters;

    private void Start()
    {
        Invoke("Setup", 0.5f);
    }

    void Setup()
    {
        SelectCharacter(0);
    }

    public void SelectCharacter(int index)
    {
        foreach(var item in availableCharacters)
        {
            item.Unselect();
        }

        selectedCharacter = index;
        availableCharacters[selectedCharacter].controlled = true;
    }
}
