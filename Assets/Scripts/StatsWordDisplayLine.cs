using UnityEngine;
using UnityEngine.UI;

namespace Donutask.Wordfall
{
    public class StatsWordDisplayLine : MonoBehaviour
    {
        [SerializeField] Image[] images;

        public void AssignWord(string word)
        {
            if (word.Length != 5)
            {
                Debug.LogError("Word must be 5 letters!");
            }

            for (int i = 0; i < word.Length; i++)
            {
                images[i].sprite = WordManager.GetLetterSprite(word[i]);
            }
        }
    }
}
