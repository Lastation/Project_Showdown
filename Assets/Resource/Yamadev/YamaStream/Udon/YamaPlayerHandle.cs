
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDK3.Data;
using VRC.SDKBase;

namespace Yamadev.YamaStream
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class YamaPlayerHandle : Listener
    {
        [SerializeField] Controller _controller;
        [SerializeField] i18n _i18n;
        [SerializeField] Sync _sync;
        [SerializeField] Permission _permission;
        [SerializeField] string _version;
        [SerializeField] TextAsset _updateLog;

        void Start() => _controller.AddListener(this);
        public void AddListener(Listener listener) => _controller.AddListener(listener);
        public string Version => _version;
        public string UpdateLog
        {
            get
            {
                if (_updateLog == null) return string.Empty; 
                return _updateLog.text;
            }
        }
        public VideoPlayerType VideoPlayerType 
        { 
            get => _controller.VideoPlayerType; 
            set
            {
                _controller.SetMeOwner();
                _controller.VideoPlayerType = value;
            }
        }
        public Track Track => _controller.Track;
        public Texture Texture => _controller.Texture;
        public bool IsPlaying => _controller.IsPlaying;
        public bool IsLoading => _controller.IsLoading;
        public bool IsLive => _controller.IsLive;
        public float VideoTime => _controller.VideoTime;
        public float Duration => _controller.Duration;
        public bool Paused 
        { 
            get => _controller.Paused;
            set
            {
                _controller.SetMeOwner();
                _controller.Paused = value;
            }
        }
        public bool Stopped 
        { 
            get => _controller.Stopped; 
            set
            {
                _controller.SetMeOwner();
                _controller.Stopped = value;
            }
        }
        public bool Loop
        { 
            get => _controller.Loop; 
            set
            {
                _controller.SetMeOwner();
                _controller.Loop = value;
            }
        }
        public float Speed 
        { 
            get => _controller.Speed; 
            set
            {
                _controller.SetMeOwner();
                _controller.Speed = value;
            }
        }
        public RepeatStatus Repeat 
        { 
            get => _controller.Repeat.ToRepeatStatus(); 
            set
            {
                _controller.SetMeOwner();
                _controller.Repeat = value.ToVector3();
            }
        }
        public bool RepeatOn => _controller.Repeat.x == 1f;
        public bool ShufflePlay 
        { 
            get => _controller.ShufflePlay; 
            set => _controller.ShufflePlay = value;
        }
        public Playlist[] Playlists => _controller.Playlists;
        public Playlist ActivePlaylist => _controller.ActivePlaylist;
        public int PlayingTrackIndex => _controller.PlayingTrackIndex;
        public Playlist QueueList => _controller.Queue;
        public Playlist HistoryList => _controller.History;
        public bool Mute 
        { 
            get => _controller.Mute;  
            set => _controller.Mute = value;
        }
        public float Volume 
        { 
            get => _controller.Volume; 
            set => _controller.Volume = value; 
        }
        public AudioSource[] AudioSources
        {
            get => _controller.AudioSources;
            set => _controller.AudioSources = value;
        }
        public float LocalDelay 
        { 
            get => _sync.LocalDelay; 
            set => _sync.LocalDelay = value; 
        }
        public KaraokeMode KaraokeMode 
        { 
            get => _sync.KaraokeMode; 
            set
            {
                _controller.SetMeOwner();
                _sync.KaraokeMode = value;
            }
        }
        public string[] KaraokeMembers 
        { 
            get => _sync.KaraokeMembers; 
            set
            {
                _controller.SetMeOwner();
                _sync.KaraokeMembers = value;
            }
        }
        public bool IsKaraokeMember => _sync.IsKaraokeMember;
        public int MaxResolution 
        { 
            get => _controller.MaxResolution; 
            set => _controller.MaxResolution = value; 
        }
        public bool MirrorInverse 
        { 
            get => _controller.MirrorInverse; 
            set => _controller.MirrorInverse = value; 
        }
        public float Emission 
        { 
            get => _controller.Emission; 
            set => _controller.Emission = value; 
        }
        public Renderer[] RenderScreens 
        { 
            get => _controller.RenderScreens; 
            set => _controller.RenderScreens = value; 
        }
        public RawImage[] RawImageScreens 
        { 
            get => _controller.RawImageScreens; 
            set => _controller.RawImageScreens = value; 
        }
        public string Language 
        { 
            get => _i18n.Language; 
            set => _i18n.Language = value == null ? _i18n.GetLocalLanguage() : value;
        }
        public PlayerPermission Permission => _permission.GetPermissionByPlayerId(Networking.LocalPlayer.playerId);
        public DataDictionary PermissionData => _permission.PermissionData;

        public void SetTime(float time)
        {
            _controller.SetMeOwner();
            _controller.SetTime(time);
        }
        public void PlayUrl(VRCUrl url)
        {
            if (!url.IsValid()) return;
            _controller.SetMeOwner();
            Track track = Track.New(_controller.VideoPlayerType, "", url);
            _controller.PlayTrack(track);
        }
        public void PlayTrack(Track track)
        {
            if (!track.GetUrl().IsValidUrl()) return;
            _controller.SetMeOwner();
            _controller.PlayTrack(track);
        }
        public void PlayTrack(Playlist playlist, int index)
        {
            _controller.SetMeOwner();
            _controller.PlayTrack(playlist, index);
        }
        public void AddTrackToQueue(Track track)
        {
            if (!track.GetUrl().IsValidUrl()) return;
            _controller.SetMeOwner();
            _controller.Queue.AddTrack(track);
        }
        public void RemoveTrackFromQueue(int index)
        {
            _controller.SetMeOwner();
            _controller.Queue.Remove(index);
        }
        public void Backward()
        {
            _controller.SetMeOwner();
            _controller.Backward();
        }
        public void Forward()
        {
            _controller.SetMeOwner();
            _controller.Forward();
        }
        public void Reload() => _controller.Reload();
        public void SetPermission(int index, PlayerPermission permission) => _permission.SetPermission(index, permission);
        public string GetTranslation(string key) => _i18n.GetValue(key);
        public override void OnVideoEnd()
        {
            if (_controller.IsOwner) SendCustomEventDelayedFrames(nameof(Forward), 1);
        }
    }
}