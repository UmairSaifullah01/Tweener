using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;


namespace THEBADDEST.Tweening
{


	[Serializable]
	public struct TweenTransform
	{

		public Tween     tween;
		public Transform targetTransform;
		public float     delay;

	}

	public class TweenSequencePlayer : MonoBehaviour
	{

		[SerializeField] bool autoPlay = false;

		[SerializeField] TweenTransform[] tweens;
		[SerializeField] UnityEvent       onComplete;

		Dictionary<int, List<TweenTransform>> tweenHierarchy;
		List<ITweener>                        tweeners = new List<ITweener>();

		void OnEnable()
		{
			CreateHierarchy();
			if (autoPlay)
				Play();
		}

		void OnDisable()
		{
			for (int i = 0; i < tweeners.Count; i++)
			{
				TweenerSolver.StopTweener(tweeners[i]);
			}
		}

		void CreateHierarchy()
		{
			tweens         = tweens.OrderBy(x => x.tween.priority).ToArray();
			tweenHierarchy = new Dictionary<int, List<TweenTransform>>();
			int priority = 0;
			foreach (var tt in tweens)
			{
				if (!priority.Equals(tt.tween.priority))
					priority = tt.tween.priority;
				AddPriority(priority, tt);
			}
		}

		void AddPriority(int priority, TweenTransform tweenTransform)
		{
			if (!tweenHierarchy.ContainsKey(priority))
			{
				tweenHierarchy.Add(priority, new List<TweenTransform>());
			}

			tweenHierarchy[priority].Add(tweenTransform);
		}


		public void Play()
		{
			StopAllCoroutines();
			StartCoroutine(PlayCoroutine());
		}

		IEnumerator PlayCoroutine()
		{
			List<TweenTransform> sequence = new List<TweenTransform>();
			
			foreach (var pair in tweenHierarchy)
			{
				for (int i = 0; i < pair.Value.Count; i++)
				{
					var tweenTransform = pair.Value[i];
					if (i == 0)
					{
						sequence.Add(tweenTransform);
					}
					else
					{
						if (tweenTransform.delay != 0)
							tweenTransform.tween.delay = tweenTransform.delay;
						StartCoroutine(tweenTransform.tween.Play(tweenTransform.targetTransform));
						tweeners.Add(tweenTransform.tween.tweener);
					}
				}
			}

			foreach (var tweenTransform in sequence)
			{
				if (tweenTransform.delay != 0)
					tweenTransform.tween.delay = tweenTransform.delay;
				IEnumerator playCoroutine = tweenTransform.tween.Play(tweenTransform.targetTransform);
				tweeners.Add(tweenTransform.tween.tweener);
				yield return playCoroutine;
			}

			onComplete?.Invoke();

			//yield return PlayCoroutine();
		}

	}


}