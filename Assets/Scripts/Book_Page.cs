using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Book_Page : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI pageText;
    [SerializeField] RawImage pageImage;
    
    public void Set_Text(string text){
        pageText.text = text;
    }

    public void Set_Image(Texture image){
        pageImage.texture = image;
    }
}
