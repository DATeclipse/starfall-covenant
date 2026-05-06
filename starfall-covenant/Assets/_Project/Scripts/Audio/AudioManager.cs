using UnityEngine;
using System.Collections.Generic;

namespace StarfallCovenant.Audio
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [SerializeField] private AudioSource bgmSource;
        [SerializeField] private AudioSource sfxSource;

        [Header("BGM Clips")]
        [SerializeField] private AudioClip mainMenuBGM;
        [SerializeField] private AudioClip worldMapBGM;
        [SerializeField] private AudioClip battleBGM;
        [SerializeField] private AudioClip bossBGM;
        [SerializeField] private AudioClip gachaBGM;

        [Header("SFX Clips")]
        [SerializeField] private AudioClip attackSFX;
        [SerializeField] private AudioClip hitSFX;
        [SerializeField] private AudioClip dodgeSFX;
        [SerializeField] private AudioClip abilitySFX;
        [SerializeField] private AudioClip victorySFX;
        [SerializeField] private AudioClip defeatSFX;
        [SerializeField] private AudioClip gachaPullSFX;
        [SerializeField] private AudioClip gachaRevealSFX;
        [SerializeField] private AudioClip buttonClickSFX;

        private float bgmVolume = 1f;
        private float sfxVolume = 1f;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void PlayBGM(string trackName)
        {
            AudioClip clip = trackName switch
            {
                "MainMenu" => mainMenuBGM,
                "WorldMap" => worldMapBGM,
                "Battle" => battleBGM,
                "Boss" => bossBGM,
                "Gacha" => gachaBGM,
                _ => null
            };

            if (clip == null || bgmSource.clip == clip) return;

            bgmSource.clip = clip;
            bgmSource.loop = true;
            bgmSource.Play();
        }

        public void StopBGM()
        {
            bgmSource.Stop();
        }

        public void PlaySFX(string sfxName)
        {
            AudioClip clip = sfxName switch
            {
                "Attack" => attackSFX,
                "Hit" => hitSFX,
                "Dodge" => dodgeSFX,
                "Ability" => abilitySFX,
                "Victory" => victorySFX,
                "Defeat" => defeatSFX,
                "GachaPull" => gachaPullSFX,
                "GachaReveal" => gachaRevealSFX,
                "ButtonClick" => buttonClickSFX,
                _ => null
            };

            if (clip != null)
            {
                sfxSource.PlayOneShot(clip, sfxVolume);
            }
        }

        public void SetBGMVolume(float volume)
        {
            bgmVolume = volume;
            bgmSource.volume = volume;
        }

        public void SetSFXVolume(float volume)
        {
            sfxVolume = volume;
        }
    }
}
