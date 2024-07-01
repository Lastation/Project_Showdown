
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class UdonChipsRankingPlayer : UdonSharpBehaviour
{
    private UCS.UdonChips udonChips = null;
    private string _playerName = "";
    [SerializeField] private UnityEngine.UI.Text PlayerRankText = null;
    [SerializeField] private UnityEngine.UI.Text PlayerNameText = null;
    [SerializeField] private UnityEngine.UI.Text PlayerUdonChipsText = null;
    [SerializeField] private UnityEngine.UI.Text PlayerUdonCoinsText = null;

    [UdonSynced, FieldChangeCallback(nameof(PlayerUdonChips))]
    private int _playerUdonChips = -1;

    [UdonSynced, FieldChangeCallback(nameof(PlayerUdonCoins))]
    private int _playerUdonCoins = -1;

    public string PlayerName
    {
        set
        {
            _playerName = value;
            PlayerNameText.text = value;
            if (value == "")
            {
                PlayerUdonChipsText.text = "";
                PlayerUdonCoinsText.text = "";
            }
        }
        get => _playerName;
    }
    public int PlayerUdonChips
    {
        set
        {
            _playerUdonChips = value;
            if (value < 0) return;
            PlayerUdonChipsText.text = value.ToString("F0");
        }
        get => _playerUdonChips;
    }

    public int PlayerUdonCoins
    {
        set
        {
            _playerUdonCoins = value;
            if (value < 0) return;
            PlayerUdonCoinsText.text = value.ToString("F0");
        }
        get => _playerUdonCoins;
    }
    void OnEnable()
    {
        if(udonChips == null && (GameObject.Find("UdonChips") != null)) udonChips = GameObject.Find("UdonChips").GetComponent<UCS.UdonChips>();
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, nameof(RequestSerialization_));
    }
    void Start()
    {
        
    }
    public void SetPlayer()
    {
        Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
    }
    public void SetUdonChips()
    {
        PlayerUdonChips = udonChips.chips;
        PlayerUdonCoins = udonChips.coin;
        RequestSerialization();
    }
    public void ResetUdonChips()
    {
        PlayerName = "";
        PlayerUdonChips = 0;
        PlayerUdonCoins = 0;
        RequestSerialization();
    }
    public void RequestSerialization_()
    {
        RequestSerialization();
    }
    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        if(Networking.IsOwner(gameObject)){
            SendCustomEventDelayedSeconds(nameof(RequestSerialization_), 5f);
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
