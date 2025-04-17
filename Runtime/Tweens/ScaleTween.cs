﻿using System.Collections;
using UnityEngine;


namespace THEBADDEST.Tweening
{


	[CreateAssetMenu(menuName = "THEBADDEST/Tweening/ScaleTween", fileName = "ScaleTween", order = 0)]
	public class ScaleTween : FromToTween
	{

		public override IEnumerator Play(Transform target)
		{
			PlayWithTarget(target);
			yield return base.Play(target);
		}

		public override void PlayWithTarget(Transform target)
		{
			tweener=target.Scale(from, to, duration);
			base.PlayWithTarget(target);
		}

	}


}