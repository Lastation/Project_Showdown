using Holdem;
using System;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;

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

public class MainSystem : UdonSharpBehaviour
{
    #region Localization
    [SerializeField]
    Localization localization;

    [SerializeField]
    Text[] text_handMenu_Button;

    private LocalizationType localizationType = LocalizationType.KOR;
    int localizationIndex;

    public void Set_Language()
    {
        localizationIndex = localizationIndex + 1 >= (int)LocalizationType.Length ? 0 : localizationIndex + 1;
        Update_Language((LocalizationType)localizationIndex);
    }

    private void Start() => Update_Language(LocalizationType.KOR);

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
    #endregion

    [SerializeField]
    Data_Player data_Player;

    public Data_Player Get_Data_Player() => data_Player;

    #region Sound Effect
    [SerializeField]
    AudioClip[] audioClip_Table;
    public AudioClip Get_AudioClip_Table(SE_Table_Index index) => audioClip_Table[(int)index];
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
    #endregion
}