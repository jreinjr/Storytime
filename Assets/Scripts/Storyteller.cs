using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class Storyteller : MonoBehaviour
{
        // These fields are set in the editor
    [SerializeField] OpenAIChatCompletion openAI;
    [SerializeField] UI_ChatPanel uI_ChatPanel;
    [SerializeField] Book_Content bookContent;

    // Private variables
    private List<Message> chatMessages = new List<Message>();
    private Function[] functions;
    
    private string currentStory = "";

    private const string MAIN_SYSTEM_PROMPT = 
        "You are a playful, witty children's book author.\n" +
        "You are writing an 8-paragraph story inspired by dictation from children.\n" + 
        "When they ask you to write or tell them a story, use your write_new_story function.\n" + 
        "Always remember your audience are young children. Write and speak age-appropriately.\n" +
        "Always speak in a kind, supportive and respectful way to the children.\n";

    private const string NEW_STORY_PROMPT = 
        "You are a playful, witty children's book author.\n" +
        "You are writing a new book inspired by dictation from children.\n" + 
        "When they give you dictation, write an 8-paragraph story based on their input.\n" + 
        "The climax of the story should be 4 paragraphs (half its total length).\n" +
        "Always remember your audience are young children. Write age-appropriately.\n";

    private const string CHANGE_STORY_PROMPT = 
        "You are a playful, witty children's book author.\n" +
        "You are writing a new book inspired by dictation from children.\n" + 
        "When they give you dictation, update the current story based on their feedback.\n" + 
        "Only modify what you have been asked to modify. Leave the rest unchanged.\n" +
        "Always remember your audience are young children. Write age-appropriately.\n";

    
    private void OnEnable() {
        uI_ChatPanel.OnSubmitButtonClicked += OnChatMessageSubmitted;
    }

    private void OnDisable() {
        uI_ChatPanel.OnSubmitButtonClicked -= OnChatMessageSubmitted;
    }

    private async void Start() {
        // System prompt
        chatMessages.Add(new Message("system", content: MAIN_SYSTEM_PROMPT));

        // List of available function signatures (for AI to decide which to use)
        functions = new Function[]{
            Get_WriteNewStory_FunctionAsJSON()
        };
    }


    public async void OnChatMessageSubmitted(string message){
        // Set system message
        chatMessages[0].content = MAIN_SYSTEM_PROMPT;

        if (!string.IsNullOrEmpty(currentStory)){
            chatMessages.Add(new Message("assistant", content: "Here is the story so far: " + currentStory));
        }

        chatMessages.Add(new Message("user", content:message));

        OpenAIChatCompletionRequest req = new OpenAIChatCompletionRequest("gpt-3.5-turbo-0613", chatMessages.ToArray(), functions);

        OpenAIChatCompletionResponse resp = await openAI.CreateChatCompletionAsync(req);

        Message responseMessage = resp.choices[0].message;
        
        // If response includes a function call
        if(responseMessage.function_call != null)
        {
            responseMessage.content = "";
            // Get function name and args from response
            string functionName = responseMessage.function_call.name.ToString();
            var functionArgs = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseMessage.function_call.arguments.ToString());

            Debug.LogFormat("Calling function {0} with args {1}", functionName, functionArgs);
            // List of available functions (for Unity to call)
            var availableFunctions = new Dictionary<string, MethodInfo>
            {
                ["write_new_story"] = GetType().GetMethod("WriteNewStory"),
            };

            // Call the function
            if(availableFunctions.TryGetValue(functionName, out MethodInfo functionToCall))
            {
                var functionResponse = functionToCall.Invoke(this, functionArgs.Values.ToArray());

                // // We include the raw output of the function response in the AI's memory of the conversation
                // // But not logged in the UI (we want it to be updated in the book)
                // chatMessages.Add(responseMessage);
                // chatMessages.Add(new Message("function", functionName, functionResponse.ToString()));

                // req = new OpenAIChatCompletionRequest("gpt-3.5-turbo-0613", chatMessages.ToArray());

                // // Call CreateChatCompletionAsync function again here with updated messages and get the second response
                // OpenAIChatCompletionResponse secondResponse = await openAI.CreateChatCompletionAsync(req);
                // Message secondMessage = secondResponse.choices[0].message;
                // chatMessages.Add(secondMessage);
                // // This is a natural language message from the AI explaining that it has successfully called the function
                // uI_ChatPanel.LogAiMessage(secondMessage.content);
            }
        }
        // If response does not include a function call, just log it
        else{
            uI_ChatPanel.LogAiMessage(responseMessage.content);
        }
    }

    public async void WriteNewStory(string prompt){   
        // Set system message
        chatMessages[0].content = NEW_STORY_PROMPT;

        OpenAIChatCompletionRequest req = new OpenAIChatCompletionRequest("gpt-3.5-turbo-0613", chatMessages.ToArray());

        OpenAIChatCompletionResponse resp = await openAI.CreateChatCompletionAsync(req);

        Message responseMessage = resp.choices[0].message;

        currentStory = responseMessage.content;
        
        bookContent.UpdateStory(currentStory);
    }


    private Function Get_WriteNewStory_FunctionAsJSON(){
        var parameters = new JObject{
            ["type"] = "object",
            ["properties"] = new JObject(),
            ["required"] = new JArray()
        };
        ((JObject)parameters["properties"]).Add("prompt", new JObject());
        ((JObject)((JObject)parameters["properties"])["prompt"]).Add("type", "string");
        ((JObject)((JObject)parameters["properties"])["prompt"]).Add("description", "all relevant details from the user needed to write the story");

        ((JArray)parameters["required"]).Add("prompt");

        Function result = new Function("write_new_story", "Useful when the user asks you to write a story", parameters);
        return result;
    } 

}
