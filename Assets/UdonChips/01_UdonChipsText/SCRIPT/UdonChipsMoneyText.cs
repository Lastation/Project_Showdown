
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VRC.SDKBase;
using VRC.Udon;

namespace UCS
{
	public class UdonChipsMoneyText : UdonSharpBehaviour
	{
		[SerializeField]
		private TextMeshProUGUI textMeshPro = null;

		[SerializeField]
		private Text text = null;

		[SerializeField]
		private string format = string.Empty;

		private UdonChips udonChips = null;

		[SerializeField]
		private bool dramRoll = false;

		[SerializeField]
		private float dramRollMinPerSec = 10;

		[SerializeField]
		private float dramRollFactorPerSec = 1f;

		float lastMoney = 0f;
		bool firstTake = true;

		private void Start()
		{
			udonChips = GameObject.Find("UdonChips").GetComponent<UdonChips>();

			if (textMeshPro == null)
			{
				textMeshPro = GetComponent<TextMeshProUGUI>();
			}

			if(text == null)
			{
				text = GetComponent<Text>();
			}
		}

		private void OnEnable()
		{
			firstTake = true;
		}

		private void Update()
		{
			UpdateText();
		}

		private void UpdateText()
		{
			if(firstTake)
			{
				firstTake = false;
				lastMoney = udonChips.chips;
				ApplyText();
			}
		
			// 
			if(lastMoney != udonChips.chips)
			{
				if(dramRoll)
				{
					float delta = lastMoney - udonChips.chips;
				
					float maxDelta = Mathf.Max( dramRollMinPerSec, Mathf.Abs( delta * dramRollFactorPerSec ) ) * Time.deltaTime;
					lastMoney = Mathf.MoveTowards( lastMoney, udonChips.chips, maxDelta );
				}
				else
				{
					lastMoney = udonChips.chips;
				}
				ApplyText();
			}
		}

		private void ApplyText()
		{
			if(string.IsNullOrEmpty( format ))
			{
				format = udonChips.format;
			}

			if(text != null)
			{
				text.text = string.Format( format, lastMoney );
			}
			if(textMeshPro != null)
			{
				textMeshPro.text = string.Format( format, lastMoney );
			}
		}
	}
}
