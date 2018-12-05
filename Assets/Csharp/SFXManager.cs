using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    [System.Serializable]
    public struct ResourceClips
    {
        public AudioClip dice;
        public AudioClip villager;
        public AudioClip food;
        public AudioClip wood;
        public AudioClip stone;
        public AudioClip shield;
    }

    public ResourceClips resources;

    public AudioClip[] villagerTasty_Male;
    public AudioClip[] villagerHungry_Male;
    public AudioClip[] throwDices;

    public AudioSource speaker;

    public void PlayRandom(AudioClip[] clips)
    {
        speaker.pitch = 1.2f;
        speaker.Stop();
        speaker.clip = clips[UnityEngine.Random.Range(0, clips.Length)];
        speaker.Play();
    }

    public void Play(GameEffect.EffectType effectType)
    {
        speaker.Stop();

        switch (effectType)
        {
            case GameEffect.EffectType.Dice:
                speaker.clip = resources.dice;
                break;
            case GameEffect.EffectType.Villager:
                speaker.clip = resources.villager;
                break;
            case GameEffect.EffectType.Wood:
                speaker.clip = resources.wood;
                break;
            case GameEffect.EffectType.Food:
                speaker.clip = resources.food;
                break;
            case GameEffect.EffectType.Stone:
                speaker.clip = resources.stone;
                break;
            case GameEffect.EffectType.Shield:
                speaker.clip = resources.shield;
                break;
        }

        speaker.Play();
    }
}
