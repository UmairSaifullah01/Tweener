using System.Collections;
using UnityEngine;


namespace THEBADDEST.Tweening
{


	[CreateAssetMenu(menuName = "THEBADDEST/Tweening/ScaleTween", fileName = "ScaleTween", order = 0)]
	public class ScaleTween : FromToTween
	{

		public override IEnumerator Play(Transform target)
		{
			tweener = target.Scale(from, to, duration);
			Init();
			yield return base.Play(target);
		}

	}


}