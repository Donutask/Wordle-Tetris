using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

namespace Donutask.Wordfall
{
    public class StatsScreen : MonoBehaviour
    {
        [SerializeField] GameObject statsScreen;

        [SerializeField] GameObject wordDisplayTemplate;
        [SerializeField] Transform wordDisplayParent;
        [SerializeField] GameObject noWordsMadeIndicator;
        [SerializeField] TextMeshProUGUI wordsMissedText, totalTilesText, lettersText, vowelsText, consonantsText, specialText, backspaceBombText, customLetterText;
        [SerializeField] Image mostCommonLetter;
        bool showingScreen;

        private void Start()
        {
            ControlsManager.pauseEvent.AddListener(CloseScreen);
        }

        private void Update()
        {
            if (GameOver.gameOver)
            {
                if (!showingScreen && ControlsManager.GetLetterMovement().x == 1)
                {
                    OpenScreen();
                }
            }
        }

        public void OpenScreen()
        {
            if (!hasCreatedScreen)
            {
                CreateScreen();
            }
            statsScreen.SetActive(true);
            showingScreen = true;
        }

        void CloseScreen()
        {
            statsScreen.SetActive(false);
            showingScreen = false;
        }

        bool hasCreatedScreen;
        void CreateScreen()
        {
            if (WordChecker.wordCount <= 0)
            {
                noWordsMadeIndicator.SetActive(true);
            }
            else
            {
                noWordsMadeIndicator.SetActive(false);

                foreach (var word in WordChecker.wordsCreated)
                {
                    GameObject obj = Instantiate(wordDisplayTemplate, wordDisplayParent);
                    obj.GetComponent<StatsWordDisplayLine>().AssignWord(word);
                }
            }

            wordsMissedText.text = "Words Missed: " + (LetterSpawner.wordsSpawned - WordChecker.wordCount);
            LetterDistributionStats();

            hasCreatedScreen = true;
        }

        void LetterDistributionStats()
        {
            int vowelCount = 0;
            int consonantCount = 0;
            int specialCount = 0;
            int blankCount = 0;
            int bombCount = 0;

            foreach (var letter in LetterSpawner.lettersSpawned)
            {
                if (WordManager.IsLetterSpecial(letter))
                {
                    specialCount++;

                    if (letter == WordManager.blank)
                    {
                        blankCount++;
                    }
                    else if (letter == WordManager.bomb)
                    {
                        bombCount++;
                    }
                }
                else
                {
                    if (WordManager.IsVowel(letter))
                    {
                        vowelCount++;
                    }
                    else
                    {
                        consonantCount++;
                    }
                }
            }

            totalTilesText.text = "Total Tiles: " + (vowelCount + consonantCount + specialCount);
            vowelsText.text = "Vowels: " + vowelCount;
            consonantsText.text = "Consonants: " + consonantCount;
            specialText.text = "Special: " + specialCount;
            backspaceBombText.text = "Backspace Bombs: " + bombCount;
            customLetterText.text = "Custom Letters: " + blankCount;


            char mostCommon = LetterSpawner.lettersSpawned.GroupBy(i => i).OrderByDescending(grp => grp.Count()).Select(grp => grp.Key).First();
            mostCommonLetter.sprite = WordManager.GetLetterSprite(mostCommon);
        }
    }
}