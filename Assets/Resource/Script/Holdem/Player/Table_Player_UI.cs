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
        [SerializeField] TextMeshProUGUI text_Time;
        [SerializeField] TextMeshPro text_state, text_potSize;
        [SerializeField] GameObject obj_UIJoin, obj_UIPlayer;
        [SerializeField] GameObject obj_TableJoin, obj_TableExit;
        [SerializeField] GameObject obj_DisplayHide;

        [SerializeField] Image[] img_button;
        [SerializeField] Image[] img_cards;

        [SerializeField] Table_System table_System;
        [SerializeField] TextMeshProUGUI[] text_raiseOption;
        [SerializeField] TextMeshPro text_spectator;

        [UdonSynced]
        int _handRank = 0;
        public int handRank
        {
            get
            {
                return _handRank;
            }
            set
            {
                _handRank = value;
            }
        }
        bool isDisplayToggle = false;
        public void Set_DisplayToggle()
        {
            isDisplayToggle = !isDisplayToggle;

            if (!isDisplayToggle)
            {
                text_raiseOption[0].text = "+100";
                text_raiseOption[1].text = "+500";
                text_raiseOption[2].text = "+1000";
                text_raiseOption[3].text = "+5000";
                text_raiseOption[4].text = "+10000";
            }
            else
            {
                text_raiseOption[0].text = $"+{100.0 / table_System.Get_TableBB()}BB";
                text_raiseOption[1].text = $"+{500.0 / table_System.Get_TableBB()}BB";
                text_raiseOption[2].text = $"+{1000.0 / table_System.Get_TableBB()}BB";
                text_raiseOption[3].text = $"+{5000.0 / table_System.Get_TableBB()}BB";
                text_raiseOption[4].text = $"+{10000.0 / table_System.Get_TableBB()}BB";
            }
        }

        Color color_button = new Color(0.0627451f, 0.509804f, 1, 1);
        string[] s_playerState = new string[8] { "", "Wait", "Turn", "Call", "Check", "Raise", "ALLIN", "Fold" };

        public void Update_Timer(float time)
        {
            text_Time.text = (30.0f - time).ToString("0.0");
            if (time > 25) text_Time.color = Color.red;
            else if (time > 20) text_Time.color = Color.yellow;
            else if (time > 10) text_Time.color = Color.white;
            else text_Time.color = Color.green;
        }

        public void Set_Rotation()
        {
            Vector3 relativePos = Networking.LocalPlayer.GetPosition() - transform.position;
            Quaternion rotation = Quaternion.LookRotation(relativePos);
            text_Handrank.transform.rotation = rotation;
        }

        public void Update_UI(string displayName, int tablePlayerChip)
        {
            text_DisplayName.text = displayName == "" ? "Join" : displayName;
            text_TablePlayerChip.text = displayName == "" ? "" : tablePlayerChip.ToString();
            Set_Player_Spectator();
        }

        public void Set_StateText(PlayerState playerState, TableState tableState)
        {
            if (text_TablePlayerChip.text == "")
            {
                text_state.text = "";
                text_state.color = Color.white;
                return;
            }

            if (tableState == TableState.Wait && playerState != PlayerState.OutOfGame && playerState != PlayerState.Fold)
                text_state.text = table_System._mainSystem.Get_HandRank(handRank);
            else
                text_state.text = $"{s_playerState[(int)playerState]}";

            if (text_state.text == "Fold")
                text_state.color = Color.red;
            else if (text_state.text == "Wait")
                text_state.color = Color.yellow;
            else
                text_state.color = Color.white;

            Set_Player_Spectator();
        }
        public void Set_BetSize(int size)
        {
            text_potSize.text = size == 0 ? "" : $"{size}";
            Set_Player_Spectator();
        }

        public void Set_Player_Spectator()
        {
            text_spectator.text = $"{text_DisplayName.text}\n{text_state.text}\n{text_potSize.text}";
            if (text_state.text == "Fold")
                text_spectator.color = Color.red;
            else if (text_state.text == "Wait")
                text_spectator.color = Color.yellow;
            else
                text_spectator.color = Color.white;
        }

        public void Set_TablePlayerUI(bool value)
        {
            obj_TableJoin.SetActive(!value);
            obj_TableExit.SetActive(value);
            obj_UIPlayer.SetActive(value);
        }
        public void Set_TablePlayerUI_Height(bool value)
        {
            float height = Networking.LocalPlayer.GetAvatarEyeHeightAsMeters() - 1.0f;
            obj_UIJoin.transform.localPosition =
                new Vector3(
                    obj_UIJoin.transform.localPosition.x, 
                    value ? height + 0.13f + table_System._mainSystem.Get_Display_Height() : 0.13f, 
                    obj_UIJoin.transform.localPosition.z);
            obj_UIPlayer.transform.localPosition =
                new Vector3(
                    obj_UIPlayer.transform.localPosition.x, 
                    value ? height + table_System._mainSystem.Get_Display_Height() : 0, 
                    obj_UIPlayer.transform.localPosition.z);
        }

        public void Set_Button_Color(bool isTurn)
        {
            for (int i = 0; i < img_button.Length; i++)
                img_button[i].color = isTurn ? color_button : Color.black;
        }

        public void Set_CallText(int value)
        {
            if (!isDisplayToggle)
                text_Call.text = value == 0 ? $"check" : $"call [ {value} ]";
            else
                text_Call.text = value == 0 ? $"check" : $"call [ {(double)value / table_System.Get_TableBB()}BB ]";
        }
        public void Set_CallText_Allin(int value)
        {
            if (!isDisplayToggle)
                text_Call.text = $"all in [ {value} ]";
            else
                text_Call.text = $"all in [ {(double)value / table_System.Get_TableBB()}BB ]";
        }
        public void Set_RaiseText(int value)
        {
            if (!isDisplayToggle)
                text_Raise.text = $"raise [ {value} ]";
            else
                text_Raise.text = $"raise [ {(double)value / table_System.Get_TableBB()}BB ]";
        }
        public void Set_RaiseText_Allin(int value)
        {
            if (!isDisplayToggle)
                text_Raise.text = $"all in [ {value} ]";
            else
                text_Raise.text = $"all in [ {(double)value / table_System.Get_TableBB()}BB ]";
        }

        public void Set_HandRankText(int value)
        {
            handRank = value;
            text_Handrank.text = table_System._mainSystem.Get_HandRank(handRank);
        }
        public void Set_CardImage(int[] table_Cards, int idx)
        {
            if (!Networking.IsOwner(gameObject))
                return;

            img_cards[0].sprite = table_System._mainSystem.Get_CardPattern()[table_Cards[0 + idx]];
            img_cards[1].sprite = table_System._mainSystem.Get_CardPattern()[table_Cards[9 + idx]];
            img_cards[2].sprite = table_System._mainSystem.Get_CardPattern()[table_Cards[18]];
            img_cards[3].sprite = table_System._mainSystem.Get_CardPattern()[table_Cards[19]];
            img_cards[4].sprite = table_System._mainSystem.Get_CardPattern()[table_Cards[20]];
            img_cards[5].sprite = table_System._mainSystem.Get_CardPattern()[table_Cards[21]];
            img_cards[6].sprite = table_System._mainSystem.Get_CardPattern()[table_Cards[22]];
        }
        public void Set_DisplayHide()
        {
            obj_DisplayHide.SetActive(!obj_DisplayHide.activeSelf);
        }

        public void Set_Owner(VRCPlayerApi value)
        {
            if (value.IsOwner(gameObject)) return;
            Networking.SetOwner(value, gameObject);
        }
    }
}