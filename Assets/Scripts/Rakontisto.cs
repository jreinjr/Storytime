using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rakontisto : MonoBehaviour
{
    [SerializeField] UI_Console console;
    [SerializeField] OpenAIChatCompletion openAI;

    List<Message> chatMessages = new List<Message>();

    private void OnEnable() {
        console.OnStoryPromptSent += OnStoryPromptSent;
    }

    private void OnDisable() {
        console.OnStoryPromptSent -= OnStoryPromptSent;
    }

    private async void OnStoryPromptSent(string prompt)
    {
        chatMessages.Add(new Message("user", content:prompt));

        OpenAIChatCompletionRequest req = new OpenAIChatCompletionRequest("gpt-3.5-turbo-0613", chatMessages.ToArray());

        OpenAIChatCompletionResponse resp = await openAI.CreateChatCompletionAsync(req);

        Message responseMessage = resp.choices[0].message;

        Debug.Log(responseMessage.content);
    }

    private void Start() {
        chatMessages.Add(new Message("system", content:"You are a creative, popular children's story book author."));
    }
}
