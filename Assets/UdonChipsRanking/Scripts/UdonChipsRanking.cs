
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class UdonChipsRanking : UdonSharpBehaviour
{
    [SerializeField] private float UpdateSpan = 3f;
    [SerializeField] private bool isRanking = true;
    [SerializeField] private bool changingColor = true;
    [SerializeField] private Color goldColor = new Color(180f/255f, 152f/255f, 27f/255f, 1f);
    [SerializeField] private Color silverColor = new Color(192f/255f, 192f/255f, 192f/255f, 1f);
    [SerializeField] private Color copperColor = new Color(184f/255f, 115f/255f, 51f/255f, 1f);
    [SerializeField] private Color otherColor = new Color(100f/255f, 100f/255f, 100f/255f, 1f);
    [SerializeField] Instance_Data instance_Data;
    [SerializeField] UdonChipsRankingPlayer[] players;
    float nextTime = 0;
    private int myPlayerId = 0;
    private bool isStarted = false;

    void Update()
    {
        if(!gameObject.activeSelf) return;

        if (isStarted == false) return;
        if(nextTime == 0f || Time.time >= nextTime)
        {
            nextTime = Time.time + UpdateSpan;
            if(isRanking == true){
                SortPlayers();
            }
        }
    }
    public void SortPlayers()
    {
        string[] playersID = new string[100];
        float[] playersUC = new float[100];
        int[] playersCoin = new int[100];
        for (int i = 0; i < 100; i++)
        {
            playersID[i] = instance_Data.get_sDisplayNames(i);
            playersUC[i] = instance_Data.get_iChips(i);
            playersCoin[i] = instance_Data.get_iCoins(i);
        }
        string tmpID;
        float tmpUC;
        int tmpCoin;
        for (int i = 0; i < 99; i++)
        {
            for (int j = 99; j > i; j--)
            {
                if (playersCoin[j] < playersCoin[j - 1])
                {
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
        for (int i = 0; i < players.Length; i++)
        {
            if (playersID[i] == "") continue;
            players[i].gameObject.transform.SetAsFirstSibling();
            if (changingColor == true)
            {
                if (i == 0)         players[i].ChangeColor(goldColor, i + 1);
                else if (i == 1)    players[i].ChangeColor(silverColor, i + 1);
                else if (i == 2)    players[i].ChangeColor(copperColor, i + 1);
                else                players[i].ChangeColor(otherColor, i + 1);
            }
        }
    }
}
