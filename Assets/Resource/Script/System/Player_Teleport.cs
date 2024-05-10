
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

[UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
public class Player_Teleport : UdonSharpBehaviour
{
    [SerializeField] Transform teleportPos;
    [SerializeField] GameObject Obj_ActiveFalse;
    [SerializeField] GameObject Obj_ActiveTrue;

    public override void Interact()
    {
        Obj_ActiveTrue.SetActive(false);
        Obj_ActiveTrue.SetActive(true);
        Networking.LocalPlayer.TeleportTo(teleportPos.position, teleportPos.rotation);
    }
}