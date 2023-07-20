using System.Collections;
using System.Collections.Generic;
using DGAI;
using UnityEngine;

public class PromptGeneration : MonoBehaviour
{
    [SerializeField] private AIManager _aiManager;

    public void HandleUserInput(string input){
        
    }

     private void Start()
    {
        //Text callback without defining passthrough value
        _aiManager.GetText("Give a short visual description of a kitten.", TestChat);
    }

    public void TestChat(string testString, string passthroughData)
    {
        Debug.Log("Text Response " + passthroughData + "\nResult:\n" + testString);
    }
}
