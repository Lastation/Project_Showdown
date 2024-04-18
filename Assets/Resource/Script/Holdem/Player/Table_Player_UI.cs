using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;

namespace Holdem
{
    public class Table_Player_UI : UdonSharpBehaviour
    {
        [SerializeField] TextMeshProUGUI text_DisplayName, text_TablePlayerChip;
        [SerializeField] TextMeshProUGUI text_Call, text_Raise;
        [SerializeField] TextMeshProUGUI text_Handrank;
        [SerializeField] TextMeshPro text_state;
        [SerializeField] GameObject obj_TablePlayerUI;
        [SerializeField] GameObject obj_TableJoin, obj_TableExit;

        [SerializeField] Image[] img_button;

        Color color_button = new Color(0.0627451f, 0.509804f, 1, 1);
        string[] s_playerState = new string[5] { "", "Wait", "Turn", "Call", "Fold" };

        public void Update_UI(string displayName, int tablePlayerChip)
        {
            text_DisplayName.text = displayName == "" ? "Join" : displayName;
            text_TablePlayerChip.text = displayName == "" ? "" : tablePlayerChip.ToString();
        }

        public void Set_StateText(PlayerState playerState)=> text_state.text = $"{s_playerState[(int)playerState]}";
        public void Set_TablePlayerUI(bool value)
        {
            obj_TableJoin.SetActive(!value);
            obj_TableExit.SetActive(value);
            obj_TablePlayerUI.SetActive(value);
        }
        public void Set_Button_Color(bool isTurn)
        {
            for (int i = 0; i <  img_button.Length; i++)
                img_button[i].color = isTurn ? color_button : Color.black;
        }

        public void Set_CallText(int value) => text_Call.text = $"call [ {value} ]";
        public void Set_CallText_Allin(int value) => text_Call.text = $"all in [ {value} ]";

        public void Set_RaiseText(int value) => text_Raise.text = $"raise [ {value} ]";
        public void Set_RaiseText_Allin(int value) => text_Raise.text = $"all in [ { value } ]";

        public void Set_HandRankText(string value) => text_Handrank.text = value;

        public void Set_Owner(VRCPlayerApi value)
        {
            if (value.IsOwner(gameObject)) return;
            Networking.SetOwner(value, gameObject);
        }
    }
}