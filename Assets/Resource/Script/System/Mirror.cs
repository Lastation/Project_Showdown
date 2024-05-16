using UdonSharp;
using UnityEngine;

namespace Assets.Resource.Script.System
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
    internal class Mirror : UdonSharpBehaviour
    {
        public GameObject Menu;
        public void Toggle_Menu() => Menu.SetActive(!Menu.activeSelf);
    }
}
