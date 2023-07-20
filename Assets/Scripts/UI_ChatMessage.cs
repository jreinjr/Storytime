using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_ChatMessage : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI chatTextUI;

    public void SetText(string text)
    {
        chatTextUI.text = text;
    }
}
