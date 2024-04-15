using Newtonsoft.Json.Linq;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;

namespace Holdem
{
    public class Data_Player : UdonSharpBehaviour
    {
        int chip = 10000;
        [SerializeField] Text text_chip;

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
    }
}
