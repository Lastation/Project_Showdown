
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
        [SerializeField] Table_System table_System;
        [SerializeField] int i_cardIndex = 0;

        [UdonSynced] bool isBlind = false;
        
        public void Start()
        {
            isBlind = true;
            Set_Pickupable(false);
        }
        public void DoSync()
        {
            Update_Blind(isBlind);
            RequestSerialization();
        }
        public override void OnDeserialization()
        {
            Update_Blind(isBlind);
        }

        public void Update_Blind(bool value) => sr_pattern.sprite = value ? null : table_System._mainSystem.Get_CardPattern()[i_cardIndex];
        public void Set_Card_Pattern()
        {
            sr_pattern.sprite = table_System._mainSystem.Get_CardPattern()[i_cardIndex];
        }

        public int Get_CardIndex() => i_cardIndex;
        public void Set_Rotation() => transform.eulerAngles = new Vector3(transform.eulerAngles.x + 180.0f, transform.eulerAngles.y, transform.eulerAngles.z);
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