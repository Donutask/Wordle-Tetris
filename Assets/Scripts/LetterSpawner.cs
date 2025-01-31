using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.InputSystem;

namespace Donutask.Wordfall
{
    public class LetterSpawner : MonoBehaviour
    {
        [SerializeField] GameObject letterPrefab;

        [SerializeField] Vector2 spawnPostiion;
        [SerializeField] float bombChance;
        [SerializeField] float blankChance;

        public static UnityEvent spawnTile = new();
        public static int wordsSpawned { get; private set; }
        public static List<char> lettersSpawned { get; private set; }

        static System.Random rng = new System.Random();

        //UI
        [SerializeField] Image[] nextIndicators;
        [SerializeField] Image storedIndicator;

        private void Start()
        {
            currentWordUnshuffled = null;
            currentLetter = null;
            wordsSpawned = 0;
            lettersSpawned = new();

            spawnTile = new();
            spawnTile.AddListener(Spawn);

            Grid.ResetGrid();
            ControlsManager.storeEvent.AddListener(Store);
            StartScreen.startGame.AddListener(Spawn);

        }

        //char nextSpawn;
        public static string currentWordUnshuffled { get; private set; }
        string currentWord;
        string nextWordUnshuffled;
        string nextWord;
        int letterIndex;
        int specialIn;
        char specialChar;

        static Letter currentLetter;

        void Spawn()
        {
            if (GameOver.gameOver)
            {
                return;
            }

            //setup
            if (currentWord == null)
            {
                NextWord();
            }
            char chosenLetter;
            specialIn--;

            if (specialIn == 0)
            {
                chosenLetter = specialChar;
            }
            else
            {
                chosenLetter = currentWord[letterIndex];

                //To get a special:
                //Must have 5 tiles on screen (10 ever), not have a bomb stored, not have a bomb already coming, and meet a ~10% chance.
                if (Grid.totalLetterCount > 10 && Grid.letterCount > 5 && !WordManager.IsLetterSpecial(storedLetter) && specialIn < 0)
                {
                    if (Random.value < bombChance)
                    {
                        specialIn = 3;
                        specialChar = WordManager.bomb;
                    }
                    else if (Random.value < blankChance)
                    {
                        specialIn = 3;
                        specialChar = WordManager.blank;
                    }
                }
                letterIndex++;
            }


            lettersSpawned.Add(chosenLetter);
            UpdateUpcomingIndicators();

            //Get next word if needed
            if (letterIndex >= 5)
            {
                NextWord();
            }

            //Create letter 
            GameObject obj = Instantiate(letterPrefab, spawnPostiion, Quaternion.identity);
            currentLetter = obj.GetComponent<Letter>();
            currentLetter.SetLetter(chosenLetter);
        }

        void NextWord()
        {
            wordsSpawned++;

            //Difficulty calculation. Each difficulty level has less anagrams
            int maxDifficulty = WordManager.solutionLists.GetLength(0) - 1;
            int difficulty;
            if (WordChecker.wordCount == 0)
            {
                difficulty = 0;
            }
            else
            {

                difficulty = Random.Range(0, WordChecker.wordCount);
                //WordChecker.wordCount + Mathf.RoundToInt(Random.Range(-1.1f, 0.9f));
            }

            //Clamp difficulty
            if (difficulty < 0)
            {
                difficulty = 0;
            }
            if (difficulty > maxDifficulty)
            {
                difficulty = maxDifficulty;
            }


            if (nextWordUnshuffled == null)
            {
                nextWordUnshuffled = WordManager.RandomWord(difficulty);
                nextWord = Shuffle(nextWordUnshuffled);
            }

            currentWordUnshuffled = nextWordUnshuffled;
            currentWord = nextWord;
            nextWordUnshuffled = WordManager.RandomWord(difficulty);
            nextWord = Shuffle(nextWordUnshuffled);
            letterIndex = 0;
        }

        //Show next letters
        void UpdateUpcomingIndicators()
        {
            string currentAndNextWord = currentWord + nextWord;

            for (int i = 0; i < nextIndicators.Length; i++)
            {
                Image indicator = nextIndicators[i];
                Sprite s;
                if (specialIn == i + 1)
                {
                    s = WordManager.GetLetterSprite(specialChar);
                }
                else
                {
                    s = WordManager.GetLetterSprite(currentAndNextWord[letterIndex + i]);
                }
                indicator.sprite = s;
            }
        }

        char storedLetter;
        public void Store()
        {
            //Must be playing to use lol
            if (BlankLetterChooser.choosingLetter || GameOver.gameOver || PauseManager.paused)
            {
                return;
            }

            if (currentLetter == null)
            {
                return;
            }

            char currentLetterLetter = currentLetter.letter;
            //if storing bomb, remove from up next
            if (currentLetterLetter == WordManager.bomb)
            {
                specialIn = -1;
            }

            if (storedLetter != default)
            {
                currentLetter.SetLetter(storedLetter);
            }
            else
            {
                currentLetter.SetLetter(currentWord[letterIndex]);
                letterIndex++;
                if (letterIndex >= 5)
                {
                    NextWord();
                }
                UpdateUpcomingIndicators();
            }

            if (storedLetter != currentLetterLetter)
            {
                AudioManager.instance.Play("Store");
            }
            storedLetter = currentLetterLetter;


            storedIndicator.sprite = WordManager.GetLetterSprite(storedLetter);
        }

        public static void OverwriteLetter(char l)
        {
            currentLetter.SetLetter(l);
        }

        public static string Shuffle(string str)
        {
            char[] array = str.ToCharArray();
            int n = array.Length;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                var value = array[k];
                array[k] = array[n];
                array[n] = value;
            }
            return new string(array);
        }
    }
}