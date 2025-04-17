using System.Collections;
using UnityEngine;


namespace THEBADDEST.Tweening
{


	/// <summary>
	/// A coroutine-based implementation of the Tweener class.
	/// Uses Unity's coroutine system for smooth tweening operations.
	/// </summary>
	internal class CorotineTweener : Tweener
	{

		#region Private Fields

		private TweenerEasing.Function cachedEasingFunction;
		private WaitForEndOfFrame      waitForEndOfFrame;
		private WaitForSeconds         waitForSeconds;
		private WaitForSecondsRealtime waitForSecondsRealtime;
		private float                  cachedDelay;
		float                          incremententalIntercept = 0f;
		#endregion


		public CorotineTweener()
		{
			waitForEndOfFrame      = new WaitForEndOfFrame();
			waitForSeconds         = new WaitForSeconds(0);
			waitForSecondsRealtime = new WaitForSecondsRealtime(0);
		}

		public override void Lerp(LerpDelegate lerp, float duration)
		{
			base.Lerp(lerp, duration);
			TweenerSolver.PlayTweener(this, LerpCoroutineInternal());
		}

		public override IEnumerator WaitForCompletion()
		{
			yield return new WaitUntil(() => !isPlaying);
		}

		protected override void CalculateDeltaTime()
		{
			deltaTime = independentTime ? Time.unscaledDeltaTime : Time.deltaTime;
		}


		private IEnumerator LerpCoroutineInternal()
		{
			isPlaying = true;
			isPaused  = false;
			yield return waitForEndOfFrame;           // Wait one frame to ensure all settings are properly set
			cachedEasingFunction = GetEaseFunction(); // Cache easing function after frame delay
			yield return SetupDelay();
			int loopsCount = 0;
			
			while (loopsCount < loops || loops == -1)
			{
				int count =  loopsCount;
				switch (loopType)
				{
					case LoopType.Linear:
						yield return LerpCoroutineSingleInternal(lerpAction);
						break;
					case LoopType.Yoyo:
						yield return LerpCoroutineSingleInternal(t => LerpActionYoyo(t, count));
						break;
					case LoopType.Incremental:
						yield return LerpCoroutineSingleInternal(t => LerpActionIncrement(t, count));
						break;
				}
				loopsCount++;
			}

			TweenerSolver.StopTweener(this);
			isPlaying = false;
			isPaused  = false;
			InvokeOnCompleteAllLoops();
		}

		
		void LerpActionIncrement(float intercept, int loopsCount)
		{
			incremententalIntercept = (intercept + loopsCount);
			lerpAction?.Invoke(incremententalIntercept);
		}
		void LerpActionYoyo(float intercept, int loopsCount)
		{
			intercept = Mathf.PingPong((intercept + loopsCount), 1);
			lerpAction?.Invoke(intercept);
		}

		IEnumerator SetupDelay()
		{
			if (delay > 0)
			{
				cachedDelay = delay;
				if (independentTime)
				{
					waitForSecondsRealtime = new WaitForSecondsRealtime(cachedDelay);
					yield return waitForSecondsRealtime;
				}
				else
				{
					waitForSeconds = new WaitForSeconds(cachedDelay);
					yield return waitForSeconds;
				}
			}
		}

		public override void Reset()
		{
			base.Reset();
			incremententalIntercept = 0f;
		}

		private IEnumerator LerpCoroutineSingleInternal(LerpDelegate lerp)
		{
			float value           = 0f;
			float intercept       = 0f;
			float inverseDuration = 1f / duration;
			while (value <= 1f)
			{
				if (isPaused)
				{
					yield return waitForEndOfFrame;
					continue;
				}

				yield return waitForEndOfFrame;
				CalculateDeltaTime();
				value     += deltaTime * inverseDuration;
				intercept =  cachedEasingFunction.Invoke(0, 1, value);
				lerp.Invoke(intercept);
			}

			yield return waitForEndOfFrame;
			lerp.Invoke(cachedEasingFunction.Invoke(0, 1, 1f));
			InvokeOnCompleteIteration();
		}

	}


}