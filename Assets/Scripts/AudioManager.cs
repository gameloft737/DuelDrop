using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public AudioMixerGroup mixerGroup;
    public Sound[] sounds;
    private void Awake()
    {
        if(instance == null){
            instance = this;
        }
        else{
            Destroy(gameObject);
            return;
        }
        
        foreach(Sound sound in sounds){
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.audioClip;

            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.outputAudioMixerGroup = mixerGroup;
        }
        
    }
    public void Play(String name){
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if(s == null){return;}
        s.source.Play();
    }
}
