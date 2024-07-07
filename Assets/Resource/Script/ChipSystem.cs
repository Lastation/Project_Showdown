
using Holdem;
using TMPro;
using UCS;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDK3.Components;
using VRC.SDKBase;

public class ChipSystem : UdonSharpBehaviour
{
    [SerializeField]
    private Text tmp_chip = null;
    [SerializeField]
    private Text tmp_coin = null;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private Camera targetCamera;

    [SerializeField]
    private GameObject chipUI;

    int lastMoney = 0;
    int lastCoin = 0;

    int prevMoney = 0;
    int prevCoin = 0;

    public void ApplyText(int chip, int coin)
    {
        int nowChip = chip - prevMoney;
        int nowCoin = coin - prevCoin;

        animator.SetTrigger("ShowUI");
        lastMoney = chip;
        lastCoin = coin;
        SendCustomEventDelayedSeconds("SetChips", 4.0f);

        tmp_chip.text = string.Concat("<b>",prevMoney, "</b> [",(nowChip < 0 ? "<color=#FF7B5A>-" : "<color=#FFE223>+"), Mathf.Abs(nowChip),"</color>]");
        tmp_coin.text = string.Concat("<b>", prevCoin, "</b> [", (nowCoin < 0 ? "<color=#FF7B5A>-" : "<color=#FFE223>+"), Mathf.Abs(nowCoin), "</color>]");
    }

    private void SetChips()
    {
        prevMoney = lastMoney;
        prevCoin = lastCoin;
    }
}