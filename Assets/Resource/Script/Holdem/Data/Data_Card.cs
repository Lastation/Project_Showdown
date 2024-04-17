
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

namespace Holdem
{
    public class Data_Card : UdonSharpBehaviour
    {
        [SerializeField] VRCPickup vrcPickup;
        [SerializeField] SpriteRenderer sr_pattern;

        [UdonSynced] bool isBlind = true;

        int i_cardIndex = 0;
        Sprite sprite = null;

        public void Start() => Set_Pickupable(false);
        public void DoSync() => RequestSerialization();
        public override void OnDeserialization()
        {
            Update_Blind(isBlind);
        }

        public void Update_Blind(bool value) => sr_pattern.sprite = value ? null : sprite;
        public void Set_Card_Pattern(Sprite value)
        {
            sr_pattern.sprite = value;
            sprite = value;
        }

        public void Set_CardIndex(int value) => i_cardIndex = value;
        public int Get_CardIndex() => i_cardIndex;

        public void Set_Pickupable(bool value) => vrcPickup.pickupable = value;
        public void Set_Blind(bool value)
        {
            isBlind = value;
            DoSync();
        }
        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            if (!Networking.IsOwner(gameObject)) return;
            DoSync();
        }
        public void Set_Owner(VRCPlayerApi value)
        {
            if (value.IsOwner(gameObject)) return;
            Networking.SetOwner(value, gameObject);
        }
    }
}