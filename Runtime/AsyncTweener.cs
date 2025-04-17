using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using System.Threading;

namespace THEBADDEST.Tweening
{
	/// <summary>
	/// An asynchronous implementation of the Tweener class.
	/// Uses C# async/await for tweening operations.
	/// </summary>
	internal class AsyncTweener : Tweener
	{
		#region Private Fields
		private Task<ITweener> task;
		private DateTime lastTime;
		private bool isPaused;
		private CancellationTokenSource cancellationTokenSource;
		#endregion

		public AsyncTweener()
		{
			cancellationTokenSource = new CancellationTokenSource();
		}

		public override void Lerp(LerpDelegate lerp, float duration)
		{
			base.Lerp(lerp, duration);
			task = LerpAsync(lerp, duration);
		}

		public override IEnumerator WaitForCompletion()
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

		public override void Pause()
		{
			if (isPlaying && !isPaused)
			{
				isPaused = true;
				cancellationTokenSource.Cancel();
				base.Pause();
			}
		}

		public override void Resume()
		{
			if (!isPlaying && isPaused)
			{
				isPaused = false;
				cancellationTokenSource = new CancellationTokenSource();
				base.Resume();
			}
		}

		private async Task<ITweener> LerpAsync(LerpDelegate lerp, float duration)
		{
			try
			{
				if (delay > 0)
				{
					await Task.Delay(Mathf.RoundToInt(1000 * delay), cancellationTokenSource.Token);
				}

				int loopsCount = loops;
				while (loopsCount > 0 || loops == -1)
				{
					if (loopType == LoopType.Linear)
					{
						await LerpAsyncSingle(lerp, duration);
					}
					else if (loopType == LoopType.Yoyo)
					{
						await LerpAsyncSingle(intercept =>
						{
							intercept = loopsCount % 2 != 0 ? intercept : 1 - intercept;
							lerp?.Invoke(intercept);
						}, duration);
					}
					loopsCount--;
				}
				cancellationTokenSource?.Cancel();
				cancellationTokenSource?.Dispose();
				InvokeOnCompleteAllLoops();
				return this;
			}
			catch (OperationCanceledException)
			{
				// Handle cancellation gracefully
				return this;
			}
		}

		private async Task LerpAsyncSingle(LerpDelegate lerp, float duration)
		{
			TweenerEasing.Function easingFunction = GetEaseFunction();
			float value = 0f;
			float inverseDuration = 1f / duration;

			while (value <= 1f)
			{
				if (isPaused)
				{
					await Task.Yield();
					continue;
				}

				await Task.Yield();
				CalculateDeltaTime();
				value += deltaTime * inverseDuration;
				float intercept = easingFunction.Invoke(0, 1, value);
				lerp.Invoke(intercept);
			}

			InvokeOnCompleteIteration();
		}

		// protected override void OnDispose()
		// {
		// 	cancellationTokenSource?.Cancel();
		// 	cancellationTokenSource?.Dispose();
		// 	base.OnDispose();
		// }
	}
}