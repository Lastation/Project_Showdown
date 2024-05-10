using UdonSharp;
using UnityEngine;

namespace CocktailSystem.src.shaker
{
    public class IceBox : UdonSharpBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other == null)
            {
                return;
            }

            var ice = other.gameObject.GetComponent<Ice>();
            if (ice == null)
            {
                return;
            }

            if (ice.HasIce())
            {
                return;
            }

            ice.SetIce();
        }
    }
}
