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
        River = 5
    }

    public class Table_Card : UdonSharpBehaviour
    {
        int[] shuffleIndex = new int[52];

        [SerializeField] Sprite[] img_cardPattern1;
        [SerializeField] Data_Card[] data_Card;
        [SerializeField] Transform[] tf_Position;

        int seed = 0;

        private void Start() => Reset_Card();

        public void Reset_Card()
        {
            if (!Networking.LocalPlayer.IsOwner(gameObject))
                return;

            for (int i = 0; i < data_Card.Length; i++)
            {
                shuffleIndex[i] = i;
                data_Card[i].Set_CardIndex(i);
                data_Card[i].Set_Card_Pattern(img_cardPattern1[i]);
                data_Card[i].Set_Blind(true);
            }
        }
        public void Shuffle_Card()
        {
            if (!Networking.LocalPlayer.IsOwner(gameObject))
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
            for (int i = 0; i < data_Card.Length; i++)
            {
                data_Card[i].transform.position = tf_Position[(int)CardPosition.Deck].position + new Vector3(0.0f, 0.0f, -0.0255f + 0.0005f * shuffleIndex[i]);
                data_Card[i].transform.rotation = tf_Position[(int)CardPosition.Deck].rotation;
                data_Card[i].Set_Blind(true);

                if (Networking.LocalPlayer.IsOwner(gameObject))
                    data_Card[i].Set_Pickupable(true);
            }
        }
        public void Set_CardPosition(int index, CardPosition cardPosition, bool isBlind)
        {
            data_Card[shuffleIndex[index]].transform.position = tf_Position[(int)cardPosition].position;
            data_Card[shuffleIndex[index]].transform.rotation = tf_Position[(int)cardPosition].rotation;
            data_Card[shuffleIndex[index]].Set_Blind(isBlind);
        }
        public Data_Card Get_Card()
        {
            index = index >= data_Card.Length - 1 ? 1 : index + 1;
            return data_Card[shuffleIndex[index - 1]];
        }
        public void Set_Owner(VRCPlayerApi value)
        {
            Networking.SetOwner(value, gameObject);

            for (int i = 0; i < data_Card.Length; i++)
            {
                data_Card[i].Set_Owner(value);
                data_Card[i].Set_Pickupable(value == default ? false : true);
            }
        }
    }
}
