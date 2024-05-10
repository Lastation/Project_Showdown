using UdonSharp;
using UnityEngine;

namespace CocktailSystem.src.shaker
{
    public class ShakerPickup : UdonSharpBehaviour
    {
        [SerializeField] private Shaker shaker;

        public override void OnPickupUseDown()
        {
            shaker.ChangeStatus();
        }
        public override void OnPickup()
        {
            shaker.Pickup();
        }

        public override void OnDrop()
        {
            shaker.Drop();
        }
    }
}
