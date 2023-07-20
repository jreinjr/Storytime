using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;
using TMPro;

public class UI_StoryBearChatScreen : MonoBehaviour
{
    public enum ChatUser
    {
        HUMAN,
        STORYBEAR
    }
    [SerializeField] GameObject aiMessagePrefab;
    [SerializeField] GameObject humanMessagePrefab;
    [SerializeField] GameObject storyMessagePrefab;
    [SerializeField] Transform chatLog;
    [SerializeField] TMP_InputField chatInput;

    private string chatHistory = "";
    public void OnSubmitButtonPressed()
    {
        LogChatMessage(chatInput.text, ChatUser.HUMAN);
        chatInput.text = "";
        // Send full chat history to server
        
    }

    public void LogChatMessage(string message, ChatUser user)
    {
        GameObject msgPrefab = (user == ChatUser.HUMAN ? humanMessagePrefab : aiMessagePrefab);
        GameObject msgGO = Instantiate(msgPrefab, chatLog);
        UI_ChatMessage msg = msgGO.GetComponent<UI_ChatMessage>();
        msg.SetText(message);
        chatHistory += string.Format("{0}: {1}\n", user.ToString(), message);
    }

    void LogStoryMessage(string title, Texture image)
    {
        GameObject msgGO = Instantiate(storyMessagePrefab, chatLog);
        UI_StoryMessage msg = msgGO.GetComponent<UI_StoryMessage>();
        msg.Initialize(title, image);
    }
}
