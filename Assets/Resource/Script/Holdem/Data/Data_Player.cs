using Newtonsoft.Json.Linq;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;

namespace Holdem
{
    public enum CardPatternType : int
    {
        Basic = 0,
        Simple = 1,
    }

    public class Data_Player : UdonSharpBehaviour
    {
        int chip = 20000;
        CardPatternType cardPatternType = CardPatternType.Basic;

        [SerializeField] Text text_chip;

        Table_Card table_Card;
        bool isPlayGame = false;

        public void DoSync() => RequestSerialization();
        public override void OnDeserialization()
        {
            Update_UI();
        }

        public void Start()
        {
            text_chip.text = chip.ToString();
        }

        public void Update_UI()
        {

        }

        public void Add_Chip(int value)
        {
            chip += value;
            text_chip.text = value.ToString();
            DoSync();
        }
        public int Get_Chip() => chip;

        public bool Get_isPlayGame() => isPlayGame;
        public void Set_isPlayGame(bool value)
        {
            isPlayGame = value;
            DoSync();
        }

        public void Set_cardPatternType(CardPatternType value) => cardPatternType = value;
        public CardPatternType Get_cardPatternType() => cardPatternType;
    }
}
