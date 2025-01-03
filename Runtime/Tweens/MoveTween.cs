using System.Collections;
using UnityEngine;


namespace THEBADDEST.Tweening
{


	[CreateAssetMenu(menuName = "THEBADDEST/Tweening/MoveTween", fileName = "MoveTween", order = 0)]
	public class MoveTween : FromToTween
	{

		public override IEnumerator Play(Transform target)
		{
			tweener.Move(target,@from, @to, duration);
			yield return base.Play(target);
		}

	}


}