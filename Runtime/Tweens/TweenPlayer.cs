using System;
using System.Collections;
using UnityEngine;


namespace THEBADDEST.Tweening
{


	public class TweenPlayer : MonoBehaviour
	{

		[SerializeField] bool  autoPlay = false;
		[SerializeField] Tween tween;
		ITweener               tweener;
		void OnEnable()
		{
			if (autoPlay)
			{
				Play();
			}
		}

		void OnDisable()
		{
			TweenerSolver.StopTweener(tweener);
		}

		void Play()
		{
			StopAllCoroutines();
			StartCoroutine(PlayCoroutine());
		}

		private IEnumerator PlayCoroutine()
		{
			var playCoroutine = tween.Play(transform);
			tweener = tween.tweener;
			yield return playCoroutine;
		}
		
	}


}