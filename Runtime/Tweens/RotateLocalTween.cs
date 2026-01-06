using System.Collections;
using UnityEngine;


namespace THEBADDEST.Tweening
{


	[CreateAssetMenu(menuName = "THEBADDEST/Tweening/RotateLocalTween", fileName = "RotateLocalTween", order = 0)]
	public class RotateLocalTween : FromToTween
	{
		

		public override void PlayWithTarget(Transform target)
		{
			tweener=target.RotateLocal(from, to, duration);
			base.PlayWithTarget(target);
		}

	}


}