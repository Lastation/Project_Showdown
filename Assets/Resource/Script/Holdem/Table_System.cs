using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace Holdem
{
    public enum TableState : int
    {
        Wait = 0,
        Hand = 1,
        Flop = 2,
        Turn = 3,
        River = 4,
        Open = 5,
    }
    public enum PlayerState : int
    {
        OutOfGame = 0,
        Wait = 1,
        Turn = 2,
        Call = 3,
        Fold = 4,
    }

    public class Table_System : UdonSharpBehaviour
    {
        /// <summary>
        /// Variables
        /// </summary>
        #region Sync Varialbes
        [UdonSynced] TableState tableState;
        [UdonSynced] PlayerState[] playerState = new PlayerState[9];
        [UdonSynced] int tableTotalPot = 0;
        [UdonSynced] int tableCallSize = 0;
        [UdonSynced] int table_BB = 200;
        [UdonSynced] int[] playerBetSize = new int[9];
        [UdonSynced] int[] table_Cards = new int[23];
        [UdonSynced] string[] s_handRank = new string[9];
        #endregion
        #region Static Variables
        [SerializeField] MainSystem mainSystem;
        [SerializeField] AudioSource audioSource;
        [SerializeField] Table_Player[] table_Players;
        [SerializeField] Table_System_UI table_System_UI;
        [SerializeField] Table_Card table_Card;
        #endregion
        #region Local Variables
        int table_TurnIndex, table_DealerIndex = 0;
        #endregion

        /// <summary>
        /// Functions
        /// </summary>
        #region State Setting
        public TableState Get_TableState() => tableState;
        public void Set_TableState(TableState state)
        {
            tableState = state;
            DoSync();
        }

        public void Set_PlayerState(int idx, PlayerState state)
        {
            playerState[idx] = state;
            Update_Syncs();
            DoSync();
        }
        public void Set_PlayerState(int idx)
        {
            for (int i = 0; i < playerState.Length; i++)
            {
                if (playerState[Get_TurnIndex(idx + i)] == PlayerState.Call)
                    playerState[Get_TurnIndex(idx + i)] = PlayerState.Wait;
            }
            playerState[idx] = PlayerState.Call;
        }
        public void Reset_PlayerState(int index)
        {
            if (!Networking.IsOwner(gameObject))
                return;

            Set_PlayerState(index, PlayerState.OutOfGame);
        }
        #endregion
        #region Chip Setting
        public void Set_TableTotalPot(int value)
        {
            tableTotalPot = value;
            DoSync();
        }
        public void Set_TableCallSize(int value)
        {
            tableCallSize = value;
            DoSync();
        }
        public int Get_TableCallSize() => tableCallSize;
        public void Set_PlayerBetSize(int idx, int value)
        {
            playerBetSize[idx] = value;
            DoSync();
        }
        #endregion
        #region Sound Effect
        public void Play_AudioClip(SE_Table_Index index) => audioSource.PlayOneShot(mainSystem.Get_AudioClip_Table((int)index));
        #endregion
        #region Action
        public void Set_BetAction(int tableNumber, int value)
        {
            if (value == -1)
            {
                Set_PlayerState(tableNumber, PlayerState.Fold);
                Set_NextTurn();
                return;
            }
            else
            {
                if (value != tableCallSize)
                    Set_PlayerState(tableNumber);
                tableCallSize = value;
                tableTotalPot += value;
                Set_NextTurn();
            }
            Update_Syncs();
            DoSync();
        }
        public void Set_NextTurn()
        {
            bool isNextStep = true;

            for (int i = 0; i < playerState.Length; i++)
            {
                if (playerState[i] == PlayerState.Wait)
                {
                    isNextStep = false;
                    break;
                }
            }

            if (isNextStep)
                Set_GameAuto();

            Set_TurnIndex(table_TurnIndex + 1);
        }
        #endregion
        #region Game
        public void Set_GameStart()
        {
            if (tableState != TableState.Wait)
                return;
            Set_GameAuto();
        }
        public void Set_GameAuto()
        {
            switch (tableState)
            {
                case TableState.Wait:
                    table_Card.Reset_CardPosition();
                    table_Card.Shuffle_Card();
                    Set_TableState(TableState.Hand);
                    table_DealerIndex += 1;
                    break;
                case TableState.Hand:
                    for (int i = 0; i < Get_TablePlayerData.Length; i++)
                    {
                        if (playerState[i] != PlayerState.Wait)
                            continue;
                        table_Card.Set_CardPosition(i, Get_TablePlayerData[i].Get_CardPosition(0), true);
                        Set_TableCard(i, table_Card.Get_CardIndex(i));
                        table_Card.Set_CardPosition(i + 9, Get_TablePlayerData[i].Get_CardPosition(1), true);
                        Set_TableCard(i + 9, table_Card.Get_CardIndex(i + 9));
                    }
                    Set_TurnIndex(table_DealerIndex);
                    Set_HandRank();
                    Set_TableState(TableState.Flop);
                    break;
                case TableState.Flop:
                    for (int i = 0; i < 3; i++)
                    {
                        table_Card.Set_CardPosition(i + 18, (CardPosition)(i + 1), true);
                        Set_TableCard(i + 18, table_Card.Get_CardIndex(i + 18));
                    }
                    Set_HandRank();
                    Set_TableState(TableState.Turn);
                    break;
                case TableState.Turn:
                    table_Card.Set_CardPosition(21, CardPosition.Turn, true);
                    Set_TableCard(21, table_Card.Get_CardIndex(21));
                    Set_HandRank();
                    Set_TableState(TableState.River);
                    break;
                case TableState.River:
                    table_Card.Set_CardPosition(22, CardPosition.Turn, true);
                    Set_TableCard(22, table_Card.Get_CardIndex(22));
                    Set_HandRank();
                    Set_TableState(TableState.Open);
                    break;
                case TableState.Open:
                    for (int i = 0; i < Get_TableCard.Length; i++)
                        table_Card.Set_Blind(i, false);
                    Set_TableState(TableState.Wait);
                    break;
            }
        }
        public int Get_TurnIndex(int value) => value % 9;
        public void Set_TurnIndex(int value)
        {
            for (int i = 0; i < table_Players.Length; i++)
            {
                if (playerState[Get_TurnIndex(i + value)] != PlayerState.Wait)
                    continue;

                table_TurnIndex = Get_TurnIndex(i + value);
                Set_PlayerState(table_TurnIndex, PlayerState.Turn);
                break;
            }
        }
        #endregion
        #region HandRank
        private string[] s_HandSuit = new string[4]
        {
            "♠",
            "♦",
            "♥",
            "♣"
        };
        private string[] s_HandNumber = new string[14]
        {
            "Null",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "Jack",
            "Queen",
            "King",
            "Ace",
        };

        HandRanking[] p_handRank = new HandRanking[9];
        HandNumber[] p_handNumber = new HandNumber[9];
        HandSuit[] p_handSuit = new HandSuit[9];

        public void Set_HandRank()
        {
            for (int i = 0; i < table_Players.Length; i++)
            {
                p_handRank[i] = Calculate_HandRank(i);
                s_handRank[i] = $"{s_HandSuit[(int)p_handSuit[i]]}{s_HandNumber[(int)p_handNumber[i]]} {mainSystem.s_HankRank((int)p_handRank[i])}";
            }
            Update_Syncs();
            DoSync();
        }

        private HandRanking Calculate_HandRank(int playerID)
        {
            int index = 0, tableSuit, tableNumber;

            int[] pair = new int[13];
            int[] suit = new int[4];
            bool[] hand = new bool[52];

            for (int i = 0; i < 7; i++)
            {
                if (i == 0) index = playerID;
                else if (i == 1) index = playerID + 9;
                else index = i + 16;

                if (table_Cards[index] == 52)
                    continue;

                tableSuit = Mathf.FloorToInt(table_Cards[index] / 13);
                tableNumber = table_Cards[index] % 13;

                suit[tableSuit]++;
                pair[tableNumber]++;
                hand[table_Cards[index]] = true;

                if (tableNumber == 0)
                {
                    p_handSuit[playerID] = (HandSuit)tableSuit;
                    p_handNumber[playerID] = HandNumber.Ace;
                }
                else if ((int)p_handNumber[playerID] < tableNumber)
                {
                    p_handSuit[playerID] = (HandSuit)tableSuit;
                    p_handNumber[playerID] = (HandNumber)tableNumber;
                }
            }

            if (isRoyalFlush(hand, playerID)) return HandRanking.RoyalFlush;
            if (isStraightFlush(hand, playerID)) return HandRanking.StraightFlush;
            if (isFourOfAKind(pair, playerID)) return HandRanking.FourOfAKind;
            if (isFullHouse(hand, pair, playerID)) return HandRanking.FullHouse;
            if (isFlush(hand, suit, playerID)) return HandRanking.Flush;
            if (isBackStraight(hand, pair, playerID)) return HandRanking.BackStraight;
            if (isMountain(hand, pair, playerID)) return HandRanking.Mountain;
            if (isStraight(hand, pair, playerID)) return HandRanking.Straight;
            if (isThreeOfAKind(hand, pair, playerID)) return HandRanking.ThreeOfAKind;
            if (isTwoPair(hand, pair, playerID)) return HandRanking.TwoPair;
            if (isOnePair(hand, pair, playerID)) return HandRanking.OnePair;

            return HandRanking.HighCard;
        }
        private bool isRoyalFlush(bool[] hand, int playerID)
        {
            for (int i = 0; i < 4; i++)
                if (hand[i * 13 + 0] == true &&
                    hand[i * 13 + 9] == true &&
                    hand[i * 13 + 10] == true &&
                    hand[i * 13 + 11] == true &&
                    hand[i * 13 + 12] == true)
                {
                    p_handSuit[playerID] = (HandSuit)i;
                    p_handNumber[playerID] = HandNumber.Ace;
                    return true;
                }
            return false;
        }
        private bool isStraightFlush(bool[] hand, int playerID)
        {
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 9; j++)
                    if (hand[i * 13 + j + 0] == true &&
                        hand[i * 13 + j + 1] == true &&
                        hand[i * 13 + j + 2] == true &&
                        hand[i * 13 + j + 3] == true &&
                        hand[i * 13 + j + 4] == true)
                    {
                        p_handSuit[playerID] = (HandSuit)i;
                        p_handNumber[playerID] = (HandNumber)(j + 4);
                        return true;
                    }
            return false;
        }
        private bool isFourOfAKind(int[] pair, int playerID)
        {
            for (int i = 0; i < pair.Length; i++)
                if (pair[i] == 4)
                {
                    p_handSuit[playerID] = HandSuit.Spade;
                    p_handNumber[playerID] = (HandNumber)i;
                    return true;
                }
            return false;
        }
        private bool isFullHouse(bool[] hand, int[] pair, int playerID)
        {
            int count = 0;

            if (isThreeOfAKind(hand, pair, playerID))
            {
                for (int i = 0; i < pair.Length; i++)
                    if (pair[i] >= 2)
                        count++;
                if (count >= 2)
                    return true;
            }
            return false;
        }
        private bool isFlush(bool[] hand, int[] suit, int playerID)
        {
            for (int i = 0; i < suit.Length; i++)
                if (suit[i] >= 5)
                {
                    p_handSuit[playerID] = (HandSuit)i;
                    for (int j = 12; i > 0; i--)
                        if (hand[i * 13 + j] == true)
                        {
                            p_handNumber[playerID] = (HandNumber)j;
                            break;
                        }
                    if (hand[i * 13] == true) p_handNumber[playerID] = HandNumber.Ace;
                    return true;
                }
            return false;
        }
        private bool isBackStraight(bool[] hand, int[] pair, int playerID)
        {
            if (pair[0] != 0 && pair[1] != 0 && pair[2] != 0 && pair[3] != 0 && pair[4] != 0)
            {
                if (hand[00] == true) p_handSuit[playerID] = HandSuit.Spade;
                else if (hand[13] == true) p_handSuit[playerID] = HandSuit.Diamond;
                else if (hand[26] == true) p_handSuit[playerID] = HandSuit.Heart;
                else if (hand[39] == true) p_handSuit[playerID] = HandSuit.Clover;
                p_handNumber[playerID] = HandNumber.Ace;
                return true;
            }
            return false;
        }
        private bool isMountain(bool[] hand, int[] pair, int playerID)
        {
            if (pair[0] != 0 && pair[10] != 0 && pair[10] != 0 && pair[11] != 0 && pair[12] != 0)
            {
                if (hand[00] == true) p_handSuit[playerID] = HandSuit.Spade;
                else if (hand[13] == true) p_handSuit[playerID] = HandSuit.Diamond;
                else if (hand[26] == true) p_handSuit[playerID] = HandSuit.Heart;
                else if (hand[39] == true) p_handSuit[playerID] = HandSuit.Clover;
                p_handNumber[playerID] = HandNumber.Ace;
                return true;
            }
            return false;
        }
        private bool isStraight(bool[] hand, int[] pair, int playerID)
        {
            for (int i = 0; i < 9; i++)
                if (pair[i] != 0 && pair[i + 1] != 0 && pair[i + 2] != 0 && pair[i + 3] != 0 && pair[i + 4] != 0)
                {
                    if (hand[i + 04] == true) p_handSuit[playerID] = HandSuit.Spade;
                    else if (hand[i + 17] == true) p_handSuit[playerID] = HandSuit.Diamond;
                    else if (hand[i + 30] == true) p_handSuit[playerID] = HandSuit.Heart;
                    else if (hand[i + 43] == true) p_handSuit[playerID] = HandSuit.Clover;
                    p_handNumber[playerID] = (HandNumber)(i + 4);
                    return true;
                }
            return false;
        }
        private bool isThreeOfAKind(bool[] hand, int[] pair, int playerID)
        {
            for (int i = 0; i < pair.Length; i++)
                if (pair[i] == 3)
                {
                    if (hand[i + 00] == true) p_handSuit[playerID] = HandSuit.Spade;
                    else if (hand[i + 13] == true) p_handSuit[playerID] = HandSuit.Diamond;
                    else if (hand[i + 26] == true) p_handSuit[playerID] = HandSuit.Heart;
                    else if (hand[i + 39] == true) p_handSuit[playerID] = HandSuit.Clover;

                    if (i == 0) p_handNumber[playerID] = HandNumber.Ace;
                    else p_handNumber[playerID] = (HandNumber)i;
                    return true;
                }
            return false;
        }
        private bool isTwoPair(bool[] hand, int[] pair, int playerID)
        {
            int count = 0;
            int[] num = new int[3];

            for (int i = 0; i < pair.Length; i++)
                if (pair[i] == 2)
                {
                    num[count] = i;
                    count++;
                }
            if (count >= 2)
            {
                if (num[0] == 0)
                {
                    if (hand[num[0] + 00] == true) p_handSuit[playerID] = HandSuit.Spade;
                    else if (hand[num[0] + 13] == true) p_handSuit[playerID] = HandSuit.Diamond;
                    else if (hand[num[0] + 26] == true) p_handSuit[playerID] = HandSuit.Heart;
                    else if (hand[num[0] + 39] == true) p_handSuit[playerID] = HandSuit.Clover;
                    p_handNumber[playerID] = HandNumber.Ace;
                }
                else if (num[2] != 0)
                {
                    if (hand[num[2] + 00] == true) p_handSuit[playerID] = HandSuit.Spade;
                    else if (hand[num[2] + 13] == true) p_handSuit[playerID] = HandSuit.Diamond;
                    else if (hand[num[2] + 26] == true) p_handSuit[playerID] = HandSuit.Heart;
                    else if (hand[num[2] + 39] == true) p_handSuit[playerID] = HandSuit.Clover;
                    p_handNumber[playerID] = (HandNumber)num[2];
                }
                else
                {
                    if (hand[num[1] + 00] == true) p_handSuit[playerID] = HandSuit.Spade;
                    else if (hand[num[1] + 13] == true) p_handSuit[playerID] = HandSuit.Diamond;
                    else if (hand[num[1] + 26] == true) p_handSuit[playerID] = HandSuit.Heart;
                    else if (hand[num[1] + 39] == true) p_handSuit[playerID] = HandSuit.Clover;
                    p_handNumber[playerID] = (HandNumber)num[1];
                }

                return true;
            }
            return false;
        }
        private bool isOnePair(bool[] hand, int[] pair, int playerID)
        {
            for (int i = 0; i < pair.Length; i++)
                if (pair[i] == 2)
                {
                    if (hand[i + 00] == true) p_handSuit[playerID] = HandSuit.Spade;
                    else if (hand[i + 13] == true) p_handSuit[playerID] = HandSuit.Diamond;
                    else if (hand[i + 26] == true) p_handSuit[playerID] = HandSuit.Heart;
                    else if (hand[i + 39] == true) p_handSuit[playerID] = HandSuit.Clover;

                    if (i == 0) p_handNumber[playerID] = HandNumber.Ace;
                    else p_handNumber[playerID] = (HandNumber)i;
                    return true;
                }
            return false;
        }
        #endregion

        /// <summary>
        /// Get & Set Functions
        /// </summary>
        public Table_Player[] Get_TablePlayerData => table_Players;
        public int Get_TablePlayerCount()
        {
            int count = 0;
            for (int i = 0; i < table_Players.Length; i++)
                if (table_Players[i].isPlaying())
                    count++;
            return count;
        }

        public void Set_TableCard(int index, int value) => table_Cards[index] = value;
        public int[] Get_TableCard => table_Cards;

        public Table_System_UI Get_TableSystemUI() => table_System_UI;

        /// <summary>
        ///  Network Fuctions
        /// </summary>
        #region Sync
        public void Start()
        {
            for (int i = 0; i < playerState.Length; i++)
                playerState[i] = PlayerState.OutOfGame;

            for (int i = 0; i < playerBetSize.Length; i++)
                playerBetSize[i] = 0;

            for (int i = 0; i < table_Cards.Length; i++)
                table_Cards[i] = 0;

            for (int i = 0; i < s_handRank.Length; i++)
                s_handRank[i] = "";
        }
        /// Just Test Delete Later
        public void Update()
        {
            if (!Networking.IsOwner(gameObject))
                return;

            if (Input.GetKeyDown(KeyCode.Q))
                Set_PlayerState(Random.Range(0, 9), (PlayerState)Random.Range(0, 6));
        }
        public void DoSync() => RequestSerialization();
        public override void OnDeserialization()
        {
            Update_Syncs();
        }
        public void Update_Syncs()
        {
            Update_PlayerState();
            Update_HandRank();
            table_System_UI.Set_TableState(9 - Get_TablePlayerCount(), tableState != TableState.Wait);
        }
        public void Update_PlayerState()
        {
            for (int i = 0; i < table_Players.Length; i++)
                table_Players[i].Get_table_Player_UI().Set_StateText(playerState[i]);
        }
        public void Update_HandRank()
        {
            for (int i = 0; i < table_Players.Length; i++)
            {
                if (!Networking.IsOwner(table_Players[i].gameObject))
                    continue;
                table_Players[i].Get_table_Player_UI().Set_HandRankText(s_handRank[i]);
            }
        }
        #endregion
        #region Networking
        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            if (!Networking.IsOwner(gameObject)) return;
            Update_Syncs();
            DoSync();
        }
        public override void OnPlayerLeft(VRCPlayerApi player)
        {
            if (!Networking.IsOwner(gameObject)) return;

            for (int i = 0; i < playerState.Length; i++)
                if (Get_TablePlayerData[i].Get_DisplayName() == player.displayName)
                    Set_PlayerState(i, PlayerState.OutOfGame);
            Update_Syncs();
            DoSync();
        }
        #endregion
    }

    public enum HandRanking : int
    {
        HighCard = 0,
        OnePair,
        TwoPair,
        ThreeOfAKind,
        Straight,
        BackStraight,
        Mountain,
        Flush,
        FullHouse,
        FourOfAKind,
        StraightFlush,
        RoyalFlush
    }
    public enum HandSuit : int
    {
        Spade = 0,
        Diamond = 1,
        Heart = 2,
        Clover = 3
    }
    public enum HandNumber : int
    {
        Ace = 13,
        King = 12,
        Queen = 11,
        Jack = 10,
        Ten = 9,
        Nine = 8,
        Eight = 7,
        Seven = 6,
        Six = 5,
        Five = 4,
        Four = 3,
        Three = 2,
        Two = 1,
    }
}
