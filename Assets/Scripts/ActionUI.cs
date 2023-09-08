using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ActionUI : MonoBehaviour
{
    #region Setup

    public static ActionUI instance;
    public GameObject image;
    public TextMeshProUGUI nameText, descText, costText;

    private void Start()
    {
        instance = this;
        bg = GetComponent<RectTransform>();
        HideActionInfo();
    }

    #endregion

    #region Action Info

    public void ShowActionInfo(Action action)
    {
        nameText.text = action.actionName;
        descText.text = action.description;
        costText.text = "AP: " + action.apCost + " Move: " + action.moveCost;

        image.SetActive(true);
    }

    public void HideActionInfo()
    {
        image.SetActive(false);
    }

    #endregion

    #region Follow Mouse

    public bool followMouse = false;
    RectTransform bg;
    public RectTransform canvas;
    public float xMultiplier = 1.87f;
    public float yMultiplier = 2.475f;

    private void Update()
    {
        if (followMouse)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            transform.position = mousePosition;

            Vector2 tooltipPos = transform.GetComponent<RectTransform>().anchoredPosition;
            float width = bg.rect.width * xMultiplier;
            float height = bg.rect.height * yMultiplier;

            if (tooltipPos.x + width > canvas.rect.width)
            {
                tooltipPos.x = canvas.rect.width - width;
            }
            else if (Mathf.Abs(tooltipPos.x) + width > canvas.rect.width)
            {
                tooltipPos.x = -canvas.rect.width + width;
            }

            if (tooltipPos.y + height > canvas.rect.height)
            {
                tooltipPos.y = canvas.rect.height - height;
            }
            else if (Mathf.Abs(tooltipPos.y) + height > canvas.rect.height)
            {
                tooltipPos.y = -canvas.rect.width + height;
            }

            transform.GetComponent<RectTransform>().anchoredPosition = tooltipPos;
        }
    }

    #endregion
}
