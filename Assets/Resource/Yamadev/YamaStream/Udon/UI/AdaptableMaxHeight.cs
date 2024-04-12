
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;

namespace Yamadev.YamaStream.UI
{
    [RequireComponent(typeof(ScrollRect))]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class AdaptableMaxHeight : UdonSharpBehaviour
    {
        [SerializeField] float _maxHeight;
        ScrollRect _scrollRect;
        RectTransform _rect;

        void Start()
        {
            _rect = GetComponent<RectTransform>();
            _scrollRect = GetComponent<ScrollRect>();
        }
        void Update()
        {
            float contentHeight = _scrollRect.content.sizeDelta.y;
            if (contentHeight <= _maxHeight && _scrollRect.enabled)
            {
                _scrollRect.enabled = false;
                _rect.sizeDelta = new Vector2(_rect.sizeDelta.x, contentHeight);
            }
            if (contentHeight > _maxHeight && !_scrollRect.enabled)
            {
                _scrollRect.enabled = true;
                _rect.sizeDelta = new Vector2(_rect.sizeDelta.x, _maxHeight);
            }
        }
    }
}