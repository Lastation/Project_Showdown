
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;

namespace Yamadev.YamaStream.UI
{
    [RequireComponent(typeof(Toggle))]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class UIToggle : UdonSharpBehaviour
    {
        [SerializeField] UIController _uiController;
        Toggle _toggle;
        Text _text;
        Image _icon;
        void Start()
        {
            _toggle = GetComponent<Toggle>();
            Transform text = transform.Find("Text");
            if (text != null) _text = text.GetComponent<Text>();
            Transform icon = transform.Find("Icon");
            if (icon != null) _icon = icon.GetComponent<Image>();
        }

        Color _color => _toggle.isOn ? _uiController.PrimaryColor : Color.white;
        public void OnChange()
        {
            if (_text != null) _text.color = _color;
            if (_icon != null) _icon.color = _color;
        }
    }
}