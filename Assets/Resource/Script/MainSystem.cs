using UdonSharp;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
namespace Holdem
{
    public enum handMenuIndex : int
    {
        JoinLog = 0,
        VideoPlayer = 1,
        Switch = 2,
        Menu = 3,
        Language = 4,
        Back = 5,
        Card = 6,
        DirectionalLight = 7,
        AvatarLight = 8,
    }
    public enum SE_Table_Index : int
    {
        Fold = 0,
        DrawCard = 1,
        Turn = 2,
        Call = 3,
        Check = 4,
        Raise = 5,
        Win = 6
    }

    public enum SE_Table_Type : int
    {
        Basic = 0,
        Type1 = 1,
    }

    [UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
    public class MainSystem : UdonSharpBehaviour
    {
        #region Localization
        [SerializeField]
        Localization localization;

        [SerializeField]
        Text[] text_handMenu_Button;

        private LocalizationType localizationType = LocalizationType.KOR;
        int localizationIndex;

        public void Set_Language_KOR()
        {
            localizationIndex = (int)LocalizationType.KOR;
            Update_Language((LocalizationType)localizationIndex);
        }
        public void Set_Language_JP()
        {
            localizationIndex = (int)LocalizationType.JP;
            Update_Language((LocalizationType)localizationIndex);
        }
        public void Set_Language_ENG()
        {
            localizationIndex = (int)LocalizationType.ENG;
            Update_Language((LocalizationType)localizationIndex);
        }

        public void Set_Language()
        {
            localizationIndex = localizationIndex + 1 >= (int)LocalizationType.Length ? 0 : localizationIndex + 1;
            Update_Language((LocalizationType)localizationIndex);
        }
        public LocalizationType Get_Language => localizationType;
        public Toggle[] languageToggle;

        private void Start()
        {
            switch (VRCPlayerApi.GetCurrentLanguage())
            {
                case "en":
                    Update_Language(LocalizationType.ENG);
                    languageToggle[0].isOn = true;
                    break;
                case "ja":
                case "ja-JP":
                    Update_Language(LocalizationType.JP);
                    languageToggle[1].isOn = true;
                    break;
                case "ko":
                case "ko-KR":
                    Update_Language(LocalizationType.KOR);
                    languageToggle[2].isOn = true;
                    break;
                default:
                    Update_Language(LocalizationType.ENG);
                    languageToggle[0].isOn = true;
                    break;
            }
        }

        public void Update_Language(LocalizationType type)
        {
            localizationType = type;

            text_handMenu_Button[(int)handMenuIndex.JoinLog].text = localization.s_JoinLog[(int)localizationType];
            text_handMenu_Button[(int)handMenuIndex.VideoPlayer].text = localization.s_VideoPlayer[(int)localizationType];
            text_handMenu_Button[(int)handMenuIndex.Switch].text = localization.s_Switch[(int)localizationType];
            text_handMenu_Button[(int)handMenuIndex.Menu].text = localization.s_Menu[(int)localizationType];
            text_handMenu_Button[(int)handMenuIndex.Language].text = localization.s_Language[(int)localizationType];
            text_handMenu_Button[(int)handMenuIndex.Back].text = localization.s_Back[(int)localizationType];
            text_handMenu_Button[(int)handMenuIndex.Card].text = localization.s_Card[(int)localizationType];
            text_handMenu_Button[(int)handMenuIndex.DirectionalLight].text = localization.s_DirectionalLight[(int)localizationType];
            text_handMenu_Button[(int)handMenuIndex.AvatarLight].text = localization.s_AvatarLight[(int)localizationType];
        }

        public string s_MenuHandle_tipText_VR => localization.s_MenuHandle_tipText_VR[(int)localizationType];
        public string s_MenuHandle_tipText_PC => localization.s_MenuHandle_tipText_PC[(int)localizationType];
        public string s_Hour => localization.s_Hour[(int)localizationType];
        public string s_Minute => localization.s_Minute[(int)localizationType];
        public string s_Before => localization.s_Before[(int)localizationType];
        public string s_DayOfWeek(int idx)
        {
            switch (localizationType)
            {
                case LocalizationType.KOR:
                    return localization.s_DayOfWeek_KOR[idx];
                case LocalizationType.JP:
                    return localization.s_DayOfWeek_JP[idx];
                case LocalizationType.ENG:
                    return localization.s_DayOfWeek_ENG[idx];
                default:
                    return null;
            }
        }
        public string s_DirectionalLight => localization.s_DirectionalLight[(int)localizationType];
        public string s_AvatarLight => localization.s_AvatarLight[(int)localizationType];
        public string s_TableState_progress => localization.s_TableState_progress[(int)localizationType];
        public string s_TableState_wait => localization.s_TableState_wait[(int)localizationType];

        public string s_HandSuit(int idx)
        {
            switch (idx)
            {
                case 0:
                    return "♠";
                case 1:
                    return "♦";
                case 2:
                    return "♥";
                case 3:
                    return "♣";
            };
            return "";
        }
        public string s_HandNumber(int idx)
        {
            switch (idx)
            {
                case 0:
                    return "NULL";
                case 1:
                    return "2";
                case 2:
                    return "3";
                case 3:
                    return "4";
                case 4:
                    return "5";
                case 5:
                    return "6";
                case 6:
                    return "7";
                case 7:
                    return "8";
                case 8:
                    return "9";
                case 9:
                    return "10";
                case 10:
                    return "J";
                case 11:
                    return "Q";
                case 12:
                    return "K";
                case 13:
                    return "A";
            };
            return "";
        }
        public string s_HankRank(int idx)
        {
            switch (localizationType)
            {
                case LocalizationType.KOR:
                    return localization.s_HankRank_KOR[idx];
                case LocalizationType.JP:
                    return localization.s_HankRank_JP[idx];
                case LocalizationType.ENG:
                    return localization.s_HankRank_ENG[idx];
                default:
                    return null;
            }
        }
        public string Get_HandRank(int value)
        {
            if (value == 0)
                return "";

            int rank = value / 1000;
            int number = (value % 1000) / 10;
            int suit = value % 10;

            return $"{s_HandSuit(3 - suit)}{s_HandNumber(number)} {s_HankRank(rank)}";
        }
        #endregion

        [SerializeField]
        Data_Player data_Player;

        public Data_Player Get_Data_Player() => data_Player;

        #region Sound Effect
        [SerializeField]
        AudioClip[] audioClip_Table;
        [SerializeField]
        AudioClip[] audioClip_Table_Type1;
        [SerializeField]
        AudioSource audioSource_Test;

        SE_Table_Type voiceType = SE_Table_Type.Basic;
        public SE_Table_Type Get_VoiceType() => voiceType;
        public void Set_VoiceType_Basic() => Set_VoiceType(SE_Table_Type.Basic);
        public void Set_VoiceType_Type1() => Set_VoiceType(SE_Table_Type.Type1);

        public void Set_VoiceType(SE_Table_Type type)
        {
            voiceType = type;
            switch(type)
            {
                case SE_Table_Type.Basic:
                    audioSource_Test.clip = Get_AudioClip_Table_Basic(SE_Table_Index.Call);
                    break;
                case SE_Table_Type.Type1:
                    audioSource_Test.clip = Get_AudioClip_Table_Type1(SE_Table_Index.Call);
                    break;
            }
            audioSource_Test.Play();
        }
        public AudioClip Get_AudioClip_Table_Basic(SE_Table_Index index) => audioClip_Table[(int)index];
        public AudioClip Get_AudioClip_Table_Type1(SE_Table_Index index)
        {
            if (audioClip_Table_Type1[(int)index] != null)
                return audioClip_Table_Type1[(int)index];
            return audioClip_Table[(int)index];
        }
        #endregion

        #region Card Sprite
        [SerializeField] Sprite[] img_cardPattern_Basic;
        [SerializeField] Sprite[] img_cardPattern_Simple;

        public Sprite[] Get_CardPattern()
        {
            switch (Get_Data_Player().Get_cardPatternType())
            {
                case CardPatternType.Basic:
                    return img_cardPattern_Basic;
                case CardPatternType.Simple:
                    return img_cardPattern_Simple;
                default:
                    return img_cardPattern_Basic;
            }
        }
        public void Set_CardPattern_Basic() => data_Player.Set_cardPatternType(CardPatternType.Basic);
        public void Set_CardPattern_Simple() => data_Player.Set_cardPatternType(CardPatternType.Simple);

        [SerializeField] Color[] color_card;
        [SerializeField] Material[] mat_cardPattern;
        [SerializeField] Slider slider_cardEmission;
        private int cardColorPatternIndex = 0;

        public void Set_Color_Pattern_White() => Set_Color(0);
        public void Set_Color_Pattern_Blue() => Set_Color(1);
        public void Set_Color_Pattern_Red() => Set_Color(2);
        public void Set_Color_Pattern_Orange() => Set_Color(3);
        public void Set_Color_Pattern_Green() => Set_Color(4);

        public void Set_Color(int _cardColorPatternIndex)
        {
            cardColorPatternIndex = _cardColorPatternIndex;
            Set_Color();
        }

        public void Set_Color()
        {
            mat_cardPattern[0].SetColor("_EmissionColor", color_card[cardColorPatternIndex] * slider_cardEmission.value);
            mat_cardPattern[1].SetColor("_Color", color_card[cardColorPatternIndex] * slider_cardEmission.value);
        }

        #endregion

        #region PlayerConfig
        /// Only use Player config local variables
        public Table_Player_UI table_Player_UI { get; set; }
        [SerializeField] Scrollbar scrollbar_display_height;
        [SerializeField] Scrollbar scrollbar_collider_height;
        [SerializeField] GameObject obj_collider_height;
        public void Set_Display_Height()
        {
            if (table_Player_UI == null) return;
            table_Player_UI.Set_TablePlayerUI_Height(true);
        }
        public float Get_Display_Height() => scrollbar_display_height.value - 0.5f + obj_collider_height.transform.localPosition.y;
        public void Set_Collider_Height()
        {
            obj_collider_height.transform.localPosition = new Vector3( 0, scrollbar_collider_height.value, 0);

            if (table_Player_UI == null) return;
            table_Player_UI.Set_TablePlayerUI_Height(true);
        }
        #endregion

        [SerializeField]
        Material rainShader;

        public void Set_RainShader_OFF() => rainShader.SetFloat("_Stop", 0);
        public void Set_RainShader_ON() => rainShader.SetFloat("_Stop", 1);
    }
}