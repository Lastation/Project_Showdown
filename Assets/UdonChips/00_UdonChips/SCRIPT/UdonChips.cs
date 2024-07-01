using UdonSharp;
using UnityEngine;

namespace UCS
{
    public class UdonChips : UdonSharpBehaviour
	{
		[Tooltip("현재소지금(초기소지금)")]
		public int chips = 0;
		public int coin = 0;
		public string format = "$ {0:F0}";
	}
}