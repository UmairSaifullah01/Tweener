﻿using System.Collections;
using UnityEngine;


namespace THEBADDEST.Tweening
{


	[CreateAssetMenu(menuName = "THEBADDEST/Tweening/MoveLocalTween", fileName = "MoveLocalTween", order = 0)]
	public class MoveLocalTween : FromToTween
	{

		public override IEnumerator Play(Transform target)
		{
			PlayWithTarget(target);
			yield return base.Play(target);
		}

		public override void PlayWithTarget(Transform target)
		{
			tweener=target.MoveLocal(from, to, duration);
			base.PlayWithTarget(target);
		}
		
	}


}