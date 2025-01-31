//using UnityEngine;
//using System;
//using Donutask.Wordfall;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using System.IO;

//namespace Donutask.Wordfall.Tests
//{
//    public class CheckWords : MonoBehaviour
//    {
//        void Start()
//        {
//        }

//        void FindMissingWordsInAnagramLists()
//        {
//            List<string> missingWords = new List<string>();
//            List<string> combinedSolutionLists = new List<string>();

//            foreach (var list in WordManager.solutionLists)
//            {
//                combinedSolutionLists.AddRange(list);
//            }

//            foreach (var word in WordManager.wordList)
//            {
//                if (!combinedSolutionLists.Contains(word) && !missingWords.Contains(word))
//                {
//                    missingWords.Add(word);
//                }
//            }

//            File.WriteAllLines("/Users/charles/Wordle Tetris/Assets/Text/DictionaryNoDuplicates.txt", missingWords);
//        }

//        [SerializeField] TextAsset listToCheck;
//        void CheckIfWordsInListAreInDictionary()
//        {
//            if (listToCheck == null)
//            {
//                return;
//            }

//            string[] lines = listToCheck.text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

//            foreach (var word in lines)
//            {
//                if (WordManager.IsValidWord(word))
//                {
//                    //ok
//                }
//                else
//                {
//                    Debug.Log(word + " is not considered valid");
//                }
//            }
//        }
//    }
//}
