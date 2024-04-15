﻿using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace Holdem
{
    public class Table_Player : UdonSharpBehaviour
    {
        [SerializeField] int tableNumber = 0;
        [SerializeField] MainSystem mainSystem = default;
        [SerializeField] Data_Table data_Table = default;
        [SerializeField] Table_Player_UI table_Player_UI = default;
        
        [UdonSynced] int actionChipSize = 0;
        [UdonSynced] string displayName = "";
        [UdonSynced] int tablePlayerChip = 0;
        [UdonSynced] bool isAction = false;

        bool isTurn = false;
        int bet = 0;

        #region Sync
        public void DoSync() => RequestSerialization();
        public override void OnDeserialization()
        {
            Debug.Log(displayName);
            Update_DisplayText();
            Update_Action();
        }
        public void Update_DisplayText()
        {
            table_Player_UI.Update_DisplayText(displayName, tablePlayerChip);
        }
        public void Update_Action()
        {
            if (!isAction)
                return;

            if (!Networking.LocalPlayer.IsOwner(data_Table.gameObject))
                return;

            data_Table.Set_Action(tableNumber, actionChipSize);
        }
        public override void OnPlayerLeft(VRCPlayerApi player)
        {
            if (!Networking.IsOwner(gameObject)) return;
            DoSync();
        }
        #endregion

        #region Enter & Exit
        public void Enter_Table()
        {
            if (displayName != "")
                return;

            if (mainSystem.Get_Data_Player().Get_isPlayGame())
                return;

            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            table_Player_UI.Set_Owner(Networking.LocalPlayer);

            displayName = Networking.LocalPlayer.displayName;
            tablePlayerChip = mainSystem.Get_Data_Player().Get_Chip();
            mainSystem.Get_Data_Player().Set_isPlayGame(true);

            table_Player_UI.Set_TablePlayerUI(true);
            Update_DisplayText();
            DoSync();
        }
        public void Exit_Table()
        {
            displayName = "";
            tablePlayerChip = 0;
            mainSystem.Get_Data_Player().Set_isPlayGame(false);
            table_Player_UI.Set_TablePlayerUI(false);
            Update_DisplayText();
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

            Action_Sync(data_Table.Get_TableCallSize());
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
        public void Add_RaiseChipSize_Reset() => Set_RaiseChipSize(data_Table.Get_TableCallSize() * 1, false);
        public void Add_RaiseChipSize_3x() => Set_RaiseChipSize(data_Table.Get_TableCallSize() * 2, false);
        public void Add_RaiseChipSize_4x() => Set_RaiseChipSize(data_Table.Get_TableCallSize() * 3, false);

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
                actionChipSize = data_Table.Get_TableCallSize() + value;

            actionChipSize = Mathf.Min(actionChipSize, mainSystem.Get_Data_Player().Get_Chip());

            if (mainSystem.Get_Data_Player().Get_Chip() == actionChipSize)
                table_Player_UI.Set_RaiseText_Allin(actionChipSize);
            else
                table_Player_UI.Set_RaiseText(actionChipSize);
            DoSync();
        }
        #endregion
    }
}
