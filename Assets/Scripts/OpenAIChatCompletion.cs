using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using System.Collections.Generic;

public class OpenAIChatCompletion : MonoBehaviour
{
    [SerializeField] string apiKey = "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";

    public async Task<OpenAIChatCompletionResponse> CreateChatCompletionAsync(OpenAIChatCompletionRequest request)
    {
        JsonSerializerSettings settings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        };
        // Serialize request object into JSON
        string jsonRequest = JsonConvert.SerializeObject(request, settings);

        Debug.Log("Request: " + jsonRequest);

        UnityWebRequest www = new UnityWebRequest("https://api.openai.com/v1/chat/completions", "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonRequest);
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");
        www.SetRequestHeader("Authorization", "Bearer " + apiKey);

        await www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
            Debug.Log("Response: " + www.downloadHandler.text);
            return null;
        }
        else
        {
            // Deserialize the JSON response into OpenAIChatCompletionResponse object
            Debug.Log("Response: " + www.downloadHandler.text);
            OpenAIChatCompletionResponse response = JsonConvert.DeserializeObject<OpenAIChatCompletionResponse>(www.downloadHandler.text);

            return response;
        }
    }
}

[System.Serializable]
public class OpenAIChatCompletionResponse
{
    public string id;
    public string object_type;
    public long created;
    public Choice[] choices;
    public Usage usage;
}

[System.Serializable]
public class Choice
{
    public int index;
    public Message message;
    public string finish_reason;
}

[System.Serializable]
public class Message
{
    public Message(string role, string name = null, string content = null, FunctionCall function_call = null){
        this.role = role;
        this.name = name;
        this.content = content;
        this.function_call = function_call;
    }

    public string role;
    public string name;
    public string content;
    public FunctionCall function_call;
}

[System.Serializable]
public class Usage
{
    public int prompt_tokens;
    public int completion_tokens;
    public int total_tokens;
}

[System.Serializable]
public class OpenAIChatCompletionRequest
{
    public OpenAIChatCompletionRequest(string model, Message[] messages, Function[] functions = null, FunctionCall function_call = null,
        double temperature = 1, double top_p = 1, int n = 1, bool stream = false, string[] stop = null, int? max_tokens = null,
        double presence_penalty = 0, double frequency_penalty = 0, Dictionary<string, double> logit_bias = null)
    {
        this.model = model;
        this.messages = messages;
        this.functions = functions;
        this.function_call = function_call;
        this.temperature = temperature;
        this.top_p = top_p;
        this.n = n;
        this.stream = stream;
        this.stop = stop;
        this.max_tokens = max_tokens;
        this.presence_penalty = presence_penalty;
        this.frequency_penalty = frequency_penalty;
        this.logit_bias = logit_bias;
    }

    public string model;
    public Message[] messages;

    public Function[] functions;
    public FunctionCall function_call;
    public double temperature;
    public double top_p;
    public int n;
    public bool stream;
    public string[] stop;
    public int? max_tokens;
    public double presence_penalty;
    public double frequency_penalty;
    public Dictionary<string, double> logit_bias;
    public string user;
}

[System.Serializable]
public class FunctionCall
{
    public string name;
    public object arguments;
}

[System.Serializable]
public class Function
{
    public Function(string name, string description = null, object parameters = null){
        this.name = name;
        this.description = description;
        this.parameters = parameters;
    }

    public string name;
    public string description;
    public object parameters;
}