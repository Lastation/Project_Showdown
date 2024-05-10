using Holdem;
using UdonSharp;
using UnityEngine;

namespace CocktailSystem.src.bartender
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
    public class Recipe : UdonSharpBehaviour
    {
        private Liqueur[] liqueurList;
        [SerializeField] private Material material;
        [SerializeField] MainSystem mainSystem;
        [SerializeField] private string[] s_cocktailName;

        private const int DIVISOR = 60;
        private const int MAX_POINT = 40;
        private int point = 0;
        private int strong = 0;

        void Start()
        {
            this.liqueurList = this.gameObject.GetComponentsInChildren<Liqueur>();
            this.Clear();
        }

        public void Clear()
        {
            this.point = 0;
            this.strong = 0;
        }

        /*
         *  満点40点
         *  大体の公約数の60点満点で計算してから丸める
         *
         *  材料1種類分量通りに入れられたら（60/材料種類数）点加算
         *  分量間違いは加算後に減点
         * 　
         *  材料が入れられていなかったりしたら減点
         * 
         */
        public void CalcPoint(int[] inputList)
        {
            var alcoholCount = 0;
            var nonAlcoholCount = 0;
            var referenceStrong = 0.0f;
            // 材料が全部入ってるか見る
            var hitLiqueurCount = 0;

            // 材料リスト
            for (var i = 0; i < this.liqueurList.Length; i++)
            {
                var hitCount = 0;
                if (this.liqueurList[i].IsAlcohol())
                {
                    alcoholCount += this.liqueurList[i].GetRequiredAmount();
                }
                else
                {
                    nonAlcoholCount += this.liqueurList[i].GetRequiredAmount();
                }

                // 投入素材リスト
                for (var hitNo = 0; hitNo < inputList.Length; hitNo++)
                {
                    if (this.liqueurList[i].GetNo() != inputList[hitNo]) continue;

                    hitCount++;
                }

                // 1件以上ヒットで加点
                if (hitCount > 0)
                {
                    hitLiqueurCount++;
                    var difference = Mathf.Abs(hitCount - this.liqueurList[i].GetRequiredAmount());
                    point += (DIVISOR / this.liqueurList.Length);

                    if (difference != 0)
                    {
                        point -= (DIVISOR / this.liqueurList.Length) / 4 * Mathf.Abs(difference);
                    }
                }
                else
                {
                    point -= DIVISOR / this.liqueurList.Length / 2;
                }
            }

            // 材料が全部入っていない場合は計算おわり
            if (hitLiqueurCount < this.liqueurList.Length)
            {
                point = -100;
                return;
            }
            
            if (nonAlcoholCount != 0)
            {
                referenceStrong = (float) alcoholCount / nonAlcoholCount;
            }

            // レシピ外のもの入れてないか確認
            // 投入した素材からアルコール濃度計算
            for (var i = 0; i < inputList.Length; i++)
            {
                // 無は判定しない
                if (inputList[i] == 0) continue;

                var hit = false;
                // 材料リスト
                for (var j = 0; j < this.liqueurList.Length; j++)
                {
                    if (this.liqueurList[j].GetNo() != inputList[i]) continue;
                    hit = true;
                    if (this.liqueurList[j].IsAlcohol())
                    {
                        alcoholCount++;
                    }
                    else
                    {
                        nonAlcoholCount++;
                    }
                }

                if (!hit)
                {
                    point -= DIVISOR / this.liqueurList.Length / 2;
                }
            }

            var inputStrong = referenceStrong;
            if (nonAlcoholCount != 0)
            {
                inputStrong = (float) alcoholCount / nonAlcoholCount;
            }

            if (referenceStrong == 0.0f)
            {
                if (inputStrong == 0.0f)
                {
                    // 全てアルコール or 全てノンアル 濃度は関係ない
                    this.strong = 0;
                }
                else
                {
                    // レシピは全てアルコールなのに、ノンアルが入っている
                    this.strong = -1;
                }
            }
            else
            {
                if (inputStrong == 0.0f)
                {
                    // ノンアル入りレシピでノンアルなし
                    this.strong = 2;
                }
                else
                {
                    // レシピも投入物も両方混ぜもの
                    var tmpStrong = referenceStrong / inputStrong;
                    if (Mathf.Abs(tmpStrong - 1.0f) < 0.1f)
                    {
                        this.strong = 0;
                    }
                    else if (tmpStrong < 1.0f && tmpStrong > 0.5f)
                    {
                        this.strong = 1;
                    }
                    else if (tmpStrong < 0.5f)
                    {
                        this.strong = 2;
                    }
                    else if (tmpStrong > 1.0f && tmpStrong <= 2.0f)
                    {
                        this.strong = -1;
                    }
                    else if (tmpStrong < 2.0f)
                    {
                        this.strong = -2;
                    }
                }
            }
            
            point = (int) (((float)point / (float)DIVISOR) * MAX_POINT);
            Debug.Log(s_cocktailName[(int)mainSystem.Get_Language] + point);
        }

        public string GetCocktailName()
        {
            return s_cocktailName[(int)mainSystem.Get_Language];
        }

        public Liqueur[] GetLiqueurList()
        {
            return this.liqueurList;
        }

        public Material GetMaterial()
        {
            return material;
        }

        public int GetPoint()
        {
            return this.point;
        }

        public int GetStrong()
        {
            return this.strong;
        }
    }
}