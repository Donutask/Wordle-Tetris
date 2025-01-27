//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;
//using Donutask.Wordfall;
//using System.Linq;
//using System.IO;

//public class AnagramSolver : MonoBehaviour
//{
//    [SerializeField] string path;

//    //https://stackoverflow.com/questions/756055/listing-all-permutations-of-a-string-integer
//    private static void Swap(ref char a, ref char b)
//    {
//        if (a == b) return;

//        var temp = a;
//        a = b;
//        b = temp;
//    }


//    static List<string> validPermutations;

//    public static void GetPermutations(char[] list)
//    {
//        int x = list.Length - 1;
//        GetPermutations(list, 0, x);
//    }

//    private static void GetPermutations(char[] list, int recursionDepth, int maxDepth)
//    {
//        if (recursionDepth == maxDepth)
//        {
//            string s = new string(list);
//            if (WordManager.wordList.Contains(s))
//            {
//                validPermutations.Add(s);
//            }
//        }
//        else
//            for (int i = recursionDepth; i <= maxDepth; i++)
//            {
//                Swap(ref list[recursionDepth], ref list[i]);
//                GetPermutations(list, recursionDepth + 1, maxDepth);
//                Swap(ref list[recursionDepth], ref list[i]);
//            }
//    }

//    //Dictionary<string, int> wordsAndAnagramCount;
//    List<List<string>> wordLists;

//    void Main()
//    {
//        wordLists = new List<List<string>>();
//        for (int i = 0; i < 25; i++)
//        {
//            wordLists.Add(new List<string>());
//        }

//        foreach (var wordleWord in WordManager.wordleAnswerList)
//        {
//            validPermutations = new List<string>();
//            GetPermutations(wordleWord.ToCharArray());

//            int anagramCount = validPermutations.Count;
//            Debug.Log(anagramCount);
//            wordLists[anagramCount].Add(wordleWord);
//        }

//        int j = -1;
//        foreach (var wordList in wordLists)
//        {
//            j++;
//            if (wordList.Count <= 0)
//            {
//                continue;
//            }

//            string combinedString = string.Join("\n", wordList);
//            File.WriteAllText(path + j + ".txt", combinedString);
//        }
//    }

//    private void Start()
//    {
//        Main();
//    }
//}
