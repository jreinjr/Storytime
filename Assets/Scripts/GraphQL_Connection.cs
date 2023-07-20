using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class GraphQL_Connection : MonoBehaviour
{
    private string url = "https://www.104700.od.internalfb.com/intern/graphiql/";

    IEnumerator Start()
    {
        string query = @"
        {
            query($request: XFBMessengerKidsStoryTimeRequest!) { 
              xfb_new_story(request : $request) {
                story
                scenes
              }
            }
        }";

        // Escape query JSON string
        string jsonQuery = JsonUtility.ToJson(new GraphQLQuery(query));

        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonQuery);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        // Include the Authorization header if necessary
        // request.SetRequestHeader("Authorization", "Bearer YOUR_TOKEN_HERE");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(request.error);
        }
        else
        {
            Debug.Log("Received: " + request.downloadHandler.text);
            // Process the response JSON as needed
        }
    }
}

public class GraphQLQuery
{
    public string query;

    public GraphQLQuery(string query)
    {
        this.query = query;
    }
}
