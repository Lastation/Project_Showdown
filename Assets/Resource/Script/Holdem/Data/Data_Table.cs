using TMPro;
using UdonSharp;
using Unity.Profiling;
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
        [UdonSynced] int tableTotalPot = 0;
        [UdonSynced] int tableCallSize = 0;
        [UdonSynced] int[] playerBetSize = new int[9];
        #endregion

        [SerializeField] MainSystem mainSystem;
        [SerializeField] AudioSource audioSource;
        [SerializeField] Table_Player[] table_Players;

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
                playerState[Random.Range(0, 9)] = (PlayerState)Random.Range(0, 4);
                Update_UI();
            }
        }
        public void DoSync() => RequestSerialization();
        public override void OnDeserialization()
        {
            Update_UI();
        }
        #endregion

        public void Update_UI()
        {
            Update_PlayerState();
        }

        #region State Setting
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
        public void Reset_PlayerState(int index)
        {
            if (!Networking.IsOwner(gameObject))
                return;

            playerState[index] = PlayerState.OutOfGame;
            Update_UI();
            DoSync();
        }
        public void Update_PlayerState()
        {
            for (int i = 0; i < table_Players.Length; i++)
                table_Players[i].Get_table_Player_UI().Update_StateText(playerState[i]);
        }
        #endregion

        #region Chip Setting
        public void Set_TableTotalPot(int value)
        {
            tableTotalPot = value;
            DoSync();
        }
        public void Set_TableCallSize(int value)
        {
            tableCallSize = value;
            DoSync();
        }
        public int Get_TableCallSize() => tableCallSize;
        public void Set_PlayerBetSize(int idx, int value)
        {
            playerBetSize[idx] = value;
            DoSync();
        }
        #endregion

        #region Sound Effect
        public void Play_AudioClip(SE_Table_Index index) => audioSource.PlayOneShot(mainSystem.Get_AudioClip_Table((int)index));
        #endregion

        #region Action
        public void Set_Action(int tableNumber, int value)
        {
            if (value == -1)
            {
                playerState[tableNumber] = PlayerState.Fold;
                return;
            }
            else
            {
                playerState[tableNumber] = PlayerState.Call;
                tableCallSize = value;
            }
        }
        #endregion

        public void Set_TablePlayerData(int idx, Table_Player data)
        {
            table_Players[idx] = data;
            DoSync();
        }
        public Table_Player Get_TablePlayerData(int idx) => table_Players[idx];
        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            if (!Networking.IsOwner(gameObject)) return;
            Update_UI();
            DoSync();
        }

        public override void OnPlayerLeft(VRCPlayerApi player)
        {
            if (!Networking.IsOwner(gameObject)) return;
            for (int i = 0; i < playerState.Length; i++)
            {
                if (Get_TablePlayerData(i).Get_DisplayName() == player.displayName)
                {
                    playerState[i] = PlayerState.OutOfGame;
                    break;
                }
            }
            Update_UI();
            DoSync();
        }
    }
}
