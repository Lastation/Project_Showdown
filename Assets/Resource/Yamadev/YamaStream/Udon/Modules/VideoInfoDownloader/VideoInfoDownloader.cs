
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.SDK3.StringLoading;
using VRC.SDKBase;
using VRC.Udon.Common.Interfaces;

namespace Yamadev.YamaStream.Modules
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class VideoInfoDownloader : Listener
    {
        [SerializeField] Controller _controller;
        VRCUrl _url = VRCUrl.Empty;

        DataDictionary supportUrls = new DataDictionary()
        {
            {"https://www.youtube.com", "YOUTUBE"},
            {"https://youtube.com", "YOUTUBE" },
            // {"https://youtu.be", "YOUTUBE" },
            {"https://www.twitch.tv", "TWITCH" },
            {"https://twitch.tv", "TWITCH" },
            {"https://www.nicovideo.jp", "NICONICO" },
            {"https://nicovideo.jp", "NICONICO" },
        };

        void Start() => _controller.AddListener(this);

        public void LoadTitle()
        {
            Track track = _controller.Track;

            if (track.GetTitle() != "" || track.GetOriginalUrl() != "") return;
            GetTitle(track.GetVRCUrl());
        }

        public string GetSite(string url)
        {
            DataList keys = supportUrls.GetKeys();
            for (int i = 0; i < keys.Count; i++)
            {
                DataToken key = keys[i];
                if (url.StartsWith(key.String))
                {
                    return supportUrls[key].String;
                }
            }
            return "";
        }

        public void GetTitle(VRCUrl url)
        {
            Debug.Log($"[<color=#ff70ab>YamaStream</color>] Try get video info: {url}");
            if (GetSite(url.Get()) == "") return;
            _url = url;
            VRCStringDownloader.LoadUrl(url, (IUdonEventReceiver)this);
        }

        public override void OnStringLoadSuccess(IVRCStringDownload result)
        {
            string site = GetSite(_url.Get());
            string title = string.Empty;
            switch (site)
            {
                case "YOUTUBE":
                    title = Utils.FindSubString(result.Result, new string[] { "\"videoDetails\":", "title\":\"" }, '"');
                    break;
                case "TWITCH":
                    title = Utils.FindSubString(result.Result, new string[] { "\"name\":\"" }, '"');
                    break;
                case "NICONICO":
                    title = Utils.FindSubString(result.Result, new string[] { "\"@type\":\"VideoObject\"", "name\":\"" }, '"');
                    break;
                default: break;
            }
            Track track = _controller.Track;
            _controller.Track = Track.New(track.GetPlayer(), title, track.GetVRCUrl(), track.GetOriginalUrl(), track.GetDetails());
        }

        public override void OnUrlChanged() => LoadTitle();
        public override void OnVideoStart()
        {
            if (_controller.IsReload) LoadTitle();
        }
    }
}