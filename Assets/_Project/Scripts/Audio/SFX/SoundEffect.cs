using System;
using UnityEngine;

namespace GameJam
{
    [Serializable]
    public struct SoundEffect
    {
        [SerializeField] private AudioClip[] clips;
        [SerializeField] private float volumeScale;

        public AudioClip[] Clips => clips;
        public float VolumeScale => volumeScale;

        public AudioClip PickRandom()
        {
            if (clips.Length == 0) return null;
            int index = UnityEngine.Random.Range(0, clips.Length);
            return clips[index];
        }
    }
}