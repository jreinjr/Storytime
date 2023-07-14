using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UI_ChatPanel : MonoBehaviour
{
    [SerializeField] Transform chatLog;
    [SerializeField] GameObject ui_humanMessage_prefab;
    [SerializeField] GameObject ui_aiMessage_prefab;
    [SerializeField] Button ui_submitMessageButton;
    [SerializeField] TMP_InputField ui_InputField;

    public delegate void InputButtonClickHandler(string text);
    public event InputButtonClickHandler OnSubmitButtonClicked;

    private void OnEnable() {
        ui_submitMessageButton.onClick.AddListener(SubmitButtonClicked);
    }

    private void OnDisable() {
        ui_submitMessageButton.onClick.RemoveListener(SubmitButtonClicked);
    }

    public void SubmitButtonClicked(){
        LogHumanMessage(ui_InputField.text);
        OnSubmitButtonClicked?.Invoke(ui_InputField.text);
        ui_InputField.text = "";
    }

    public void LogHumanMessage(string message){
        GameObject newHumanMessageGO = Instantiate(ui_humanMessage_prefab, chatLog);
        newHumanMessageGO.GetComponent<TextMeshProUGUI>().text = message;
    }

    public void LogAiMessage(string message){
        GameObject newAiMessageGO = Instantiate(ui_aiMessage_prefab, chatLog);
        newAiMessageGO.GetComponent<TextMeshProUGUI>().text = message;
    }
}
