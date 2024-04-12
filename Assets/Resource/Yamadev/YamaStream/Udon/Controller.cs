
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components.Video;
using VRC.SDKBase;

namespace Yamadev.YamaStream
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public partial class Controller : Listener
    {
        [SerializeField] Animator _videoPlayerAnimator;
        [SerializeField] VideoPlayerHandle[] _videoPlayerHandles;
        [SerializeField] float _retryAfterSeconds = 5;
        [SerializeField] int _maxErrorRetry = 3;
        [SerializeField] string _timeFormat = @"hh\:mm\:ss";
        [SerializeField, UdonSynced, FieldChangeCallback(nameof(VideoPlayerType))] VideoPlayerType _videoPlayerType;
        [SerializeField, UdonSynced, FieldChangeCallback(nameof(Loop))] bool _loop = false;
        [UdonSynced, FieldChangeCallback(nameof(Paused))] bool _paused = false;
        [UdonSynced, FieldChangeCallback(nameof(Stopped))] bool _stopped = true;
        [UdonSynced, FieldChangeCallback(nameof(Speed))] float _speed = 1f;
        [UdonSynced, FieldChangeCallback(nameof(Repeat))] Vector3 _repeat = new Vector3(0f, 0f, 999999f);
        Listener[] _listeners = { };
        bool _isLocal = false;
        int _errorRetryCount = 0;
        bool _isReload = false;
        float _lastSetTime = 0f;
        float _setTimeCooling = 0.6f;
        bool _initilized = false;

        void Start() => initilize();
        void Update()
        {
            checkRepeat();
            renderScreen();
        }

        void initilize()
        {
            if (_initilized) return;
            initilizeTrack();
            initilizeAudio();
            initilizeScreen();
            Loop = _loop;
            _videoPlayerAnimator.Rebind();
            foreach (VideoPlayerHandle handle in _videoPlayerHandles)
                handle.Listener = this;
            _initilized = true;
        }

        public void AddListener(Listener listener)
        {
            if (Array.IndexOf(_listeners, listener) >= 0) return;
            _listeners = _listeners.Add(listener);
        }

        public bool IsLocal => _isLocal;
        public VideoPlayerType VideoPlayerType
        {
            get => _videoPlayerType;
            set 
            {
                if (_videoPlayerType == value) return;
                VideoPlayerHandle.Stop();
                _videoPlayerType = value;
                updateProperties();
                if (value != VideoPlayerType.UnityVideoPlayer) Speed = 1f;
                foreach (Listener listener in _listeners) listener.OnPlayerChanged();
            }
        }
        public VideoPlayerHandle VideoPlayerHandle
        {
            get
            {
                foreach (VideoPlayerHandle handle in _videoPlayerHandles) 
                    if (handle.VideoPlayerType == _videoPlayerType) return handle;
                return null;
            }
        }

        public bool Paused
        {
            get => _paused;
            set
            {
                _paused = value;
                if (_paused) VideoPlayerHandle.Pause();
                else VideoPlayerHandle.Play();
            }
        }
        public bool Stopped
        {
            get => _stopped;
            set
            {
                _stopped = value;
                if (value) VideoPlayerHandle.Stop();
            }
        }
        public bool Loop
        {
            get => _loop;
            set
            {
                _loop = value;
                foreach (VideoPlayerHandle handle in _videoPlayerHandles)
                    handle.Loop = value;
                foreach (Listener listener in _listeners) listener.OnLoopChanged();
            }
        }
        public float Speed
        {
            get => _speed;
            set
            {
                _speed = VideoPlayerType == VideoPlayerType.UnityVideoPlayer ? value : 1f;
                if (!_stopped) _videoPlayerAnimator.SetFloat("Speed", value);
                foreach (Listener listener in _listeners) listener.OnSpeedChanged();
            }
        }
        void checkRepeat()
        {
            if (!IsPlaying || !_repeatStatus.IsOn()) return;
            if (VideoTime > _repeatStatus.GetEndTime() || VideoTime < _repeatStatus.GetStartTime()) SetTime(_repeatStatus.GetStartTime());
        }
        public Vector3 Repeat
        {
            get => _repeat;
            set
            {
                _repeat = value;
                foreach (Listener listener in _listeners) listener.OnRepeatChanged();
            }
        }
        public bool IsPlaying => VideoPlayerHandle.IsPlaying;
        public float Duration => VideoPlayerHandle.Duration;
        public float VideoTime => VideoPlayerHandle.Time;
        public bool IsLoading => VideoPlayerHandle.IsLoading;
        public bool IsReload => _isReload;
        public bool IsLive => float.IsInfinity(Duration);
        public bool IsOwner => Networking.IsOwner(gameObject);
        RepeatStatus _repeatStatus => _repeat.ToRepeatStatus();

        public void Reload() => PlayTrack(Track, true);

        public void ErrorRetry()
        {
            if (IsPlaying) return;
            _resolveTrack.Invoke();
            foreach (Listener listener in _listeners) listener.OnVideoRetry();
        }

        public void SetTime(float time)
        {
            if (IsLive || Time.time - _lastSetTime < _setTimeCooling) return;
            if (_repeatStatus.IsOn() && (time < _repeatStatus.GetStartTime() || time > _repeatStatus.GetEndTime())) return;
            VideoPlayerHandle.Time = time;
            _lastSetTime = Time.time;
            foreach (Listener listener in _listeners) listener.OnSetTime(time);
        }

        public void SendCustomVideoEvent(string eventName)
        {
            foreach (Listener listener in _listeners) listener.SendCustomEvent(eventName);
        }

        #region Video Event
        public override void OnVideoReady()
        {
            _videoPlayerAnimator.SetFloat("Speed", _speed);
            foreach (Listener listener in _listeners) listener.OnVideoReady();
        }
        public override void OnVideoStart() 
        {
            Paused = _paused;
            _errorRetryCount = 0;
            _stopped = false;
            foreach (Listener listener in _listeners) listener.OnVideoStart();
            _isReload = false;
        }
        public override void OnVideoPlay()
        {
            _paused = false;
            foreach (Listener listener in _listeners) listener.OnVideoPlay();
        }
        public override void OnVideoPause()
        {
            _paused = true;
            foreach (Listener listener in _listeners) listener.OnVideoPause();
        }
        public override void OnVideoStop()
        {
            if (!_isReload)
            {
                _paused = false;
                _stopped = true;
                Repeat = new Vector3(0f, 0f, 999999f);
                if (!string.IsNullOrEmpty(Track.GetUrl())) _history.AddTrack(Track);
                Track = Track.New(_videoPlayerType, string.Empty, VRCUrl.Empty);
            }
            foreach (Listener listener in _listeners) listener.OnVideoStop();
        }
        public override void OnVideoLoop()
        {
            foreach (Listener listener in _listeners) listener.OnVideoLoop();
        }
        public override void OnVideoEnd()
        {
            foreach (Listener listener in _listeners) listener.OnVideoEnd();
        }
        public override void OnVideoError(VideoError videoError)
        {
            if (videoError != VideoError.InvalidURL && videoError != VideoError.AccessDenied && videoError != VideoError.PlayerError)
            {
                if (_errorRetryCount < _maxErrorRetry)
                {
                    _errorRetryCount++;
                    SendCustomEventDelayedSeconds(nameof(ErrorRetry), _retryAfterSeconds);
                } else _errorRetryCount = 0;
            }
            foreach (Listener listener in _listeners) listener.OnVideoError(videoError);
        }
        #endregion
    }
}