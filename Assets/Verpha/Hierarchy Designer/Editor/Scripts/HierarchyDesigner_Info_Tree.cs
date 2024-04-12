#if UNITY_EDITOR
using UnityEngine;

namespace Verpha.HierarchyDesigner
{
    [System.Serializable]
    public class HierarchyDesigner_Info_Tree
    {
        private Color color = Color.white;

        public Color Color
        {
            get { return color; }
            set { color = value; }
        }

        public HierarchyDesigner_Info_Tree(Color color)
        {
            this.color = color;
        }
    }
}
#endif