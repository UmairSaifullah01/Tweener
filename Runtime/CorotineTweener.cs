using System.Collections;
using UnityEngine;


namespace THEBADDEST.Tweening
{


	public class CorotineTweener : Tweener
	{

		WaitUntil iterator=> new WaitUntil(()=>!isPlaying);
		public override void Lerp(LerpDelegate lerp, float duration)
		{
			TweenerSolver.PlayTweener(this, LerpCoroutineInternal(lerp, duration));
		}

		public override IEnumerator GetIterator()
		{
			return iterator;
		}

		protected override void CalculateDeltaTime()
		{
			deltaTime = independentTime ? Time.unscaledDeltaTime : Time.deltaTime;
		}

		private IEnumerator LerpCoroutineInternal(LerpDelegate lerp, float duration)
		{
			isPlaying = true;
			yield return independentTime ? new WaitForSecondsRealtime(delay) : new WaitForSeconds(delay);
			int loopsCount = this.loops;
			while (loopsCount>0 || loops==-1)
			{
				if (loopType == LoopType.Linear)
				{
					yield return LerpCoroutineSingleInternal(lerp, duration);
				}
				else if (loopType == LoopType.Yoyo)
				{
					int cache = loopsCount;
					yield return LerpCoroutineSingleInternal(intercept =>
					{
						intercept = cache % 2 != 0 ? intercept : 1 - intercept;
						lerp?.Invoke(intercept);
					}, duration);
				}
				loopsCount--;
			}
			isPlaying = false;
			onCompleteAllLoopsDelegate?.Invoke();
			
		}

		private IEnumerator LerpCoroutineSingleInternal(LerpDelegate lerp, float duration)
		{
			TweenerEasing.Function easingFunction = GetEaseFunction();
			float                  value          = 0.0f;
			float                  intercept      = 0;
			while (value <= 1f)
			{
				yield return null;
				CalculateDeltaTime();
				value += deltaTime / duration;
				intercept = easingFunction.Invoke(0, 1, value);
				lerp.Invoke(intercept);
			}
			yield return null;
			value = 1f;
			lerp.Invoke(easingFunction.Invoke(0, 1, value));
			onCompleteIterationDelegate?.Invoke();
		}
		
	}


}