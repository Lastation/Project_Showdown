
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
}

public class MainSystem : UdonSharpBehaviour
{
    [SerializeField]
    Localization localization;

    [SerializeField]
    Text[] text_handMenu_Button;

    private LocalizationType localizationType = LocalizationType.KOR;

    public void Set_Language_KOR() => Update_Language(LocalizationType.KOR);
    public void Set_Language_JP() => Update_Language(LocalizationType.JP);
    public void Set_Language_ENG() => Update_Language(LocalizationType.ENG);

    private void Awake() => Update_Language(LocalizationType.KOR);

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
}
