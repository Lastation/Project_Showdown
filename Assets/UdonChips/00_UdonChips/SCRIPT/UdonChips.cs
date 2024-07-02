using UdonSharp;
using UnityEngine;

namespace UCS
{
    public class UdonChips : UdonSharpBehaviour
	{
		public int chips = 0;
		public int coin = 0;
		public string format = "$ {0:F0}";
	}
}