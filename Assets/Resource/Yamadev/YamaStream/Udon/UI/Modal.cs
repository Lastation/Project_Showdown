
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;

namespace Yamadev.YamaStream.UI
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class Modal : UdonSharpBehaviour
    {
        [SerializeField] Text _title;
        [SerializeField] Text _message;
        [SerializeField] ScrollRect _scrollRect;
        [SerializeField] Button _close;
        [SerializeField] Button _execute;
        [SerializeField] Button _execute2;
        [SerializeField] Text _closeText;
        [SerializeField] Text _executeText;
        [SerializeField] Text _execute2Text;
        [SerializeField] float _maxHeight;
        UdonEvent _event;
        UdonEvent _event2;

        RectTransform _scrollRectTransform => _scrollRect.GetComponent<RectTransform>();
        public bool IsActive => gameObject.activeSelf;

        public string Title
        {
            get => _title.text;
            set => _title.text = value;
        }

        public string Message
        {
            get => _message.text;
            set
            {
                _message.text = value;
                AdaptMaxHeight();
            }
        }

        public UdonEvent Callback
        {
            get => _event;
            set => _event = value;
        }

        public void Open() => gameObject.SetActive(true);
        public void Close() => gameObject.SetActive(false);
        public void Execute() => _event.Invoke();
        public void Execute2() => _event2.Invoke();

        public void Show(string title, string message, UdonEvent callback, UdonEvent callback2, string closeText, string executeText, string execute2Text, bool showClose = true, bool showExecute = true, bool showExecute2 = true)
        {
            Title = title;
            Message = message;
            _event = callback;
            _event2 = callback2;
            _closeText.text = closeText;
            _executeText.text = executeText;
            _execute2Text.text = execute2Text;
            _close.gameObject.SetActive(showClose);
            _execute.gameObject.SetActive(showExecute);
            _execute2.gameObject.SetActive(showExecute2);
            Open();
        }
        public void Show(string title, string message, UdonEvent callback, string closeText, string executeText, bool showClose = true, bool showExecute = true)
            => Show(title, message, callback, null, closeText, executeText, "", showClose, showExecute, false);
        void AdaptMaxHeight()
        {
            float contentHeight = _scrollRect.content.sizeDelta.y;
            if (contentHeight <= _maxHeight && _scrollRect.enabled)
            {
                _scrollRect.vertical = false;
                _scrollRectTransform.sizeDelta = new Vector2(_scrollRectTransform.sizeDelta.x, contentHeight);
            }
            if (contentHeight > _maxHeight && !_scrollRect.enabled)
            {
                _scrollRect.vertical = true;
                _scrollRectTransform.sizeDelta = new Vector2(_scrollRectTransform.sizeDelta.x, _maxHeight);
            }
        }
    }
}