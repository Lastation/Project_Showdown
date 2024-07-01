
using Holdem;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class EKE_BroadcastCollider : UdonSharpBehaviour
{
    /**
     * 
     * 제작(MADE)
     * 에케EKE(파란대나무숲BlueBambooForest)
     * https://bbforest.net
     * https://eke.booth.pm
     * 
     */

    [Tooltip("목소리 증폭 크기 : 0-24\nAudio gain size : 0-24\nDefault : 0")]
    public int gain = 20      ;
    [Tooltip("목소리 선명도 : 0-1000\nAudio obvious : 0-1000\nDefault : 1000")]
    public int obvious = 1000;
    [Tooltip("아래 목록에 있는 플레이어에게만 작동합니다.\nOnly works players on the list below.")]
    public bool whitelist = false;
    [Tooltip("화이트리스트 유저들\nWhitelist players")]
    public string[] Players = { "에케EKE", "nickname1" };

    [SerializeField]
    private Table_System table_System;
    [SerializeField]
    private AudioClip[] audioClip_Broadcast;
    [SerializeField]
    private AudioSource audioSource;

    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        if (whitelist)
        {
            foreach (var nick in Players)
            {
                if (player.displayName == nick)
                {
                    Broadcast(player, true);
                }
            }
        }
        else
        {
            if (Networking.IsOwner(player, table_System.gameObject))
            {
                Broadcast(player, true);
                audioSource.clip = audioClip_Broadcast[0];
                audioSource.Play();
            }
        }
    }

    public override void OnPlayerTriggerExit(VRCPlayerApi player)
    {
        if (Networking.IsOwner(player, table_System.gameObject))
        {
            Broadcast(player, false);
            audioSource.clip = audioClip_Broadcast[1];
            audioSource.Play();
        }
    }

    public void Broadcast(VRCPlayerApi player, bool check)
    {
        if (check)
        {
            Debug.Log("전역방송: 활성화: " + player.displayName);
            player.SetVoiceDistanceFar(10000);
            player.SetVoiceVolumetricRadius(obvious);
            player.SetVoiceGain(gain);
        }
        else
        {
            Debug.Log("전역방송: 비활성화: " + player.displayName);
            player.SetVoiceDistanceFar(25);
            player.SetVoiceVolumetricRadius(0);
            player.SetVoiceGain(15);
        }
    }
}
