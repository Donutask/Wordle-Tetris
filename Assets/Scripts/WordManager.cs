using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Linq;
using UnityEngine.Events;
using UnityEngine.AddressableAssets;
using System.Threading.Tasks;
using UnityEngine.ResourceManagement.AsyncOperations;

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
        public static readonly char[] alphabet = "abcdefghijklmnopqrstuvwxyz⌫?".ToCharArray();
        public const char bomb = '⌫';
        public const char blank = '?';

        /// <summary>
        /// Array of all accepted words
        /// </summary>
        public static string[] wordList;
        public static string[][] solutionLists;


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
    };

        //For inspector
        [SerializeField] string[] solutionFileKeys;
        [SerializeField] string remainingWordsKey;

        [SerializeField] Sprite[] letterSprites;
        public static UnityEvent onWordListsLoaded = new();

        static WordManager Instance;

        private void Awake()
        {
            Instance = this;
            usedWords = new List<string>();
        }

        private async void Start()
        {
            if (wordList == null)
                await LoadWordLists();
        }

        /// <summary>
        /// Loads files into arrays. Now async!
        /// </summary>
        private async Task LoadWordLists()
        {
            List<string> wordListList = new List<string>();

            solutionLists = new string[solutionFileKeys.Length][];
            for (int i = 0; i < solutionFileKeys.Length; i++)
            {
                solutionLists[i] = await LoadTextAssetAsync(solutionFileKeys[i]);
                wordListList.AddRange(solutionLists[i]);
            }

            string[] missingWords = await LoadTextAssetAsync(remainingWordsKey);
            wordListList.AddRange(missingWords);

            wordList = wordListList.ToArray();


            onWordListsLoaded.Invoke();
        }

        //Splits the lines too
        private async Task<string[]> LoadTextAssetAsync(string key)
        {
            AsyncOperationHandle<TextAsset> handle = Addressables.LoadAssetAsync<TextAsset>(key);
            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                return handle.Result.text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None); ;
            }
            else
            {
                Debug.LogError("Failed to load text asset");
                return null;
            }
        }

        public static char RandomLetter()
        {
            return alphabet[UnityEngine.Random.Range(0, alphabet.Length)];
        }

        static List<string> usedWords;
        /// <summary>
        /// Random word that is in a specified solution list
        /// 0 = easiest
        /// 5 = hardest
        /// </summary>
        /// <returns></returns>
        public static string RandomWord(int difficulty)
        {
            var list = solutionLists[difficulty];

            string word = null;
            int rolls = 0;
            //Choose a random word, preferring one that hasn't been chosen already.
            while (word == null || usedWords.Contains(word))
            {
                word = list[UnityEngine.Random.Range(0, list.Length)];
                word = word.Trim();

                //If given a couple chances to choose new one, give up
                rolls++;
                if (rolls > 3)
                {
                    return word;
                }
            }

            //Add to list (if not already in it) to prevent duplicates
            usedWords.Add(word);
            return word;
        }

        public static bool IsLetterSpecial(char letter)
        {
            return letter == bomb || letter == blank;
        }

        public static bool IsVowel(char letter)
        {
            return letter == 'a' || letter == 'e' || letter == 'i' || letter == 'o' || letter == 'u';
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