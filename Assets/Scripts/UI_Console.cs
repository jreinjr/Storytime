using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_Console : MonoBehaviour
{
    [SerializeField] TMP_InputField protagonistInfo;
    [SerializeField] TMP_InputField settingInfo;
    [SerializeField] TMP_InputField companionInfo;
    [SerializeField] TMP_InputField conflictInfo;
    [SerializeField] TMP_InputField lessonInfo;
    [SerializeField] TMP_Dropdown targetAgeInfo;

    public delegate void MessageEventDelegate(string prompt);
    public event MessageEventDelegate OnStoryPromptSent;

    public void SendStoryPrompt(){
        string storyPrompt = string.Format(@" 
        Write an 8-paragraph story for {0}.
        The story is set in {1}
        The main character is {2}.
        The main character's companion is {3}.
        The story's conflict involves {4}.
        The lesson of the story is {5}.
        The story should be appropriate for {0}.
        Write 8 paragraphs, including exposition (1 paragraph), rising action (1 paragraph), climax (4 paragraphs), falling action (1 paragraph) and conclusion (1 paragraph).
        ",
        targetAgeInfo.options[targetAgeInfo.value].text,
        settingInfo.text,
        protagonistInfo.text,
        companionInfo.text,
        conflictInfo.text,
        lessonInfo.text
        );

        OnStoryPromptSent?.Invoke(storyPrompt);
    }

}
