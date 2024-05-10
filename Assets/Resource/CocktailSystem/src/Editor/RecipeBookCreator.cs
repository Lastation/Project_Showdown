using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UI;

namespace CocktailSystem.src.Editor
{
    public class RecipeBookCreator : EditorWindow
    {
        private GameObject recipeBookParent;
        private Dropdown dropdown;
        
        [MenuItem ("こあられのアトリエ/レシピブック生成")]
        public static void  ShowWindow () {
            EditorWindow.GetWindow(typeof(RecipeBookCreator));
        }

        void OnGUI()
        {
            GUILayout.Label("入力項目", UnityEditor.EditorStyles.boldLabel);
            recipeBookParent = EditorGUILayout.ObjectField("レシピの親(RecipeBook)", recipeBookParent, typeof(GameObject),true) as GameObject;
            dropdown = EditorGUILayout.ObjectField("レシピ本のドロップダウン", dropdown, typeof(Dropdown),true) as Dropdown;
            if (GUILayout.Button("Create")) Create();
        }

        void Create()
        {
            if (recipeBookParent == null)
            {
                Debug.Log("レシピの親を選択してください");
                return;
            }
            
            if (dropdown == null)
            {
                Debug.Log("ドロップダウンを選択してください");
                return;
            }
            dropdown.options.Clear();

            foreach (Transform child in recipeBookParent.transform)
            {
                if (child.name == "失敗したカクテル")
                {
                    continue;
                }
                dropdown.options.Add(new Dropdown.OptionData{text = child.name});
            }
            Debug.Log("ドロップダウンを作成しました");
        }
    }
}
