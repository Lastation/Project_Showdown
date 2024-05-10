using UdonSharp;
using UnityEngine;

namespace CocktailSystem.src.shaker
{
    public class MajorCol : UdonSharpBehaviour
    {
        private Major major;
        [SerializeField] private bool isLongSide;
        
        void Start()
        {
            this.major = this.transform.parent.gameObject.GetComponent<Major>();
        }

        public Major GetMajor()
        {
            return major;
        }

        public bool IsLongSide()
        {
            return this.isLongSide;
        }
    }
}
