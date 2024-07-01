
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

[UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
public class Player_Teleport : UdonSharpBehaviour
{
    [SerializeField] Transform teleportPos;
    [SerializeField] bool isTrigger = false;

    public override void Interact()
    {
        Networking.LocalPlayer.TeleportTo(teleportPos.position, teleportPos.rotation);
    }

    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        if (!isTrigger) return;

        Networking.LocalPlayer.TeleportTo(teleportPos.position, teleportPos.rotation);
    }
}