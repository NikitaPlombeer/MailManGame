using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Utils;

namespace DwarfTrains.Sound
{
    public class GlobalSoundManager: OdinSingleton<GlobalSoundManager>
    {
        public Dictionary<string, AudioSource> audioSources = new Dictionary<string, AudioSource>();
        public bool isMuted = false;
        public List<string> bannedSounds = new List<string>();

        public void PlaySound(string soundName)
        {
            if (isMuted) return;
            if (bannedSounds.Contains(soundName)) return;

            if (audioSources.ContainsKey(soundName))
            {
                audioSources[soundName].Play();
            }
            else
            {
                Debug.LogError($"Sound {soundName} not found");
            }
        }
        
        public void StopSound(string soundName)
        {
            if (audioSources.ContainsKey(soundName))
            {
                audioSources[soundName].Stop();
            }
            else
            {
                Debug.LogError($"Sound {soundName} not found");
            }
        }
    }


}
