using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DGAI
{
    public class GenAIUsageExampleCallbacks : MonoBehaviour
    {
        //References to the UI elements we will set
        [SerializeField] private RawImage textureImageA1;
        [SerializeField] private RawImage textureImageA2;
        [SerializeField] private RawImage textureImageB;
        [SerializeField] private RawImage followupTexture;
        [SerializeField] private Text text;

        //We need a reference to the AI Manager
        [SerializeField] private AIManager _aiManager;
        
        private void Start()
        {
            //Text callback without defining passthrough value
            _aiManager.GetText("Give a short visual description of a kitten.", TestChat);
            
            //Make input parameters for A
            AIManager.MetaGenImageInput paramsA = new AIManager.MetaGenImageInput("A");
            paramsA.model = "sticker";
            paramsA.num_images = 2;
            
            //The action used in this callback has the function defined in the editor
            //The passthrough data of A and B is used to determine what to do with each image when that image is returned
            _aiManager.GetImage(paramsA, TestTextures, "A");
            _aiManager.GetImage("B", TestTexture, "B");
        }
        private void TestChat(string testString, string passthroughData)
        {
            //get the callback response from the text endpoint
            Debug.Log("Text Response " + passthroughData + "\nResult:\n" + testString);
            text.text = testString;
            
            //Use the information from the text to make an image
            //give the passthrough C to tell the callback where to stick the image
            _aiManager.GetImage("An adorable orange tabby kitten", TestTexture, "C");
        }
        public void TestTexture(Texture testTexture, string passthroughData)
        {
            //get a texture from the image endpoint
            //The passthrough data of C and B is used to determine what to do with each image when that image is returned
            Debug.Log("Got Texture " + passthroughData);
            switch (passthroughData)
            {
                case "B":
                    textureImageB.texture = testTexture;
                    break;
                case "C":
                    followupTexture.texture = testTexture;
                    break;
                default:
                    Debug.LogError("Unrecognized passthrough data");
                    break;
            }
        }

        public void TestTextures(Texture[] testTextures, string passthroughData)
        {
            //get multiple textures from the image endpoint
            Debug.Log("Got Texture " + passthroughData);
            switch (passthroughData)
            {
                case "A":
                    textureImageA1.texture = testTextures[0];
                    textureImageA2.texture = testTextures[1];
                    break;
                default:
                    Debug.LogError("Unrecognized passthrough data");
                    break;
            }
        }
    }
}
