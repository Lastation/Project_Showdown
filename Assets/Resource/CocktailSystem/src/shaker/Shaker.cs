using CocktailSystem.src.bartender;
using Holdem;
using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using VRC.SDKBase;

namespace CocktailSystem.src.shaker
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class Shaker : UdonSharpBehaviour
    {
        [SerializeField] private int[] liqueurList = new int[10];
        private int index = 0;
        [SerializeField] private int SHAKE_LIMIT = 50;
        [SerializeField] private int TIME_LIMIT = 50;
        [SerializeField] private Animator shakerAnim;
        
        [SerializeField] private Text textUi;
        [SerializeField] private GameObject recipe;
        [SerializeField] private GameObject spoutPoint;
        [SerializeField] private AudioClip[] audioClip;
        [SerializeField] private AudioClip[] audioClipMizu;
        [SerializeField] private MainSystem mainSystem;
        
        private Liqueur[] materials;
        private CocktailInfo cocktailInfo;
        private Vector3 oldPosition;
        private Vector3 oldAcceleration;
        private Recipe[] recipeList;
        private float count = 0f;
        private float makeTime = 0f;
        private AudioSource[] sources;
        private bool isMaked = false;

        [UdonSynced(UdonSyncMode.None)] private bool notShakeing = false;
        [UdonSynced(UdonSyncMode.None)] private int shakeCount = 0;
        [UdonSynced(UdonSyncMode.None)] private bool isLong = false;
        [UdonSynced(UdonSyncMode.None)] private int reserveLiqueurNo = 0;
        [UdonSynced(UdonSyncMode.None)] private bool reserveUsed = true;

        /**
         * 0 シェーカーが空
         * 1 liqueurが入っている
         * 2 シェイク済み
         */
        [UdonSynced(UdonSyncMode.None)] private int progress = 0;

        [UdonSynced(UdonSyncMode.None)] private int iceCount = 0;

        /**
         * 0 閉じてる
         * 1 大きいほうが開いてる
         * 2 閉じてる(シェイク中)
         * 3 小さい方が開いてる
         */
        [UdonSynced(UdonSyncMode.None)] private int shakerStatus = 0;

        private string text;

        // マテリアルではなく材料の意味
        [SerializeField] private GameObject materialParent;

        public CocktailInfo GetCocktailInfo()
        {
            return cocktailInfo;
        }

        private void Start()
        {
            this.oldPosition = this.transform.position;
            this.cocktailInfo = this.gameObject.GetComponent<CocktailInfo>();
            this.recipeList = recipe.GetComponentsInChildren<Recipe>();
            this.sources = this.gameObject.GetComponents<AudioSource>();
            this.ClearValues();
        }

        public void SyncedClear()
        {
            if (Networking.IsOwner(Networking.LocalPlayer, gameObject))
            {
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(ClearValues));
            }
        }
        
        public void ClearValues()
        {
            this.index = 0;
            this.progress = 0;
            this.shakeCount = 0;
            this.text = "";
            this.iceCount = 0;
            this.cocktailInfo.Clear();
            this.makeTime = 0f;
            this.notShakeing = true;
            this.isMaked = false;
            this.reserveLiqueurNo = 0;

            for (var i = 0; i < liqueurList.Length; i++)
            {
                liqueurList[i] = 0;
            }

            Debug.Log("Cleared!");
        }

        void Update()
        {
            if (progress != 0)
            {
                this.makeTime += Time.deltaTime;
            }

            // 0.1秒単位で処理
            count += Time.deltaTime;
            if (count < 0.1)
            {
                return;
            }

            if (!reserveUsed)
            {
                this.AddLiqueur();
            }

            count = 0f;
            this.textUi.text = this.text;
            shakerAnim.SetInteger("status", this.shakerStatus);
            shakerAnim.SetInteger("iceCount", this.iceCount);

            // 口が閉じているならパーティクルを無効化
            if (this.shakerStatus != 3)
            {
                spoutPoint.SetActive(false);
            }

            // シェイク済み以外はパーティクルを無効化
            if (this.progress != 2)
            {
                spoutPoint.SetActive(false);
            }

            // カクテル未作成なら処理する必要なし
            if (this.progress == 0)
            {
                return;
            }

            var position = this.transform.position;
            var acceleration = position - oldPosition;
            // 閉じてるときのみ混ぜられる
            if (shakerStatus == 2 && this.progress != 0)
            {
                if (Vector3.Dot(acceleration, oldAcceleration) < 0)
                {
                    if (!notShakeing)
                    {
                        if (iceCount > 0)
                        {
                            sources[shakeCount % sources.Length].clip = audioClip[shakeCount % audioClip.Length];
                        }
                        else
                        {
                            sources[shakeCount % sources.Length].clip = audioClipMizu[shakeCount % audioClipMizu.Length];
                        }
                        
                        sources[shakeCount % sources.Length].Play();
                        shakeCount++;
                        RequestSerialization();
                    }
                }
            }

            oldPosition = position;
            oldAcceleration = acceleration;

            // 小さい蓋が開いてて、カクテルが作られていないなら作る
            if (isMaked == false && shakerStatus == 3)
            {
                isMaked = true;
                this.CocktailMake();
                this.progress = 2;
                RequestSerialization();
            }

            // 注ぐ処理
            if (this.progress != 2) return;
            if (this.shakerStatus != 3) return;

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

        public void ChangeStatus()
        {
            Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
            if (!Networking.IsOwner(Networking.LocalPlayer, gameObject)) return;
            
            this.shakerStatus = (shakerStatus + 1) % 4;
            RequestSerialization();
            if (this.shakerStatus == 0)
            {
                SyncedClear();
            }
        }

        public void AddReserve(int liqueurNo)
        {
            // シェーカーが開いてないと入れられない
            if (shakerStatus != 1)
            {
                return;
            }

            if (liqueurNo == 0)
            {
                return;
            }

            if (Networking.IsOwner(Networking.LocalPlayer, gameObject))
            {
                this.reserveLiqueurNo = liqueurNo;
                RequestSerialization();
                SendCustomEventDelayedSeconds(nameof(SyncedReserveUnUsed), 0.5f);
            }
        }

        public void SyncedReserveUnUsed()
        {
            if (Networking.IsOwner(Networking.LocalPlayer, gameObject))
            {
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(ReserveUnUsed));
            }
        }

        public void ReserveUnUsed()
        {
            this.reserveUsed = false;
        }

        private void AddLiqueur()
        {
            if (this.index == this.liqueurList.Length)
            {
                return;
            }

            if (this.reserveLiqueurNo == 0)
            {
                return;
            }

            if (this.progress == 0)
            {
                this.progress = 1;
            }

            this.liqueurList[index] = this.reserveLiqueurNo;
            this.index++;
            
            if (this.index == this.liqueurList.Length)
            {
                this.reserveUsed = true;
                this.ViewRefresh();
                return;
            }
            
            if (this.isLong)
            {
                this.liqueurList[index] = this.reserveLiqueurNo;
                this.index++;
            }

            this.reserveUsed = true;
            this.ViewRefresh();
        }

        private void ViewRefresh()
        {
            if (materials == null)
            {
                materials = this.materialParent.GetComponentsInChildren<Liqueur>();
            }

            var view = "";
            for (var i = 0; i < this.liqueurList.Length; i++)
            {
                for (var j = 0; j < materials.Length; j++)
                {
                    if (materials[j].GetNo() != this.liqueurList[i]) continue;

                    view += materials[j].GetName(mainSystem.Get_Language) + "\n";
                }
            }

            this.text = view;
        }

        private void CocktailMake()
        {
            this.cocktailInfo.Clear();

            for (var j = 0; j < this.recipeList.Length; j++)
            {
                this.recipeList[j].CalcPoint(this.liqueurList);
            }

            var point = 0;
            var strong = 0;
            var cocktailNo = 0;
            for (var i = 0; i < this.recipeList.Length; i++)
            {
                if (this.recipeList[i].GetPoint() < point)
                {
                    this.recipeList[i].Clear();
                    continue;
                }

                point = this.recipeList[i].GetPoint();
                strong = this.recipeList[i].GetStrong();
                cocktailNo = i;
                this.recipeList[i].Clear();
            }

            var tmpMakeTime = this.makeTime;

            // 技術点40点
            // 振る回数:20点
            // 氷:20点
            // タイム:20点
            if (shakeCount >= SHAKE_LIMIT)
            {
                point += 20;
                // 振る回数が多い場合、1回につきタイムに+0.5
                tmpMakeTime += (float) (shakeCount - SHAKE_LIMIT) / 2;
            }
            else
            {
                point += (int) ((float) this.shakeCount / SHAKE_LIMIT * 20);
            }

            if (tmpMakeTime >= TIME_LIMIT)
            {
                var timePoint = 20 - ((int) tmpMakeTime - (int)TIME_LIMIT);
                if (timePoint < -20)
                {
                    timePoint = -20;
                }

                point += timePoint;
            }
            else
            {
                point += 20;
            }

            if (this.iceCount == 1)
            {
                point += 10;
            }
            else if (this.iceCount == 2)
            {
                point += 15;
            }
            else if (this.iceCount == 3)
            {
                point += 20;
            }

            this.cocktailInfo.SetMaterial(this.recipeList[cocktailNo].GetMaterial());
            this.cocktailInfo.SetPoint(point);
            this.cocktailInfo.SetCocktailName(this.recipeList[cocktailNo].GetCocktailName());
            this.cocktailInfo.AddAmount(100);

            // 接頭語を考える
            var prefix = "";

            if (this.iceCount == 0)
            {
                prefix += "ぬるい ";
            }

            if (strong == 1)
            {
                prefix += "濃いめの ";
            }
            else if (strong >= 2)
            {
                prefix += "強烈な ";
            }
            else if (strong == -1)
            {
                prefix += "薄めの ";
            }
            else if (strong <= -2)
            {
                prefix += "うっっすい ";
            }

            this.cocktailInfo.SetPrefix(prefix);
            this.text = this.cocktailInfo.getText();
            this.text += "\n" + "タイム：" + this.makeTime + "(+ペナルティ：" + (tmpMakeTime　- this.makeTime) + "秒)";
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!Networking.IsOwner(Networking.LocalPlayer, gameObject))
            {
                return;
            }
            
            if (other == null)
            {
                return;
            }

            // シェーカーが開いてないと入れられない
            if (shakerStatus != 1)
            {
                return;
            }

            var ice = other.gameObject.GetComponent<Ice>();
            if (ice == null)
            {
                return;
            }

            if (!ice.HasIce())
            {
                return;
            }
            
            this.iceCount += 1;
            if (this.iceCount >= 3)
            {
                this.iceCount = 3;
            }
            
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(PlayIceSound));
            
            ice.ReSetIce();
        }

        public void PlayIceSound()
        {
            if (this.progress == 0)
            {
                this.progress = 1;
            }
            
            Debug.Log("play");
            sources[0].clip = audioClip[iceCount % audioClip.Length];
            sources[0].Play();
        }

        public void SetIsLong(bool longside)
        {
            this.isLong = longside;
            RequestSerialization();
        }

        public void Pickup()
        {
            Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
            this.notShakeing = false;
            SendCustomEventDelayedSeconds(nameof(Serialization), 0.4f);
        }

        public void Drop()
        {
            this.notShakeing = true;
        }

        public void Serialization()
        {
            RequestSerialization();
        }
    }
}