using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Book_Content : MonoBehaviour
{
    [SerializeField] List<Book_Page> bookPages;

    public void UpdateStory(string newStory)
    {
        string[] separators = new string[] { "\n\n", "\n" };
        string[] paragraphs = newStory.Split(separators, StringSplitOptions.RemoveEmptyEntries);

        // Hardcoded page count for now
        for (int i = 0; i < 8; i++)
        {
            bookPages[i].Set_Text(paragraphs[i]);
        }
    }

    private void Start() {
        if (bookPages == null) bookPages = new List<Book_Page>();
    }
}
