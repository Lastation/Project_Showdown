
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace Yamadev.YamaStream
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class Playlist : UdonSharpBehaviour
    {
        [SerializeField] Controller _controller;
        [SerializeField] bool _isStatic = true;
        [SerializeField] bool _isQueue = false;
        [SerializeField] bool _isHistory = false;
        [SerializeField, UdonSynced] string _playlistName = "";
        [SerializeField, UdonSynced] VideoPlayerType[] _videoPlayerTypes = new VideoPlayerType[0];
        [SerializeField, UdonSynced] string[] _titles = new string[0];
        [SerializeField, UdonSynced] VRCUrl[] _urls = new VRCUrl[0];
        [SerializeField, UdonSynced] string[] _originalUrls = new string[0];

        public string PlaylistName => _playlistName;
        public bool IsStatic => _isStatic;
        public int Length => _urls.Length;

        public Track GetTrack(int index)
        {
            if (index > Length) return null;
            return Track.New(_videoPlayerTypes[index], _titles[index], _urls[index], _originalUrls[index]);
        }

        public void AddTrack(Track track)
        {
            _videoPlayerTypes = _videoPlayerTypes.Add(track.GetPlayer());
            _titles = _titles.Add(track.GetTitle());
            _originalUrls = _originalUrls.Add(track.GetOriginalUrl());
            _urls = _urls.Add(track.GetVRCUrl());
            SendEvent();
        }

        public void Remove(int index)
        {
            _videoPlayerTypes = _videoPlayerTypes.Remove(index);
            _titles = _titles.Remove(index);
            _originalUrls = _originalUrls.Remove(index);
            _urls = _urls.Remove(index);
            SendEvent();
        }
        public void SendEvent()
        {
            if (_isQueue) _controller.SendCustomVideoEvent(nameof(Listener.OnQueueUpdated));
            if (_isHistory) _controller.SendCustomVideoEvent(nameof(Listener.OnHistoryUpdated));
        }

        public override void OnDeserialization()
        {
            SendEvent();
        }
    }
}