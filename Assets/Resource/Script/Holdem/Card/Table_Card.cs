using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace Holdem
{
    public enum CardPosition : int
    {
        Deck = 0,
        Flop1 = 1,
        Flop2 = 2,
        Flop3 = 3,
        Turn = 4,
        River = 5,
    }

    public class Table_Card : UdonSharpBehaviour
    {
        [SerializeField] Table_System table_System;
        [SerializeField] Data_Card[] data_Card;
        [SerializeField] Transform[] tf_Position;
        int[] shuffleIndex = new int[52];
        int seed = 0;
        CardPatternType cardPatternType = CardPatternType.Null;

        private void Start() => Init_Card();

        public void Init_Card()
        {
            Set_CardPattern();

            if (!Networking.IsOwner(gameObject))
                return;

            for (int i = 0; i < data_Card.Length; i++)
            {
                shuffleIndex[i] = i;
                data_Card[i].Set_Blind(true);
            }
        }
        public void Reset_Card()
        {
            Reset_CardPosition();
            Shuffle_Card();
        }
        public void Shuffle_Card()
        {
            if (!Networking.IsOwner(gameObject))
                return;

            seed = Random.Range(int.MinValue, int.MaxValue);
            Random.InitState(seed);

            for (int i = 0; i < data_Card.Length; i++)
            {
                int rand = Random.Range(i, data_Card.Length);
                int tmp = shuffleIndex[i];
                shuffleIndex[i] = shuffleIndex[rand];
                shuffleIndex[rand] = tmp;
            }
        }
        public void Reset_CardPosition()
        {
            Set_CardPattern();

            for (int i = 0; i < data_Card.Length; i++)
            {
                data_Card[i].transform.rotation = tf_Position[(int)CardPosition.Deck].rotation;
                data_Card[i].transform.position = tf_Position[(int)CardPosition.Deck].position;
                data_Card[i].Set_Blind(true);
            }
        }
        public void Set_Pickupable(bool isPickupable)
        {
            for (int i = 0; i < data_Card.Length; i++)
                data_Card[i].Set_Pickupable(isPickupable);
        }
        public void Set_CardPattern()
        {
            if (cardPatternType == table_System._mainSystem.Get_Data_Player().Get_cardPatternType())
                return;

            cardPatternType = table_System._mainSystem.Get_Data_Player().Get_cardPatternType();

            for (int i = 0; i < data_Card.Length; i++)
                data_Card[i].Set_Card_Pattern();
        }
        public void Set_CardPosition(int index, Transform cardPosition, bool isBlind)
        {
            data_Card[index].transform.rotation = cardPosition.rotation;
            data_Card[index].transform.position = cardPosition.position;
            data_Card[index].Set_Card_Index(shuffleIndex[index]);
            data_Card[index].Set_Blind(isBlind);
        }

        public void Set_CardPosition(int index, CardPosition cardPosition, bool isBlind)
        {
            data_Card[index].transform.rotation = tf_Position[(int)cardPosition].rotation;
            data_Card[index].transform.position = tf_Position[(int)cardPosition].position;
            data_Card[index].Set_Card_Index(shuffleIndex[index]);
            data_Card[index].Set_Blind(isBlind);
        }
        public void Set_CardRotation(int index) => data_Card[index].Set_Rotation();

        public void Set_Blind(int index, bool isBlind)
        {
            data_Card[index].Set_Blind(isBlind);
        }
        public int Get_CardIndex(int index) => data_Card[index].Get_CardIndex();
        public int Get_Seed() => seed;

        public void Set_Owner(VRCPlayerApi value)
        {
            for (int i = 0; i < data_Card.Length; i++)
                data_Card[i].Set_Owner(value);

            if (value.IsOwner(gameObject)) return;
            Networking.SetOwner(value, gameObject);
        }
    }
}