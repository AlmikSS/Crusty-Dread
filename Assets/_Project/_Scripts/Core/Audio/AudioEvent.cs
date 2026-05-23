using TriInspector;
using UnityEngine;

namespace Core.Audio
{
    [CreateAssetMenu(fileName = "AudioEvent", menuName = "Audio/Audio Event")]
    public class AudioEvent : ScriptableObject
    {
        [SerializeField] private AudioClip[] _clips;
        [SerializeField, MinMaxSlider(0f, 10f)] private Vector2 _pitchRange;
        [SerializeField, MinMaxSlider(0f, 1f)] private Vector2 _volumeRange;
        [SerializeField] private AudioEventType _eventType;

        public AudioClip[] Clips => _clips;
        public Vector2 PitchRange => _pitchRange;
        public Vector2 VolumeRange => _volumeRange;
        public AudioEventType EventType => _eventType;
    }

    public enum AudioEventType
    {
        Sfx,
        UI,
        Heart,
    }
}