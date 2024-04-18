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

        [SerializeField] MainSystem mainSystem;
        [SerializeField] Sprite[] img_cardPattern_Basic, img_cardPattern_Simple;
        [SerializeField] Data_Card[] data_Card;
        [SerializeField] Transform[] tf_Position;

        CardPatternType cardPatternType;
        int[] shuffleIndex = new int[52];
        int seed = 0;

        private void Start() => Reset_Card();

        public void Reset_Card()
        {
            Set_CardPattern(mainSystem.Get_Data_Player().Get_cardPatternType());

            if (!Networking.IsOwner(gameObject))
                return;

            for (int i = 0; i < data_Card.Length; i++)
            {
                shuffleIndex[i] = i;
                data_Card[i].Set_CardIndex(i);
                data_Card[i].Set_Blind(true);
            }
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
            Set_CardPattern(mainSystem.Get_Data_Player().Get_cardPatternType());

            for (int i = 0; i < data_Card.Length; i++)
            {
                data_Card[i].transform.position = tf_Position[(int)CardPosition.Deck].position + new Vector3(0.0f, 0.0f, -0.0255f + 0.0005f * shuffleIndex[i]);
                data_Card[i].transform.rotation = tf_Position[(int)CardPosition.Deck].rotation;
                data_Card[i].Set_Blind(true);

                if (Networking.IsOwner(gameObject))
                    data_Card[i].Set_Pickupable(true);
            }
        }
        public void Set_CardPattern(CardPatternType cardPatternType)
        {
            if (this.cardPatternType == cardPatternType)
                return;

            this.cardPatternType = cardPatternType;

            Sprite[] patterns = img_cardPattern_Basic;

            switch(cardPatternType)
            {
                case CardPatternType.Basic:
                    patterns = img_cardPattern_Basic;
                    break;
                case CardPatternType.Simple:
                    patterns = img_cardPattern_Simple;
                    break;
            }

            for (int i = 0; i < data_Card.Length; i++)
            {
                data_Card[i].Set_Card_Pattern(patterns[i]);
            }
        }
        public void Set_CardPosition(int index, Transform cardPosition, bool isBlind)
        {
            data_Card[shuffleIndex[index]].transform.position = cardPosition.position;
            data_Card[shuffleIndex[index]].transform.rotation = cardPosition.rotation;
            data_Card[shuffleIndex[index]].Set_Blind(isBlind);
        }
        public void Set_CardPosition(int index, CardPosition cardPosition, bool isBlind)
        {
            data_Card[shuffleIndex[index]].transform.position = tf_Position[(int)cardPosition].position;
            data_Card[shuffleIndex[index]].transform.rotation = tf_Position[(int)cardPosition].rotation;
            data_Card[shuffleIndex[index]].Set_Blind(isBlind);
        }

        public void Set_Blind(int index, bool isBlind) => data_Card[Get_CardIndex(index)].Set_Blind(isBlind);
        public int Get_CardIndex(int index) => shuffleIndex[index];

        public void Set_Owner(VRCPlayerApi value)
        {
            for (int i = 0; i < data_Card.Length; i++)
            {
                data_Card[i].Set_Owner(value);
                data_Card[i].Set_Pickupable(value == default ? false : true);
            }

            if (value.IsOwner(gameObject)) return;
            Networking.SetOwner(value, gameObject);
        }
    }
}
