using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;
using System.Text;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DGAI
{
#if UNITY_EDITOR
    [CustomEditor(typeof(AIManager))]
    public class AIManagerEditor : Editor 
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            if (EditorGUILayout.LinkButton("Link to oauth page"))
                Application.OpenURL("https://www.internalfb.com/intern/oauth");
        }
    }
#endif
    public class AIManager : MonoBehaviour
    {

        #region Classes
        [Serializable]
        public class MetaGenTextInput
        {
            //https://www.internalfb.com/intern/staticdocs/metagen/docs/api-reference/www/reference/docgen/Types/GenAITextInputParameters/
            public string prompt = "";
            public int max_tokens = 512;
            public float temperature = 0.5f;
            public float top_p = 0.5f;
            public float repetition_penalty = 0.5f;
            public string model = "multiray_llm";
            //https://www.internalfb.com/intern/staticdocs/metagen/docs/api-reference/www/reference/docgen/Enums/GenAIModel/
            public MetaGenTextInput(string setPrompt)
            {
                prompt = setPrompt;
            }
        }
        /*  MetaGen Text Response
        {
            "success": true,
            "response": {
                "candidates": [
                {
                    "text": "Hello! How can I help you today? Is there something specific you would like to chat about or ask? I'm here to assist with any questions you may have.",
                    "response_id": "65375684-b64d-4545-84d4-856bce746269",
                    "finish_reason": "stopped",
                    "output_integrity_decision": "no_decision"
                }
                ],
                "usage": {
                    "num_prompt_tokens": 11,
                    "num_completion_tokens": 37,
                    "num_total_tokens": 48
                },
                "input_integrity_decision": "no_decision"
            }
        }
        */
        [Serializable]
        public class MetaGenTextOutput
        {
            public bool success;
            public MetaGenTextResponse response;
        }

        [Serializable]
        public class MetaGenTextResponse
        {
            public MetaGenTextCandidate[] candidates;
            public MetaGenTextUsage usage;
            public string input_integrity_decision;
        }

        [Serializable]
        public class MetaGenTextCandidate
        {
            public string text;
            public string response_id;
            public string finish_reason;
            public string output_integrity_decision;
        }

        [Serializable]
        public class MetaGenTextUsage
        {
            public int num_prompt_tokens;
            public int num_completion_tokens;
            public int num_total_tokens;
        }
        
        
        [Serializable]
        public class MetaGenImageInput
        {
            //https://www.internalfb.com/intern/staticdocs/metagen/docs/api-reference/www/reference/docgen/Types/GenAIImageInputOptions/
            public string prompt;
            public int num_images = 1;
            public string model = "photorealism";
            /*models include
             sticker
             photorealism
             text2image_ldm_toadette   this is an old model
             https://www.internalfb.com/intern/staticdocs/metagen/docs/api-reference/www/reference/docgen/Enums/GenAIModel/
             */
            public MetaGenImageInput(string setPrompt)
            {
                prompt = setPrompt;
            }
        }
        /*MetaGen Image Response
         {
       "success": true,
       "response": [
          {
             "prompt": "\"An adorable kitten\"}]",
             "uri": "https://scontent-lhr8-2.xx.fbcdn.net/o1/v/t0/f1/m247/123456_438079165_13-07-2023-03-21-25.jpeg?_nc_ht=scontent-lhr8-2.xx.fbcdn.net&_nc_cat=110&ccb=9-4&oh=00_AfAS5iHBX3GDneScHLDeLLV4oN4EilV7h3fKEpma2nWDyQ&oe=64B17270&_nc_sid=5b3566",
             "media_type": "image",
             "temporary_handle": "genai_m4/123456_438079165_13-07-2023-03-21-25.jpeg",
             "response_id": "b2731c3d-479d-4c8b-acc8-83736f52091a",
             "request_id": "942ac66e-da61-42fc-a132-607cd6c8e731"
          }
       ]
    }
    */
        [Serializable]
        public class MetaGenImageOutput
        {
            public bool success;
            public MetaGenImageResponse[] response;
        }

        [Serializable]
        public class MetaGenImageResponse
        {
            public string prompt;
            public string uri;
            public string media_type;
            public string temporary_handle;
            public string response_id;
            public string request_id;
        }
        #endregion
        
        [SerializeField] private bool test = false;
        [Header("To get your access token go to \nhttps://www.internalfb.com/intern/oauth\nand click on MetaGen > Generate Token")]
        [SerializeField] private string MetaGenAccessToken;
        
        #region Testing
        private void Start()
        {
            if (test)
            {
                string passthroughData = "This is a test.";
                RunTest("Tell me about an adorable kitty.", passthroughData);
            }
        }
        private void RunTest(string testInput, string passthroughData)
        {
            GetText(testInput, TestChat, passthroughData);
            GetImage(testInput, TestTexture, passthroughData);
        }
        public void TestChat(string testString, string passthroughData)
        {
            Debug.Log("Text Response " + passthroughData + "\nResult:\n" + testString);
        }
        public void TestTexture(Texture testTexture, string passthroughData)
        {
            Debug.Log("Got Sprite " + passthroughData);
            gameObject.AddComponent<Canvas>();
            GameObject spriteGO = new GameObject("Sprite");
            spriteGO.transform.SetParent(transform);
            spriteGO.AddComponent<UnityEngine.UI.RawImage>().texture = testTexture;
        }
        #endregion

        #region Text

        public void GetText(string input, Action<string, string> callback, string passthroughData = "")
        {
            if (MetaGenAccessToken == "")
            {
                Debug.LogError("Please go to https://www.internalfb.com/intern/oauth to get an access token");
                return;
            }
            StartCoroutine(MetaGenTextCoroutine(new MetaGenTextInput(input), callback, passthroughData));
        }
        public void GetText(MetaGenTextInput input, Action<string, string> callback, string passthroughData = "")
        {
            if (MetaGenAccessToken == "")
            {
                Debug.LogError("Please go to https://www.internalfb.com/intern/oauth to get an access token");
                return;
            }
            StartCoroutine(MetaGenTextCoroutine(input, callback, passthroughData));
        }


        private IEnumerator MetaGenTextCoroutine(MetaGenTextInput input, Action<string, string> callback, string passthroughData)
        {
            input.prompt = CleanForJSON(input.prompt);
            string secret = "access_token=" + MetaGenAccessToken;
            string url = "https://interngraph.intern.facebook.com/metagen/chat_completion/"+
                         "?caller=metagen_unity&messages=[{\"role\":\"user\",\"text\":\""+
                         input.prompt+
                         "\"}]&options={"+
                         "\"max_tokens\":"+input.max_tokens+","+
                         "\"temperature\":"+input.temperature+","+
                         "\"top_p\":"+input.top_p+","+
                         "\"repetition_penalty\":"+input.repetition_penalty+","+
                         "\"crypto_auth_tokens\":null,"+
                         "\"model\":\""+input.model+"\""+
                         "}";
            using (UnityWebRequest request = new UnityWebRequest(url))
            {
                request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(secret));
                request.downloadHandler = new DownloadHandlerBuffer();
                request.method = UnityWebRequest.kHttpVerbPOST;
                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("accept", "application/json");
                request.disposeUploadHandlerOnDispose = true;
                request.disposeDownloadHandlerOnDispose = true;
                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError(request.error);
                }
                else
                {
                    Debug.Log(request.downloadHandler.text);
                    MetaGenTextOutput chatResponse = JsonUtility.FromJson<MetaGenTextOutput>(request.downloadHandler.text);
                    string resultText = chatResponse.response.candidates[0].text;
                    callback(resultText, passthroughData);
                }
            }
        }
        #endregion
        #region Images
        public void GetImage(string input, Action<Texture, string> callback, string passthroughData)
        {
            if (MetaGenAccessToken == "")
            {
                Debug.LogError("Please go to https://www.internalfb.com/intern/oauth to get an access token");
                return;
            }
            StartCoroutine(MetaGenImageCoroutine(new MetaGenImageInput(input), callback, null, passthroughData));
        }

        public void GetImage(MetaGenImageInput input, Action<Texture, string> callback, string passthroughData)
        {
            if (MetaGenAccessToken == "")
            {
                Debug.LogError("Please go to https://www.internalfb.com/intern/oauth to get an access token");
                return;
            }
            StartCoroutine(MetaGenImageCoroutine(input, callback, null, passthroughData));
        }

        public void GetImage(MetaGenImageInput input, Action<Texture[], string> callback, string passthroughData)
        {
            if (MetaGenAccessToken == "")
            {
                Debug.LogError("Please go to https://www.internalfb.com/intern/oauth to get an access token");
                return;
            }
            StartCoroutine(MetaGenImageCoroutine(input, null, callback, passthroughData));
        }


        private IEnumerator MetaGenImageCoroutine(MetaGenImageInput input, Action<Texture, string> callback, Action<Texture[], string> callback2, string passthroughData)
        {
            input.prompt = CleanForJSON(input.prompt);
            string secret = "access_token=" + MetaGenAccessToken;
            string url = "https://interngraph.intern.facebook.com/metagen/image_generation/"+
                         "?caller=metagen_unity&parameters={"+
                         "\"num_images\":"+input.num_images+","+
                         "\"crypto_auth_tokens\":null,"+
                         "\"model\":\""+input.model+"\""+
                         "}&prompt=\""+
                         input.prompt+
                         "\"}]";
            using (UnityWebRequest request = new UnityWebRequest(url))
            {
                request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(secret));
                request.downloadHandler = new DownloadHandlerBuffer();
                request.method = UnityWebRequest.kHttpVerbPOST;
                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("accept", "application/json");
                request.disposeUploadHandlerOnDispose = true;
                request.disposeDownloadHandlerOnDispose = true;
                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError(request.error);
                }
                else
                {
                    Texture[] textures = new Texture[input.num_images];
                    Debug.Log(request.downloadHandler.text);
                    MetaGenImageOutput response = JsonUtility.FromJson<MetaGenImageOutput>(request.downloadHandler.text);
                    for (int i = 0; i < textures.Length; i++)
                    {
                        using (UnityWebRequest request2 = UnityWebRequestTexture.GetTexture(response.response[i].uri))
                        {
                            yield return request2.SendWebRequest();
                            if (request2.result != UnityWebRequest.Result.Success)
                            {
                                Debug.Log(request2.error);
                                Debug.Log(request2.downloadHandler.error);
                            }
                            else
                            {
                                //if you want a sprite then you can use this call in your own function
                                //Sprite mySprite = Sprite.Create((Texture2D)myTexture, new Rect(0.0f, 0.0f, myTexture.width, myTexture.height), new Vector2(0.5f, 0.5f), 100.0f);
                                textures[i] = ((DownloadHandlerTexture)request2.downloadHandler).texture;
                            }
                        }
                    }

                    if (callback != null)
                    {
                        callback(textures[0], passthroughData);
                    }

                    if (callback2 != null)
                    {
                        callback2(textures, passthroughData);
                    }
                }
            }
        }
        #endregion
        #region Util

        private float BytesToFloat(byte firstByte, byte secondByte)
        {
            // Convert two bytes to one short (little endian)
            short s = (short)((secondByte << 8) | firstByte);

            // Convert to range from -1 to (just below) 1
            return s / 32768.0F;
        }
        private string CleanForJSON(string s)
        {
            if (s == null || s.Length == 0)
            {
                return "";
            }

            char c = '\0';
            int i;
            int len = s.Length;
            StringBuilder sb = new StringBuilder(len + 4);

            for (i = 0; i < len; i ++)
            {
                c = s[i];
                switch (c)
                {
                    case '\\':
                    case '"':
                        sb.Append('\\');
                        sb.Append(c);
                        break;
                    case '/':
                        sb.Append('\\');
                        sb.Append(c);
                        break;
                    case '\b':
                        sb.Append("\\b");
                        break;
                    case '\t':
                        sb.Append("\\t");
                        break;
                    case '\n':
                        sb.Append("\\n");
                        break;
                    case '\f':
                        sb.Append("\\f");
                        break;
                    case '\r':
                        sb.Append("\\r");
                        break;
                    default:
                        sb.Append(c);
                        break;
                }
            }
            return sb.ToString();
        }
        #endregion
    } 
}
