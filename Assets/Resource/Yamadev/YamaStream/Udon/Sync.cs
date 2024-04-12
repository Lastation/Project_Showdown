
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace Yamadev.YamaStream
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class Sync : Listener
    {
        [SerializeField] Controller _controller;
        [SerializeField] LatencyManager _latencyManager;
        [SerializeField, Range(1f, 10f)] float SyncFrequency = 5.0f;
        [SerializeField, Range(0f, 1f)] float SyncMargin = 0.3f;
        [UdonSynced] float _syncTime = 0f;
        [UdonSynced] int _serverTimeMilliseconds = 0;
        [UdonSynced, FieldChangeCallback(nameof(KaraokeMode))] KaraokeMode _karaokeMode = KaraokeMode.None;
        [UdonSynced, FieldChangeCallback(nameof(KaraokeMembers))] string[] _karaokeMembers = new string[0];
        float _lastSync = 0f;
        float _localDelay = 0f;
        float _networkDelay = 0f;
        float _defaultKaraokeDelay = 0.5f;

        void Start() => _controller.AddListener(this);
        void Update() 
        { 
            if (_controller.IsPlaying && Time.time - SyncFrequency > _lastSync) DoSync(); 
        }

        public float LocalDelay
        {
            get => _localDelay;
            set
            {
                _localDelay = value;
                DoSync(true);
                _controller.SendCustomVideoEvent(nameof(Listener.OnLocalDelayChanged));
            }
        }

        public bool IsKaraokeMember => Networking.LocalPlayer != null ? Array.IndexOf(_karaokeMembers, Networking.LocalPlayer.displayName) >= 0 : false;

        public KaraokeMode KaraokeMode
        {
            get => _karaokeMode;
            set
            {
                _karaokeMode = value;
                if (value == KaraokeMode.None) KaraokeMembers = new string[0];
                _controller.SendCustomVideoEvent(nameof(Listener.OnKaraokeModeChanged));
            }
        }

        public string[] KaraokeMembers
        {
            get => _karaokeMembers;
            set
            {
                if (IsKaraokeMember ^ Array.IndexOf(value, Networking.LocalPlayer.displayName) >= 0)
                {
                    _karaokeMembers = value;
                    ForceSync();
                } else _karaokeMembers = value;
                _controller.SendCustomVideoEvent(nameof(Listener.OnKaraokeMemberChanged));
            }
        }

        public float NetworkDelay => _latencyManager != null ? _networkDelay : 0.1f;
        public float KaraokeDelay => _karaokeMode == KaraokeMode.None ? 0f :
                !IsKaraokeMember ? -NetworkDelay :
                NetworkDelay == 0f ? _defaultKaraokeDelay :
                _karaokeMode == KaraokeMode.Karaoke ? _defaultKaraokeDelay + NetworkDelay * 2 :
                _karaokeMode == KaraokeMode.Dance ? NetworkDelay : 0f;

        public float VideoStandardDelay => KaraokeDelay + _localDelay;

        public float SyncTime
        {
            get => _syncTime;
            set
            {
                _syncTime = Mathf.Clamp(value, 0f, _controller.Duration);
                _serverTimeMilliseconds = Networking.GetServerTimeInMilliseconds();
            }
        }
        public void ClearSync()
        {
            _syncTime = 0f;
            _serverTimeMilliseconds = 0;
        }
        public override void OnDeserialization() => DoSync(true);

        public float NetworkOffset => _controller.Paused ? 0 : (Networking.GetServerTimeInMilliseconds() - _serverTimeMilliseconds) / 1000f * _controller.Speed;
        public void ForceSync() => DoSync(true);
        public void DoSync(bool force = false)
        {
            if (_controller.IsLive || _serverTimeMilliseconds == 0 || _controller.IsLive || _controller.Stopped) return;
            float targetTime = Mathf.Clamp(_syncTime + NetworkOffset + VideoStandardDelay, 0f, _controller.Duration);
            float timeMargin = Mathf.Abs(_controller.VideoTime - targetTime);
            if (force || timeMargin >= SyncMargin) _controller.VideoPlayerHandle.Time = targetTime;
            _lastSync = Time.time;
        }

        #region Video Event
        public override void OnPlayerChanged()
        {
            if (_controller.IsOwner && !_controller.IsLocal) _controller.RequestSerialization();
        }
        public override void OnUrlChanged()
        {
            if (_controller.IsOwner && !_controller.IsLocal) _controller.RequestSerialization();
        }
        public override void OnVideoStart()
        {
            if (_controller.IsOwner && !_controller.IsReload && !_controller.IsLocal)
            {
                SyncTime = 0f;
                RequestSerialization();
            }
            DoSync();
            if (KaraokeMode != KaraokeMode.None) SendCustomEventDelayedSeconds(nameof(ForceSync), 1f);
        }
        public override void OnVideoPlay()
        {
            if (KaraokeMode != KaraokeMode.None) SendCustomEventDelayedSeconds(nameof(ForceSync), 1f);
            if (_controller.IsOwner && !_controller.IsLocal)
            {
                _controller.RequestSerialization();
                SyncTime = _controller.VideoTime - VideoStandardDelay;
                this.SyncVariables();
            }
        }
        public override void OnVideoPause()
        {
            if (_controller.IsOwner && !_controller.IsLocal)
            {
                _controller.RequestSerialization();
                SyncTime = _controller.VideoTime - VideoStandardDelay;
                this.SyncVariables();
            }
        }
        public override void OnSetTime(float time)
        {
            if (_controller.IsOwner && !_controller.IsLocal)
            {
                SyncTime = time - VideoStandardDelay;
                this.SyncVariables();
            }
        }
        public override void OnVideoStop()
        {
            if (_controller.IsOwner && !_controller.IsReload && !_controller.IsLocal)
            {
                _controller.RequestSerialization();
                ClearSync();
                this.SyncVariables();
            }
        }
        public override void OnVideoLoop()
        {
            if (_controller.IsOwner && !_controller.IsReload && !_controller.IsLocal)
            {
                _controller.RequestSerialization();
                SyncTime = 0f;
                this.SyncVariables();
            }
        }
        public override void OnSpeedChanged()
        {
            if (_controller.IsOwner && !_controller.IsLocal)
            {
                _controller.RequestSerialization();
                SyncTime = _controller.VideoTime - VideoStandardDelay;
                this.SyncVariables();
            }
        }
        public override void OnLoopChanged()
        {
            if (_controller.IsOwner && !_controller.IsLocal)
            {
                _controller.RequestSerialization();
            }
        }
        public override void OnRepeatChanged()
        {
            if (_controller.IsOwner && !_controller.IsLocal)
            {
                _controller.RequestSerialization();
            }
        }
        public override void OnLatencyUpdated()
        {
            if (_latencyManager == null) return;
            _networkDelay = Mathf.Clamp(_latencyManager.GetServerDelayseconds(), 0, 1);
        }
        public override void OnKaraokeModeChanged()
        {
            if (_karaokeMode != KaraokeMode.None && _latencyManager != null) _latencyManager.RequestRecord();
            if (!_controller.IsLocal)
            {
                if (_controller.IsOwner) this.SyncVariables();
                DoSync(true);
            }
        }
        public override void OnKaraokeMemberChanged()
        {
            if (!_controller.IsLocal && _controller.IsOwner) this.SyncVariables();
        }
        public override void OnQueueUpdated()
        {
            if (_controller.IsOwner && !_controller.IsLocal) _controller.Queue.SyncVariables();
        }
        public override void OnHistoryUpdated()
        {
            if (_controller.IsOwner && !_controller.IsLocal) _controller.History.SyncVariables();
        }
        #endregion
    }
}