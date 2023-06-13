using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundMaster : MonoBehaviour
{
    private static AudioSource _source;
    private static AudioSource _musicSource;
    private static AudioListener _listener;
    public static float _volume { get; private set; }
    private static Dictionary<string, AudioClip> _sounds;
    private static List<string> _soundsPlaying;
    [SerializeField]
    private List<AudioClip> Sounds;
    
    private void Awake()
    {
        _soundsPlaying = new List<string>();
        _musicSource = GetComponent<AudioSource>();
        _source = gameObject.AddComponent<AudioSource>();
        _source.volume = 0.3f;
     
        _volume = 0.45f;
        _listener = GetComponent<AudioListener>();
        _sounds = new Dictionary<string, AudioClip>();
        foreach (var item in Sounds)
        {
            _sounds.Add(item.name, item);
        }
        Sounds.Clear();
    }

    public static void PlayOneShot(string SoundName)
    {
        PlayOneShot(SoundName, 1);
    }
    public static void PlayOneShot(string SoundName,float volumeScale)
    {
        if(_soundsPlaying.Contains(SoundName))
        {
            return;
        }

        if (!_sounds.ContainsKey(SoundName))
        {
            Debug.Log("No sound: " + SoundName);
            return;
        }

        _source.PlayOneShot(_sounds[SoundName],volumeScale);
        Utility.Invoke(FindObjectOfType<SoundMaster>(), () =>
         {
             _soundsPlaying.Remove(SoundName);
         }, 0.1f);
        _soundsPlaying.Add(SoundName);
    }
    public static void ChangeVolume(float volume)
    {
        _volume = volume;
        if (_volume == 0)
        {
            _source.volume = 0;
        }
        else
        {
            _source.volume = (-1.0f/(1.3f*_volume - 2.0f)) -0.5f;
        }
    }
}
