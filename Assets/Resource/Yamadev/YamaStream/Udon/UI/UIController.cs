
using System;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDK3.Components;
using VRC.SDK3.Components.Video;
using VRC.SDK3.Data;
using VRC.SDKBase;
using Yamadev.YamaStream.UI;

namespace Yamadev.YamaStream
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class UIController : Listener
    {
        [Header("YamaPlayer Handle")]
        [SerializeField] YamaPlayerHandle _playerHandle;

        [Header("Color")]
        [SerializeField] Color _primaryColor = new Color(240f / 256f, 98f / 256f, 146f / 256f, 1.0f);
        [SerializeField] Color _secondaryColor = new Color(248f / 256f, 187f / 256f, 208f / 256f, 31f / 256f);
        [SerializeField] Color _ownerColor;
        [SerializeField] Color _adminColor;
        [SerializeField] Color _editorColor;
        [SerializeField] Color _viewerColor;

        [Header("Video Player")]
        [SerializeField] Toggle _unityPlayer;
        [SerializeField] Toggle _avProPlayer;

        [Header("Modal")]
        [SerializeField] Modal _modal;

        [Header("Track")]
        [SerializeField] VRCUrlInputField _urlInputField;
        [SerializeField] Text _title;
        [SerializeField] Text _url;

        [Header("Progress")]
        [SerializeField] Text _videoTime;
        [SerializeField] Text _duration;
        [SerializeField] Slider _progress;

        [Header("Playback")]
        [SerializeField] Button _play;
        [SerializeField] Button _pause;
        [SerializeField] Button _loop;
        [SerializeField] Button _loopOff;
        [SerializeField] Button _reload;
        [SerializeField] Slider _speedSlider;
        [SerializeField] Text _speedText;
        [SerializeField] RangeSlider _repeatSlider;
        [SerializeField] Toggle _repeatOff;
        [SerializeField] Toggle _repeat;
        [SerializeField] Text _repeatStartTime;
        [SerializeField] Text _repeatEndTime;
        [SerializeField] Text _localDelayText;
        [SerializeField] Toggle _karaokeModeOff;
        [SerializeField] Toggle _karaokeModeKaraoke;
        [SerializeField] Toggle _karaokeModeDance;
        [SerializeField] GameObject _karaokeModal;

        [Header("Playlist")]
        [SerializeField] Button _inorderPlay;
        [SerializeField] Button _shufflePlay;
        [SerializeField] VRCUrlInputField _addVideoField;
        [SerializeField] VRCUrlInputField _addLiveField;
        [SerializeField] Transform _playlists;
        [SerializeField] bool _hasQueue = true;
        [SerializeField] bool _hasHistory = true;
        [SerializeField] Transform _playlistContent;
        [SerializeField] Text _playlistName;
        [SerializeField] Transform _playlistTracksContent;

        [Header("Audio")]
        [SerializeField] Button _mute;
        [SerializeField] Button _muteOff;
        [SerializeField] Slider _volume;

        [Header("Permission")]
        [SerializeField] GameObject _permissionEntry;
        [SerializeField] GameObject _permissionPage;
        [SerializeField] Transform _permissionContent;

        [Header("Loading")]
        [SerializeField] GameObject _loading;
        [SerializeField] Animator _loadingAnimator;
        [SerializeField] Text _message;

        [Header("Screen Renderer")]
        [SerializeField] Slider _emissionSlider;
        [SerializeField] Text _emissionText;
        [SerializeField] Toggle _maxResolution144;
        [SerializeField] Toggle _maxResolution240;
        [SerializeField] Toggle _maxResolution360;
        [SerializeField] Toggle _maxResolution480;
        [SerializeField] Toggle _maxResolution720;
        [SerializeField] Toggle _maxResolution1080;
        [SerializeField] Toggle _maxResolution2160;
        [SerializeField] Toggle _maxResolution4320;
        [SerializeField] Toggle _mirrorInversion;
        [SerializeField] Toggle _mirrorInversionOff;
        [SerializeField] RawImage _preview;

        [Header("Version")]
        [SerializeField] Text _versionText;
        [SerializeField] Text _updateLog;

        [Header("Debug")]
        [SerializeField, HideInInspector] Text _trackTitle;
        [SerializeField, HideInInspector] Text _trackUrl;
        [SerializeField, HideInInspector] Text _trackDisplayUrl;
        [SerializeField, HideInInspector] Text _networkDelay;
        [SerializeField, HideInInspector] Text _videoOffset;

        [Header("Translation - SideBar")]
        [SerializeField] Text _options;
        [SerializeField] Text _settings;
        [SerializeField] Text _playlist;
        [SerializeField] Text _videoSearch;
        [SerializeField] Text _version;

        [Header("Translation - General")]
        [SerializeField] Text _settingsTitle;
        [SerializeField] Text _playback;
        [SerializeField] Text _videoAndAudio;
        [SerializeField] Text _other;
        [SerializeField] Text _videoPlayer;
        [SerializeField] Text _videoPlayerDesc;
        [SerializeField] Text _playbackSpeed;
        [SerializeField] Text _playbackSpeedDesc;
        [SerializeField] Text _slower;
        [SerializeField] Text _faster;
        [SerializeField] Text _repeatPlay;
        [SerializeField] Text _repeatPlayDesc;
        [SerializeField] Text _repeatOnText;
        [SerializeField] Text _repeatOffText;
        [SerializeField] Text _maxResolution;
        [SerializeField] Text _maxResolutionDesc;
        [SerializeField] Text _mirrorInversionTitle;
        [SerializeField] Text _mirrorInversionDesc;
        [SerializeField] Text _mirrorInversionOnText;
        [SerializeField] Text _mirrorInversionOffText;
        [SerializeField] Text _brightness;
        [SerializeField] Text _brightnessDesc;
        [SerializeField] Text _karaokeModeText;
        [SerializeField] Text _karaokeModeDesc;
        [SerializeField] Text _karaokeModeOnText;
        [SerializeField] Text _danceModeOnText;
        [SerializeField] Text _karaokeModeOffText;
        [SerializeField] Text _localDelay;
        [SerializeField] Text _localDelayDesc;
        [SerializeField] Text _languageSelect;
        [SerializeField] Text _permissionTitle;
        [SerializeField] Text _permissionDesc;

        [Header("Translation - Video Search")]
        [SerializeField] Text _videoSearchTitle;
        [SerializeField] Text _inputKeyword;
        [SerializeField] Text _inLoading;

        [Header("Translation - Playlist")]
        [SerializeField] Text _playlistTitle;
        [SerializeField] Text _playQueue;
        [SerializeField] Text _playHistory;
        [SerializeField] Text _addVideoLink;
        [SerializeField] Text _addLiveLink;

        string _timeFormat = @"hh\:mm\:ss";
        bool _progressDrag = false;

        void Start()
        {
            if (_playerHandle == null) return;
            _playerHandle.AddListener(this);
            SendCustomEventDelayedFrames(nameof(UpdateUI), 3);
            SendCustomEventDelayedFrames(nameof(GeneratePlaylistView), 3);
            SendCustomEventDelayedFrames(nameof(UpdateTranslation), 3);
            if (_preview != null && Array.IndexOf(_playerHandle.RawImageScreens, _preview) < 0) 
                _playerHandle.RawImageScreens = _playerHandle.RawImageScreens.Add(_preview);
            if (_versionText != null) _versionText.text = $"YamaPlayer v{_playerHandle.Version}";
            if (_updateLog != null) _updateLog.text = _playerHandle.UpdateLog;
        }
        void Update()
        {
            if (_playerHandle.Stopped) return;
            updateProgress();
        }

        public Color PrimaryColor => _primaryColor;
        public Color SecondaryColor => _secondaryColor;

        public bool CheckPermission()
        {
            if ((int)_playerHandle.Permission >= (int)PlayerPermission.Editor) return true;
            if (_modal != null) _modal.Show(
                _playerHandle.GetTranslation("noPermission"), 
                _playerHandle.GetTranslation("noPermissionMessage"), 
                UdonEvent.Empty(), 
                _playerHandle.GetTranslation("close"), 
                string.Empty, 
                true, 
                false);
            return false;
        }
        public void SetUnityPlayer()
        {
            UpdateUI();
            if (_playerHandle.VideoPlayerType == VideoPlayerType.UnityVideoPlayer || !CheckPermission()) return;
            if (_modal == null || _playerHandle.Stopped)
            {
                SetUnityPlayerEvent();
                return;
            }
            if (_modal != null) _modal.Show(
                _playerHandle.GetTranslation("confirmChangePlayer"), 
                _playerHandle.GetTranslation("confirmChangePlayerMessage"), 
                UdonEvent.New(this, nameof(SetUnityPlayerEvent)), 
                _playerHandle.GetTranslation("cancel"), 
                _playerHandle.GetTranslation("continue"));
        }
        public void SetUnityPlayerEvent()
        {
            _playerHandle.VideoPlayerType = VideoPlayerType.UnityVideoPlayer;
            if (_modal != null) _modal.Close();
        }
        public void SetAVProPlayer()
        {
            UpdateUI();
            if (_playerHandle.VideoPlayerType == VideoPlayerType.AVProVideoPlayer || !CheckPermission()) return;
            if (_modal == null || _playerHandle.Stopped)
            {
                SetAVProPlayerEvent();
                return;
            }
            if (_modal != null) _modal.Show(
                _playerHandle.GetTranslation("confirmChangePlayer"), 
                _playerHandle.GetTranslation("confirmChangePlayerMessage"), 
                UdonEvent.New(this, nameof(SetAVProPlayerEvent)), 
                _playerHandle.GetTranslation("cancel"),
                _playerHandle.GetTranslation("continue"));
        }
        public void SetAVProPlayerEvent()
        {
            _playerHandle.VideoPlayerType = VideoPlayerType.AVProVideoPlayer;
            if (_modal != null) _modal.Close();
        }
        public void PlayUrl()
        {
            if (_urlInputField == null || !_urlInputField.GetUrl().IsValid()) return;
            if (!CheckPermission())
            {
                _urlInputField.SetUrl(VRCUrl.Empty);
                return;
            }
            if (_playerHandle.Stopped || _modal == null)
            {
                PlayUrlEvent();
                return;
            }
            _modal.Show(
                _playerHandle.GetTranslation("playUrl"), 
                _playerHandle.GetTranslation("confirmPlayUrlMessage"), 
                UdonEvent.New(this, nameof(AddUrlToQueueEvent)), 
                UdonEvent.New(this, nameof(PlayUrlEvent)),
                _playerHandle.GetTranslation("cancel"),
                _playerHandle.GetTranslation("confirmAddQueue"),
                _playerHandle.GetTranslation("confirmPlayUrl"));
        }
        public void AddUrlToQueueEvent()
        {
            _playerHandle.AddTrackToQueue(Track.New(_playerHandle.VideoPlayerType, "", _urlInputField.GetUrl()));
            _urlInputField.SetUrl(VRCUrl.Empty);
            if (_modal != null) _modal.Close();
        }
        public void PlayUrlEvent()
        {
            _playerHandle.PlayUrl(_urlInputField.GetUrl());
            _urlInputField.SetUrl(VRCUrl.Empty);
            if (_modal != null) _modal.Close();
        }
        public void Play()
        {
            if (!CheckPermission()) return;
            _playerHandle.Paused = false;
        }
        public void Pause()
        {
            if (!CheckPermission()) return;
            _playerHandle.Paused = true;
        }
        public void Stop()
        {
            if (!CheckPermission()) return;
            _playerHandle.Stopped = true;
        }
        public void ProgressDrag() => _progressDrag = true;
        public void SetTime()
        {
            _progressDrag = false;
            if (!_progress || !CheckPermission()) return;
            _playerHandle.SetTime(_playerHandle.Duration * _progress.value);
        }
        public void Loop()
        {
            if (!CheckPermission()) return;
            _playerHandle.Loop = true;
        }
        public void LoopOff()
        {
            if (!CheckPermission()) return;
            _playerHandle.Loop = false;
        }
        public void Reload() => _playerHandle.Reload();
        public void Repeat()
        {
            if (!CheckPermission()) return;
            RepeatStatus status = _playerHandle.Repeat;
            status.TurnOn();
            _playerHandle.Repeat = status;
        }
        public void RepeatOff()
        {
            if (!CheckPermission()) return;
            RepeatStatus status = _playerHandle.Repeat;
            status.TurnOff();
            _playerHandle.Repeat = status;
        }
        public void SetRepeatStart()
        {
            if (_repeatSlider != null && _playerHandle.IsPlaying)
            {
                if (!_playerHandle.Repeat.IsOn())
                {
                    RepeatStatus status = _playerHandle.Repeat;
                    status.SetStartTime(_playerHandle.IsLive ? 0f : Mathf.Clamp(_playerHandle.Duration * _repeatSlider.SliderLeft.value, 0f, _playerHandle.Duration));
                    _playerHandle.Repeat = status;
                }
                else _repeatSlider.SliderLeft.value = _playerHandle.Repeat.GetStartTime() / _playerHandle.Duration;
            }
        }
        public void SetRepeatEnd()
        {
            if (_repeatSlider != null && _playerHandle.IsPlaying)
            {
                if (!_playerHandle.Repeat.IsOn())
                {
                    RepeatStatus status = _playerHandle.Repeat;
                    status.SetEndTime(_playerHandle.IsLive ? 999999f : Mathf.Clamp(_playerHandle.Duration * _repeatSlider.SliderRight.value, 0f, _playerHandle.Duration));
                    _playerHandle.Repeat = status;
                }
                else _repeatSlider.SliderRight.value = _playerHandle.Repeat.GetEndTime() / _playerHandle.Duration;
            }
        }
        public void SetShuffle()
        {
            if (!CheckPermission()) return;
            _playerHandle.ShufflePlay = true;
        }
        public void SetShuffleOff()
        {
            if (!CheckPermission()) return;
            _playerHandle.ShufflePlay = false;
        }
        public void AddVideoToQueue()
        {
            if (_addVideoField == null || string.IsNullOrEmpty(_addVideoField.GetUrl().Get())) return;
            if (!CheckPermission())
            {
                _addVideoField.SetUrl(VRCUrl.Empty);
                return;
            }
            _playerHandle.AddTrackToQueue(Track.New(VideoPlayerType.UnityVideoPlayer, "", _addVideoField.GetUrl()));
            _addVideoField.SetUrl(VRCUrl.Empty);
        }
        public void AddLiveToQueue() 
        { 
            if (_addLiveField == null || string.IsNullOrEmpty(_addLiveField.GetUrl().Get())) return;
            if (!CheckPermission())
            {
                _addLiveField.SetUrl(VRCUrl.Empty);
                return;
            }
            _playerHandle.AddTrackToQueue(Track.New(VideoPlayerType.AVProVideoPlayer, "", _addLiveField.GetUrl()));
            _addLiveField.SetUrl(VRCUrl.Empty);
        }
        int getSelectedPlaylistIndex()
        {
            if (_playlists == null) return -1;
            Toggle[] toggles = _playlists.GetComponentsInChildren<Toggle>();
            for (int i = 0; i < toggles.Length; i++)
            {
                if (toggles[i].isOn) return i;
            }
            return -1;
        }
        int historyIndex => !_hasHistory ? -1 : _hasQueue ? 1 : 0;
        public void PlayPlaylistTracks()
        {
            if (_playlistTracksContent == null || !CheckPermission()) return;
            Toggle[] toggles = _playlistTracksContent.GetComponentsInChildren<Toggle>();
            Toggle activeToggle = toggles.GetFirstActiveToggle();
            int selectedPlaylistIndex = getSelectedPlaylistIndex();
            if (_hasQueue && selectedPlaylistIndex == 0) return;
            if (activeToggle != null && selectedPlaylistIndex >= 0)
            {
                activeToggle.SetIsOnWithoutNotify(false);
                Playlist playlist;
                if (selectedPlaylistIndex == historyIndex) playlist = _playerHandle.HistoryList;
                else
                {
                    if (_hasQueue) selectedPlaylistIndex -= 1;
                    if (_hasHistory) selectedPlaylistIndex -= 1;
                    playlist = _playerHandle.Playlists[selectedPlaylistIndex];
                }
                _playerHandle.PlayTrack(playlist, activeToggle.transform.GetSiblingIndex());
                GeneratePlaylistTracksView();
            }
        }
        public void RemoveFromQueue()
        {
            if (_playlistTracksContent == null || !CheckPermission()) return;
            Toggle[] toggles = _playlistTracksContent.GetComponentsInChildren<Toggle>();
            Toggle activeToggle = toggles.GetFirstActiveToggle();
            int index = activeToggle.transform.GetSiblingIndex();
            if (activeToggle != null && index < _playerHandle.QueueList.Length) _playerHandle.RemoveTrackFromQueue(index);

        }
        public void Backward()
        {
            if (!CheckPermission()) return;
            _playerHandle.Backward();
        }
        public void Forward()
        {
            if (!CheckPermission()) return;
            _playerHandle.Forward();
        }
        public void SetSpeed() 
        {
            if (!CheckPermission()) return;
            if (_speedSlider != null) _playerHandle.Speed = _speedSlider.value / 20f; 
        }
        public void Mute() => _playerHandle.Mute = true;
        public void MuteOff() => _playerHandle.Mute = false;
        public void SetVolume() => _playerHandle.Volume = _volume.value;
        public void Subtract50ms() => _playerHandle.LocalDelay -= 0.05f;
        public void Subtract100ms() => _playerHandle.LocalDelay -= 0.1f;
        public void Add50ms() => _playerHandle.LocalDelay += 0.05f;
        public void Add100ms() => _playerHandle.LocalDelay += 0.1f;
        public void SetEmission() 
        { 
            if (_emissionSlider != null) _playerHandle.Emission = _emissionSlider.value; 
        }
        public void SetMirrorInverse() => _playerHandle.MirrorInverse = true;
        public void SetMirrorInverseOff() => _playerHandle.MirrorInverse = false;
        public void SetMaxResolution144() => _playerHandle.MaxResolution = 144;
        public void SetMaxResolution240() => _playerHandle.MaxResolution = 240;
        public void SetMaxResolution360() => _playerHandle.MaxResolution = 360;
        public void SetMaxResolution480() => _playerHandle.MaxResolution = 480;
        public void SetMaxResolution720() => _playerHandle.MaxResolution = 720;
        public void SetMaxResolution1080() => _playerHandle.MaxResolution = 1080;
        public void SetMaxResolution2160() => _playerHandle.MaxResolution = 2160;
        public void SetMaxResolution4320() => _playerHandle.MaxResolution = 4320;
        public void SetLanguageAuto() => _playerHandle.Language = null;
        public void SetLanguageJapanese() => _playerHandle.Language = "ja";
        public void SetLanguageChineseChina() => _playerHandle.Language = "zh-cn";
        public void SetLanguageChineseTaiwan() => _playerHandle.Language = "zh-tw";
        public void SetLanguageKorean() => _playerHandle.Language = "ko";
        public void SetLanguageEnglish() => _playerHandle.Language = "en";
        public void SetKaraokeModeOff()
        {
            if (!CheckPermission()) return;
            _playerHandle.KaraokeMode = KaraokeMode.None;
        }
        public void SetKaraokeModeKaraoke()
        {
            if (!CheckPermission()) return;
            _playerHandle.KaraokeMode = KaraokeMode.Karaoke;
        }
        public void SetKaraokeModeDance()
        {
            if (!CheckPermission()) return;
            _playerHandle.KaraokeMode = KaraokeMode.Dance;
        }
        public void JoinKaraokeMembers()
        {
            if (_playerHandle.IsKaraokeMember) return;
            _playerHandle.KaraokeMembers = _playerHandle.KaraokeMembers.Add(Networking.LocalPlayer.displayName);
            if (_modal != null && _modal.IsActive) OpenKaraokeMemberModal();
        }
        public void LeaveKaraokeMembers()
        {
            if (!_playerHandle.IsKaraokeMember) return;
            _playerHandle.KaraokeMembers = _playerHandle.KaraokeMembers.Remove(Networking.LocalPlayer.displayName);
            if (_modal != null && _modal.IsActive) OpenKaraokeMemberModal();
        }
        public void OpenKaraokeMemberModal()
        {
            if (_modal == null || _playerHandle.KaraokeMode == KaraokeMode.None) return;
            UdonEvent callback = _playerHandle.IsKaraokeMember ? UdonEvent.New(this, nameof(LeaveKaraokeMembers)) : UdonEvent.New(this, nameof(JoinKaraokeMembers));
            string executeText = _playerHandle.IsKaraokeMember ? _playerHandle.GetTranslation("leaveMember") : _playerHandle.GetTranslation("joinMember");
            _modal.Show(_playerHandle.GetTranslation("karaokeMember"), string.Join("\n", _playerHandle.KaraokeMembers), callback, _playerHandle.GetTranslation("close"), executeText);
        }
        public void SetPermission()
        {
            if (_permissionContent == null) return;
            Toggle[] toggles = _permissionContent.GetComponentsInChildren<Toggle>(true);
            Toggle activeToggle = toggles.GetFirstActiveToggle();
            if (activeToggle != null)
            {
                activeToggle.SetIsOnWithoutNotify(false);
                Dropdown dropdown = activeToggle.transform.Find("Dropdown").GetComponent<Dropdown>();
                if (dropdown != null)
                {
                    PlayerPermission playerPermission = PlayerPermission.Viewer;
                    if (dropdown.value == 1) playerPermission = PlayerPermission.Editor;
                    if (dropdown.value == 0) playerPermission = PlayerPermission.Admin;
                    _playerHandle.SetPermission(activeToggle.transform.GetSiblingIndex(), playerPermission);
                }
            }
        }
        public void GeneratePlaylistView()
        {
            if (_playlistContent == null) return;
            for (int i = 0; i < _playlistContent.childCount; i++)
            {
                Transform item = _playlistContent.GetChild(i);
                item.gameObject.SetActive(false);
                if (i > _playerHandle.Playlists.Length - 1) continue;
                Playlist playlist = _playerHandle.Playlists[i];
                Text name = item.Find("Text") != null ? item.Find("Text").GetComponent<Text>() : null;
                if (name != null) name.text = _playerHandle.Playlists[i].PlaylistName;
                Text trackCount = item.transform.Find("TrackCount") != null ? item.transform.Find("TrackCount").GetComponent<Text>() : null;
                if (trackCount != null) trackCount.text = playlist.Length > 0 ? $"{_playerHandle.GetTranslation("total")} {playlist.Length} {_playerHandle.GetTranslation("tracks")}" : string.Empty;
                item.gameObject.SetActive(true);
            }
        }
        void drawPlaylistTrack(int index, Track track, bool isPlaying)
        {
            if (_playlistTracksContent == null) return;
            Transform trackItem = _playlistTracksContent.GetChild(index);
            Text title = trackItem.Find("Title") != null ? trackItem.Find("Title").GetComponent<Text>() : null;
            if (title != null)
            {
                title.text = track.HasTitle() ? track.GetTitle() : track.GetUrl();
                title.color = isPlaying ? _primaryColor : Color.white;
            }
            Text url = trackItem.Find("Url") != null ? trackItem.Find("Url").GetComponent<Text>() : null;
            if (url != null) url.text = track.HasTitle() ? track.GetUrl() : string.Empty;
            Text number = trackItem.Find("No") != null ? trackItem.Find("No").GetComponent<Text>() : null;
            if (number != null)
            {
                number.text = $"{index + 1}";
                number.gameObject.SetActive(!isPlaying);
            }
            Image playingMark = trackItem.Find("PlayingMark") != null ? trackItem.Find("PlayingMark").GetComponent<Image>() : null;
            if (playingMark != null) playingMark.gameObject.SetActive(isPlaying);
        }
        void clearTracksView()
        {
            if (_playlistTracksContent != null)
                for (int j = 0; j < _playlistTracksContent.childCount - 2; j++)
                    _playlistTracksContent.GetChild(j).gameObject.SetActive(false);
            _playlistTracksContent.GetChild(_playlistTracksContent.childCount - 1).gameObject.SetActive(false);
            _playlistTracksContent.GetChild(_playlistTracksContent.childCount - 2).gameObject.SetActive(false);
        }
        public void GenerateQueueTracksView()
        {
            Toggle[] toggles = _playlists.GetComponentsInChildren<Toggle>();
            Toggle activeToggle = toggles.GetFirstActiveToggle();
            if (activeToggle == null) return;
            if (_playlistName != null) _playlistName.text = _playerHandle.GetTranslation("playQueue");
            if (_playlistTracksContent != null)
            {
                for (int i = 0; i < _playlistTracksContent.childCount - 2; i++)
                {
                    Transform trackItem = _playlistTracksContent.GetChild(i);
                    if (trackItem.GetComponent<Toggle>() != null) trackItem.GetComponent<Toggle>().enabled = false;
                    if (trackItem.Find("Remove") != null) trackItem.Find("Remove").gameObject.SetActive(true);
                    trackItem.gameObject.SetActive(false);
                    if (i > _playerHandle.QueueList.Length - 1) continue;
                    Track track = _playerHandle.QueueList.GetTrack(i);
                    drawPlaylistTrack(i, track, false);
                    trackItem.gameObject.SetActive(true);
                }
                _playlistTracksContent.GetChild(_playlistTracksContent.childCount - 1).gameObject.SetActive(true);
                _playlistTracksContent.GetChild(_playlistTracksContent.childCount - 2).gameObject.SetActive(true);
            }
        }
        public void GenerateHistoryTracksView()
        {
            Toggle[] toggles = _playlists.GetComponentsInChildren<Toggle>();
            Toggle activeToggle = toggles.GetFirstActiveToggle();
            if (activeToggle == null) return;
            if (_playlistName != null) _playlistName.text = _playerHandle.GetTranslation("playHistory");
            if (_playlistTracksContent != null)
            {
                for (int i = 0; i < _playlistTracksContent.childCount - 2; i++)
                {
                    Transform trackItem = _playlistTracksContent.GetChild(i);
                    trackItem.gameObject.SetActive(false);
                    if (trackItem.GetComponent<Toggle>() != null) trackItem.GetComponent<Toggle>().enabled = true;
                    if (trackItem.Find("Remove") != null) trackItem.Find("Remove").gameObject.SetActive(false);
                    if (i > _playerHandle.HistoryList.Length - 1) continue;
                    Track track = _playerHandle.HistoryList.GetTrack(i);
                    drawPlaylistTrack(i, track, false);
                    trackItem.gameObject.SetActive(true);
                }
            }
        }
        public void GeneratePlaylistTracksView()
        {
            if (_playlists == null) return;
            Toggle[] toggles = _playlists.GetComponentsInChildren<Toggle>();
            if (_playlistName != null) _playlistName.text = string.Empty;
            clearTracksView();
            for (int i = 0; i < toggles.Length; i++)
            {
                Toggle toggle = toggles[i];
                if (!toggle.isOn) continue;
                if (_hasQueue && i == 0)
                {
                    GenerateQueueTracksView();
                    return;
                }
                if (_hasHistory && i == historyIndex)
                {
                    GenerateHistoryTracksView();
                    return;
                }
                if (toggle.transform.GetSiblingIndex() > _playerHandle.Playlists.Length - 1) continue;
                Playlist playlist = _playerHandle.Playlists[toggle.transform.GetSiblingIndex()];
                if (_playlistName != null) _playlistName.text = playlist.PlaylistName;
                if (_playlistTracksContent != null)
                {
                    for (int j = 0; j < _playlistTracksContent.childCount - 2; j++)
                    {
                        Transform trackItem = _playlistTracksContent.GetChild(j);
                        if (trackItem.GetComponent<Toggle>() != null) trackItem.GetComponent<Toggle>().enabled = true;
                        if (trackItem.Find("Remove") != null) trackItem.Find("Remove").gameObject.SetActive(false);
                        if (j > playlist.Length - 1) continue;
                        Track track = playlist.GetTrack(j);
                        bool isPlaying = playlist == _playerHandle.ActivePlaylist && _playerHandle.PlayingTrackIndex == j;
                        drawPlaylistTrack(j, track, isPlaying);
                        trackItem.gameObject.SetActive(true);
                    }
                }
            }
        }

        public void UpdateUI()
        {
            updatePlayerView();
            updateProgress();
            updatePlaybackView();
            updateTrackView();
            updateAudioView();
            updateScreenView();
            updateLoadingView();
            updatePermissionView();
        }

        void updateProgress()
        {
            if (_videoTime != null) _videoTime.text = TimeSpan.FromSeconds(_playerHandle.VideoTime).ToString(_timeFormat);
            if (_duration != null) _duration.text = _playerHandle.IsLive ? "Live" : TimeSpan.FromSeconds(_playerHandle.Duration).ToString(_timeFormat);
            if (_progress != null && !_progressDrag) _progress.SetValueWithoutNotify(_playerHandle.IsLive ? 1f : Mathf.Clamp(_playerHandle.Duration == 0f ? 0f : _playerHandle.VideoTime / _playerHandle.Duration, 0f, 1f));
        }

        void updatePlayerView()
        {
            if (_unityPlayer != null) _unityPlayer.SetIsOnWithoutNotify(_playerHandle.VideoPlayerType == VideoPlayerType.UnityVideoPlayer);
            if (_avProPlayer != null) _avProPlayer.SetIsOnWithoutNotify(_playerHandle.VideoPlayerType == VideoPlayerType.AVProVideoPlayer);
        }

        void updatePlaybackView()
        {
            if (_play != null) _play.gameObject.SetActive(!_playerHandle.IsPlaying);
            if (_pause != null) _pause.gameObject.SetActive(_playerHandle.IsPlaying);
            if (_loop != null) _loop.gameObject.SetActive(!_playerHandle.Loop);
            if (_loopOff != null) _loopOff.gameObject.SetActive(_playerHandle.Loop);
            if (_speedSlider != null) _speedSlider.SetValueWithoutNotify((float)Math.Round(_playerHandle.Speed * 20));
            if (_speedText != null) _speedText.text = $"{_playerHandle.Speed:F2}x";
            if (_repeatOff != null) _repeatOff.SetIsOnWithoutNotify(!_playerHandle.Repeat.IsOn());
            if (_repeat != null) _repeat.SetIsOnWithoutNotify(_playerHandle.Repeat.IsOn());
            if (_repeatSlider != null && _repeatStartTime != null && _repeatEndTime != null)
            {
                string notSetText = _playerHandle.GetTranslation("notSet");
                string startText = _playerHandle.Repeat.GetStartTime() == 0 ? notSetText : TimeSpan.FromSeconds(_playerHandle.Repeat.GetStartTime()).ToString(_timeFormat);
                string endText = _playerHandle.Repeat.GetEndTime() >= _playerHandle.Duration || _playerHandle.IsLive ? notSetText : TimeSpan.FromSeconds(_playerHandle.Repeat.GetEndTime()).ToString(_timeFormat);
                _repeatSlider.SliderLeft.SetValueWithoutNotify(_playerHandle.IsLive || !_playerHandle.IsPlaying ? 0f : Mathf.Clamp(_playerHandle.Repeat.GetStartTime() / _playerHandle.Duration, 0f, 1f));
                _repeatSlider.SliderRight.SetValueWithoutNotify(_playerHandle.IsLive || !_playerHandle.IsPlaying ? 1f : Mathf.Clamp(_playerHandle.Repeat.GetEndTime() / _playerHandle.Duration, 0f, 1f));
                _repeatStartTime.text = $"{_playerHandle.GetTranslation("start")}(A): {startText}";
                _repeatEndTime.text = $"{_playerHandle.GetTranslation("end")}(B): {endText}";
            }
            if (_localDelayText != null) _localDelayText.text = (Mathf.Round(_playerHandle.LocalDelay * 100) / 100).ToString();
            if (_inorderPlay != null) _inorderPlay.gameObject.SetActive(!_playerHandle.ShufflePlay);
            if (_shufflePlay != null) _shufflePlay.gameObject.SetActive(_playerHandle.ShufflePlay);
        }

        void updateTrackView()
        {
            Track track = _playerHandle.Track;
            if (_title != null) _title.text = track.HasTitle() ? track.GetTitle() : track.GetUrl();
            if (_url != null) _url.text = track.HasTitle() ? track.GetUrl() : string.Empty;
        }

        void updateAudioView()
        {
            if (_mute != null) _mute.gameObject.SetActive(!_playerHandle.Mute);
            if (_muteOff != null) _muteOff.gameObject.SetActive(_playerHandle.Mute);
            if (_volume != null) _volume.SetValueWithoutNotify(_playerHandle.Volume);
        }

        void updateScreenView()
        {
            if (_mirrorInversion != null) _mirrorInversion.SetIsOnWithoutNotify(_playerHandle.MirrorInverse);
            if (_mirrorInversionOff != null) _mirrorInversionOff.SetIsOnWithoutNotify(!_playerHandle.MirrorInverse);
            if (_maxResolution144 != null) _maxResolution144.SetIsOnWithoutNotify(_playerHandle.MaxResolution == 144);
            if (_maxResolution240 != null) _maxResolution240.SetIsOnWithoutNotify(_playerHandle.MaxResolution == 240);
            if (_maxResolution360 != null) _maxResolution360.SetIsOnWithoutNotify(_playerHandle.MaxResolution == 360);
            if (_maxResolution480 != null) _maxResolution480.SetIsOnWithoutNotify(_playerHandle.MaxResolution == 480);
            if (_maxResolution720 != null) _maxResolution720.SetIsOnWithoutNotify(_playerHandle.MaxResolution == 720);
            if (_maxResolution1080 != null) _maxResolution1080.SetIsOnWithoutNotify(_playerHandle.MaxResolution == 1080);
            if (_maxResolution2160 != null) _maxResolution2160.SetIsOnWithoutNotify(_playerHandle.MaxResolution == 2160);
            if (_maxResolution4320 != null) _maxResolution4320.SetIsOnWithoutNotify(_playerHandle.MaxResolution == 4320);
            if (_preview != null) _preview.enabled = _playerHandle.IsPlaying;
            if (_emissionSlider != null) _emissionSlider.SetValueWithoutNotify(_playerHandle.Emission);
            if (_emissionText != null) _emissionText.text = $"{Mathf.Ceil(_playerHandle.Emission * 100)}%";
        }

        void updateKaraokeView()
        {
            if (_karaokeModeOff != null) _karaokeModeOff.SetIsOnWithoutNotify(_playerHandle.KaraokeMode == KaraokeMode.None);
            if (_karaokeModeKaraoke != null) _karaokeModeKaraoke.SetIsOnWithoutNotify(_playerHandle.KaraokeMode == KaraokeMode.Karaoke);
            if (_karaokeModeDance != null) _karaokeModeDance.SetIsOnWithoutNotify(_playerHandle.KaraokeMode == KaraokeMode.Dance);
            if (_karaokeModal != null) _karaokeModal.SetActive(_playerHandle.KaraokeMode != KaraokeMode.None);
        }

        void updateErrorView(VideoError videoError)
        {
            if (_loading != null) _loading.SetActive(true);
            if (_loadingAnimator != null) _loadingAnimator.SetBool("Loading", false);
            if (_message == null) return;
            switch (videoError)
            {
                case VideoError.Unknown:
                    _message.text = _playerHandle.GetTranslation("unknownErrorMessage");
                    break;
                case VideoError.InvalidURL:
                    _message.text = _playerHandle.GetTranslation("invalidUrlMessage");
                    break;
                case VideoError.AccessDenied:
                    _message.text = _playerHandle.GetTranslation("accessDeniedMessage");
                    break;
                case VideoError.RateLimited:
                    _message.text = _playerHandle.GetTranslation("rateLimitedMessage");
                    break;
                case VideoError.PlayerError:
                    _message.text = _playerHandle.GetTranslation("playerErrorMessage");
                    break;
                default:
                    break;
            }
        }

        void updateLoadingView()
        {
            if (_loading != null) _loading.SetActive(_playerHandle.IsLoading);
            if (_loadingAnimator != null) _loadingAnimator.SetBool("Loading", _playerHandle.IsLoading);
            if (_message != null) _message.text = _playerHandle.GetTranslation("videoLoadingMessage");
        }

        void updatePermissionView()
        {
            bool showPage = _playerHandle.Permission == PlayerPermission.Owner || _playerHandle.Permission == PlayerPermission.Admin;
            if (_permissionEntry != null) _permissionEntry.SetActive(showPage);
            if (_permissionPage != null && !showPage && _permissionPage.activeSelf) _permissionPage.SetActive(false);
            if (_permissionContent == null) return;
            for (int i = 0; i < _permissionContent.childCount; i++)
            {
                Transform item = _permissionContent.GetChild(i);
                item.gameObject.SetActive(false);
                if (i >= _playerHandle.PermissionData.Count) continue;
                DataToken value = _playerHandle.PermissionData.GetValues()[i];
                value.DataDictionary.TryGetValue("displayName", TokenType.String, out DataToken displayName);
                item.Find("Name").GetComponent<Text>().text = displayName.String;

                PlayerPermission permission = (PlayerPermission)value.DataDictionary["permission"].Int;
                bool noEdit = (int)_playerHandle.Permission <= (int)permission;
                if (noEdit)
                {
                    item.Find("Label").GetComponent<Text>().text = permission == PlayerPermission.Owner ? "Owner" : "Admin";
                    item.Find("Label").gameObject.SetActive(noEdit);
                    item.Find("Dropdown").gameObject.SetActive(!noEdit);
                }

                switch (permission)
                {
                    case PlayerPermission.Owner:
                        item.Find("Mark").GetComponent<Image>().color = _ownerColor;
                        break;
                    case PlayerPermission.Admin:
                        item.Find("Mark").GetComponent<Image>().color = _adminColor;
                        item.Find("Dropdown").GetComponent<Dropdown>().SetValueWithoutNotify(0);
                        break;
                    case PlayerPermission.Editor:
                        item.Find("Mark").GetComponent<Image>().color = _editorColor;
                        item.Find("Dropdown").GetComponent<Dropdown>().SetValueWithoutNotify(1);
                        break;
                    case PlayerPermission.Viewer:
                        item.Find("Mark").GetComponent<Image>().color = _viewerColor;
                        item.Find("Dropdown").GetComponent<Dropdown>().SetValueWithoutNotify(2);
                        break;
                    default:
                        break;
                }
                item.gameObject.SetActive(true);
            }
        }

        public void UpdateTranslation()
        {
            if (_options != null) _options.text = _playerHandle.GetTranslation("options");
            if (_settings != null) _settings.text = _playerHandle.GetTranslation("settings");
            if (_playlist != null) _playlist.text = _playerHandle.GetTranslation("playlist");
            if (_videoSearch != null) _videoSearch.text = _playerHandle.GetTranslation("videoSearch");
            if (_version != null) _version.text = _playerHandle.GetTranslation("version");
            if (_settingsTitle != null) _settingsTitle.text = _playerHandle.GetTranslation("settingsTitle");
            if (_playback != null) _playback.text = _playerHandle.GetTranslation("playback");
            if (_videoAndAudio != null) _videoAndAudio.text = _playerHandle.GetTranslation("videoAndAudio");
            if (_other != null) _other.text = _playerHandle.GetTranslation("other");
            if (_videoPlayer != null) _videoPlayer.text = $"{_playerHandle.GetTranslation("videoPlayer")}<size=100>(Global)</size>";
            if (_videoPlayerDesc != null) _videoPlayerDesc.text = _playerHandle.GetTranslation("videoPlayerDesc");
            if (_playbackSpeed != null) _playbackSpeed.text = $"{_playerHandle.GetTranslation("playbackSpeed")}<size=100>(Global)</size>";
            if (_playbackSpeedDesc != null) _playbackSpeedDesc.text = _playerHandle.GetTranslation("playbackSpeedDesc");
            if (_slower != null) _slower.text = _playerHandle.GetTranslation("slower");
            if (_faster != null) _faster.text = _playerHandle.GetTranslation("faster");
            if (_repeatPlay != null) _repeatPlay.text = $"{_playerHandle.GetTranslation("repeatPlay")}<size=100>(Global)</size>";
            if (_repeatPlayDesc != null) _repeatPlayDesc.text = _playerHandle.GetTranslation("repeatPlayDesc");
            if (_repeatOnText != null) _repeatOnText.text = _playerHandle.GetTranslation("repeatOn");
            if (_repeatOffText != null) _repeatOffText.text = _playerHandle.GetTranslation("repeatOff");
            if (_maxResolution != null) _maxResolution.text = _playerHandle.GetTranslation("maxResolution");
            if (_maxResolutionDesc != null) _maxResolutionDesc.text = _playerHandle.GetTranslation("maxResolutionDesc");
            if (_mirrorInversionTitle != null) _mirrorInversionTitle.text = _playerHandle.GetTranslation("mirrorInversion");
            if (_mirrorInversionDesc != null) _mirrorInversionDesc.text = _playerHandle.GetTranslation("mirrorInversionDesc");
            if (_mirrorInversionOnText != null) _mirrorInversionOnText.text = _playerHandle.GetTranslation("mirrorInversionOn");
            if (_mirrorInversionOffText != null) _mirrorInversionOffText.text = _playerHandle.GetTranslation("mirrorInversionOff");
            if (_brightness != null) _brightness.text = _playerHandle.GetTranslation("brightness");
            if (_brightnessDesc != null) _brightnessDesc.text = _playerHandle.GetTranslation("brightnessDesc");
            if (_karaokeModeText != null) _karaokeModeText.text = $"{_playerHandle.GetTranslation("karaokeMode")}<size=100>(Global)</size>";
            if (_karaokeModeDesc != null) _karaokeModeDesc.text = _playerHandle.GetTranslation("karaokeModeDesc");
            if (_karaokeModeOnText != null) _karaokeModeOnText.text = _playerHandle.GetTranslation("karaokeModeOn");
            if (_danceModeOnText != null) _danceModeOnText.text = _playerHandle.GetTranslation("danceModeOn");
            if (_karaokeModeOffText != null) _karaokeModeOffText.text = _playerHandle.GetTranslation("karaokeModeOff");
            if (_localDelay != null) _localDelay.text = _playerHandle.GetTranslation("localOffset");
            if (_localDelayDesc != null) _localDelayDesc.text = _playerHandle.GetTranslation("localOffsetDesc");
            if (_languageSelect != null) _languageSelect.text = _playerHandle.GetTranslation("languageSelect");

            if (_videoSearchTitle != null) _videoSearchTitle.text = _playerHandle.GetTranslation("videoSearchTitle");
            if (_inputKeyword != null) _inputKeyword.text = _playerHandle.GetTranslation("inputKeyword");
            if (_inLoading != null) _inLoading.text = _playerHandle.GetTranslation("inLoading");

            if (_playlistTitle != null) _playlistTitle.text = _playerHandle.GetTranslation("playlistTitle");
            if (_playQueue != null) _playQueue.text = _playerHandle.GetTranslation("playQueue");
            if (_playHistory != null) _playHistory.text = _playerHandle.GetTranslation("playHistory");
            if (_addVideoLink != null) _addVideoLink.text = _playerHandle.GetTranslation("addVideoLink");
            if (_addLiveLink != null) _addLiveLink.text = _playerHandle.GetTranslation("addLiveLink");
            if (_permissionTitle != null) _permissionTitle.text = _playerHandle.GetTranslation("permission");
            if (_permissionDesc != null) _permissionDesc.text = $"<color=#64B5F6>Owner</color>\t\t\t{_playerHandle.GetTranslation("ownerPermission")}\r\n<color=#BA68C8>Admin</color>\t\t\t{_playerHandle.GetTranslation("adminPermission")}\r\n<color=#81C784>Editor</color>\t\t\t{_playerHandle.GetTranslation("editorPermission")}\r\n<color=#FFB74D>Viewer</color>\t\t\t{_playerHandle.GetTranslation("viewerPermission")}";
        }

        public override void OnVideoReady() => UpdateUI();
        public override void OnVideoStart() => UpdateUI();
        public override void OnVideoEnd() => UpdateUI();
        public override void OnVideoPlay() => UpdateUI();
        public override void OnVideoPause() => UpdateUI();
        public override void OnVideoStop() => UpdateUI();
        public override void OnVideoError(VideoError videoError) => updateErrorView(videoError);
        public override void OnPlayerChanged() => UpdateUI();
        public override void OnLoopChanged() => updatePlaybackView();
        public override void OnRepeatChanged() => updatePlaybackView();
        public override void OnSpeedChanged() => updatePlaybackView();
        public override void OnLocalDelayChanged() => updatePlaybackView();
        public override void OnShufflePlayChanged() => updatePlaybackView();
        public override void OnTrackUpdated() => updateTrackView();
        public override void OnUrlChanged()
        {
            updateLoadingView();
            GeneratePlaylistTracksView();
        }
        public override void OnQueueUpdated() => GeneratePlaylistTracksView();
        public override void OnHistoryUpdated() => GeneratePlaylistTracksView();
        public override void OnVolumeChanged() => updateAudioView();
        public override void OnMuteChanged() => updateAudioView();
        public override void OnMaxResolutionChanged() => updateScreenView();
        public override void OnMirrorInversionChanged() => updateScreenView();
        public override void OnEmissionChanged() => updateScreenView();
        public override void OnKaraokeModeChanged() => updateKaraokeView();
        public override void OnLanguageChanged() 
        { 
            UpdateUI();
            UpdateTranslation();
            GeneratePlaylistView();
        }
        public override void OnPermissionChanged() => updatePermissionView();
    }
}