using System.Collections;
using Core.Audio;
using Core.EventSystem;
using Core.ServiceLocatorDI;
using TriInspector;
using UnityEngine;

namespace GamePlay.Props
{
    public class FishingRod : MonoBehaviour
    {
        [SerializeField] private AudioEvent _heartbeatAudioEvent;
        [SerializeField, MinMaxSlider(1, 15f)] private Vector2 _timeBeforeBite;
        [SerializeField] private float _timeToCatch;

        private EventBus _eventBus;
        private AudioSystem _audioSystem;
        private AudioSource _currentAudioSource;
        private Coroutine _heartbeatCoroutine;
        private bool _isFishing;
        private bool _isFishOnHook;

        private void Start()
        {
            _audioSystem = ServiceLocator.Get<AudioSystem>();
            _eventBus = ServiceLocator.Get<EventBus>();
        }
        
        public void Release()
        {
            if (_isFishing)
            {
                StopFishing();
                return;
            }

            _heartbeatCoroutine = StartCoroutine(HeartbeatCoroutine());
        }

        public void StopFishing()
        {
            if (_isFishOnHook)
            {
                Debug.Log("YOOOO!!");
                _isFishOnHook = false;
            }
            
            if (_heartbeatCoroutine != null)
                StopCoroutine(_heartbeatCoroutine);
                
            if (_currentAudioSource != null)
            {
                _currentAudioSource.Stop();
                _currentAudioSource = null;
            }
                
            _heartbeatCoroutine = null;
            _isFishing = false;
            _eventBus.Publish(new FishingEvent(_isFishing));
        }

        private IEnumerator HeartbeatCoroutine()
        {
            _isFishing = true;
            _currentAudioSource = _audioSystem.Play(_heartbeatAudioEvent, transform.position, true);
            _eventBus.Publish(new FishingEvent(_isFishing));
            
            var time = Random.Range(_timeBeforeBite.x, _timeBeforeBite.y);
            var elapsed = 0f;
            var startVolume = _currentAudioSource.volume;

            while (elapsed < time)
            {
                elapsed += Time.deltaTime;
                _currentAudioSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / time);
                yield return null;
            }
            
            _currentAudioSource.volume = 0f;
            _currentAudioSource.Stop();
            _isFishOnHook = true;

            yield return new WaitForSeconds(_timeToCatch);
            
            if (_isFishOnHook)
                Debug.Log("Ohhh");
            
            _isFishOnHook = false;
            _isFishing = false;
        }
    }
}