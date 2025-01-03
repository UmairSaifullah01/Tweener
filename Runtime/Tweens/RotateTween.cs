using System.Collections;
using UnityEngine;


namespace THEBADDEST.Tweening
{


	[CreateAssetMenu(menuName = "THEBADDEST/Tweening/RotateTween", fileName = "RotateTween", order = 0)]
	public class RotateTween : FromToTween
	{

		public override IEnumerator Play(Transform target)
		{
			tweener.Rotate(target, @from, @to, duration);
			yield return base.Play(target);
		}

	}


}