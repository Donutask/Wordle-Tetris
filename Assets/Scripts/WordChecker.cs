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
            GiveScore(0);

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

            Grid.AssignLetter(l);

            //Give score (special letters don't give any score)
            if (allowScore && WordManager.letterValues.TryGetValue(l.letter, out int scoreValue))
            {
                Instance.GiveScore(scoreValue);
            }

            //Check if words are made when enough letters are down
            if (Grid.letterCount >= 5)
            {
                Instance.CheckForLines();
            }
        }

        /// <summary>
        /// Check every row to see if a word has been made
        /// </summary>
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

            GiveScore(scorePerLine);
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
        void GiveScore(int points)
        {
            //If you would exceed integer limit with your score
            if (score + points < 0 && score > 0)
            {
                score = int.MaxValue;
                GameOver.Instance.EndGame();
            }
            else
            {
                if (points > 1)
                {
                    StartCoroutine(AnimateScore(score, score + points));
                }
                else
                {
                    scoreText.text = "Score: " + (score + points);
                }
                score += points;
            }

        }

        IEnumerator AnimateScore(int from, int to)
        {
            float t = 0;
            const float duration = 0.5f;

            while (t < duration)
            {
                t += Time.deltaTime;
                scoreText.text = "Score: " + Mathf.Round(from + (InSine(t / duration) * (to - from)));
                yield return null;
            }

            //make sure lol
            scoreText.text = "Score: " + score;
        }

        static float InSine(float t) => 1 - (float)Mathf.Cos(t * Mathf.PI / 2);
    }
}