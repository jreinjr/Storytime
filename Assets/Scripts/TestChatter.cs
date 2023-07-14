using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class TestChatter : MonoBehaviour
{
    [SerializeField] OpenAIChatCompletion openAI;

    [SerializeField] UI_ChatPanel uI_ChatPanel;

    List<Message> chatMessages = new List<Message>();

    private void OnEnable() {
        uI_ChatPanel.OnSubmitButtonClicked += OnChatMessageSubmitted;
    }

    private void OnDisable() {
        uI_ChatPanel.OnSubmitButtonClicked -= OnChatMessageSubmitted;
    }

    private async void Start() {

        chatMessages.Add(new Message("system", content:"You are a helpful AI. You have the ability to guess the user's name with your guess_user_name method. You have the ability to write a poem with your with your write_a_poem method. "));


    }



    public async void OnChatMessageSubmitted(string message){
        
        chatMessages.Add(new Message("user", content:message));

        OpenAIChatCompletionRequest req = new OpenAIChatCompletionRequest("gpt-3.5-turbo-0613", chatMessages.ToArray());

        OpenAIChatCompletionResponse resp = await openAI.CreateChatCompletionAsync(req);

        Message responseMessage = resp.choices[0].message;
        
        if(responseMessage.function_call != null)
        {
            responseMessage.content = "";
            string functionName = responseMessage.function_call.name.ToString();
            var functionArgs = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseMessage.function_call.arguments.ToString());
            Debug.LogFormat("Calling function {0} with args {1}", functionName, functionArgs);
            var availableFunctions = new Dictionary<string, MethodInfo>
            {
                ["update_current_story"] = GetType().GetMethod("UpdateCurrentStory"),
            };

            if(availableFunctions.TryGetValue(functionName, out MethodInfo functionToCall))
            {
                var functionResponse = functionToCall.Invoke(this, functionArgs.Values.ToArray());

                chatMessages.Add(responseMessage);
                chatMessages.Add(new Message("function", functionName, functionResponse.ToString()));

                req = new OpenAIChatCompletionRequest("gpt-3.5-turbo-0613", chatMessages.ToArray());

                // Call CreateChatCompletionAsync function again here with updated messages and get the second response
                OpenAIChatCompletionResponse secondResponse = await openAI.CreateChatCompletionAsync(req);
                Message secondMessage = secondResponse.choices[0].message;
                chatMessages.Add(secondMessage);
                uI_ChatPanel.LogAiMessage(secondMessage.content);
            }
        }
        else{
            uI_ChatPanel.LogAiMessage(responseMessage.content);
        }
    }



    public void UpdateCurrentStory(string newStory){
        
    }

    private Function Get_UpdateCurrentStory_FunctionSignature(){
        var parameters = new JObject{
            ["type"] = "object",
            ["properties"] = new JObject(),
            ["required"] = new JArray()
        };
        ((JObject)parameters["properties"]).Add("newStory", new JObject());
        ((JObject)((JObject)parameters["properties"])["newStory"]).Add("type", "string");
        ((JObject)((JObject)parameters["properties"])["newStory"]).Add("description", "the updated version of the story");

        ((JArray)parameters["required"]).Add("newStory");

        Function result = new Function("update_current_story", "Useful when you want to show the user an updated version of the current story", parameters);
        return result;
    } 
}
