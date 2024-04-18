
using Holdem;
using System.Data;
using UdonSharp;
using UnityEngine;
using UnityEngine.Tilemaps;
using VRC.SDKBase;
using VRC.Udon;

public class Table_Dealer : UdonSharpBehaviour
{
    [UdonSynced] string displayName;
    [UdonSynced] int playerId;
    [UdonSynced] int table_Pot;

    [SerializeField] Table_System table_System;
    [SerializeField] Table_Card table_Card;
    [SerializeField] Table_Dealer_UI table_Dealer_UI;

    int table_TurnIndex, table_DealerIndex = 0;

    #region Sync
    public void Start()
    {
        displayName = "";
        playerId = 0; 
        table_Pot = 0;
    }
    public void DoSync() => RequestSerialization();
    public override void OnDeserialization()
    {
        UpdateSync();
    }
    public void UpdateSync()
    {
        Update_DisplayText();
    }
    public void Update_DisplayText()
    {
        table_System.Get_TableSystemUI().Set_Dealer_Displayname(displayName);
    }
    #endregion

    #region Enter & Exit
    public void Enter_Table()
    {
        if (displayName != "")
            return;

        Set_Owner(Networking.LocalPlayer);
        table_Card.Set_Owner(Networking.LocalPlayer);

        displayName = Networking.LocalPlayer.displayName;
        playerId = Networking.LocalPlayer.playerId;
        UpdateSync();
        DoSync();
    }
    public void Exit_Table()
    {
        displayName = "";
        UpdateSync();
        DoSync();
    }

    public void Set_Owner(VRCPlayerApi value)
    {
        if (value.IsOwner(gameObject)) return;
        Networking.SetOwner(value, gameObject);
    }
    #endregion

    #region Game
    public void Set_Game_Start()
    {
        if (table_System.Get_TableState() != TableState.Wait)
            return;
        Set_Game_Auto();
    }
    public void Set_Game_Auto()
    {
        switch (table_System.Get_TableState())
        {
            case TableState.Wait:
                table_Card.Reset_CardPosition();
                table_Card.Shuffle_Card();
                table_System.Set_TableState(TableState.Hand);
                table_DealerIndex += 1;
                break;
            case TableState.Hand:
                for (int i = 0; i < table_System.Get_TablePlayerData.Length; i++)
                {
                    if (table_System.Get_PlayerState[i] != PlayerState.Wait)
                        continue;
                    table_Card.Set_CardPosition(i, table_System.Get_TablePlayerData[i].Get_CardPosition(0), true);
                    table_System.Set_TableCard(i, table_Card.Get_CardIndex(i));
                    table_Card.Set_CardPosition(i + 9, table_System.Get_TablePlayerData[i].Get_CardPosition(1), true);
                    table_System.Set_TableCard(i + 9, table_Card.Get_CardIndex(i + 9));
                }
                Set_TableTurnIndex(table_DealerIndex);
                table_System.Set_HandRank();
                table_System.Set_TableState(TableState.Flop);
                break;
            case TableState.Flop:
                for (int i = 0; i < 3; i++)
                {
                    table_Card.Set_CardPosition(i + 18, (CardPosition)(i + 1), true);
                    table_System.Set_TableCard(i + 18, table_Card.Get_CardIndex(i + 18));
                }
                table_System.Set_HandRank();
                table_System.Set_TableState(TableState.Turn);
                break;
            case TableState.Turn:
                table_Card.Set_CardPosition(21, CardPosition.Turn, true);
                table_System.Set_TableCard(21, table_Card.Get_CardIndex(21));
                table_System.Set_HandRank();
                table_System.Set_TableState(TableState.River);
                break;
            case TableState.River:
                table_Card.Set_CardPosition(22, CardPosition.Turn, true);
                table_System.Set_TableCard(22, table_Card.Get_CardIndex(22));
                table_System.Set_HandRank();
                table_System.Set_TableState(TableState.Open);
                break;
            case TableState.Open:
                for (int i = 0; i < table_System.Get_TableCard.Length; i++)
                    table_Card.Set_Blind(i, false);
                table_System.Set_TableState(TableState.Wait);
                break;
        }
    }
    public void 
    #endregion

    public int Get_TableTurnIndex(int value) => value % 9;
    public void Set_TableTurnIndex(int value)
    {
        for (int i = 0; i < table_System.Get_TablePlayerData.Length; i++)
        {
            if (table_System.Get_PlayerState[Get_TableTurnIndex(i + value)] != PlayerState.Wait)
                continue;

            table_TurnIndex = Get_TableTurnIndex(i + value);
            table_System.Set_PlayerState(table_TurnIndex, PlayerState.Turn);
            break;
        }
    }
    public Table_Dealer_UI Get_Table_Dealer_UI() => table_Dealer_UI;

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
        UpdateSync();
        DoSync();
    }
}
