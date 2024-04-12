using UdonSharp;
using System;
using UnityEngine;
using VRC.SDK3.Data;

namespace Yamadev.YamaStream
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class i18n : UdonSharpBehaviour
    {
        [SerializeField] Controller _controller;
        [SerializeField] TextAsset _translation;
        DataToken _translationData;
        bool _initilized;
        string _language;
        string _defaultLanguage = "en";

        void Start() => Initilize();

        public void Initilize()
        {
            if (_initilized) return;
            if (!VRCJson.TryDeserializeFromJson(_translation.text, out _translationData))
            {
                Debug.Log($"[<color=#ff70ab>YamaStream</color>] Failed deserialize translation json: {_translationData.ToString()}");
                return;
            }
            _initilized = true;
            Language = GetLocalLanguage();
        }

        public string Language
        {
            get => _language;
            set
            {
                _language = value;
                _controller.SendCustomVideoEvent(nameof(Listener.OnLanguageChanged));
            }
        }

        public string GetValue(string key)
        {
            if (!_initilized) Initilize();
            if (_translationData.DataDictionary.TryGetValue(_language, out var tr))
                if (tr.DataDictionary.TryGetValue(key, out var value)) return value.String;
            return string.Empty;
        }

        public string GetLocalLanguage()
        {
            TimeZoneInfo tz = TimeZoneInfo.Local;
            switch (tz.Id)
            {
                case "Tokyo Standard Time":
                    return "ja";
                case "Taipei Standard Time":
                    return "zh-tw";
                case "China Standard Time":
                    return "zh-cn";
                case "Korea Standard Time":
                    return "ko";
                case "North Korea Standard Time":
                    return "ko";
                default:
                    return _defaultLanguage;
            }
        }
    }
}