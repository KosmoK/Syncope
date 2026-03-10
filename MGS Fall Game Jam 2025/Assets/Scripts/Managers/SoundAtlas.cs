using System;
using System.Collections.Generic;
using UnityEngine;

public class SoundAtlas : MonoBehaviour
{
    [Serializable]
    private class Entry
    {
        [SerializeField] string name;
        [SerializeField] AudioClip clip;

        public string getName()
        {
            return name;
        }
        public AudioClip getClip()
        {
            return clip;
        }
    }

    [SerializeField] List<Entry> atlas;
    private Dictionary<string, AudioClip> entries;
    void Start()
    {
        createDict();
    }

    private void createDict()
    {
        entries = new Dictionary<string, AudioClip>();

        foreach (Entry entry in atlas)
        {
            entries.Add(entry.getName(), entry.getClip());
        }
    }
    
    public AudioClip GetClip(string clip)
    {
        if (entries[clip])
        {
            return entries[clip];
        } else
        {
            return null;
        }
    }

    public void playSound(string name, AudioSource audioSource, float volume = 1)
    {
        AudioClip clip;
        if (!entries.TryGetValue(name, out clip))
        {
            Debug.LogError($"Clip {name} not in atlas");
        }
        
        audioSource.PlayOneShot(clip, volume);
    }
}
