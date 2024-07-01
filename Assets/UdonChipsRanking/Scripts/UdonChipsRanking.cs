
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class UdonChipsRanking : UdonSharpBehaviour
{
    [SerializeField] private float UpdateSpan = 3f;
    [SerializeField] private bool isRanking = true;
    [SerializeField] private bool changingColor = true;
    [SerializeField] private Color goldColor = new Color(180f/255f, 152f/255f, 27f/255f, 1f);
    [SerializeField] private Color silverColor = new Color(192f/255f, 192f/255f, 192f/255f, 1f);
    [SerializeField] private Color copperColor = new Color(184f/255f, 115f/255f, 51f/255f, 1f);
    [SerializeField] private Color otherColor = new Color(100f/255f, 100f/255f, 100f/255f, 1f);
    float nextTime = 0;
    [SerializeField] UdonChipsRankingPlayer[] players;
    [SerializeField] Instance_Data instance_Data;
    private int myPlayerId = 0;
    private bool isStarted = false;
    void Start()
    {
        
    }
    void Update()
    {
        if(!gameObject.activeSelf) return;

        if (isStarted == false) return;
        if(nextTime == 0f || Time.time >= nextTime)
        {
            nextTime = Time.time + UpdateSpan;
            players[myPlayerId].SetUdonChips();
            RemoveBlank();
            if(isRanking == true){
                SortPlayers();
            }
        }
    }
    public void SortPlayers()
    {
        int playerCount = VRCPlayerApi.GetPlayerCount();
        VRCPlayerApi[] nowPlayers = new VRCPlayerApi[players.Length];
        int[] playersID = new int[playerCount];
        float[] playersUC = new float[playerCount];
        int[] playersCoin = new int[playerCount];
        VRCPlayerApi.GetPlayers(nowPlayers);
        int cnt = 0;
        for(int i = 0;i < nowPlayers.Length;i++){
            if(Utilities.IsValid(nowPlayers[i]) == false){
                //playersID[i] = -1;
                //playersUC[i] = 0;
                continue;
            }
            playersID[cnt] = nowPlayers[cnt].playerId;
            playersUC[cnt] = players[playersID[cnt]].PlayerUdonChips;
            playersCoin[cnt] = players[playersID[cnt]].PlayerUdonCoins;
            cnt++;
        }
        int tmpID;
        float tmpUC;
        int tmpCoin;
        for(int i = 0;i < playerCount - 1;i++){
            for(int j = playerCount - 1;j > i;j--){
                if(playersCoin[j] < playersCoin[j-1]){
                    tmpID = playersID[j];
                    tmpUC = playersUC[j];
                    tmpCoin = playersCoin[j];
                    playersID[j] = playersID[j - 1];
                    playersUC[j] = playersUC[j - 1];
                    playersCoin[j] = playersCoin[j - 1];
                    playersID[j - 1] = tmpID;
                    playersUC[j - 1] = tmpUC;
                    playersCoin[j - 1] = tmpCoin;
                }
            }
        }
        for(int i = 0;i < playerCount;i++){
            if(playersID[i] < 0) continue;
            players[playersID[i]].gameObject.transform.SetAsFirstSibling();
            if(changingColor == true){
                if(i == playerCount - 1){
                    players[playersID[i]].ChangeColor(goldColor, playerCount - i);
                } else if(i == playerCount - 2){
                    players[playersID[i]].ChangeColor(silverColor, playerCount - i);
                } else if(i == playerCount - 3){
                    players[playersID[i]].ChangeColor(copperColor, playerCount - i);
                } else {
                    players[playersID[i]].ChangeColor(otherColor, playerCount - i);
                }
            }
        }
    }
    public void RemoveBlank()
    {
        for (int i = 0; i < players.Length; i++)
        {
            players[i].gameObject.transform.SetAsLastSibling();
        }
        foreach (var player in players)
        {
            if (player.PlayerName == "")
            {
                player.gameObject.transform.SetAsLastSibling();
                if (changingColor == true) player.ChangeColor(otherColor);
            }
        }
    }
    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        if (player == Networking.LocalPlayer)
        {
            myPlayerId = Networking.LocalPlayer.playerId;
            players[myPlayerId].SetPlayer();
            players[myPlayerId].SetUdonChips();
            isStarted = true;
        }
        UpdatePlayerName();
    }
    public override void OnPlayerLeft(VRCPlayerApi player)
    {
        if (Networking.IsOwner(instance_Data.gameObject))
            instance_Data.Save_PlayerData(player.displayName, players[player.playerId].PlayerUdonChips, players[player.playerId].PlayerUdonCoins);

        if (player == Networking.LocalPlayer)
            players[myPlayerId].ResetUdonChips();
        players[myPlayerId].PlayerName = "";
        UpdatePlayerName();
    }
    public void UpdatePlayerName()
    {
        for (int i = 0; i < players.Length; i++)
        {
            var player = VRCPlayerApi.GetPlayerById(i);
            if (player == null)
            {
                players[i].PlayerName = "";
            }
            else
            {
                players[i].PlayerName = player.displayName;
            }
        }
    }
}
