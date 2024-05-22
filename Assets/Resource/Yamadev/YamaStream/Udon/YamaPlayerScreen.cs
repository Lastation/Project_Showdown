
using UdonSharp;
using UnityEngine;

namespace Yamadev.YamaStream
{
    [RequireComponent(typeof(Renderer))]
    [UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
    public class YamaPlayerScreen : UdonSharpBehaviour
    {
        [SerializeField] YamaPlayerHandle _playerHandle;
        void Start()
        {
            if (_playerHandle == null) return;
            Renderer screen = GetComponent<Renderer>();
            _playerHandle.RenderScreens = _playerHandle.RenderScreens.Add(screen);
        }
    }
}