
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.SDK3.StringLoading;
using VRC.SDKBase;
using Yamadev.YamachanWebUnit;

namespace Yamadev.YamaStream.Modules
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class Youtube : Receiver
    {
        [SerializeField] Controller _controller;
        [SerializeField] Client _client;
        [SerializeField] VRCUrl _callbackUrl = new VRCUrl("https://api.yamachan.moe/vrchat/search?target=youtube");
        UdonSharpBehaviour[] _listeners = { };
        bool _isLoding = false;
        DataList _json = new DataList();

        public void AddListener(UdonSharpBehaviour listener) => _listeners = _listeners.Add(listener);
        public bool IsLoading => _isLoding;
        public int Percentage => _client.GetPercentage();
        public DataList Results => _json;

        public void Search(string keyword)
        {
            if (_client.IsLoading) return;
            _isLoding = true;
            _client.Request(_callbackUrl, keyword, this);
            foreach (UdonSharpBehaviour listener in _listeners) listener.SendCustomEvent("OnRequestStart");
        }

        public override void OnRequestSuccess(IVRCStringDownload result)
        {
            if (!VRCJson.TryDeserializeFromJson(result.Result, out var json) || json.TokenType != TokenType.DataList)
            {
                foreach (UdonSharpBehaviour listener in _listeners) listener.SendCustomEvent("OnRequestError");
                return;
            }
            _json = json.DataList;
            _isLoding = false;
            foreach (UdonSharpBehaviour listener in _listeners) listener.SendCustomEvent("OnRequestSuccess");
        }

        public override void OnRequestError()
        {
            _isLoding = false;
            foreach (UdonSharpBehaviour listener in _listeners) listener.SendCustomEvent("OnRequestError");
        }

        public Track GetTrackByIndex(int index)
        {
            Track track = Track.New(VideoPlayerType.UnityVideoPlayer, "", VRCUrl.Empty);
            if (_json.TryGetValue(index, TokenType.DataDictionary, out var v))
                if (v.DataDictionary.TryGetValue("title", TokenType.String, out var title) &&
                    v.DataDictionary.TryGetValue("id", TokenType.String, out var id) &&
                    v.DataDictionary.TryGetValue("live", TokenType.Boolean, out var live))
                    track = Track.New(live.Boolean ? VideoPlayerType.AVProVideoPlayer : VideoPlayerType.UnityVideoPlayer, title.String, VRCUrl.Empty, $"https://www.youtube.com/watch?v={id}");
            return track;
        }

        public void PlayIndex(int index)
        {
            Track track = GetTrackByIndex(index);
            _controller.PlayTrack(track);
        }

        public void AddToQueue(int index)
        {
            Track track = GetTrackByIndex(index);
            _controller.Queue.AddTrack(track);
            if (!_controller.IsLocal) _controller.Queue.SyncVariables();
        }
    }
}