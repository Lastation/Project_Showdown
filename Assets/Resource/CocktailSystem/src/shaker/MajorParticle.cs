using UdonSharp;
using UnityEngine;

namespace CocktailSystem.src.shaker
{
    public class MajorParticle : UdonSharpBehaviour
    {
        private Major major;

        private void Start()
        {
            this.major = this.transform.parent.gameObject.GetComponent<Major>();
        }

        private void OnParticleCollision(GameObject other)
        {
            if (null == other)
            {
                return;
            }

            if (null == major.GetLiqueur())
            {
                return;
            }

            var shaker = other.GetComponent<Shaker>();
            if (shaker == null)
            {
                return;
            }


            // 一発目だけリキュールの情報入れたい
            if (major.GetLiqueurFlag())
            {
                shaker.AddReserve(this.major.GetLiqueur().GetNo());
                if (this.major.GetUseSide() == 2)
                {
                    shaker.SetIsLong(true);
                }
                else
                {
                    shaker.SetIsLong(false);
                }
                major.ResetLiqueurFlag();
            }
            
            major.ReduceAmount(2);
            if (major.GetAmount() <= 0)
            {
                this.major.Clear();
            }
        }
    }
}
