using Newtonsoft.Json.Linq;
using System;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;

namespace Holdem
{
    [Serializable]
    public enum CardPatternType : int
    {
        Null = 0,
        Basic = 1,
        Simple = 2,
    }

    public class Data_Player : UdonSharpBehaviour
    {
        int chip = 20000;
        [SerializeField] CardPatternType cardPatternType;
        [SerializeField] Text text_chip;

        bool isPlayGame = false;

        public void DoSync()
        {
            Update_UI();
            RequestSerialization();
        }
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
            text_chip.text = chip.ToString();
        }

        public void Pay_Chip(int value)
        {
            chip -= value;
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
