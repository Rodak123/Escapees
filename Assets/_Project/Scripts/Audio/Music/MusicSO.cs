using UnityEngine;

namespace GameJam
{
    [CreateAssetMenu(fileName = "Music", menuName = "Music")]
    public class MusicSO : ScriptableObject
    {
        [Header("Info")]
        [SerializeField] private string songName = "Song";
        [SerializeField] private string songArtist = "Artist";

        [Header("Music")]
        [SerializeField] private AudioClip songClip;

        public string SongName => songName;
        public string SongArtist => songArtist;

        public AudioClip SongClip => songClip;

        public override string ToString()
        {
            return $"{songName} by {songArtist}";
        }
    }
}