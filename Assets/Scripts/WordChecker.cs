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

        const int scorePerLine = 50;
        [SerializeField] TextMeshProUGUI scoreText;
        [SerializeField] ParticleSystem confetti;

        static WordChecker Instance;
        private void Awake()
        {
            Instance = this;
            score = 0;
            UpdateScore();

            wordsCreated = new List<string>();
        }

        /// <summary>
        /// Assigns to grid, handles bombs, gives scores, initiates word checking
        /// </summary>
        /// <param name="l"></param>
        public static void LockInLetter(Letter l)
        {
            //Bombs don't persist, rather they blow up their row
            if (l.letter == WordManager.bomb)
            {
                Instance.ClearColumn(Mathf.RoundToInt(l.transform.position.x));
                return;
            }

            Grid.AssignLetter(l);

            //Give score (special letters don't give any score)
            if (WordManager.letterValues.TryGetValue(l.letter, out int scoreValue))
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

            //Show confetti centred on row
            confetti.transform.position = new Vector3(Grid.width / 2, y);
            confetti.Play();

            AudioManager.instance.Play("Line Clear");

            score += scorePerLine; UpdateScore();
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