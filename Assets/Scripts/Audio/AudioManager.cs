// =============================================================================
// AudioManager.cs — RULE//BREAK
// Singleton AudioManager. Plays music and SFX via AudioSources.
// AudioClips are assigned in the Inspector. If a clip is null, the call is
// silently ignored so missing audio never crashes the game.
// =============================================================================
using UnityEngine;
using RuleBreak.Save;

namespace RuleBreak.Audio
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        // ── Audio Sources ─────────────────────────────────────────────────────
        [Header("Sources")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private AudioSource ambientSource;

        // ── Music Clips ───────────────────────────────────────────────────────
        [Header("Music")]
        [SerializeField] private AudioClip mainMenuMusic;
        [SerializeField] private AudioClip level01Music;
        [SerializeField] private AudioClip level02Music;
        [SerializeField] private AudioClip level03Music;
        [SerializeField] private AudioClip level04Music;
        [SerializeField] private AudioClip finalRevealMusic;

        // ── SFX Clips ─────────────────────────────────────────────────────────
        [Header("SFX")]
        [SerializeField] private AudioClip sfxGravityShift;
        [SerializeField] private AudioClip sfxTimeFreeze;
        [SerializeField] private AudioClip sfxTimeUnfreeze;
        [SerializeField] private AudioClip sfxDimensionShift;
        [SerializeField] private AudioClip sfxCorruption;
        [SerializeField] private AudioClip sfxCheckpoint;
        [SerializeField] private AudioClip sfxDeath;
        [SerializeField] private AudioClip sfxFragment;
        [SerializeField] private AudioClip sfxUIHover;
        [SerializeField] private AudioClip sfxUIClick;
        [SerializeField] private AudioClip sfxPortal;
        [SerializeField] private AudioClip sfxLevelComplete;
        [SerializeField] private AudioClip sfxEnergyDenied;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            ApplyVolumeSettings();
        }

        // ── Volume ────────────────────────────────────────────────────────────
        public void ApplyVolumeSettings()
        {
            float master = SaveManager.GetMasterVolume();
            if (musicSource)   musicSource.volume   = SaveManager.GetMusicVolume() * master;
            if (sfxSource)     sfxSource.volume     = SaveManager.GetSFXVolume()   * master;
            if (ambientSource) ambientSource.volume  = 0.4f * master;
        }

        // ── Music ─────────────────────────────────────────────────────────────
        public void PlayMusic(AudioClip clip, bool loop = true)
        {
            if (clip == null || musicSource == null) return;
            if (musicSource.clip == clip) return;
            musicSource.clip = clip;
            musicSource.loop = loop;
            musicSource.Play();
        }

        public void PlayLevelMusic(int level)
        {
            switch (level)
            {
                case 0: PlayMusic(mainMenuMusic);   break;
                case 1: PlayMusic(level01Music);    break;
                case 2: PlayMusic(level02Music);    break;
                case 3: PlayMusic(level03Music);    break;
                case 4: PlayMusic(level04Music);    break;
                case 99: PlayMusic(finalRevealMusic); break;
            }
        }

        public void StopMusic() { if (musicSource) musicSource.Stop(); }

        // ── SFX ───────────────────────────────────────────────────────────────
        void PlaySFX(AudioClip clip)
        {
            if (clip == null || sfxSource == null) return;
            sfxSource.PlayOneShot(clip);
        }

        public void PlayGravityShift()    => PlaySFX(sfxGravityShift);
        public void PlayTimeFreeze()      => PlaySFX(sfxTimeFreeze);
        public void PlayTimeUnfreeze()    => PlaySFX(sfxTimeUnfreeze);
        public void PlayDimensionShift()  => PlaySFX(sfxDimensionShift);
        public void PlayCorruption()      => PlaySFX(sfxCorruption);
        public void PlayCheckpoint()      => PlaySFX(sfxCheckpoint);
        public void PlayDeath()           => PlaySFX(sfxDeath);
        public void PlayFragment()        => PlaySFX(sfxFragment);
        public void PlayUIHover()         => PlaySFX(sfxUIHover);
        public void PlayUIClick()         => PlaySFX(sfxUIClick);
        public void PlayPortal()          => PlaySFX(sfxPortal);
        public void PlayLevelComplete()   => PlaySFX(sfxLevelComplete);
        public void PlayEnergyDenied()    => PlaySFX(sfxEnergyDenied);
    }
}
