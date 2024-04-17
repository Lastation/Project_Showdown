
using Holdem;
using System.Data;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Table_Dealer : UdonSharpBehaviour
{
    [UdonSynced] int[] table_Cards = new int[23];
    [UdonSynced] string displayName = "";
    [UdonSynced] int playerId = 0;
    [UdonSynced] int table_Pot;

    [SerializeField] Table_Card table_Card;
    [SerializeField] Table_Dealer_UI table_Dealer_UI;

    #region Sync
    public void DoSync() => RequestSerialization();
    public override void OnDeserialization()
    {
        Update_DisplayText();
    }
    public void Update_DisplayText()
    {
        //table_Dealer_UI
    }
    #endregion

    #region Enter & Exit
    public void Enter_Table()
    {
        if (displayName != "")
            return;

        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        table_Card.Set_Owner(Networking.LocalPlayer);

        displayName = Networking.LocalPlayer.displayName;
        playerId = Networking.LocalPlayer.playerId;
        //table_Dealer_UI.Set_TablePlayerUI();
        Update_DisplayText();
        DoSync();
    }
    public void Exit_Table()
    {
        displayName = "";
        //table_Dealer_UI.Set_TablePlayerUI();
        Update_DisplayText();
        DoSync();
    }
    #endregion

    public override void OnPlayerLeft(VRCPlayerApi player)
    {
        if (!Networking.IsOwner(gameObject)) return;
        if (player.playerId != playerId) return;
        displayName = "";
        playerId = 0;
        Update_DisplayText();
        DoSync();
    }

}
