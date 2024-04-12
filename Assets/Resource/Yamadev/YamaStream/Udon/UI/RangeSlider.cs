
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;


namespace Yamadev.YamaStream.UI
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class RangeSlider : UdonSharpBehaviour
    {
        public Slider SliderLeft;
        public Slider SliderRight;
        public RectTransform Fill;

        bool _initilized = false;
        float _width;

        void Start() => Initilize();

        public void Initilize()
        {
            if (_initilized) return;
            _width = GetComponent<RectTransform>().rect.width;
            _initilized = true;
        }

        public void OnSliderValueChanged()
        {
            if (SliderLeft.value > SliderRight.value) SliderRight.value = SliderLeft.value;
            if (SliderRight.value < SliderLeft.value) SliderLeft.value = SliderRight.value;

            FitFillArea();
        }

        public void FitFillArea()
        {
            if (!_initilized) Initilize();

            float left = _width * SliderLeft.value;
            float right = _width * (1 - SliderRight.value);
            Fill.offsetMin = new Vector2(left, -7);
            Fill.offsetMax = new Vector2(-right, -7);
        }
    }
}