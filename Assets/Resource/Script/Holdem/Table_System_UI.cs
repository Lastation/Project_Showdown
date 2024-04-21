using TMPro;
using UdonSharp;
using UnityEngine;

namespace Holdem
{
    public class Table_System_UI : UdonSharpBehaviour
    {
        [SerializeField] MainSystem mainSystem;

        [SerializeField] TextMeshProUGUI text_Dealer;
        [SerializeField] TextMeshProUGUI text_TableState;
        [SerializeField] TextMeshProUGUI text_TablePot;

        public void Set_Dealer_Displayname(string value) => text_Dealer.text = $"Dealer : {value}";
        public void Set_TableState(int count, bool isProgress)
        {
            switch (isProgress)
            {
                case true:
                    text_TableState.text = mainSystem.s_TableState_progress;
                    break;
                case false:
                    text_TableState.text = $"{mainSystem.s_TableState_wait} : {count}";
                    break;
            }
        }
        public void Set_TablePot(int value) => text_TablePot.text = value == 0 ? "" : $"Table Pot : {value}";
    }
}