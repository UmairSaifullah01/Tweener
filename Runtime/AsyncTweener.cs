using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;


namespace THEBADDEST.Tweening
{


	public class AsyncTweener : Tweener
	{

		Task<ITweener> task;
		DateTime       lastTime ;
		public override void Lerp(LerpDelegate lerp, float duration)
		{
			task = LerpAsync(lerp, duration);
		}

		public override IEnumerator GetEnumerator()
		{
			yield return task.GetAwaiter();
		}

		protected override void CalculateDeltaTime()
		{
			var currentTime = DateTime.Now;
			var deltaTimeSpan = currentTime - lastTime;
			lastTime = currentTime;
			deltaTime = (float)deltaTimeSpan.TotalMilliseconds / 1000f;
		}

		private async Task<ITweener> LerpAsync(LerpDelegate lerp, float duration)
		{
			await Task.Delay(Mathf.RoundToInt(1000*delay));
			for (int i = 0; i < loops; i++)
			{
				if (loopType == LoopType.Linear)
				{
					await LerpAsyncSingle(lerp, duration);
				}
				else if (loopType == LoopType.Yoyo)
				{
					int cache = i;
					await LerpAsyncSingle(intercept =>
					{
						intercept = cache % 2 != 0 ? intercept : 1 - intercept;
						lerp?.Invoke(intercept);
					}, duration);
				}
			}

			onCompleteAllLoopsDelegate?.Invoke();
			return this;
		}

		private async Task LerpAsyncSingle(LerpDelegate lerp, float duration)
		{
			TweenerEasing.Function easingFunction = GetEaseFunction();
			float                  value          = 0.0f;
			while (value <= 1f)
			{
				await Task.Yield();
				CalculateDeltaTime();
				value += deltaTime / duration;
				float intercept = easingFunction.Invoke(0, 1, value);
				lerp.Invoke(intercept);
			}

			onCompleteIterationDelegate?.Invoke();
		}

	}


}