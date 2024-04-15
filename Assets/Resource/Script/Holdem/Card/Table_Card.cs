
using Holdem;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

namespace Holdem
{
    public class Table_Card : UdonSharpBehaviour
    {
        [UdonSynced]
        int[] shuffleIndex = new int[52];
        int[] shufflePrev = new int[52];

        [SerializeField]
        Data_Card[] data_Card;

        [SerializeField]
        Transform tf_cardPosition;

        int index = 0;

        private void Awake()
        {
            Reset_Card();
        }

        public void DoSync()
        {
            RequestSerialization();
        }

        public override void OnDeserialization()
        {

        }

        public void Reset_Card()
        {
            if (!Networking.LocalPlayer.IsOwner(gameObject))
                return;

            for (int i = 0; i < data_Card.Length; i++)
            {
                shuffleIndex[i] = i;
                shufflePrev[i] = -1;
            }
            index = 0;

            DoSync();
        }

        public void Set_Owner(VRCPlayerApi value)
        {
            Networking.SetOwner(value, gameObject);

            for (int i = 0; i < data_Card.Length; i++)
            {
                data_Card[i].Set_Owner(value);
                data_Card[i].Set_Pickupable(value == default ? false : true);
            }
        }
    }
}
