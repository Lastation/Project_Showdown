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
    DrawCard = 0,
    Call = 1,
    Check = 2,
    Raise = 3,
    Fold = 4
}

public class MainSystem : UdonSharpBehaviour
{
    #region Localization
    [SerializeField]
    Localization localization;

    [SerializeField]
    Text[] text_handMenu_Button;

    private LocalizationType localizationType = LocalizationType.KOR;

    public void Set_Language_KOR() => Update_Language(LocalizationType.KOR);
    public void Set_Language_JP() => Update_Language(LocalizationType.JP);
    public void Set_Language_ENG() => Update_Language(LocalizationType.ENG);

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
    #endregion

    [SerializeField]
    Data_Player data_Player;

    public Data_Player Get_Data_Player() => data_Player;

    #region Sound Effect
    [SerializeField]
    AudioClip[] audioClip_Table;
    public AudioClip Get_AudioClip_Table(int index) => audioClip_Table[index];
    #endregion

    #region Color
    [SerializeField]
    Color color_Table_Card_Frame;
    [SerializeField]
    Slider slider_Table_Card_Frame;
    [SerializeField]
    Text text_Table_Card_Frame;
    [SerializeField]
    Image Image_Table_Card_Frame;

    [SerializeField]
    Material mat_Table_Card_Frame;

    public void Set_Color_Card_Frame()
    {
        int value = (int)slider_Table_Card_Frame.value > 16777215 ? 16777215 : (int)slider_Table_Card_Frame.value;
        string hexCode = $"#{value.ToString("X")}";
        text_Table_Card_Frame.text = hexCode;
        color_Table_Card_Frame = ConvertHexToRGB(hexCode);
        Image_Table_Card_Frame.color = color_Table_Card_Frame;
        mat_Table_Card_Frame.SetColor("_Color", color_Table_Card_Frame);
    }

    [SerializeField]
    Color color_Table_Card_Pattern;
    [SerializeField]
    Slider slider_Table_Card_Pattern;
    [SerializeField]
    Text text_Table_Card_Pattern;
    [SerializeField]
    Image Image_Table_Card_Pattern;

    [SerializeField]
    Material mat_Table_Card_Pattern;

    public void Set_Color_Card_Pattern()
    {
        int value = (int)slider_Table_Card_Pattern.value > 16777215 ? 16777215 : (int)slider_Table_Card_Pattern.value;
        string hexCode = $"#{value.ToString("X")}";
        text_Table_Card_Pattern.text = hexCode;
        color_Table_Card_Pattern = ConvertHexToRGB(hexCode);
        Image_Table_Card_Pattern.color = color_Table_Card_Pattern;
        mat_Table_Card_Pattern.SetColor("_Color", color_Table_Card_Pattern);
    }

    public Color ConvertHexToRGB(string hexValue)
    {
        int hexColor = Convert.ToInt32(hexValue.Replace("#", ""), 16);
        int red = (hexColor >> 16) & 0xFF;
        int green = (hexColor >> 8) & 0xFF;
        int blue = hexColor & 0xFF;
        return new Color(red / 255.0f, green / 255.0f, blue / 255.0f);
    }
    #endregion
}