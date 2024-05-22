
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;

namespace Yamadev.YamaStream
{
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(VRCSpatialAudioSource))]
    [UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
    public class YamaPlayerAudioSource : UdonSharpBehaviour
    {
        [SerializeField] YamaPlayerHandle _playerHandle;
        void Start()
        {
            if (_playerHandle == null) return;
            AudioSource audioSource = GetComponent<AudioSource>();
            _playerHandle.AudioSources = _playerHandle.AudioSources.Add(audioSource);
        }
    }
}