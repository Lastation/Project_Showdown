using UdonSharp;

public enum LocalizationType : int
{
    KOR = 0,
    JP = 1,
    ENG = 2,
    Length,
}

public class Localization : UdonSharpBehaviour
{
    [ReadOnly] public string[] s_MenuHandle_tipText_VR = new string[3]
    {
        "왼손 트리거를 빠르게 두 번 누르면 메뉴가 열립니다",
        "左手トリガーを素早く二回押すとメニューが開きます",
        "Quickly press the left hand trigger twice to open the menu"
    };
    [ReadOnly] public string[] s_MenuHandle_tipText_PC = new string[3]
    {
        "Tab키를 누르면서 메뉴를 조작해주세요",
        "Tabキーを押しながらメニューを操作してください",
        "Press the Tab key to manipulate the menu"
    };
    [ReadOnly] public string[] s_Hour = new string[3]
    {
        "시간",
        "時間",
        "H"
    };
    [ReadOnly] public string[] s_Minute = new string[3]
    {
        "분",
        "分",
        "M"
    };
    [ReadOnly] public string[] s_Before = new string[3]
    {
        "전",
        "前",
        "before"
    };

    [ReadOnly] public string[] s_DayOfWeek_KOR = new string[7]
    {
       "일요일", "월요일", "화요일", "수요일", "목요일", "금요일", "토요일"
    }; 
    [ReadOnly] public string[] s_DayOfWeek_JP = new string[7]
    {
       "日曜日", "月曜日", "火曜日", "水曜日", "木曜日", "金曜日", "土曜日"
    }; 
    [ReadOnly] public string[] s_DayOfWeek_ENG = new string[7]
    {
       "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"
    };

    [ReadOnly] public string[] s_JoinLog = new string[3]
    {
        "입장로그",
        "Joinログ",
        "Join log"
    };
    [ReadOnly] public string[] s_VideoPlayer = new string[3]
    {
        "배경음악",
        "背景音楽",
        "background music"
    };
    [ReadOnly] public string[] s_Switch = new string[3]
    {
        "스위치",
        "スイッチ",
        "Switch"
    };

    [ReadOnly] public string[] s_Language = new string[3]
    {
        "언어설정",
        "言語設定",
        "Language"
    };
    [ReadOnly] public string[] s_Menu = new string[3]
    {
        "메뉴",
        "メニュー",
        "Menu"
    };
    [ReadOnly] public string[] s_Back = new string[3]
    {
        "돌아가기",
        "戻る",
        "Back"
    };
    [ReadOnly] public string[] s_Card = new string[3]
    {
        "카드설정",
        "カード設定",
        "Card settings"
    };
}