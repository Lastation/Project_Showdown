using CocktailSystem.src.shaker;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;

namespace CocktailSystem.src
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
    public class DebugUI : UdonSharpBehaviour
    {
        [SerializeField] private Dropdown dropdown;
        [SerializeField] private GameObject parent;
        private Liqueur[] materials;
        [SerializeField] private Shaker shaker;

        private void Start()
        {
            materials = this.parent.GetComponentsInChildren<Liqueur>();
        }

        public override void Interact()
        {
            Debug.Log(dropdown.itemText);
            shaker.AddReserve(dropdown.value);
            return;
        }
        
        public void changeProg()
        {
            shaker.ChangeStatus();
        }
    }
}
