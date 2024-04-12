
using VRC.SDK3.Data;
using UnityEngine;
using UnityEngine.UI;
using UdonSharp;

namespace Yamadev.YamaStream.Modules
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class YoutubeUIHandle : Listener
    {
        [SerializeField] YamaPlayerHandle _playerHandle;
        [SerializeField] Youtube _youtube;
        [SerializeField] Text _keyword;
        [SerializeField] Transform _content;
        [SerializeField] GameObject _loadingPage;
        [SerializeField] Text _loadingText;

        void Start() 
        {
            _playerHandle.AddListener(this);
            _youtube.AddListener(this);
        }
        void Update()
        {
            if (!_youtube.IsLoading) return;
            if (_loadingText != null)
                _loadingText.text = $"{_playerHandle.GetTranslation("inLoading")}({_youtube.Percentage})...";
        }

        public void Search()
        {
            if (_keyword == null || string.IsNullOrEmpty(_keyword.text)) return;
            _youtube.Search(_keyword.text);
        }
        public void Play()
        {
            if (_content == null) return;
            Toggle[] toggles = _content.GetComponentsInChildren<Toggle>();
            Toggle toggle = toggles.GetFirstActiveToggle();
            if (toggle != null)
            {
                toggle.SetIsOnWithoutNotify(false);
                int index = toggle.transform.GetSiblingIndex();
                _youtube.PlayIndex(index);
            }
        }
        public void AddToQueue()
        {
            if (_content == null) return;
            Toggle[] toggles = _content.GetComponentsInChildren<Toggle>();
            Toggle toggle = toggles.GetFirstActiveToggle();
            if (toggle != null)
            {
                toggle.SetIsOnWithoutNotify(false);
                int index = toggle.transform.GetSiblingIndex();
                _youtube.AddToQueue(index);
            }
        }

        void generateView()
        {
            if (_content == null) return;
            for (int i = 0; i < _content.childCount; i++)
            {
                _content.GetChild(i).gameObject.SetActive(false);
                if (i >= _youtube.Results.Count) continue;
                if (_youtube.Results.TryGetValue(i, TokenType.DataDictionary, out var v))
                {
                    Text title = _content.GetChild(i).Find("Title").GetComponent<Text>();
                    Text channelTitle = _content.GetChild(i).Find("Channel").GetComponentInChildren<Text>();
                    Text desc = _content.GetChild(i).Find("Description").GetComponentInChildren<Text>();
                    Text id = _content.GetChild(i).Find("ID").GetComponent<Text>();
                    // RawImage thumbnail = _contents.GetChild(i).Find("Thumbnail").GetComponent<RawImage>();

                    if (v.DataDictionary.TryGetValue("title", TokenType.String, out var titleValue))
                        title.text = titleValue.String;
                    if (v.DataDictionary.TryGetValue("channelTitle", TokenType.String, out var channelTitleValue))
                        channelTitle.text = channelTitleValue.String;
                    if (v.DataDictionary.TryGetValue("description", TokenType.String, out var descValue))
                        desc.text = descValue.String;
                    if (v.DataDictionary.TryGetValue("id", TokenType.String, out var idValue))
                        id.text = idValue.String;

                    InputField inputField = _content.GetChild(i).Find("Actions").GetComponentInChildren<InputField>();
                    if (inputField != null) inputField.text = $"https://www.youtube.com/watch?v={idValue.String}";
                }
                _content.GetChild(i).gameObject.SetActive(true);
            }
        }
        void updateTranslation()
        {
            if (_content == null) return;
            for (int i = 0; i < _content.childCount; i++)
            {
                Transform child = _content.GetChild(i);
                Transform copyLink = child.Find("Actions/CopyLink/Text");
                if (copyLink != null) copyLink.GetComponent<Text>().text = _playerHandle.GetTranslation("copyLink");
                Transform playVideo = child.Find("Actions/Play/Text");
                if (playVideo != null) playVideo.GetComponent<Text>().text = _playerHandle.GetTranslation("playVideo");
                Transform addQueue = child.Find("Actions/AddToQueue/Text");
                if (addQueue != null) addQueue.GetComponent<Text>().text = _playerHandle.GetTranslation("addQueue");
            }
        }

        public void OnRequestStart() => _loadingPage.SetActive(true);
        public void OnRequestSuccess()
        {
            generateView();
            _loadingPage.SetActive(false);
        }
        public void OnRequestError() => _loadingPage.SetActive(false);
        public override void OnLanguageChanged() => updateTranslation();
    }
}