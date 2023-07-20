using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_StoryMessage : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI storyTitleUI;
    [SerializeField] RawImage titleImage;

    public void Initialize(string title, Texture image)
    {
        storyTitleUI.text = title;
        titleImage.texture = image;
    }

    public void OnPreviewButtonPressed()
    {
        StorytimeDemoController.Instance.ActivatePreviewScreen();
    }

    public void OnRegenerateButtonPressed()
    {
        Debug.Log("RegeneratePressed");
    }
}
