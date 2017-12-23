using UnityEngine;
using System.Collections;

namespace HWRWeaponSystem
{
	public class EnemyDead : MonoBehaviour
	{

		public int ScorePlus = 1;
		public int MoneyPlus = 20;
	
		void Start ()
		{
		
		}

		void Update ()
		{
	
		}
	
		public void OnDead ()
		{
			BuyMenu buymenu = (BuyMenu)GameObject.FindObjectOfType (typeof(BuyMenu));
			if (buymenu) {
				buymenu.Money += MoneyPlus;
				buymenu.Score += ScorePlus;
			}
		
		}
	}
}