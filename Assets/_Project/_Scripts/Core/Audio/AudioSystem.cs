using System;
using System.Collections;
using System.Collections.Generic;
using Core.ServiceLocatorDI;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

namespace Core.Audio
{
    public sealed class AudioSystem : MonoBehaviour, IService
    {
        [SerializeField] private int _audioSourcePoolAmount;
        [SerializeField] private AudioMixerGroup _sfxAudioMixerGroup;
        [SerializeField] private AudioMixerGroup _uiAudioMixerGroup;
        [SerializeField] private AudioMixerGroup _heartAudioMixerGroup;
        
        private readonly Queue<AudioSource> _audioSourcePool = new();
        
        public void Construct()
        {
            ServiceLocator.Register(this);
            //DontDestroyOnLoad(gameObject);
            
            InitializePool();
        }

        public AudioSource Play(AudioEvent audioEvent, Vector3 position = default, bool isLoop = false)
        {
            var mixerGroup = _sfxAudioMixerGroup;
            switch (audioEvent.EventType)
            {
                case AudioEventType.Sfx:
                    break;
                case AudioEventType.UI:
                    mixerGroup = _uiAudioMixerGroup;
                    break;
                case AudioEventType.Heart:
                    mixerGroup = _heartAudioMixerGroup;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var audioSource = GetFromPool(position);
            
            audioSource.clip = audioEvent.Clips[Random.Range(0, audioEvent.Clips.Length)];
            audioSource.loop = isLoop;
            audioSource.pitch = Random.Range(audioEvent.PitchRange.x, audioEvent.PitchRange.y);
            audioSource.volume = Random.Range(audioEvent.VolumeRange.x, audioEvent.VolumeRange.y);
            audioSource.outputAudioMixerGroup = mixerGroup;
            
            audioSource.Play();
            StartCoroutine(ReturnToPool(audioSource));
            return audioSource;
        }

        public void ChangeVolume(AudioEventType audioEventType, float volume)
        {
            switch (audioEventType)
            {
                case AudioEventType.Sfx:
                    _sfxAudioMixerGroup.audioMixer.SetFloat("Volume_SFX", volume);
                    break;
                case AudioEventType.UI:
                    _uiAudioMixerGroup.audioMixer.SetFloat("Volume_UI", volume);
                    break;
                case AudioEventType.Heart:
                    _heartAudioMixerGroup.audioMixer.SetFloat("Volume_Heart", volume);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public float GetVolume(AudioEventType audioEventType)
        {
            var volume = 0f;
            
            switch (audioEventType)
            {
                case AudioEventType.Sfx:
                    _sfxAudioMixerGroup.audioMixer.GetFloat("Volume_SFX", out volume);
                    break;
                case AudioEventType.UI:
                    _uiAudioMixerGroup.audioMixer.GetFloat("Volume_UI", out volume);
                    break;
                case AudioEventType.Heart:
                    _heartAudioMixerGroup.audioMixer.GetFloat("Volume_Heart", out volume);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            return volume;
        }

        private void OnDestroy()
        {
            ServiceLocator.Unregister(this);
        }

        private AudioSource GetFromPool(Vector3 position)
        {
            if (_audioSourcePool.Count <= 0)
                SpawnAudioSource();
            
            var audioSource = _audioSourcePool.Dequeue();
            audioSource.transform.localPosition = position;
            audioSource.gameObject.SetActive(true);
            return audioSource;
        }

        private IEnumerator ReturnToPool(AudioSource audioSource)
        {
            yield return new WaitWhile(() => audioSource.isPlaying);
            ResetAudioSource(audioSource);
            _audioSourcePool.Enqueue(audioSource);
        }

        private void InitializePool()
        {
            if (_audioSourcePool.Count >= _audioSourcePoolAmount)
                return;
            
            for (var i = 0; i < _audioSourcePoolAmount; i++)
            {
                SpawnAudioSource();
            }
        }

        private void SpawnAudioSource()
        {
            var newAudioSource = new GameObject("AudioSource").AddComponent<AudioSource>();
            newAudioSource.transform.SetParent(transform);
            ResetAudioSource(newAudioSource);
            _audioSourcePool.Enqueue(newAudioSource);
        }

        private void ResetAudioSource(AudioSource audioSource)
        {
            audioSource.playOnAwake = false;
            audioSource.clip = null;
            audioSource.outputAudioMixerGroup = null;
            audioSource.volume = 0;
            audioSource.pitch = 0;
            audioSource.transform.localPosition = Vector3.zero;
            audioSource.gameObject.SetActive(false);
        }
    }
}