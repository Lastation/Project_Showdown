
using Holdem;
using System;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;


namespace Yamadev.VRCHandMenu
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
    public class TimeHandle : UdonSharpBehaviour
    {
        [SerializeField]
        Text DateText;
        [SerializeField]
        Text TimeText;
        [SerializeField]
        MainSystem mainSystem;

        float _timeGap = 0f;
        TimeEventListener[] _listeners;

        void Start()
        {

        }

        void Update()
        {
            if (_timeGap < 1.0f) { 
                _timeGap += Time.deltaTime;
            }

            _timeGap = 0f;
            updateTime();

            if (_listeners != null) foreach (var i in _listeners) i.TimeSecondEvent();
        }

        public void AddListener(UdonSharpBehaviour listener)
        {
            if (_listeners == null) _listeners = new TimeEventListener[0];
            TimeEventListener[] ret = new TimeEventListener[_listeners.Length + 1];
            _listeners.CopyTo(ret, 0);
            ret[_listeners.Length] = (TimeEventListener)listener;
            _listeners = ret;
        }

        void updateTime()
        {
            if (!DateText || !TimeText) { return; }

            DateTime now = DateTime.Now;
            DateText.text = $"{now.ToString("yyyy/MM/dd")} {mainSystem.s_DayOfWeek((int)now.DayOfWeek)}";
            TimeText.text = now.ToString("HH:mm:ss");
        }

    }

}