using CocktailSystem.src.glass;
using UdonSharp;
using UnityEngine;

namespace CocktailSystem.src.shaker
{
    public class ShakerParticle : UdonSharpBehaviour
    {
        private Shaker shaker;

        private void Start()
        {
            this.shaker = this.transform.parent.gameObject.GetComponent<Shaker>();
        }

        private void OnParticleCollision(GameObject other)
        {
            if (this.shaker.GetCocktailInfo().GetAmount() <= 0)
            {
                return;
            }
            
            if (other == null)
            {
                return;
            }
            

            var glass = other.GetComponent<Glass>();

            if (glass == null)
            {
                return;
            }

            if (glass.IsEmpty())
            {
                glass.SetCocktailInfo(this.shaker.GetCocktailInfo());
                glass.SetShakerName(this.shaker.GetCocktailInfo().GetShakerName());
                return;
            }

            if (!glass.GetShakerName().Equals(this.shaker.GetCocktailInfo().GetShakerName()))
            {
                return;
            }

            glass.AddAmount();
            this.shaker.GetCocktailInfo().ReduceAmount();
            if (this.shaker.GetCocktailInfo().GetAmount() <= 0)
            {
                this.shaker.SyncedClear();
            }
        }
    }
}