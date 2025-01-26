using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using UnityEngine.Events;
using KaimiraGames;
using UnityEngine.UI;
using UnityEngine.InputSystem;

namespace Donutask.Wordfall
{
    public class LetterSpawner : MonoBehaviour
    {
        [SerializeField] GameObject letterPrefab;

        [SerializeField] Vector2 spawnPostiion;
        [SerializeField] float bombChance;
        public static UnityEvent spawnTile = new();

        static System.Random rng = new System.Random();

        //UI
        [SerializeField] Image nextIndicator;
        [SerializeField] Image afterNextIndicator;
        [SerializeField] Image afterAfterNextIndicator;
        [SerializeField] Image storedIndicator;

        private void Start()
        {
            currentWordUnshuffled = null;

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
        int bombIn;
        Letter currentLetter;

        void Spawn()
        {
            //setup
            if (currentWord == null)
            {
                currentWordUnshuffled = WordManager.RandomWord();
                currentWord = Shuffle(currentWordUnshuffled);
                nextWordUnshuffled = WordManager.RandomWord();
                nextWord = Shuffle(nextWordUnshuffled);
                letterIndex = 0;
            }
            char chosenLetter;
            bombIn--;

            if (bombIn == 0)
            {
                chosenLetter = WordManager.bomb;
            }
            else
            {
                chosenLetter = currentWord[letterIndex];

                //To get a bomb:
                //Must have 10 tiles on screen, not have a bomb stored, not have a bomb already coming, and meet a ~10% chance.
                if (Grid.letterCount > 10 && storedLetter != WordManager.bomb && bombIn < 0 && Random.value < bombChance)
                {
                    bombIn = 3;
                }
                letterIndex++;
            }


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
            currentWordUnshuffled = nextWordUnshuffled;
            currentWord = nextWord;
            nextWordUnshuffled = WordManager.RandomWord();
            nextWord = Shuffle(nextWordUnshuffled);
            letterIndex = 0;
        }

        void UpdateUpcomingIndicators()
        {
            string currentAndNextWord = currentWord + nextWord;

            //Show next letter
            Sprite s1;
            if (bombIn == 1)
            {
                s1 = WordManager.GetLetterSprite(WordManager.bomb);
            }
            else
            {
                s1 = WordManager.GetLetterSprite(currentAndNextWord[letterIndex]);
            }
            nextIndicator.sprite = s1;


            Sprite s2;
            if (bombIn == 2)
            {
                s2 = WordManager.GetLetterSprite(WordManager.bomb);
            }
            else
            {
                s2 = WordManager.GetLetterSprite(currentAndNextWord[letterIndex + 1]);
            }
            afterNextIndicator.sprite = s2;

            Sprite s3;
            if (bombIn == 3)
            {
                s3 = WordManager.GetLetterSprite(WordManager.bomb);
            }
            else
            {
                s3 = WordManager.GetLetterSprite(currentAndNextWord[letterIndex + 2]);
            }
            afterAfterNextIndicator.sprite = s3;
        }


        char storedLetter;
        public void Store()
        {
            if (currentLetter == null)
            {
                return;
            }

            char currentLetterLetter = currentLetter.letter;
            //if storing bomb, remove from up next
            if (currentLetterLetter == WordManager.bomb)
            {
                bombIn = -1;
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