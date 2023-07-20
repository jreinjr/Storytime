using System;
using System.Collections;
using System.Collections.Generic;
using DGAI;
using Unity.VisualScripting;
using UnityEngine;

public class StorytimeDemoController : MonoBehaviourSingleton<StorytimeDemoController>
{
    [SerializeField] UI_StoryBearChatScreen chatScreen;
    [SerializeField] UI_StoryPreviewScreen previewScreen;
    [SerializeField] UI_StoryPlayScreen playScreen;
    [SerializeField] private AIManager _aiManager;


    void Start(){
        chatScreen.LogChatMessage("Hello there! I'm so excited to be your friendly children's story author today. I can't wait to hear the ideas you have for your story and help you bring them to life. \n Let's start with the main character - what is their name? What do they look like? The more detail you provide, the better.",
        UI_StoryBearChatScreen.ChatUser.STORYBEAR);
    }

    public void OnUserChatInput(string text)
    {
        _aiManager.GetText("You are a friendly children's story author. Use the following chat log with a child to generate a story based on their ideas. " + text,
        StoryCallback);
    }

    void StoryCallback(string teststring, string passthroughData){
        Debug.LogFormat("teststring: {0} passthroughdata{1}");
    }

    public void ActivateChatScreen()
    {
        previewScreen.gameObject.SetActive(false);
        playScreen.gameObject.SetActive(false);
        chatScreen.gameObject.SetActive(true);
    }

    public void ActivatePreviewScreen()
    {
        previewScreen.gameObject.SetActive(true);
        playScreen.gameObject.SetActive(false);
        chatScreen.gameObject.SetActive(true);
    }

    public void ActivatePlayScreen()
    {
        previewScreen.gameObject.SetActive(false);
        playScreen.gameObject.SetActive(true);
        chatScreen.gameObject.SetActive(false);
    }

}
