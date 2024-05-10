using UdonSharp;
using UnityEngine;

namespace CocktailSystem.src
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class CocktailInfo : UdonSharpBehaviour
    {
        private string shakerName;
        private int amount = 0;
        [UdonSynced(UdonSyncMode.None)]
        private string cocktailName = "";
        [UdonSynced(UdonSyncMode.None)]
        private int point = 0;
        [UdonSynced(UdonSyncMode.None)]
        private string prefix = "";
        private Material mat = null;
        
        void Start()
        {
            shakerName = this.gameObject.name;
        }

        public void Clear()
        {
            this.amount = 0;
            this.cocktailName = "";
            this.point = 0;
            this.prefix = "";
        }

        public void AddAmount(int amount)
        {
            this.amount += amount;
        }

        public void ReduceAmount()
        {
            this.amount--;
        }

        public int GetAmount()
        {
            return amount;
        }

        public string GetShakerName()
        {
            return this.shakerName;
        }

        public void SetCocktailName(string name)
        {
            this.cocktailName = name;
            RequestSerialization();
        }

        public void SetPoint(int point)
        {
            this.point = point;
            RequestSerialization();
        }
        
        public void SetPrefix(string prefix)
        {
            this.prefix = prefix;
            RequestSerialization();
        }

        public string getText()
        {
            return prefix + cocktailName + "\n"
                   + point + "点";
        }

        public void SetMaterial(Material material)
        {
            this.mat = material;
        }

        public Material GetMaterial()
        {
            return this.mat;
        }
    }
}
