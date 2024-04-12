
using UnityEngine;

namespace Yamadev.YamaStream
{
    public partial class Controller : Listener
    {
        [SerializeField] bool _mute;
        [SerializeField, Range(0f, 1f)] float _volume;
        [SerializeField] AudioSource[] _audioSources;

        void initilizeAudio()
        {
            foreach (AudioSource audioSource in _audioSources)
            {
                audioSource.volume = _volume;
                audioSource.mute = _mute;
            }
        }

        public float Volume
        {
            get { return _volume; }
            set
            {
                _volume = value;
                foreach (AudioSource audioSource in _audioSources)
                    audioSource.volume = value;
                SendCustomVideoEvent(nameof(Listener.OnVolumeChanged));
            }
        }

        public bool Mute
        {
            get { return _mute; }
            set
            {
                _mute = value;
                foreach (AudioSource audioSource in _audioSources)
                    audioSource.mute = value;
                SendCustomVideoEvent(nameof(Listener.OnMuteChanged));
            }
        }

        public AudioSource[] AudioSources
        {
            get => _audioSources;
            set
            {
                _audioSources = value;
                initilizeAudio();
            }
        }
    }
}