
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

    [SerializeField] Table_System table_System;
    [SerializeField] Table_Card table_Card;
    [SerializeField] Table_Dealer_UI table_Dealer_UI;


    #region Enter & Exit
    public void Enter_Table()
    {
        if (displayName != "")
            return;

        Set_Owner(Networking.LocalPlayer);
        table_System.Set_Owner(Networking.LocalPlayer);
        table_Dealer_UI.Set_Owner(Networking.LocalPlayer);
        table_Card.Set_Owner(Networking.LocalPlayer);

        displayName = Networking.LocalPlayer.displayName;
        playerId = Networking.LocalPlayer.playerId;
        table_Dealer_UI.Set_TableDealerUI(true);
        table_Card.Set_Pickupable(false);
        table_Card.Init_Card();
        DoSync();
    }
    public void Exit_Table()
    {
        displayName = "";
        table_Dealer_UI.Set_TableDealerUI(false);
        table_Card.Set_Pickupable(false);
        DoSync();
    }
    public void Set_Owner(VRCPlayerApi value)
    {
        if (value.IsOwner(gameObject)) return;
        Networking.SetOwner(value, gameObject);
    }
    #endregion

    public Table_Dealer_UI Get_Table_Dealer_UI() => table_Dealer_UI;

    #region Sync
    public void Start()
    {
        displayName = "";
        playerId = 0;
    }
    public void DoSync()
    {
        UpdateSync();
        RequestSerialization();
    }
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
        table_Dealer_UI.Update_UI(displayName);
    }
    #endregion
    #region Networking
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
        DoSync();
    }
    #endregion
}
