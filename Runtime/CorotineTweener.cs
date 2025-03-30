using System.Collections;
using UnityEngine;

namespace THEBADDEST.Tweening
{
	/// <summary>
	/// A coroutine-based implementation of the Tweener class.
	/// Uses Unity's coroutine system for smooth tweening operations.
	/// </summary>
	public class CorotineTweener : Tweener
	{
		#region Private Fields
		private TweenerEasing.Function cachedEasingFunction;
		private WaitForEndOfFrame waitForEndOfFrame;
		private WaitForSeconds waitForSeconds;
		private WaitForSecondsRealtime waitForSecondsRealtime;
		private float cachedDelay;
		#endregion

		public CorotineTweener()
		{
			waitForEndOfFrame = new WaitForEndOfFrame();
			waitForSeconds = new WaitForSeconds(0);
			waitForSecondsRealtime = new WaitForSecondsRealtime(0);
		}

		public override void Lerp(LerpDelegate lerp, float duration)
		{
			base.Lerp(lerp, duration);
			TweenerSolver.PlayTweener(this, LerpCoroutineInternal(lerp, duration));
		}

		public override IEnumerator WaitForCompletion()
		{
			yield return new WaitUntil(() => !isPlaying);
		}

		protected override void CalculateDeltaTime()
		{
			deltaTime = independentTime ? Time.unscaledDeltaTime : Time.deltaTime;
		}
		

		private IEnumerator LerpCoroutineInternal(LerpDelegate lerp, float duration)
		{
			isPlaying = true;
			isPaused = false;
			yield return waitForEndOfFrame; // Wait one frame to ensure all settings are properly set

			cachedEasingFunction = GetEaseFunction(); // Cache easing function after frame delay

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

			int loopsCount = loops;
			if (loopType == LoopType.Linear)
			{
				while (loopsCount > 0 || loops == -1)
				{
					yield return LerpCoroutineSingleInternal(lerp, duration);
					loopsCount--;
				}
			}
			else if (loopType == LoopType.Yoyo)
			{
				while (loopsCount > 0 || loops == -1)
				{
					yield return LerpCoroutineSingleInternal(intercept =>
					{
						intercept = loopsCount % 2 != 0 ? intercept : 1 - intercept;
						lerp?.Invoke(intercept);
					}, duration);
					loopsCount--;
				}
			}

			TweenerSolver.StopTweener(this);
			isPlaying = false;
			isPaused = false;
			InvokeOnCompleteAllLoops();
		}

		private IEnumerator LerpCoroutineSingleInternal(LerpDelegate lerp, float duration)
		{
			float value = 0f;
			float intercept = 0f;
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
				value += deltaTime * inverseDuration;
				intercept = cachedEasingFunction.Invoke(0, 1, value);
				lerp.Invoke(intercept);
			}

			yield return waitForEndOfFrame;
			lerp.Invoke(cachedEasingFunction.Invoke(0, 1, 1f));
			InvokeOnCompleteIteration();
		}
	}
}