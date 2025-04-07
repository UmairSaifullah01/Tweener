using System.Collections;
using UnityEngine;


namespace THEBADDEST.Tweening
{


	[CreateAssetMenu(menuName = "THEBADDEST/Tweening/MoveLocalTween", fileName = "MoveLocalTween", order = 0)]
	public class MoveLocalTween : FromToTween
	{

		public override IEnumerator Play(Transform target)
		{
			tweener=target.MoveLocal(from, to, duration);
			Init();
			yield return base.Play(target);
		}

	}


}