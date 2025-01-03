using System.Collections;
using UnityEngine;


namespace THEBADDEST.Tweening
{


	[CreateAssetMenu(menuName = "THEBADDEST/Tweening/RotateLocalTween", fileName = "RotateLocalTween", order = 0)]
	public class RotateLocalTween : FromToTween
	{

		public override IEnumerator Play(Transform target)
		{
			tweener.RotateLocal(target, @from, @to, duration);
			yield return base.Play(target);
		}

	}


}