using CocktailSystem.src.shaker;
using UdonSharp;
using UnityEngine;

namespace CocktailSystem.src.bottle
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
    public class BottleParticle : UdonSharpBehaviour
    {
        private BottleCore bottle;

        private void Start()
        {
            this.bottle = this.transform.parent.gameObject.GetComponent<BottleCore>();
        }

        private void OnParticleCollision(GameObject other)
        {
            if (other == null)
            {
                return;
            }

            var majorCol = other.GetComponent<MajorCol>();
            if (majorCol == null)
                return;

            // パーティクル一個目でリキュールの情報をセット
            if (!majorCol.GetMajor().GetLiqueurFlag())
            {
                majorCol.GetMajor().SetLiqueur(bottle.GetLiqueur());

                // どっちに入れてるか見る
                majorCol.GetMajor().SetUseSide(majorCol.IsLongSide() ? 2 : 1);
                majorCol.GetMajor().SetMaterial(bottle.GetMaterial());
            }
            
            // 入れてる方向が同じなら量が増える
            if ((majorCol.GetMajor().GetUseSide() == 2) == majorCol.IsLongSide())
            {
                majorCol.GetMajor().AddAmount(2);
            }
        }
    }
}