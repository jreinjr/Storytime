using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorytimeDemoController : MonoBehaviourSingleton<StorytimeDemoController>
{
    [SerializeField] GraphQL_Connection graphQL_connection;
    [SerializeField] UI_StoryBearChatScreen chatScreen;
    [SerializeField] UI_StoryPreviewScreen previewScreen;
    [SerializeField] UI_StoryPlayScreen playScreen;

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
