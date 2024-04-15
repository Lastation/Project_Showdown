
using Holdem;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Table_Dealer : UdonSharpBehaviour
{
    [UdonSynced] int[] tableCard = new int[23];

    #region Sync
    public void DoSync() => RequestSerialization();
    public override void OnDeserialization()
    {
        Update_UI();
    }
    public void Update_UI()
    {

    }
    public override void OnPlayerLeft(VRCPlayerApi player)
    {
        if (!Networking.IsOwner(gameObject)) return;
        DoSync();
    }
    #endregion

}
