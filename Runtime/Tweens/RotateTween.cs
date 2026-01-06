using System.Collections;
using UnityEngine;


namespace THEBADDEST.Tweening
{


	[CreateAssetMenu(menuName = "THEBADDEST/Tweening/RotateTween", fileName = "RotateTween", order = 0)]
	public class RotateTween : FromToTween
	{
		

		public override void PlayWithTarget(Transform target)
		{
			tweener=target.Rotate(from, to, duration);
			base.PlayWithTarget(target);
		}

	}


}