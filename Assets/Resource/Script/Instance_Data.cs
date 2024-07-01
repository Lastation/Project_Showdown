
using Holdem;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

public class Instance_Data : UdonSharpBehaviour
{
    [SerializeField]
    private string[] sDelaerName;
    [SerializeField]
    private Data_Player data_Player;

    [UdonSynced]
    private string[] sDisplayNames = new string[100];
    [UdonSynced]
    private int[] iChips = new int[100];
    [UdonSynced]
    private int[] iCoins = new int[100];

    private bool loadData = true;

    void Start()
    {
        for (int i = 0; i < sDisplayNames.Length; i++)
        {
            sDisplayNames[i] = "";
            iChips[i] = 0;
            iCoins[i] = 0;
        }
    }

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        if (!Networking.IsOwner(gameObject))
            return;

        for (int i = 0; i < sDisplayNames.Length; i++)
        {
            if (sDisplayNames[i] == "")
            {
                sDisplayNames[i] = player.displayName;
                iChips[i] = 20000;
                iCoins[i] = 0;
                break;
            }
        }
        DoSync();
    }

    public bool DealerCheck(string name)
    {
        for (int i = 0; i< sDelaerName.Length; i++)
            if (name == sDelaerName[i])
                return true;
        return false;
    }

    public void Save_PlayerData(string name, int chip, int coin)
    {
        for (int i = 0; i < sDisplayNames.Length; i++)
        {
            if (sDisplayNames[i] == name)
            {
                iChips[i] = chip;
                iCoins[i] = coin;
                Debug.Log($"[{i}] - {name} : chip = {chip}, coin = {coin} saved");
                break;
            }
        }
    }

    ///  Network Fuctions
    #region Sync
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
        if (!loadData)
            return;
        SendCustomEventDelayedSeconds("Load_Datas", 2.0f);
        loadData = false;
    }

    public void Load_Datas()
    {
        if (sDisplayNames[0] == "")
        {
            SendCustomEventDelayedSeconds("Load_Datas", 2.0f);
            return;
        }

        for (int i = 0; i < sDisplayNames.Length; i++)
        {
            if (Networking.LocalPlayer.displayName == sDisplayNames[i])
            {
                data_Player.Set_Chip(iChips[i]);
                data_Player.Set_Coin(iCoins[i]);
                loadData = false;
                return;
            }
        }

        data_Player.Set_Chip(20000);
        data_Player.Set_Coin(0);
    }
    #endregion
}