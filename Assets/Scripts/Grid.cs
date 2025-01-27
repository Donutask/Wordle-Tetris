using UnityEngine;

/// <summary>
/// The x and y pos of the transform of each letter corresponds to the i and j index in the 2 dimensional array
/// </summary>
namespace Donutask.Wordfall
{
    public class Grid : MonoBehaviour
    {
        public const int width = 5, height = 7;

        public static Letter[,] letters;
        public static int letterCount { get; private set; }
        public static int totalLetterCount { get; private set; }


        public static void ResetGrid()
        {
            letters = new Letter[width, height];
            letterCount = 0;
            totalLetterCount = 0;
        }

        public static void AssignLetter(Letter l)
        {
            Vector2Int pos = RoundPosition(l.transform);
            letters[pos.x, pos.y] = l;

            letterCount++;
            totalLetterCount++;
        }

        public static void UnassignLetter(Letter l)
        {
            Vector2Int pos = RoundPosition(l.transform);
            UnassignLetter(pos);
        }
        public static void UnassignLetter(Vector2Int pos)
        {
            letters[pos.x, pos.y] = null;
            letterCount--;
        }
        public static void MoveLetter(Letter l, Vector2Int toDir)
        {
            Vector2Int pos = RoundPosition(l.transform);
            letters[pos.x, pos.y] = null;

            Vector2Int toPos = pos + toDir;
            letters[toPos.x, toPos.y] = l;
        }

        public static bool DoesLetterExistInDirection(Letter letter, Vector2Int dir)
        {
            Vector2Int pos = RoundPosition(letter.transform) + dir;
            return DoesLetterExistAt(pos);
        }

        /// <summary>
        /// False for out of bounds
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static bool DoesLetterExistAt(Vector2Int pos)
        {
            if (pos.x < 0 || pos.x >= width)
            {
                return false;
            }
            if (pos.y < 0 || pos.y >= height)
            {
                return false;
            }
            return letters[pos.x, pos.y] != null;
        }

        public static bool TryGetLetter(Vector2Int pos, out Letter letter)
        {
            letter = letters[pos.x, pos.y];
            return letter != null;
        }

        public static Vector2Int RoundPosition(Transform t)
        {
            return new(Mathf.RoundToInt(t.position.x), Mathf.RoundToInt(t.position.y));
        }
    }
}