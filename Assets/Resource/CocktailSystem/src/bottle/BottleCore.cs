using Holdem;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;

namespace CocktailSystem.src.bottle
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
    public class BottleCore : UdonSharpBehaviour
    {
        [SerializeField] private GameObject spoutPoint;
        [SerializeField] private Liqueur liqueur;
        [SerializeField] private Text textUi;
        [SerializeField] private GameObject liqueurListParent;
        [SerializeField] private Renderer _renderer;
        [SerializeField] private MainSystem mainSystem;
        LocalizationType localizationType = LocalizationType.KOR;

        public bool initFlag { get; set; }

        private void SetText()
        {
            var liqueurList = this.liqueurListParent.GetComponentsInChildren<Liqueur>();
            for (int i = 0; i < liqueurList.Length; i++)
            {
                if (liqueurList[i].GetNo() == this.liqueur.GetNo())
                {
                    this.textUi.text = liqueurList[i].GetName(mainSystem.Get_Language);
                    if (liqueurList[i].GetName(mainSystem.Get_Language) != "")
                    {
                        this.initFlag = true;
                    }
                }
            }
        }

        private void Update()
        {
            if (mainSystem.Get_Language != localizationType)
            {
                initFlag = false;
                localizationType = mainSystem.Get_Language;
            }

            if (!this.initFlag)
            {
                this.SetText();
            }
            
            if (Mathf.Abs(this.transform.rotation.x) >= 0.4 ||
                Mathf.Abs(this.transform.rotation.z) >= 0.4)
            {
                spoutPoint.SetActive(true);
            }
            else
            {
                spoutPoint.SetActive(false);
            }
        }

        public Liqueur GetLiqueur()
        {
            return this.liqueur;
        }

        public Material GetMaterial()
        {
            return _renderer.material;
        }
    }
}