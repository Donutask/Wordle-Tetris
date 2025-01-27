using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

namespace Donutask.Wordfall
{
    public class WordChecker : MonoBehaviour
    {
        public static int score { get; private set; }
        public static List<string> wordsCreated { get; private set; }
        public static int wordCount
        {
            get
            {
                return wordsCreated.Count;
            }
        }
        const int scorePerLine = 50;
        [SerializeField] TextMeshProUGUI scoreText;
        [SerializeField] ParticleSystem confetti;
        ParticleSystem.TextureSheetAnimationModule conffettiLetters;

        static WordChecker Instance;
        private void Awake()
        {
            Instance = this;
            score = 0;
            UpdateScore();

            wordsCreated = new List<string>();

            conffettiLetters = confetti.textureSheetAnimation;
        }

        /// <summary>
        /// Assigns to grid, handles bombs, gives scores, initiates word checking
        /// </summary>
        /// <param name="l"></param>
        public static void LockInLetter(Letter l, bool allowScore = true)
        {
            //Bombs don't persist, rather they blow up their row
            if (l.letter == WordManager.bomb)
            {
                Instance.ClearColumn(Mathf.RoundToInt(l.transform.position.x));
                return;
            }

            //Shuffles well shuffle the row and then disappear
            if (l.letter == WordManager.shuffle)
            {
                Instance.ShuffleRow(Mathf.RoundToInt(l.transform.position.y));
                return;
            }

            Grid.AssignLetter(l);

            //Give score (special letters don't give any score)
            if (allowScore && WordManager.letterValues.TryGetValue(l.letter, out int scoreValue))
            {
                score += scoreValue;
                Instance.UpdateScore();
            }

            //Check if words are made when enough letters are down
            if (Grid.letterCount >= 5)
            {
                Instance.CheckForLines();
            }
        }


        void CheckForLines()
        {
            for (int y = 0; y < Grid.height; y++)
            {
                if (CheckRow(y, out string word))
                {
                    wordsCreated.Add(word);
                    ClearRow(y);
                    ClearConfetti(y, word);
                }
            }

        }

        bool CheckRow(int y, out string word)
        {
            word = "";

            for (int x = 0; x < Grid.width; x++)
            {
                Vector2Int checkPos = new Vector2Int(x, y);
                if (Grid.TryGetLetter(checkPos, out Letter letter))
                {
                    word += letter.letter;
                }
                else
                {
                    return false;
                }
            }


            return WordManager.IsValidWord(word);
        }

        /// <summary>
        /// THIS DOES NOT WORK AT ALL
        /// sooo tired
        /// </summary>
        /// <param name="y"></param>
        void ShuffleRow(int y)
        {
            ////Get all positions in row
            //List<Vector2Int> positions = new List<Vector2Int>();
            //for (int x = 0; x < Grid.width; x++)
            //{
            //    positions.Add(new Vector2Int(x, y));
            //}

            ////Shuffle
            //int n = positions.Count;
            //while (n > 1)
            //{
            //    n--;
            //    int k = rng.Next(n + 1);
            //    var value = positions[k];
            //    positions[k] = positions[n];
            //    positions[n] = value;
            //}


            //NEW ALGORITHM
            //Get letters in the line
            string currentLine = "";
            for (int x = 0; x < Grid.width; x++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                if (Grid.TryGetLetter(pos, out Letter letter))
                {
                    currentLine += letter.letter;
                }
            }

            string target = ShuffleToTryToMakeWord(currentLine);

            //move letter to correspond to letter position in shuffled string
            Dictionary<Vector2Int, Letter> letters = new();
            Vector2Int[] newPositions = new Vector2Int[Grid.width];

            for (int x = 0; x < Grid.width; x++)
            {
                Vector2Int pos = new Vector2Int(x, y);

                if (Grid.TryGetLetter(pos, out Letter letter))
                {
                    for (int j = 0; j < target.Length; j++)
                    {
                        if (letter.letter == target[j] && !letters.ContainsKey(pos))
                        {
                            newPositions[x] = new Vector2Int(j, y);
                            letters.Add(pos, letter);
                        }
                    }
                }
            }

            //need to move all objects in order so they don't over write 
            //apply movement afterwards to not overwrite (this might still do that lol)
            for (int x = 0; x < Grid.width; x++)
            {
                Vector2Int key = new Vector2Int(x, y);
                Vector2Int newPos = newPositions[x];
                if (Grid.TryGetLetter(key, out Letter letter))
                {
                    Grid.MoveLetterTo(letter, newPos);
                    letter.MoveTo(newPos);
                }
            }
        }

        string ShuffleToTryToMakeWord(string input)
        {
            int rolls = 0;
            while (rolls < 100)
            {
                rolls++;

                //100 attempts to try to make a word
                for (int i = 0; i < 25; i++)
                {
                    string shuffled = LetterSpawner.Shuffle(input);
                    string attemptedWord = shuffled + WordManager.alphabet[i];
                    if (WordManager.IsValidWord(attemptedWord) && shuffled != input)
                    {
                        Debug.Log(attemptedWord + " would be valid");
                        //remove last char that was added 
                        return attemptedWord.Remove(attemptedWord.Length - 1);
                    }
                }
            }
            return input;
        }

        void ClearRow(int y)
        {
            for (int x = 0; x < Grid.width; x++)
            {
                Vector2Int checkPos = new Vector2Int(x, y);

                if (Grid.TryGetLetter(checkPos, out Letter letter))
                {
                    letter.Clear();
                }

                //Make things above fall down
                for (int i = 1; i < Grid.height - y; i++)
                {
                    Vector2Int above = checkPos + (Vector2Int.up * i);
                    if (Grid.TryGetLetter(above, out Letter blockAbove))
                    {
                        blockAbove.Fall();
                    }
                }
            }

            AudioManager.instance.Play("Line Clear");

            score += scorePerLine; UpdateScore();
        }

        void ClearConfetti(int y, string word)
        {
            //Show confetti centred on row
            confetti.transform.position = new Vector3(Grid.width / 2, y);
            for (int i = 0; i < word.Length; i++)
            {
                conffettiLetters.SetSprite(i, WordManager.GetLetterSprite(word[i]));
            }
            confetti.Play();

        }
        /// <summary>
        /// delete all tiles in column
        /// </summary>
        /// <param name="xPos"></param>
        void ClearColumn(int xPos)
        {
            AudioManager.instance.Play("Bomb");

            for (int y = 0; y < Grid.height; y++)
            {
                Vector2Int checkPos = new Vector2Int(xPos, y);

                if (Grid.TryGetLetter(checkPos, out Letter letterBlock))
                {
                    letterBlock.Clear();
                }
                else
                {
                    return;
                }
            }
        }

        /// <summary>
        /// Show score text
        /// </summary>
        void UpdateScore()
        {
            scoreText.text = "Score: " + score;
        }
    }
}