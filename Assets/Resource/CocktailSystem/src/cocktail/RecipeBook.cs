using CocktailSystem.src.bartender;
using Holdem;
using UdonSharp;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UI;

namespace CocktailSystem.src.cocktail
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
    public class RecipeBook : UdonSharpBehaviour
    {
        [SerializeField] private GameObject recipeParent;
        [SerializeField] private GameObject LiqueurParent;
        [SerializeField] private Text text;
        [SerializeField] private MainSystem mainSystem;
        [SerializeField] private Dropdown[] dropdown;

        private Recipe[] recipeList;
        private Liqueur[] liqueurList;
        private Text text_dropdown;

        void Start()
        {
            recipeList = recipeParent.GetComponentsInChildren<Recipe>();
            liqueurList = LiqueurParent.GetComponentsInChildren<Liqueur>();
        }
        
        public void viewRecipe()
        {
            for (int i = 0; i < 3; i++)
            {
                if ((int)mainSystem.Get_Language != i)
                    dropdown[i].gameObject.SetActive(false);
                else
                {
                    dropdown[i].gameObject.SetActive(true);
                    text_dropdown = dropdown[i].captionText;
                }
            }

            for (int i = 0; i < recipeList.Length; i++)
            {
                if (recipeList[i].GetCocktailName() != text_dropdown.text)
                {
                    continue;
                }

                var tmpLiqueurList = recipeList[i].GetLiqueurList();
                var tmpText = "";
                for (var j = 0; j < tmpLiqueurList.Length; j++)
                {
                    for (var k = 0; k < this.liqueurList.Length; k++)
                    {
                        if (tmpLiqueurList[j].GetNo() == liqueurList[k].GetNo())
                        {
                            tmpText += liqueurList[k].GetName(mainSystem.Get_Language) + " x" + tmpLiqueurList[j].GetRequiredAmount() + "\n";
                            break;
                        }
                    }
                    
                }

                text.text = tmpText;
                return;
            }
        }
    }
}
