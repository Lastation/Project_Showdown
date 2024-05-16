
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

[UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
public class Player_Teleport : UdonSharpBehaviour
{
    [SerializeField] Transform teleportPos;

    public override void Interact()
    {
        Networking.LocalPlayer.TeleportTo(teleportPos.position, teleportPos.rotation);
    }
}