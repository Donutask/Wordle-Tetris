using UnityEngine;
using System;
using Donutask.Wordfall;

public class CheckWords : MonoBehaviour
{
    [SerializeField] TextAsset listToCheck;

    void Start()
    {
        if (listToCheck == null)
        {
            return;
        }

        string[] lines = listToCheck.text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

        foreach (var word in lines)
        {
            if (WordManager.IsValidWord(word))
            {
                //ok
            }
            else
            {
                Debug.Log(word + " is not considered valid");
            }
        }
    }


}
