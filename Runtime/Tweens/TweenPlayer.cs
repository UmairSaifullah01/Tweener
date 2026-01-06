using System;
using System.Collections;
using UnityEngine;


namespace THEBADDEST.Tweening
{


	public class TweenPlayer : MonoBehaviour
	{

		[SerializeField] bool autoPlay = false;
		[SerializeField] Tween tween;
		ITweener tweener;

		void OnEnable()
		{
			if (autoPlay)
			{
				Play();
			}
		}

		void OnDisable()
		{
			if (tweener == null) return;
			TweenerSolver.StopTweener(tweener);
		}

		void Play()
		{
			tween.PlayWithTarget(transform);
			tweener = tween.tweener;
		}

	}


}