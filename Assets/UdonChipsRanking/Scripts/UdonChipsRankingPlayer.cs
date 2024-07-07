
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class UdonChipsRankingPlayer : UdonSharpBehaviour
{
    [SerializeField] private UnityEngine.UI.Text PlayerRankText = null;
    [SerializeField] private UnityEngine.UI.Text PlayerNameText = null;
    [SerializeField] private UnityEngine.UI.Text PlayerUdonChipsText = null;
    [SerializeField] private UnityEngine.UI.Text PlayerUdonCoinsText = null;

    public string PlayerName
    {
        set
        {
            PlayerNameText.text = value;
            if (value == "")
            {
                PlayerUdonChipsText.text = "";
                PlayerUdonCoinsText.text = "";
            }
        }
    }
    public int PlayerUdonChips
    {
        set
        {
            if (value < 0) return;
            PlayerUdonChipsText.text = value.ToString("F0");
        }
    }

    public int PlayerUdonCoins
    {
        set
        {
            if (value < 0) return;
            PlayerUdonCoinsText.text = value.ToString("F0");
        }
    }
    public void ChangeColor(Color color, int index = 99)
    {
        if (PlayerNameText.text == "")
        {
            PlayerRankText.text = "";
            PlayerUdonChipsText.text = "";
            PlayerUdonCoinsText.text = "";
            return;
        }

        string sub = "th";
        PlayerRankText.color = color;
        switch (index)
        {
            case 1:
            case 21:
            case 31:
                sub = "st";
                break;
            case 2:
            case 22:
            case 32:
                sub = "nd";
                break;
            case 3:
            case 23:
            case 33:
                sub = "rd";
                break;
        }
        PlayerRankText.text = (index).ToString() + sub;
    }
}
