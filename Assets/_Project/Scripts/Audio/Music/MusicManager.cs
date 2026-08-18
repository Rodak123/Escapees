using System;
using Rodak.Utils.Singleton;
using UnityEngine;

namespace GameJam
{
    [DefaultExecutionOrder(-50)]
    public class MusicManager : SingletonMonoBehaviour<MusicManager>
    {
        private struct MusicState
        {
            public MusicSO Music;
            public float Time;

            public override string ToString()
            {
                return $"{Music} at {Time}s";
            }
        }

        private static MusicState? PersistedMusicState { get; set; }

        [SerializeField] private AudioSource musicAudioSource;

        private MusicSO currentMusic;

        public event Action<MusicSO> OnCurrentMusicEnded;
        public MusicSO CurrentMusic => currentMusic;

        private float lastAudioSourceTime;

        protected override void Awake()
        {
            if (PersistedMusicState.HasValue)
            {
                PlayMusic(PersistedMusicState.Value.Music, PersistedMusicState.Value.Time);
                PersistedMusicState = null;
            }
        }

        private void Update()
        {
            if (currentMusic == null) return;

            if (!musicAudioSource.isPlaying)
            {
                StopMusic(0);
            }
            else
            {
                lastAudioSourceTime = musicAudioSource.time;
            }
        }

        protected override void OnDestroy()
        {
            if (currentMusic == null) return;
            PersistedMusicState = new()
            {
                Music = currentMusic,
                Time = lastAudioSourceTime,
            };
        }

        public void PlayMusicOrContinue(MusicSO music, float fromTime = 0, float fadeInDuration = 0)
        {
            if (music == null || currentMusic == music) return;
            PlayMusic(music, fromTime, fadeInDuration);
        }

        public void PlayMusic(MusicSO music, float fromTime = 0, float fadeInDuration = 0)
        {
            if (music == null) return;

            currentMusic = music;

            musicAudioSource.clip = music.SongClip;
            musicAudioSource.time = Mathf.Clamp(fromTime, 0, music.SongClip.length);

            StartCoroutine(AudioUtils.FadeIn(musicAudioSource, fadeInDuration));
        }

        public void StopMusic(float fadeOutDuration)
        {
            if (currentMusic == null) return;

            StartCoroutine(AudioUtils.FadeOut(musicAudioSource, fadeOutDuration));

            MusicSO lastMusic = currentMusic;

            currentMusic = null;
            OnCurrentMusicEnded?.Invoke(lastMusic);
        }
    }
}