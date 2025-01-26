using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Linq;

/// <summary>
/// Loads word lists and provides opt
/// </summary>
namespace Donutask.Wordfall
{
    public class WordManager : MonoBehaviour
    {
        /// <summary>
        /// All letters in english alphabet (and backspace symbol for bomb)
        /// </summary>
        public static readonly char[] alphabet = "abcdefghijklmnopqrstuvwxyz⌫".ToCharArray();
        public const char bomb = '⌫';

        /// <summary>
        /// Array of all accepted words
        /// </summary>
        public static string[] wordList;
        public static string[] wordleAnswerList;


        /// <summary>
        /// 1 point - A, E, I, O, U, L, N, S, T, R
        /// 2 points - D, G
        /// 3 points - B, C, M, P
        /// 4 points - F, H, V, W, Y
        /// 5 points - K
        /// 8 points - J, X
        /// 10 points - Q, Z
        /// </summary>
        public static Dictionary<char, int> letterValues = new Dictionary<char, int>()
    {
        { 'a', 1 },
        { 'e', 1 },
        { 'i', 1 },
        { 'o', 1 },
        { 'u', 1 },
        { 'l', 1 },
        { 'n', 1 },
        { 's', 1 },
        { 't', 1 },
        { 'r', 1 },
        { 'd', 2 },
        { 'g', 2 },
        { 'b', 3 },
        { 'c', 3 },
        { 'm', 3 },
        { 'p', 3 },
        { 'f', 4 },
        { 'h', 4 },
        { 'v', 4 },
        { 'w', 4 },
        { 'y', 4 },
        { 'k', 5 },
        { 'j', 8 },
        { 'x', 8 },
        { 'q', 10 },
        { 'z', 10 },
        { bomb, 0 },

    };

        //For inspector
        [SerializeField] TextAsset dictionaryFile;
        [SerializeField] TextAsset wordleAnswerFile;
        [SerializeField] Sprite[] letterSprites;

        static WordManager Instance;

        private void Awake()
        {
            Instance = this;
            LoadWordLists();
        }

        /// <summary>
        /// Loads files into arrays
        /// </summary>
        private void LoadWordLists()
        {
            wordList = dictionaryFile.text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            wordleAnswerList = wordleAnswerFile.text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
        }

        public static char RandomLetter()
        {
            return alphabet[UnityEngine.Random.Range(0, alphabet.Length)];
        }
        /// <summary>
        /// Random word that is in the wordle answer list (so it isn't totaly obscure)
        /// </summary>
        /// <returns></returns>
        public static string RandomWord()
        {
            string word = wordleAnswerList[UnityEngine.Random.Range(0, wordleAnswerList.Length)];
            return word;
        }

        public static bool IsValidWord(string word)
        {
            if (string.IsNullOrWhiteSpace(word))
            {
                Debug.LogError("No string given to check");
                return false;
            }
            if (word.Length != 5)
            {
                Debug.Log("Words must be 5 letters long");
                return false;
            }

            return wordList.Contains(word);
        }

        public static Sprite GetLetterSprite(char letter)
        {
            if (letter == default)
            {
                return null;
            }
            return Instance.letterSprites[Array.FindIndex(alphabet, x => x == letter)];
        }
    }
}