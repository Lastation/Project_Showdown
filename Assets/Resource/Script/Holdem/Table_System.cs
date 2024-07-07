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
        Check = 4,
        Raise = 5,
        ALLIN = 6,
        Fold = 7
    }
    public enum HandRanking : int
    {
        HighCard = 0,
        OnePair,
        TwoPair,
        ThreeOfAKind,
        Straight,
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

    public class Table_System : UdonSharpBehaviour
    {
        /// Variables
        #region Sync Varialbes
        [UdonSynced] TableState tableState;
        [UdonSynced] PlayerState[] playerState = new PlayerState[9];
        [UdonSynced] int _tableTotalPot = 0;
        [UdonSynced] int[] tableSidePot = new int[9];
        [UdonSynced] int tableCallSize = 200;
        [UdonSynced] int table_BB = 200;
        [UdonSynced] int[] table_Cards = new int[23];
        [UdonSynced] int[] handRank = new int[9];
        #endregion

        #region Static Variables
        [SerializeField] int[] playerBetSize = new int[9];
        [SerializeField] int[] playerChip = new int[9];
        [SerializeField] MainSystem mainSystem;
        [SerializeField] AudioSource audioSource;
        [SerializeField] Table_Player[] table_Players;
        [SerializeField] Table_System_UI table_System_UI;
        [SerializeField] Table_Card table_Card;
        [SerializeField] GameObject obj_dealerBtn;
        [SerializeField] GameObject obj_turnArrow;
        #endregion
        #region Local Variables
        int table_TurnIndex, table_DealerIndex = 0;
        int tableTotalPot
        {
            get => _tableTotalPot;
            set
            {
                int chipSize = value - _tableTotalPot;

                for (int i = 0; i < tableSidePot.Length; i++)
                {
                    if (!Get_PlayerInGame(i))
                        continue;

                    if (table_Players[i].Get_TablePlayerChip() < chipSize && table_Players[i].Get_BetSize() < chipSize)
                        tableSidePot[i] += Mathf.Max(table_Players[i].Get_TablePlayerChip(), table_Players[i].Get_BetSize());
                    else
                        tableSidePot[i] += chipSize;
                }
                _tableTotalPot = value;
            }
        }

        public MainSystem _mainSystem => mainSystem;
        #endregion

        /// Functions
        #region State Setting
        public TableState Get_TableState() => tableState;
        public void Set_TableState(TableState state)
        {
            tableState = state;

            for (int i = 0; i < playerBetSize.Length; i++)
                playerBetSize[i] = 0;
            DoSync();
        }
        public void Set_PlayerState(int idx, PlayerState state)
        {
            playerState[idx] = state;

            if (state == PlayerState.Turn)
            {
                obj_turnArrow.transform.position = table_Players[table_TurnIndex].Get_DealerButtonPosition.position;
                obj_turnArrow.transform.rotation = table_Players[table_TurnIndex].Get_DealerButtonPosition.rotation;
            }

            DoSync();
        }
        public void Set_PlayerStateWait()
        {
            for (int i = 0; i < playerState.Length; i++)
                if (Get_PlayerInGame(i) && playerState[i] != PlayerState.ALLIN)
                    playerState[i] = PlayerState.Wait;
        }
        public void Reset_PlayerState(int index)
        {
            if (!Networking.IsOwner(gameObject))
                return;

            Set_PlayerState(index, PlayerState.OutOfGame);
        }
        #endregion
        #region Chip Setting
        public void Set_TableCallSize(int value)
        {
            tableCallSize = value;
            DoSync();
        }
        public int Get_TableCallSize() => tableCallSize;
        public int Get_TableRaiseSize() => tableCallSize == 0 ? table_BB : tableCallSize;
        public int Get_TableBB() => table_BB;
        public int Get_TableSB() => table_BB / 2;
        public int Get_TablePot(int index) => tableSidePot[index];
        public void Set_PlayerBetSize(int idx, int value) => playerBetSize[idx] = value;
        #endregion
        #region Sound Effect
        public void Play_AudioClip_Fold_Basic() => audioSource.PlayOneShot(mainSystem.Get_AudioClip_Table_Basic(SE_Table_Index.Fold));
        public void Play_AudioClip_DrawCard_Basic() => audioSource.PlayOneShot(mainSystem.Get_AudioClip_Table_Basic(SE_Table_Index.DrawCard));
        public void Play_AudioClip_Turn_Basic() => audioSource.PlayOneShot(mainSystem.Get_AudioClip_Table_Basic(SE_Table_Index.Turn));
        public void Play_AudioClip_Call_Basic() => audioSource.PlayOneShot(mainSystem.Get_AudioClip_Table_Basic(SE_Table_Index.Call));
        public void Play_AudioClip_Check_Basic() => audioSource.PlayOneShot(mainSystem.Get_AudioClip_Table_Basic(SE_Table_Index.Check));
        public void Play_AudioClip_Raise_Basic() => audioSource.PlayOneShot(mainSystem.Get_AudioClip_Table_Basic(SE_Table_Index.Raise));
        public void Play_AudioClip_Fold_Type1() => audioSource.PlayOneShot(mainSystem.Get_AudioClip_Table_Type1(SE_Table_Index.Fold));
        public void Play_AudioClip_DrawCard_Type1() => audioSource.PlayOneShot(mainSystem.Get_AudioClip_Table_Type1(SE_Table_Index.DrawCard));
        public void Play_AudioClip_Turn_Type1() => audioSource.PlayOneShot(mainSystem.Get_AudioClip_Table_Type1(SE_Table_Index.Turn));
        public void Play_AudioClip_Call_Type1() => audioSource.PlayOneShot(mainSystem.Get_AudioClip_Table_Type1(SE_Table_Index.Call));
        public void Play_AudioClip_Check_Type1() => audioSource.PlayOneShot(mainSystem.Get_AudioClip_Table_Type1(SE_Table_Index.Check));
        public void Play_AudioClip_Raise_Type1() => audioSource.PlayOneShot(mainSystem.Get_AudioClip_Table_Type1(SE_Table_Index.Raise));
        public void Play_AudioClip(SE_Table_Index index)
        {
            string type = "Basic";
            switch(table_Players[table_TurnIndex].Get_VoiceType())
            {
                case SE_Table_Type.Basic:
                    type = "Basic";
                    break;
                case SE_Table_Type.Type1:
                    type = "Type1";
                    break;
            }

            switch(index)
            {
                case SE_Table_Index.Fold:
                    SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Play_AudioClip_Fold_" + type);
                    return;
                case SE_Table_Index.DrawCard:
                    SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Play_AudioClip_DrawCard_" + type);
                    return;
                case SE_Table_Index.Turn:
                    SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Play_AudioClip_Turn_" + type);
                    return;
                case SE_Table_Index.Call:
                    SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Play_AudioClip_Call_" + type);
                    return;
                case SE_Table_Index.Check:
                    SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Play_AudioClip_Check_" + type);
                    return;
                case SE_Table_Index.Raise:
                    SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Play_AudioClip_Raise_" + type);
                    return;
            }
        }
        #endregion
        #region Action
        public void Set_BetAction(int index, int value)
        {
            if (value == -1)
            {
                int playerCount = 0;
                Set_PlayerState(index, PlayerState.Fold);
                Play_AudioClip(SE_Table_Index.Fold);

                for (int i = 0; i < playerState.Length; i++)
                    if (Get_PlayerInGame(i))
                        playerCount++;

                if (playerCount <= 1)
                {
                    Set_TableState(TableState.Open);
                    Set_GameAuto();
                    return;
                }

                Set_NextTurn();
                return;
            }
            else
            {
                if (table_Players[index].Get_TablePlayerChip() == 0)
                {
                    Set_PlayerState(index, PlayerState.ALLIN);
                    Set_PlayerStateWait();
                    Play_AudioClip(SE_Table_Index.Call);
                }
                else if (value == 0)
                {
                    Set_PlayerState(index, PlayerState.Check);
                    Play_AudioClip(SE_Table_Index.Check);
                }
                else if (value != tableCallSize)
                {
                    Play_AudioClip(SE_Table_Index.Raise);
                    Set_PlayerStateWait();
                    Set_PlayerState(index, PlayerState.Raise);
                }
                else
                {
                    Set_PlayerState(index, PlayerState.Call);
                    Play_AudioClip(SE_Table_Index.Call);
                }

                tableTotalPot += value - playerBetSize[index];

                if (tableCallSize < value) tableCallSize = value;
                Set_PlayerBetSize(index, value);
                Set_NextTurn();
            }
        }

        public void Set_ExitCheck()
        {
            if (!Networking.IsOwner(gameObject))
                return;
            SendCustomEventDelayedSeconds("Set_ExitPlayer", 2.0f);
        }

        public void Set_ExitPlayer()
        {
            for (int i = 0; i < playerState.Length; i++)
            {
                if (Get_TablePlayerData[i].Get_DisplayName() != "")
                    continue;
                else
                {
                    if (table_TurnIndex == i)
                    {
                        Set_TurnIndex(table_TurnIndex, true);
                        break;
                    }
                }
                playerState[i] = PlayerState.OutOfGame;
                handRank[i] = 0;
            }
            DoSync();
        }
        public void Set_NextTurn()
        {
            bool isNextStep = true;
            bool isAllPlayerAllin = true;

            for (int i = 0; i < playerState.Length; i++)
            {
                if (playerState[i] == PlayerState.Wait)
                {
                    isNextStep = false;
                    break;
                }
            }
            for (int i = 0; i < playerState.Length; i++)
            {
                if (Get_PlayerInGame(i) && playerState[i] != PlayerState.ALLIN)
                {
                    isAllPlayerAllin = false;
                    break;
                }
            }
            if (isAllPlayerAllin)
            {
                while (tableState != TableState.Wait)
                    Set_GameAuto();
                return;
            }
            if (isNextStep)
            {
                tableCallSize = 0;
                Set_PlayerStateWait();
                Set_GameAuto();
            }

            if (tableState != TableState.Wait)
                Set_TurnIndex(table_TurnIndex + 1);
            DoSync();
        }
        #endregion
        #region Game
        public void Set_GameReset()
        {
            for (int i = 0; i < table_Players.Length; i++)
                if (table_Players[i].isPlaying() && table_Players[i].Get_TablePlayerChip() < 200)
                    table_Players[i].SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Exit_Table");

            tableState = TableState.Wait;
            Set_GameAuto();
            DoSync();
        }
        public void Set_GameStart()
        {
            if (tableState != TableState.Wait)
                return;

            if (Get_TablePlayerCount() < 2)
                return;
            ResetGame();
            tableState = TableState.Hand;
            Set_GameAuto();
            DoSync();
        }
        public void Set_GameAuto()
        {
            switch (tableState)
            {
                case TableState.Wait:
                    ResetGame();
                    break;
                case TableState.Hand:
                    for (int i = 0; i < Get_TablePlayerData.Length; i++)
                    {
                        playerChip[i] = Get_TablePlayerData[i].Get_TablePlayerChip();

                        if (!Get_PlayerInGame(i))
                            continue;
                        table_Card.Set_CardPosition(i, Get_TablePlayerData[i].Get_CardPosition[0], true);
                        Set_TableCard(i, table_Card.Get_CardIndex(i));
                        table_Card.Set_CardPosition(i + 9, Get_TablePlayerData[i].Get_CardPosition[1], true);
                        Set_TableCard(i + 9, table_Card.Get_CardIndex(i + 9));
                        table_System_UI.Set_PlayerName(Get_TablePlayerData[i].Get_DisplayName(), i);
                    }

                    obj_dealerBtn.transform.position = table_Players[Get_TurnSetting(table_DealerIndex)].Get_DealerButtonPosition.position;

                    Set_HandRank();
                    Set_TableState(TableState.Flop);
                    Play_AudioClip(SE_Table_Index.DrawCard);
                    int turn = Get_TurnSetting(table_TurnIndex + 1);
                    table_Players[turn].SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Set_PaySB");
                    playerBetSize[turn] = Get_TableSB();
                    turn = Get_TurnSetting(table_TurnIndex + 1);
                    table_Players[turn].SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Set_PayBB");
                    playerBetSize[turn] = Get_TableBB();
                    tableTotalPot += Get_TableBB() + Get_TableSB();
                    Set_TurnIndex(turn + 1);
                    break;
                case TableState.Flop:
                    for (int i = 0; i < 3; i++)
                    {
                        table_Card.Set_CardPosition(i + 18, (CardPosition)(i + 1), false);
                        Set_TableCard(i + 18, table_Card.Get_CardIndex(i + 18));
                    }
                    Set_HandRank();
                    Set_TableState(TableState.Turn);
                    Play_AudioClip(SE_Table_Index.DrawCard);
                    break;
                case TableState.Turn:
                    table_Card.Set_CardPosition(21, CardPosition.Turn, false);
                    Set_TableCard(21, table_Card.Get_CardIndex(21));
                    Set_HandRank();
                    Set_TableState(TableState.River);
                    Play_AudioClip(SE_Table_Index.DrawCard);
                    break;
                case TableState.River:
                    table_Card.Set_CardPosition(22, CardPosition.River, false);
                    Set_TableCard(22, table_Card.Get_CardIndex(22));
                    Set_HandRank();
                    Set_TableState(TableState.Open);
                    Play_AudioClip(SE_Table_Index.DrawCard);
                    break;
                case TableState.Open:
                    if (Get_TablePlayerWaitCount() > 1)
                    {
                        for (int i = 0; i < 18; i++)
                        {
                            if (Get_PlayerInGame(i % 9))
                            {
                                table_Card.Set_Blind(i, false);
                                table_Card.Set_CardRotation(i);
                            }
                        }
                        Set_HandRank();
                    }
                    Set_TableState(TableState.Wait);
                    obj_turnArrow.transform.position = transform.position + Vector3.down;
                    Set_GameEnd();
                    break; 
            }
            DoSync();
        }

        private void ResetGame()
        {
            table_BB = 200;
            tableCallSize = table_BB;
            _tableTotalPot = 0;
            for (int i = 0; i < table_Cards.Length; i++)
                table_Cards[i] = 52;
            table_Card.Reset_Card();
            Reset_HandRank();

            for (int i = 0; i < Get_TablePlayerData.Length; i++)
            {
                if (Get_TablePlayerData[i].isPlaying()) playerState[i] = PlayerState.Wait;
                else playerState[i] = PlayerState.OutOfGame;
                playerBetSize[i] = 0;
                tableSidePot[i] = 0;

            }

            int dealerTurn = Get_TurnIndex(table_DealerIndex + 1);
            for (int i = 0; i < Get_TablePlayerData.Length; i++)
            {
                if (playerState[Get_TurnIndex(dealerTurn + i)] != PlayerState.Wait)
                    continue;

                table_DealerIndex = Get_TurnIndex(dealerTurn + i);
                break;
            }
        }

        public bool Get_PlayerInGame(int index) => !(playerState[index] == PlayerState.Fold || playerState[index] == PlayerState.OutOfGame);
        public int Get_TurnIndex(int value) => value % 9;
        public void Set_TurnIndex(int value, bool isExitPlayer = false)
        {
            for (int i = 0; i < table_Players.Length; i++)
            {
                if (playerState[Get_TurnIndex(i + value)] != PlayerState.Wait)
                    continue;

                table_TurnIndex = Get_TurnIndex(i + value);
                Set_PlayerState(table_TurnIndex, PlayerState.Turn);
                table_Players[table_TurnIndex].SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Set_Turn");
                return;
            }

            if (!isExitPlayer)
                return;
            Set_GameAuto();
        }
        public int Get_TurnSetting(int value)
        {
            for (int i = 0; i < playerState.Length; i++)
            {
                if (playerState[Get_TurnIndex(value + i)] != PlayerState.Wait)
                    continue;
                table_TurnIndex = Get_TurnIndex(value + i);
                return table_TurnIndex;
            }
            return table_TurnIndex;
        }
        #endregion
        #region HandRank
        HandRanking[] p_handRank = new HandRanking[9];
        HandNumber[] p_handNumber = new HandNumber[9];
        HandSuit[] p_handSuit = new HandSuit[9];
        int[] hands = new int[9];
        int[] kiker = new int[9];
        int highHand = 0;

        public void Set_HandRank()
        {
            for (int i = 0; i < table_Players.Length; i++)
            {
                if (!Get_PlayerInGame(i))
                {
                    handRank[i] = 0;
                    continue;
                }
                p_handRank[i] = Calculate_HandRank(i);
                handRank[i] = (int)p_handRank[i] * 1000 + (int)p_handNumber[i] * 10 + 3 - (int)p_handSuit[i];
            }
        }
        public void Reset_HandRank()
        {
            for (int i = 0; i < 9; i++)
            {
                p_handRank[i] = HandRanking.HighCard;
                p_handNumber[i] = HandNumber.Two;
                p_handSuit[i] = HandSuit.Clover;
            }
        }

        private HandRanking Calculate_HandRank(int playerID)
        {
            int index = 0, tableSuit, tableNumber;
            kiker[playerID] = 0;

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
                    if (i == 0) p_handNumber[playerID] = HandNumber.Ace;
                    else p_handNumber[playerID] = (HandNumber)i;
                    pair[i] = 0;
                    KikerCheck(1, pair, playerID);
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
                {
                    for (int i = pair.Length - 1; i > 0; i--)
                    {
                        if (pair[i] >= 2)
                        {
                            if (p_handNumber[playerID] != (HandNumber)i)
                            {
                                kiker[playerID] = i;
                                break;
                            }
                        }
                    }
                    return true;
                }
            }
            return false;
        }
        private bool isFlush(bool[] hand, int[] suit, int playerID)
        {
            for (int i = 0; i < suit.Length; i++)
                if (suit[i] >= 5)
                {
                    kiker[playerID] = 0;
                    p_handSuit[playerID] = (HandSuit)i;
                    for (int j = 12; j > 0; j--)
                        if (hand[i * 13 + j] == true)
                        {
                            p_handNumber[playerID] = (HandNumber)j;
                            break;
                        }

                    int count = 5;

                    if (hand[i * 13] == true)
                    {
                        p_handNumber[playerID] = HandNumber.Ace;
                        kiker[playerID] += (int)HandNumber.Ace;
                        count -= 1;
                    }
                    for (int j = 12; j > 0; j--)
                    {
                        if (count <= 0)
                            break;
                        if (hand[i * 13 + j] == true)
                        {
                            kiker[playerID] += j;
                            count -= 1;
                        }
                    }
                    return true;

                }
            return false;
        }
        private bool isStraight(bool[] hand, int[] pair, int playerID)
        {
            if (pair[0] != 0 && pair[9] != 0 && pair[10] != 0 && pair[11] != 0 && pair[12] != 0)
            {
                if (hand[0] == true) p_handSuit[playerID] = HandSuit.Spade;
                else if (hand[13] == true) p_handSuit[playerID] = HandSuit.Diamond;
                else if (hand[26] == true) p_handSuit[playerID] = HandSuit.Heart;
                else if (hand[39] == true) p_handSuit[playerID] = HandSuit.Clover;
                p_handNumber[playerID] = HandNumber.Ace;
                return true;
            }
            for (int i = 8; i >= 0; i--)
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
            if (pair[0] == 3)
            {
                if (hand[00] == true) p_handSuit[playerID] = HandSuit.Spade;
                else if (hand[13] == true) p_handSuit[playerID] = HandSuit.Diamond;
                else if (hand[26] == true) p_handSuit[playerID] = HandSuit.Heart;
                else if (hand[39] == true) p_handSuit[playerID] = HandSuit.Clover;
                p_handNumber[playerID] = HandNumber.Ace;
                pair[0] = 0;
                KikerCheck(2, pair, playerID);
                return true;
            }

            for (int i = pair.Length - 1; i > 1; i--)
                if (pair[i] == 3)
                {
                    if (hand[i + 00] == true) p_handSuit[playerID] = HandSuit.Spade;
                    else if (hand[i + 13] == true) p_handSuit[playerID] = HandSuit.Diamond;
                    else if (hand[i + 26] == true) p_handSuit[playerID] = HandSuit.Heart;
                    else if (hand[i + 39] == true) p_handSuit[playerID] = HandSuit.Clover;
                    p_handNumber[playerID] = (HandNumber)i;
                    pair[i] = 0;
                    KikerCheck(2, pair, playerID);
                    return true;
                }
            return false;
        }
        private bool isTwoPair(bool[] hand, int[] pair, int playerID)
        {
            int count = 0;

            if (pair[0] == 2)
            {
                if (hand[0 + 00] == true) p_handSuit[playerID] = HandSuit.Spade;
                else if (hand[0 + 13] == true) p_handSuit[playerID] = HandSuit.Diamond;
                else if (hand[0 + 26] == true) p_handSuit[playerID] = HandSuit.Heart;
                else if (hand[0 + 39] == true) p_handSuit[playerID] = HandSuit.Clover;
                p_handNumber[playerID] = HandNumber.Ace;
                pair[0] = 0;
                count++;
            }
            for (int i = pair.Length - 1; i > 1; i--)
                if (pair[i] == 2)
                {
                    if (count == 0)
                    {
                        if (hand[i + 00] == true) p_handSuit[playerID] = HandSuit.Spade;
                        else if (hand[i + 13] == true) p_handSuit[playerID] = HandSuit.Diamond;
                        else if (hand[i + 26] == true) p_handSuit[playerID] = HandSuit.Heart;
                        else if (hand[i + 39] == true) p_handSuit[playerID] = HandSuit.Clover;
                        p_handNumber[playerID] = (HandNumber)i;
                    }
                    if (count < 2)
                        pair[i] = 0;
                    count++;
                }
            if (count >= 2)
            {
                KikerCheck(1, pair, playerID);
                return true;
            }
            return false;
        }
        private bool isOnePair(bool[] hand, int[] pair, int playerID)
        {
            if (pair[0] == 2)
            {
                if (hand[00] == true) p_handSuit[playerID] = HandSuit.Spade;
                else if (hand[13] == true) p_handSuit[playerID] = HandSuit.Diamond;
                else if (hand[26] == true) p_handSuit[playerID] = HandSuit.Heart;
                else if (hand[39] == true) p_handSuit[playerID] = HandSuit.Clover;
                p_handNumber[playerID] = HandNumber.Ace;
                pair[0] = 0;
                KikerCheck(3, pair, playerID);
                return true;
            }
            for (int i = pair.Length - 1; i > 0; i--)
                if (pair[i] == 2)
                {
                    if (hand[i + 00] == true) p_handSuit[playerID] = HandSuit.Spade;
                    else if (hand[i + 13] == true) p_handSuit[playerID] = HandSuit.Diamond;
                    else if (hand[i + 26] == true) p_handSuit[playerID] = HandSuit.Heart;
                    else if (hand[i + 39] == true) p_handSuit[playerID] = HandSuit.Clover;
                    p_handNumber[playerID] = (HandNumber)i;
                    pair[i] = 0;
                    KikerCheck(3, pair, playerID);
                    return true;
                }
            return false;
        }

        private void KikerCheck(int count, int[] pair, int playerID)
        {
            int rcount = count;

            for (int i = 0; i < pair[0]; i++)
            {
                if (rcount <= 0)
                    return;
                kiker[playerID] += i;
            }
            for (int i = pair.Length - 1; i > 1; i--)
            {
                if (rcount <= 0)
                    return;
                for (int j = 0; j < pair[i]; j++)
                {
                    if (rcount <= 0)
                        return;
                    kiker[playerID] += i;
                }
            }
        }
        #endregion
        #region GameSet
        public string Get_CardName(int value)
        {
            if (value > 52 || value < 0)
                return "";

            int suit, number;

            number = value % 13 == 0 ? 13 : value % 13;
            suit = value / 13;

            return $"{mainSystem.s_HandSuit(suit)}{mainSystem.s_HandNumber(number)}";
        }
        public void Set_GameEnd()
        {
            DoSync();
            for (int i = 0; i < table_Players.Length; i++)
            {
                if (!Get_PlayerInGame(i))
                    hands[i] = 0;
                else
                    hands[i] = (int)p_handRank[i] * 10000 + (int)p_handNumber[i] * 100 + kiker[i];
            }
            KikerCheck();
        }

        void KikerCheck()
        {
            if (tableTotalPot <= 0)
            {
                for (int i = 0; i < hands.Length; i++)
                    if (hands[i] != -1)
                        tableSidePot[i] = 0;
                DoSync();
                SendCustomEventDelayedSeconds("Add_EndGamePot", 1.0f);
                return;
            }
            else
            {
                int index = 0, count = 0;
                bool isKikerCheck = false;
                highHand = 0;

                for (int i = 0; i < table_Players.Length; i++)
                {
                    if (tableSidePot[i] <= 0 || hands[i] < 0)
                        continue;

                    count++;

                    if (highHand < hands[i])
                    {
                        highHand = hands[i];
                        index = i;
                        isKikerCheck = false;
                    }
                    else if (highHand == hands[i])
                        isKikerCheck = true;
                }

                if (count == 0)
                {
                    _tableTotalPot = 0;
                    KikerCheck();
                }
                else if (!isKikerCheck)
                    Set_SidePot(index);
                else
                {
                    Add_EndGamePot_Chap();
                }
            }

        }
        void Set_SidePot(int index)
        {
            int sidePotCalculate = tableSidePot[index];
            hands[index] = -1;

            for (int i = 0; i < table_Players.Length; i++)
            {
                if (hands[i] == -1) continue;
                if (highHand != hands[i])
                {
                    tableSidePot[i] = sidePotCalculate > tableSidePot[i] ? 0 : tableSidePot[i] - sidePotCalculate;
                    continue;
                }
            }
            _tableTotalPot -= sidePotCalculate;
            KikerCheck();
        }

        public void Add_EndGamePot()
        {
            for (int i = 0; i < table_Players.Length; i++)
                if (table_Players[i].isPlaying() && playerState[i] != PlayerState.OutOfGame)
                    table_Players[i].SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Add_EndGamePot");
        }
        public void Add_EndGamePot_Chap()
        {
            int chapCount = 0;
            int minSidePot = int.MaxValue;
            int maxSidePot = int.MinValue;

            for (int i = 0; i < table_Players.Length; i++)
            {
                if (highHand != hands[i]) continue;
                if (hands[i] == -1) continue;

                if (minSidePot > tableSidePot[i]) minSidePot = tableSidePot[i];
                if (maxSidePot < tableSidePot[i]) maxSidePot = tableSidePot[i];

                chapCount++;
            }

            for (int i = 0; i < table_Players.Length; i++)
            {
                if (hands[i] == -1) continue;
                if (highHand != hands[i])
                {
                    tableSidePot[i] -= maxSidePot;
                    continue;
                }

                tableSidePot[i] = tableSidePot[i] - maxSidePot + minSidePot / chapCount;
                hands[i] = -1;
            }
            _tableTotalPot -= maxSidePot;
            table_System_UI.Set_KikerCheck(false);
            KikerCheck();
        }
        #endregion
        #region GiveChip
        public void Add_Chip_P1() => Add_Chip_Player(0);
        public void Add_Chip_P2() => Add_Chip_Player(1);
        public void Add_Chip_P3() => Add_Chip_Player(2);
        public void Add_Chip_P4() => Add_Chip_Player(3);
        public void Add_Chip_P5() => Add_Chip_Player(4);
        public void Add_Chip_P6() => Add_Chip_Player(5);
        public void Add_Chip_P7() => Add_Chip_Player(6);
        public void Add_Chip_P8() => Add_Chip_Player(7);
        public void Add_Chip_P9() => Add_Chip_Player(8);

        public void Add_Chip_Player(int index)
        {
            if (!table_Players[index].isPlaying())
                return;
            table_Players[index].SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Add_Chip");
        }
        #endregion

        /// Get & Set Functions
        public Table_Player[] Get_TablePlayerData => table_Players;
        public int Get_TablePlayerCount()
        {
            int count = 0;
            for (int i = 0; i < table_Players.Length; i++)
                if (table_Players[i].isPlaying())
                    count++;
            return count;
        }
        public int Get_TablePlayerWaitCount()
        {
            int count = 0;
            for (int i = 0; i < table_Players.Length; i++)
                if (Get_PlayerInGame(i))
                    count++;
            return count;
        }
        public void Set_TableCard(int index, int value) => table_Cards[index] = value;
        public int[] Get_TableCard => table_Cards;
        public Table_System_UI Get_TableSystemUI() => table_System_UI;

        ///  Network Fuctions
        #region Sync
        public void Start()
        {
            for (int i = 0; i < playerState.Length; i++)
                playerState[i] = PlayerState.OutOfGame;

            for (int i = 0; i < playerBetSize.Length; i++)
                playerBetSize[i] = 0;

            for (int i = 0; i < table_Cards.Length; i++)
                table_Cards[i] = 0;

            for (int i = 0; i < handRank.Length; i++)
                handRank[i] = 0;
        }
        public void DoSync()
        {
            Update_Syncs();
            RequestSerialization();
        }
        public override void OnDeserialization()
        {
            Update_Syncs();
        }
        public void Update_Syncs()
        {
            Update_PlayerState();
            Update_HandRank();
            Update_PlayerUI();
            Update_CardPattern();
            Update_PlayerReset();
            table_System_UI.Set_TableState(9 - Get_TablePlayerCount(), tableState != TableState.Wait);
            table_System_UI.Set_TablePot(tableTotalPot);
            table_System_UI.Set_Table_PlayerPot(tableSidePot);
        }
        public void Update_PlayerState()
        {
            for (int i = 0; i < table_Players.Length; i++)
                table_Players[i].Get_table_Player_UI().Set_StateText(playerState[i], Get_TableState());
        }
        public void Update_HandRank()
        {
            for (int i = 0; i < table_Players.Length; i++)
            {
                if (!Networking.IsOwner(table_Players[i].gameObject))
                    continue;
                table_Players[i].Get_table_Player_UI().Set_HandRankText(handRank[i]);
            }
        }
        public void Update_PlayerUI()
        {
            for (int i = 0; i < table_Players.Length; i++)
            {
                if (!Networking.IsOwner(table_Players[i].gameObject))
                    continue;
                table_Players[i].Get_table_Player_UI().Set_CardImage(table_Cards, i);
            }
        }
        public void Update_CardPattern()
        {
            table_Card.Set_CardPattern();
        }
        public void Update_PlayerReset()
        {
            if (tableState != TableState.Wait)
                return;

            for (int i = 0; i < table_Players.Length; i++)
            {
                if (!Networking.IsOwner(table_Players[i].gameObject))
                    continue;

                table_Players[i].Action_Reset();
            }
        }
        #endregion

        #region Networking
        public void Set_Owner(VRCPlayerApi value)
        {
            if (value.IsOwner(gameObject)) return;
            Networking.SetOwner(value, gameObject);
            Networking.SetOwner(value, obj_dealerBtn);
            Networking.SetOwner(value, obj_turnArrow);
        }
        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            if (!Networking.IsOwner(gameObject)) return;
            DoSync();
        }
        public override void OnPlayerLeft(VRCPlayerApi player)
        {
            if (!Networking.IsOwner(gameObject)) return;

            Set_ExitCheck();
            DoSync();
        }
        #endregion
    }
}
