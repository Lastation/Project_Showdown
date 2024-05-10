using System;
using UdonSharp;
using UnityEngine;

namespace CocktailSystem.src.shaker
{
    public class Ice : UdonSharpBehaviour
    {
        [UdonSynced(UdonSyncMode.None)]
        private bool iceFlag = false;
        [SerializeField] private Animator anim;

        private void Update()
        {
            anim.SetBool("iceFlag", iceFlag);
        }

        public bool HasIce()
        {
            return iceFlag;
        }

        public void SetIce()
        {
            this.iceFlag = true;
        }
        
        public void ReSetIce()
        {
            this.iceFlag = false;
        }
    }
}
