using UnityEngine;

public static class Easing
{
    static float InSine(float t) => 1 - (float)Mathf.Cos(t * Mathf.PI / 2);
    static float OutSine(float t) => (float)Mathf.Sin(t * Mathf.PI / 2);


    public static float EaseIn(float from, float to, float time, float duration)
    {
        return from + (InSine(time / duration) * (to - from));
    }

    public static float EaseOut(float from, float to, float time, float duration)
    {
        return from + (OutSine(time / duration) * (to - from));
    }
}
