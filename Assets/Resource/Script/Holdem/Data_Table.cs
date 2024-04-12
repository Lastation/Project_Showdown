using System;
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon.Common.Interfaces;

namespace Holdem
{
    public enum TableState : int
    {
        Wait = 0,
        Hand = 1,
        Flop = 2,
        Turn = 3,
        River = 4,
        Open = 5,
    }

    public enum PlayerState : int
    {
        OutOfGame = 0,
        Wait = 1,
        Turn = 2,
        Call = 3,
        Fold = 4,
    }

    public class Data_Table : UdonSharpBehaviour
    {
        #region Sync Varialbes
        [UdonSynced] TableState tableState;
        [UdonSynced] PlayerState[] playerState = new PlayerState[9];
        Data_Player[] data_player = new Data_Player[9];
        #endregion

        [SerializeField]
        TextMeshPro[] text_playerState;

        #region Sync
        public void Start()
        {
            if (!Networking.IsOwner(gameObject))
                SendCustomNetworkEvent(NetworkEventTarget.Owner, nameof(DoSync));
        }

        public void Update()
        {
            if (!Networking.IsOwner(gameObject))
                return;

            if (Input.GetKeyDown(KeyCode.Q))
            {
                playerState[UnityEngine.Random.Range(0, 9)] = (PlayerState)UnityEngine.Random.Range(0, 4);
                UpdateUI();
            }
        }

        public void DoSync()
        {
            RequestSerialization();
        }
        public override void OnDeserialization()
        {
            UpdateUI();
        }
        #endregion

        public void UpdateUI()
        {

            for (int i = 0; i < text_playerState.Length; i++)
            {
                PlayerState state = playerState[i];
                text_playerState[i].text = $"{state}";
            }
        }

        public void Set_TableState(TableState state)
        {
            tableState = state;
            DoSync();
        }
        public void Set_PlayerState(int idx, PlayerState state)
        {
            playerState[idx] = state;
            DoSync();
        }
        public void Set_PlayerData(int idx,  Data_Player data)
        {
            data_player[idx] = data;
            DoSync();
        }
    }
}
