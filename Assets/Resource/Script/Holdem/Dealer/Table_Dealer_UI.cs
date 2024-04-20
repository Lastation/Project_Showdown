
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Holdem
{
    public class Table_Dealer_UI : UdonSharpBehaviour
    {
        [SerializeField] TextMeshProUGUI text_DisplayName;
        [SerializeField] GameObject obj_TableDealerUI;
        [SerializeField] GameObject obj_TableJoin, obj_TableExit;

        public void Update_UI(string displayName)
        {
            text_DisplayName.text = displayName == "" ? "Join" : displayName;
        }

        public void Set_TableDealerUI(bool value)
        {
            obj_TableJoin.SetActive(!value);
            obj_TableExit.SetActive(value);
            obj_TableDealerUI.SetActive(value);
        }

        public void Set_Owner(VRCPlayerApi value)
        {
            if (value.IsOwner(gameObject)) return;
            Networking.SetOwner(value, gameObject);
        }
    }
}