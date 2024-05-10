
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

namespace Yamadev.VRCHandMenu
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class SwitchHandle : UdonSharpBehaviour
    {
        [SerializeField]
        Image _icon;

        [SerializeField]
        GameObject targetObject;

        Color _activeColor = new Color(0.0f, 200.0f / 255.0f, 83.0f / 255.0f, 255.0f);
        Color _inactiveColor = new Color(197.0f / 255.0f, 17.0f / 255.0f, 98.0f / 255.0f, 255.0f);

        public void SetActive()
        {
            targetObject.SetActive(true); 
            _icon.color = _activeColor;
        }

        public void SetInactive()
        {
            targetObject.SetActive(false); 
            _icon.color = _inactiveColor;
        }
    }
}