using System;
using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;

namespace CocktailSystem.src.glass
{
    public class Glass : UdonSharpBehaviour
    {
        [SerializeField] private Text textUi;
        private CocktailInfo cocktailInfo;

        [SerializeField] private Renderer rend;
        
        private string text;

        [UdonSynced(UdonSyncMode.None)]
        private int amount;

        private bool empty = true;

        private int count = 0;
        private bool isPickup = false;
        private string shakerName = "";
        [SerializeField] private Animator anim;

        private void Update()
        {
            count++;
            if (count <= 15)
            {
                return;
            }

            anim.SetFloat("amount", (float)this.amount / 100);

            if (amount <= 0)
            {
                this.Clear();
            }

            textUi.text = this.text;
            if (isPickup)
            {
                amount -= 1;
            }
        }

        private void Clear()
        {
            this.cocktailInfo = null;
            this.amount = 0;
            this.empty = true;
            this.text = "";
            this.shakerName = "";
        }

        public void SetCocktailInfo(CocktailInfo info)
        {
            this.cocktailInfo = info;
            this.text = this.cocktailInfo.getText();
            this.empty = false;
            this.amount++;
            rend.material = info.GetMaterial();
        }

        public void SetShakerName(string tmpName)
        {
            this.shakerName = tmpName;
        }

        public string GetShakerName()
        {
            return this.shakerName;
        }

        public bool IsEmpty()
        {
            return this.empty;
        }

        public void AddAmount()
        {
            this.amount++;
            if (amount >= 100)
            {
                amount = 100;
            }
        }

        public override void OnPickupUseDown()
        {
            this.isPickup = true;
        }

        public override void OnPickupUseUp()
        {
            this.isPickup = false;
        }
    }
}