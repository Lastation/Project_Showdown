using Holdem;
using UdonSharp;
using UnityEngine;

namespace CocktailSystem.src
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
    public class Liqueur : UdonSharpBehaviour
    {
        [SerializeField] private int no;
        [SerializeField] private bool isAlcohol;
        [SerializeField] private int requiredAmount = 1;
        [SerializeField] private string[] s_liqueurName;

        public int GetNo()
        {
            return no;
        }

        public string GetName(LocalizationType type)
        {
            return s_liqueurName[(int)type];
        }

        public bool IsAlcohol()
        {
            return isAlcohol;
        }

        public int GetRequiredAmount()
        {
            return requiredAmount;
        }
    }
}