using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    [SerializeField] SoundGroup[] sounds;
    [SerializeField] AudioSource mainSource;

    private void Awake()
    {
        instance = this;
    }

    public void Play(string sound)
    {
        SoundGroup s = sounds.First((s) => s.name == sound);

        //random sound
        int index = 0;
        if (s.clips.Length > 1)
        {
            index = Random.Range(0, s.clips.Length);
        }

        //adjust volume and pitch
        mainSource.pitch = s.pitch * (1f + Random.Range(-s.pitchVariance / 2f, s.pitchVariance / 2f));
        //play
        mainSource.PlayOneShot(s.clips[index], s.volume);
    }
}

[System.Serializable]
public class SoundGroup
{
    public string name;
    public AudioClip[] clips;

    [Range(0f, 1f)]
    public float volume = .75f;
    [Range(0f, 1f)]
    public float volumeVariance = .1f;

    [Range(.1f, 3f)]
    public float pitch = 1f;
    [Range(0f, 1f)]
    public float pitchVariance = .1f;
}
