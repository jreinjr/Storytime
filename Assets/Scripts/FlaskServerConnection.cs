using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FlaskServerConnection : MonoBehaviour
{
    private string flaskServerURL = "http://localhost:5000/process";

    void Start(){
        SendStringToServer("Hey Wurld");
    }

    // Use this function to send a string to the Flask server
    public void SendStringToServer(string inputString)
    {
        StartCoroutine(PostRequest(inputString));
    }

    private IEnumerator PostRequest(string inputString)
    {
        // Create a new request, add the input string as JSON data
        var request = new UnityWebRequest(flaskServerURL, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes("{\"inputString\": \"" + inputString + "\"}");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // Wait for the response and then get the data
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Received: " + request.downloadHandler.text);
        }
        else
        {
            Debug.Log("Error: " + request.error);
        }
    }
}