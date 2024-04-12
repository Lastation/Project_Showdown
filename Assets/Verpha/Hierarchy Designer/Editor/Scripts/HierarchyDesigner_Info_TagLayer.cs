#if UNITY_EDITOR
using UnityEngine;

namespace Verpha.HierarchyDesigner
{
    [System.Serializable]
    public class HierarchyDesigner_Info_TagLayer
    {
        private Color textColor = Color.white;
        private FontStyle fontStyle = FontStyle.Normal;
        private int fontSize = 12;

        public Color TextColor
        {
            get { return textColor; }
            set { textColor = value; }
        }

        public FontStyle FontStyle
        {
            get { return fontStyle; }
            set { fontStyle = value; }
        }

        public int FontSize
        {
            get { return fontSize; }
            set { fontSize = value; }
        }

        public HierarchyDesigner_Info_TagLayer(Color textColor, FontStyle fontStyle, int fontSize)
        {
            this.textColor = textColor;
            this.fontStyle = fontStyle;
            this.fontSize = fontSize;
        }
    }
}
#endif