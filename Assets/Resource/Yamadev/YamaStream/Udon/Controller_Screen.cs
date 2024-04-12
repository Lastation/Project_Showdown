
using UnityEngine;
using UnityEngine.UI;

namespace Yamadev.YamaStream
{
    public partial class Controller : Listener
    {
        [SerializeField] Animator _animator;
        [SerializeField] int _maxResolution;
        [SerializeField] bool _mirrorInverse = true;
        [SerializeField, Range(0f, 1f)] float _emission = 1f;
        [SerializeField] Renderer[] _renderScreens;
        [SerializeField] RawImage[] _rawImageScreens;
        [SerializeField] Material _lod;
        MaterialPropertyBlock _properties;

        void initilizeScreen()
        {
            _animator.Rebind();
            MirrorInverse = _mirrorInverse;
            MaxResolution = _maxResolution;
            Emission = _emission;
            updateProperties();
        }

        public MaterialPropertyBlock MaterialProperty
        {
            get
            {
                if (_properties == null ) _properties = new MaterialPropertyBlock();
                return _properties;
            }
        }
        public Texture Texture => VideoPlayerHandle.Texture;

        public Renderer[] RenderScreens 
        { 
            get => _renderScreens; 
            set => _renderScreens = value;
        }
        public RawImage[] RawImageScreens 
        { 
            get => _rawImageScreens; 
            set => _rawImageScreens = value;
        }

        public int MaxResolution
        {
            get => _maxResolution;
            set
            {
                bool valueChanged = value != _maxResolution;
                _maxResolution = value;
                _animator.SetFloat("Resolution", value / 4320f);
                _animator.Update(0f);
                if (!_stopped) SendCustomEventDelayedFrames(nameof(Reload), 1);
                SendCustomVideoEvent(nameof(Listener.OnMaxResolutionChanged));
            }
        }

        public bool MirrorInverse
        {
            get => _mirrorInverse;
            set
            {
                _mirrorInverse = value;
                MaterialProperty.SetInt("_InversionInMirror", value ? 1 : 0);
                SendCustomVideoEvent(nameof(Listener.OnMirrorInversionChanged));
            }
        }

        public float Emission
        {
            get => _emission;
            set
            {
                _emission = value;
                MaterialProperty.SetFloat("_Emission", value);
                SendCustomVideoEvent(nameof(Listener.OnEmissionChanged));
            }
        }

        void renderScreen()
        {
            if (!IsPlaying || Texture == null) return;

            MaterialProperty.SetTexture("_MainTex", Texture);
            foreach (Renderer renderer in _renderScreens) renderer.SetPropertyBlock(_properties, 0);
            foreach (RawImage image in _rawImageScreens) image.texture = Texture;
            if (_lod != null) _lod.SetTexture("_MainTex", Texture);
        }

        void updateProperties()
        {
            int isAVPro = VideoPlayerType == VideoPlayerType.AVProVideoPlayer ? 1 : 0;
            MaterialProperty.SetInt("_AVPro", isAVPro);
            foreach (RawImage image in _rawImageScreens) image.material.SetInt("_AVPro", isAVPro);
            if (_lod != null) _lod.SetInt("_AVPro", isAVPro);
        }
    }
}