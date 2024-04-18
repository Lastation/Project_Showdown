using System.Data;
using System.Runtime.Remoting.Messaging;
using UdonSharp;
using UnityEngine;
using UnityEngine.Playables;
using VRC.SDKBase;

namespace Holdem
{
    public class Table_Player : UdonSharpBehaviour
    {
        [SerializeField] int tableNumber = 0;
        [SerializeField] MainSystem mainSystem = default;
        [SerializeField] Table_System table_System = default;
        [SerializeField] Table_Player_UI table_Player_UI = default;
        [SerializeField] Transform[] tf_cardPosition;
        
        [UdonSynced] int actionChipSize;
        [UdonSynced] string displayName;
        [UdonSynced] int playerId;
        [UdonSynced] int tablePlayerChip;
        [UdonSynced] bool isAction;

        bool isTurn = false;
        int bet = 0;

        #region Sync
        public void Start()
        {
            actionChipSize = 0;
            displayName = "";
            playerId = 0;
            tablePlayerChip = 0;
            isAction = false;
        }
        public void DoSync() => RequestSerialization();
        public override void OnDeserialization()
        {
            Update_Syncs();
        }
        public void Update_Syncs()
        {
            Update_DisplayText();
            Update_Action();
            Update_Table();
        }
        public void Update_DisplayText()
        {
            table_Player_UI.Update_UI(displayName, tablePlayerChip);
            table_System.Update_PlayerState();
        }
        public void Update_Action()
        {
            if (!isAction)
                return;

            if (!Networking.IsOwner(table_System.gameObject))
                return;

            table_System.Set_BetAction(tableNumber, actionChipSize);
        }
        public void Update_Table()
        {
            table_System.Update_Syncs();
        }
        #endregion

        #region Enter & Exit
        public void Enter_Table()
        {
            if (playerId != 0)
                return;

            if (mainSystem.Get_Data_Player().Get_isPlayGame())
                return;

            Set_Owner(Networking.LocalPlayer);
            table_Player_UI.Set_Owner(Networking.LocalPlayer);

            displayName = Networking.LocalPlayer.displayName;
            playerId = Networking.LocalPlayer.playerId;
            tablePlayerChip = mainSystem.Get_Data_Player().Get_Chip();
            mainSystem.Get_Data_Player().Set_isPlayGame(true);
            table_Player_UI.Set_TablePlayerUI(true);
            Update_Syncs();
            DoSync();
        }
        public void Exit_Table()
        {
            displayName = "";
            playerId = 0;
            tablePlayerChip = 0;
            mainSystem.Get_Data_Player().Set_isPlayGame(false);
            table_Player_UI.Set_TablePlayerUI(false);
            Update_Syncs();
            DoSync();
        }
        #endregion

        #region Turn
        public void Set_Turn()
        {
            isTurn = true;
            table_Player_UI.Set_Button_Color(isTurn);
            Add_RaiseChipSize_Reset();
        }
        public void Action_Call()
        {
            if (!isTurn)
                return;

            Action_Sync(table_System.Get_TableCallSize());
        }
        public void Action_Raise()
        {
            if (!isTurn) 
                return;

            Action_Sync(actionChipSize);
        }
        public void Action_Fold()
        {
            if (!isTurn) 
                return;

            Action_Sync(-1);
        }
        public void Action_Sync(int value)
        {
            actionChipSize = value;

            if (actionChipSize != -1)
                mainSystem.Get_Data_Player().Add_Chip(actionChipSize - bet);

            bet = value == -1 ? 0 : actionChipSize;

            isAction = true;
            isTurn = false;
            DoSync();
        }
        #endregion

        #region Raise
        public void Add_RaiseChipSize_Reset() => Set_RaiseChipSize(table_System.Get_TableCallSize() * 1, false);
        public void Add_RaiseChipSize_3x() => Set_RaiseChipSize(table_System.Get_TableCallSize() * 2, false);
        public void Add_RaiseChipSize_4x() => Set_RaiseChipSize(table_System.Get_TableCallSize() * 3, false);
        public void Add_RaiseChipSize_100() => Set_RaiseChipSize(100, true);
        public void Add_RaiseChipSize_500() => Set_RaiseChipSize(500, true);
        public void Add_RaiseChipSize_1000() => Set_RaiseChipSize(1000, true);
        public void Add_RaiseChipSize_5000() => Set_RaiseChipSize(5000, true);
        public void Add_RaiseChipSize_10000() => Set_RaiseChipSize(10000, true);
        public void Add_RaiseChipSize_Allin() => Set_RaiseChipSize(int.MaxValue, true);
        public void Set_RaiseChipSize(int value, bool isAdd)
        {
            if (!isTurn)
                return;

            if (isAdd)
                actionChipSize += value;
            else
                actionChipSize = table_System.Get_TableCallSize() + value;

            actionChipSize = Mathf.Min(actionChipSize, mainSystem.Get_Data_Player().Get_Chip());

            if (mainSystem.Get_Data_Player().Get_Chip() == actionChipSize)
                table_Player_UI.Set_RaiseText_Allin(actionChipSize);
            else
                table_Player_UI.Set_RaiseText(actionChipSize);
            DoSync();
        }
        #endregion

        public bool isPlaying() => displayName == "" ? false : true;
        public Table_Player_UI Get_table_Player_UI() => table_Player_UI;
        public string Get_DisplayName() => displayName;

        public Transform Get_CardPosition(int idx) => tf_cardPosition[idx];

        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            if (!Networking.IsOwner(gameObject)) return;
            DoSync();
        }
        public override void OnPlayerLeft(VRCPlayerApi player)
        {
            if (!Networking.IsOwner(gameObject)) return;
            if (player.playerId != playerId) return;
            displayName = "";
            playerId = 0;
            Update_Syncs();
            DoSync();
        }
        public void Set_Owner(VRCPlayerApi value)
        {
            if (value.IsOwner(gameObject)) return;
            Networking.SetOwner(value, gameObject);
        }
    }
}