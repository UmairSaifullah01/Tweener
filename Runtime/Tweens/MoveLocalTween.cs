using System.Collections;
using UnityEngine;


namespace THEBADDEST.Tweening
{


	[CreateAssetMenu(menuName = "THEBADDEST/Tweening/MoveLocalTween", fileName = "MoveLocalTween", order = 0)]
	public class MoveLocalTween : FromToTween
	{

		public override IEnumerator Play(Transform target)
		{
			Init();
			tweener.MoveLocal(target, @from, @to, duration);
			yield return base.Play(target);
		}

	}


}