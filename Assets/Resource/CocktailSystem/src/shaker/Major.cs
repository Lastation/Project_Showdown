using System.Runtime.CompilerServices;
using UdonSharp;
using UnityEngine;

namespace CocktailSystem.src.shaker
{
    public class Major : UdonSharpBehaviour
    {
        [SerializeField] private GameObject shortPoint;
        [SerializeField] private GameObject longPoint;

        [SerializeField] private Animator anim;
        private Liqueur liqueur;
        [SerializeField] private Transform level;
        [SerializeField] private Renderer renderer;

        private int amount;
        private bool liqueurFlag = false;
        
        /**
         * 0 決まってない
         * 1 短い方
         * 2 長い方
         */
        [UdonSynced(UdonSyncMode.None)]
        private int useSide;

        private void Update()
        {
            if (null == liqueur)
            {
                anim.SetFloat("shortSide", 0f);
                anim.SetFloat("longSide", 0f);
                shortPoint.SetActive(false);
                longPoint.SetActive(false);
                Clear();
                return;
            }

            if (useSide == 0)
            {
                anim.SetFloat("shortSide", 0f);
                anim.SetFloat("longSide", 0f);
                shortPoint.SetActive(false);
                longPoint.SetActive(false);
                Clear();
                return;
            }

            anim.SetFloat(this.useSide == 1 ? "shortSide" : "longSide", (float) this.amount / 100);
            if (Mathf.Abs(level.rotation.x) >= 0.4 ||
                Mathf.Abs(level.transform.rotation.z) >= 0.4)
            {
                if (useSide == 2)
                {
                    longPoint.SetActive(true);
                    shortPoint.SetActive(false);
                }
                else if (useSide == 1)
                {
                    shortPoint.SetActive(true);
                    longPoint.SetActive(false);
                }
            }
            else
            {
                if (useSide == 2)
                {
                    shortPoint.SetActive(false);
                    longPoint.SetActive(false);
                }
                else if (useSide == 1)
                {
                    longPoint.SetActive(false);
                    shortPoint.SetActive(false);
                }
            }
        }

        public void Clear()
        {
            liqueur = null;
            this.useSide = 0;
            amount = 0;
            liqueurFlag = false;
        }

        public void SetUseSide(int tmp)
        {
            this.useSide = tmp;
            this.level.rotation = Quaternion.Euler(0, 0, 0);
        }

        public int GetUseSide()
        {
            return this.useSide;
        }

        public Liqueur GetLiqueur()
        {
            return this.liqueur;
        }

        public void SetLiqueur(Liqueur tmp)
        {
            this.liqueur = tmp;
            this.liqueurFlag = true;
        }

        public int GetAmount()
        {
            return amount;
        }

        public bool GetLiqueurFlag()
        {
            return this.liqueurFlag;
        }

        public void ResetLiqueurFlag()
        {
            this.liqueurFlag = false;
        }

        public void ReduceAmount(int tmp)
        {
            amount -= tmp;
        }

        public void AddAmount(int tmp)
        {
            amount += tmp * useSide;

            if (amount >= 100)
            {
                amount = 100;
            }
        }

        public void SetMaterial(Material material)
        {
            this.renderer.material = material;
        }
    }
}