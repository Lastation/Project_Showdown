
using UnityEngine;
using UdonSharp;
using VRC.SDK3.Data;
using VRC.SDK3.StringLoading;
using VRC.SDKBase;
using Yamadev.YamachanWebUnit;

namespace Yamadev.YamaStream
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class YoutubeList : Receiver
    {
        [SerializeField] Client _client;
        [SerializeField] VRCUrl _callbackUrl = new VRCUrl("http://api.yamachan.moe/vrchat/list?target=youtube");
        UdonSharpBehaviour[] _listeners = { };
        bool _isLoding = false;
        DataDictionary _results = new DataDictionary();

        public void AddListener(UdonSharpBehaviour listener) => _listeners = _listeners.Add(listener);

        public bool IsLoading => _isLoding;
        public int Percentage => _client.GetPercentage();
        public DataDictionary Results => _results;

        public void GetList(string id)
        {
            if (_client.IsLoading) return;
            _isLoding = true;
            _client.Request(_callbackUrl, id, this);
            foreach (UdonSharpBehaviour listener in _listeners) listener.SendCustomEvent("OnRequestStart");
        }

        public override void OnRequestSuccess(IVRCStringDownload result)
        {
            if (!VRCJson.TryDeserializeFromJson(result.Result, out var json) || json.TokenType != TokenType.DataDictionary)
            {
                foreach (UdonSharpBehaviour listener in _listeners) listener.SendCustomEvent("OnRequestSuccess");
                return;
            }
            _results = json.DataDictionary;
            foreach (UdonSharpBehaviour listener in _listeners) listener.SendCustomEvent("OnRequestSuccess");
        }

        public override void OnRequestError()
        {
            _isLoding = false;
            foreach (UdonSharpBehaviour listener in _listeners) listener.SendCustomEvent("OnRequestError");
        }
    }
}