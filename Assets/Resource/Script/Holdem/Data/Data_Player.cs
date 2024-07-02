using System;
using UCS;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using static UnityEngine.Rendering.DebugUI;

namespace Holdem
{
    [Serializable]
    public enum CardPatternType : int
    {
        Null = 0,
        Basic = 1,
        Simple = 2,
    }

    [UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
    public class Data_Player : UdonSharpBehaviour
    {
        public int chip
        {
            get => udonChips.chips;
            set
            {
                udonChips.chips = value;
                text_chip.text = chip.ToString();
            }
        }
        public int coin
        {
            get => udonChips.coin;
            set
            {
                udonChips.coin = value;
                text_coin.text = coin.ToString();
            }
        }
        [SerializeField] CardPatternType cardPatternType;
        [SerializeField] Text text_chip;
        [SerializeField] Text text_coin;
        [SerializeField] UdonChips udonChips;

        bool isPlayGame = false;

        public void Start()
        {
            text_chip.text = chip.ToString();
            text_coin.text = coin.ToString();
        }

        public void Pay_Chip(int value) => chip -= value;
        public void Add_Chip(int value) =>  chip += value;
        public void Set_Chip(int value) => chip = value;
        public void Add_Coin(int value) => coin += value;
        public void Set_Coin(int value) => coin = value;

        public void Rebine_Chip()
        {
            if (coin < 20 || chip > 200)
                return;
            coin -= 20;
            chip = 20000;
        }

        public int Get_Chip() => chip;

        public bool Get_isPlayGame() => isPlayGame;
        public void Set_isPlayGame(bool value) => isPlayGame = value;

        public void Set_cardPatternType(CardPatternType value) => cardPatternType = value;
        public CardPatternType Get_cardPatternType() => cardPatternType;
    }
}
