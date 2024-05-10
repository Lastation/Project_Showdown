using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace Holdem
{
    public enum TableBlindCheck
    {
        Default = 0,
        SmallBlind = 1,
        BigBlind = 2
    }

    public class Table_Player : UdonSharpBehaviour
    {
        [SerializeField] int tableNumber = 0;
        [SerializeField] Table_System table_System = default;
        [SerializeField] Table_Player_UI table_Player_UI = default;
        [SerializeField] Transform[] tf_cardPosition;
        [SerializeField] Transform tf_dealerButtonPosition;
        
        [UdonSynced] int actionChipSize;
        [UdonSynced] string displayName;
        [UdonSynced] int playerId;
        [UdonSynced] int tablePlayerChip;
        [UdonSynced] bool isAction;
        [UdonSynced] int betSize = 0;

        TableState tableState = TableState.Wait;
        bool isTurn = false;
        TableBlindCheck tableBlindCheck = TableBlindCheck.Default;

        #region Enter & Exit
        public void Enter_Table()
        {
            if (playerId != 0)
                return;

            if (table_System._mainSystem.Get_Data_Player().Get_Chip() == 0)
                return;

            if (table_System._mainSystem.Get_Data_Player().Get_isPlayGame())
                return;

            Set_Owner(Networking.LocalPlayer);
            table_Player_UI.Set_Owner(Networking.LocalPlayer);

            displayName = Networking.LocalPlayer.displayName;
            playerId = Networking.LocalPlayer.playerId;

            tablePlayerChip = table_System._mainSystem.Get_Data_Player().Get_Chip();
            table_System._mainSystem.Get_Data_Player().Set_isPlayGame(true);

            table_System._mainSystem.table_Player_UI = table_Player_UI;
            table_Player_UI.Set_TablePlayerUI(true);
            table_Player_UI.Set_TablePlayerUI_Height(true);
            table_Player_UI.Set_Button_Color(false);
            DoSync();
        }
        public void Exit_Table()
        {
            if (!Networking.IsOwner(gameObject)) return;

            displayName = "";
            playerId = 0;
            tablePlayerChip = 0;

            table_System._mainSystem.Get_Data_Player().Set_isPlayGame(false);
            table_System._mainSystem.table_Player_UI = null;

            table_Player_UI.Set_TablePlayerUI(false);
            table_Player_UI.Set_TablePlayerUI_Height(false);
            table_Player_UI.Set_Button_Color(false);
            table_System.Set_ExitPlayer(tableNumber);
            DoSync();
        }
        #endregion
        #region EndGame
        public void Add_EndGamePot()
        {
            if (!Networking.IsOwner(gameObject)) return;

            table_System._mainSystem.Get_Data_Player().Add_Chip(table_System.Get_TablePot(tableNumber));
            tablePlayerChip = table_System._mainSystem.Get_Data_Player().Get_Chip();
            DoSync();
        }
        #endregion

        #region Turn
        public int Get_BetSize() => betSize;
        public void Set_PayBB()
        {
            if (!Networking.IsOwner(gameObject)) return;

            betSize = table_System.Get_TableBB();
            table_System._mainSystem.Get_Data_Player().Pay_Chip(table_System.Get_TableBB());
            tablePlayerChip = table_System._mainSystem.Get_Data_Player().Get_Chip();
            tableBlindCheck = TableBlindCheck.BigBlind;
            DoSync();
        }
        public void Set_PaySB()
        {
            if (!Networking.IsOwner(gameObject)) return;

            betSize = table_System.Get_TableSB();
            table_System._mainSystem.Get_Data_Player().Pay_Chip(table_System.Get_TableSB());
            tablePlayerChip = table_System._mainSystem.Get_Data_Player().Get_Chip();
            tableBlindCheck = TableBlindCheck.SmallBlind;
            DoSync();
        }
        public void Set_Turn()
        {
            if (!Networking.IsOwner(gameObject)) return;

            if (tableState != table_System.Get_TableState())
            {
                tableState = table_System.Get_TableState();
                betSize = 0;
            }

            switch (tableBlindCheck)
            {
                case TableBlindCheck.SmallBlind:
                    betSize = table_System.Get_TableSB();
                    break;
                case TableBlindCheck.BigBlind:
                    betSize = table_System.Get_TableBB();
                    break;
            }
            tableBlindCheck = TableBlindCheck.Default;

            isTurn = true;
            isAction = false;
            table_Player_UI.Set_Button_Color(isTurn);
            if (table_System._mainSystem.Get_Data_Player().Get_Chip() <= table_System.Get_TableCallSize() - betSize)
                table_Player_UI.Set_CallText_Allin(table_System._mainSystem.Get_Data_Player().Get_Chip());
            else
                table_Player_UI.Set_CallText(table_System.Get_TableCallSize() - betSize);
            Add_RaiseChipSize_Reset();
        }
        public void Action_Call()
        {
            if (!isTurn)
                return;

            int value = table_System.Get_TableCallSize() - betSize;

            if (value > table_System._mainSystem.Get_Data_Player().Get_Chip())    value = table_System._mainSystem.Get_Data_Player().Get_Chip();
            else                                                    value = table_System.Get_TableCallSize();
            Action_Sync(value);
        }
        public void Action_Reset()
        {
            if (!Networking.IsOwner(gameObject))
                return;

            isTurn = false;
            isAction = false;
            betSize = 0;
            table_Player_UI.Set_Button_Color(isTurn);

            if (table_System._mainSystem.Get_Data_Player().Get_Chip() <= table_System.Get_TableCallSize() - betSize)
                table_Player_UI.Set_CallText_Allin(table_System._mainSystem.Get_Data_Player().Get_Chip());
            else
                table_Player_UI.Set_CallText(table_System.Get_TableCallSize() - betSize);
            Add_RaiseChipSize_Reset();
            DoSync();
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
            {
                if (table_System._mainSystem.Get_Data_Player().Get_Chip() == value)
                    table_System._mainSystem.Get_Data_Player().Pay_Chip(value);
                else
                    table_System._mainSystem.Get_Data_Player().Pay_Chip(actionChipSize - betSize);
            }

            betSize = value == -1 ? 0 : actionChipSize;

            isAction = true;
            isTurn = false;
            table_Player_UI.Set_Button_Color(isTurn);
            tablePlayerChip = table_System._mainSystem.Get_Data_Player().Get_Chip();
            DoSync();
        }
        #endregion
        #region Raise
        public void Add_RaiseChipSize_Reset() => Set_RaiseChipSize(table_System.Get_TableRaiseSize() * 1, false);
        public void Add_RaiseChipSize_3x() => Set_RaiseChipSize(table_System.Get_TableRaiseSize() * 2, false);
        public void Add_RaiseChipSize_4x() => Set_RaiseChipSize(table_System.Get_TableRaiseSize() * 3, false);
        public void Add_RaiseChipSize_100() => Set_RaiseChipSize(100, true);
        public void Add_RaiseChipSize_500() => Set_RaiseChipSize(500, true);
        public void Add_RaiseChipSize_1000() => Set_RaiseChipSize(1000, true);
        public void Add_RaiseChipSize_5000() => Set_RaiseChipSize(5000, true);
        public void Add_RaiseChipSize_10000() => Set_RaiseChipSize(10000, true);
        public void Add_RaiseChipSize_Allin() => Set_RaiseChipSize(table_System._mainSystem.Get_Data_Player().Get_Chip(), true);
        public void Set_RaiseChipSize(int value, bool isAdd)
        {
            if (!isTurn)
                return;

            if (isAdd)
                actionChipSize += value + betSize;
            else
                actionChipSize = table_System.Get_TableRaiseSize() + value;

            actionChipSize = Mathf.Min(actionChipSize - betSize, table_System._mainSystem.Get_Data_Player().Get_Chip());

            if (table_System._mainSystem.Get_Data_Player().Get_Chip() <= actionChipSize)
                table_Player_UI.Set_RaiseText_Allin(actionChipSize);
            else
                table_Player_UI.Set_RaiseText(actionChipSize);
            DoSync();
        }
        #endregion

        public bool isPlaying() => displayName == "" ? false : true;
        public Table_Player_UI Get_table_Player_UI() => table_Player_UI;
        public string Get_DisplayName() => displayName;
        public int Get_TablePlayerChip() => tablePlayerChip;

        public Transform[] Get_CardPosition => tf_cardPosition;
        public Transform Get_DealerButtonPosition => tf_dealerButtonPosition;

        #region Sync
        public void Start()
        {
            actionChipSize = 0;
            displayName = "";
            playerId = 0;
            tablePlayerChip = 0;
            isAction = false;
        }
        public void DoSync()
        {
            Update_Syncs();
            RequestSerialization();
        }
        public override void OnDeserialization()
        {
            Update_Syncs();
        }
        public void Update_Syncs()
        {
            Update_DisplayText();
            Update_Action();
            Update_BetSize();
        }
        public void Update_DisplayText()
        {
            table_Player_UI.Update_UI(displayName, tablePlayerChip);
        }
        public void Update_Action()
        {
            if (!Networking.IsOwner(table_System.gameObject)) return;
            if (!isAction) return;
            table_System.Set_BetAction(tableNumber, actionChipSize);
        }
        public void Update_BetSize()
        {
            table_Player_UI.Set_BetSize(betSize);
        }
        #endregion
        #region Networking
        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            if (!Networking.IsOwner(gameObject)) return;
            if (!isPlaying()) return;
        }
        public override void OnPlayerLeft(VRCPlayerApi player)
        {
            if (!Networking.IsOwner(gameObject)) return;
            if (player.playerId != playerId) return;
            Action_Reset();
            displayName = "";
            playerId = 0;

            if (Networking.IsOwner(table_System.gameObject)) table_System.Set_ExitPlayer(tableNumber);
        }
        public void Set_Owner(VRCPlayerApi value)
        {
            if (value.IsOwner(gameObject)) return;
            Networking.SetOwner(value, gameObject);
        }
        #endregion
    }
}