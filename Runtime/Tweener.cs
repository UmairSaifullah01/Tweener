using System.Collections;
using UnityEngine;

namespace THEBADDEST.Tweening
{
	/// <summary>
	/// Base class for all tweeners in the system.
	/// Provides core tweening functionality and common properties.
	/// </summary>
	internal abstract class Tweener : ITweener
	{
		#region Events
		private CallbackDelegate onCompleteAllLoopsDelegate;
		private CallbackDelegate onCompleteIterationDelegate;

		public event CallbackDelegate OnCompleteAllLoops
		{
			add => onCompleteAllLoopsDelegate += value;
			remove => onCompleteAllLoopsDelegate -= value;
		}

		public event CallbackDelegate OnCompleteIteration
		{
			add => onCompleteIterationDelegate += value;
			remove => onCompleteIterationDelegate -= value;
		}
		#endregion

		#region Protected Fields
		protected int loops = 1;
		protected LoopType loopType = LoopType.Linear;
		protected TweenerEasing.Ease ease = TweenerEasing.Ease.Linear;
		protected AnimationCurve easeCurve = AnimationCurve.Linear(0, 0, 1, 1);
		protected float deltaTime = Time.deltaTime;
		protected bool independentTime = false;
		protected float delay = 0.0f;
		protected bool isPlaying = false;
		protected LerpDelegate lerpAction = null;
		protected float duration = 0;
		protected bool isPaused = false;
		protected float elapsedTime = 0f;
		protected float elapsedDelay = 0f;
		protected bool delayComplete = false;
		protected int currentLoop = 0;
		protected bool startupDone = false;
		protected UpdateType updateType = UpdateType.Normal;
		#endregion

		#region Properties
		public bool IsPlaying => isPlaying;
		public float Duration => duration;
		public float Delay => delay;
		public int Loops => loops;
		public LoopType LoopType => loopType;
		public TweenerEasing.Ease Ease => ease;
		public AnimationCurve EaseCurve => easeCurve;
		public bool IsIndependentTime => independentTime;
		#endregion

		protected virtual TweenerEasing.Function GetEaseFunction()
		{
			if (ease == TweenerEasing.Ease.Curve)
			{
				return TweenerEasing.GetEasingFunction(easeCurve);
			}

			return TweenerEasing.GetEasingFunction(ease);
		}

		protected virtual void CalculateDeltaTime()
		{
			deltaTime = independentTime ? Time.unscaledDeltaTime : Time.deltaTime;
		}

		public ITweener SetEase(TweenerEasing.Ease ease)
		{
			this.ease = ease;
			return this;
		}

		public ITweener SetEase(AnimationCurve easeCurve)
		{
			this.ease = TweenerEasing.Ease.Curve;
			this.easeCurve = easeCurve ?? throw new System.ArgumentNullException(nameof(easeCurve));
			return this;
		}

		public ITweener SetLoops(int loops = 1, LoopType loopType = LoopType.Linear)
		{
			if (loops < -1)
				throw new System.ArgumentException("Loops must be -1 (infinite) or greater than 0", nameof(loops));

			this.loops = loops;
			this.loopType = loopType;
			return this;
		}

		public ITweener SetDelay(float seconds)
		{
			if (seconds < 0)
				throw new System.ArgumentException("Delay cannot be negative", nameof(seconds));

			delay = seconds;
			return this;
		}

		public ITweener SetTime(bool independent = false)
		{
			independentTime = independent;
			return this;
		}

		public virtual void Lerp(LerpDelegate lerp, float duration)
		{
			if (duration <= 0)
				throw new System.ArgumentException("Duration must be greater than 0", nameof(duration));

			this.lerpAction = lerp ?? throw new System.ArgumentNullException(nameof(lerp));
			this.duration = duration;
			
			// Reset state
			elapsedTime = 0f;
			elapsedDelay = 0f;
			delayComplete = delay <= 0f;
			currentLoop = 0;
			startupDone = false;
			isPlaying = true;
			isPaused = false;

			// Add to active list
			TweenerSolver.AddActiveTweener(this, updateType);
		}

		public void Reverse()
		{
			if (lerpAction == null)
				throw new System.InvalidOperationException("Cannot reverse a tweener that hasn't been initialized with a lerp action");

			isPlaying = false;
			TweenerSolver.MarkForKilling(this);
			Lerp(t => lerpAction.Invoke(1 - t), duration);
		}

		public void Kill()
		{
			isPlaying = false;
			TweenerSolver.MarkForKilling(this);
			onCompleteAllLoopsDelegate?.Invoke();
		}

		/// <summary>
		/// Resets all fields to their default values.
		/// </summary>
		 public virtual void Reset()
		{
			// Reset events
			onCompleteAllLoopsDelegate = null;
			onCompleteIterationDelegate = null;

			// Reset protected fields
			loops = 1;
			loopType = LoopType.Linear;
			ease = TweenerEasing.Ease.Linear;
			easeCurve = AnimationCurve.Linear(0, 0, 1, 1);
			deltaTime = Time.deltaTime;
			independentTime = false;
			delay = 0.0f;
			isPlaying = false;
			lerpAction = null;
			duration = 0;
			isPaused = false;
			elapsedTime = 0f;
			elapsedDelay = 0f;
			delayComplete = false;
			currentLoop = 0;
			startupDone = false;
			updateType = UpdateType.Normal;
		}

		public virtual void Pause()
		{
			if (isPlaying && !isPaused)
			{
				isPaused = true;
			}
		}

		public virtual void Resume()
		{
			if (isPlaying && isPaused)
			{
				isPaused = false;
			}
		}

		public abstract IEnumerator WaitForCompletion();

		public virtual ITweener OnComplete( CallbackDelegate onComplete, bool singleIteration = false)
		{
			if(singleIteration) onCompleteIterationDelegate += onComplete.Invoke;
			else onCompleteAllLoopsDelegate += onComplete.Invoke;
			return this;
		}
		protected void InvokeOnCompleteIteration()
		{
			onCompleteIterationDelegate?.Invoke();
		}

		protected void InvokeOnCompleteAllLoops()
		{
			onCompleteAllLoopsDelegate?.Invoke();
		}

		/// <summary>
		/// Updates the tweener with the given delta time.
		/// Returns true if the tweener should be killed (completed).
		/// </summary>
		public bool UpdateTween(float deltaTime, float unscaledDeltaTime)
		{
			if (!isPlaying || isPaused) return false;

			float tDeltaTime = independentTime ? unscaledDeltaTime : deltaTime;
			
			// Handle delay
			if (!delayComplete)
			{
				elapsedDelay += tDeltaTime;
				if (elapsedDelay >= delay)
				{
					delayComplete = true;
					tDeltaTime = elapsedDelay - delay;
				}
				else
				{
					return false; // Still in delay
				}
			}

			// Startup
			if (!startupDone)
			{
				startupDone = true;
				elapsedTime = 0f;
				currentLoop = 0;
			}

			// Update elapsed time
			elapsedTime += tDeltaTime;
			
			// Check if we need to loop
			while (elapsedTime >= duration && (loops == -1 || currentLoop < loops))
			{
				elapsedTime -= duration;
				currentLoop++;
				InvokeOnCompleteIteration();
				
				if (loops != -1 && currentLoop >= loops)
				{
					elapsedTime = duration;
					break;
				}
			}

			// Calculate intercept (0 to 1)
			float intercept = duration > 0 ? elapsedTime / duration : 1f;
			intercept = Mathf.Clamp01(intercept);

			// Apply easing
			var easeFunc = GetEaseFunction();
			float easedIntercept = easeFunc.Invoke(0, 1, intercept);

			// Apply loop type
			switch (loopType)
			{
				case LoopType.Yoyo:
					if (currentLoop % 2 == 1)
						easedIntercept = 1f - easedIntercept;
					break;
				case LoopType.Incremental:
					easedIntercept = intercept + currentLoop;
					break;
			}

			// Apply lerp
			lerpAction?.Invoke(easedIntercept);

			// Check if complete
			bool isComplete = (loops != -1 && currentLoop >= loops && elapsedTime >= duration);
			if (isComplete)
			{
				isPlaying = false;
				InvokeOnCompleteAllLoops();
				return true; // Mark for killing
			}

			return false;
		}

		/// <summary>
		/// Sets the position of the tweener (used by sequences).
		/// </summary>
		public void SetPosition(float normalizedPosition)
		{
			float intercept = Mathf.Clamp01(normalizedPosition);
			var easeFunc = GetEaseFunction();
			float easedIntercept = easeFunc.Invoke(0, 1, intercept);
			
			// Apply loop type if needed
			switch (loopType)
			{
				case LoopType.Yoyo:
					if (currentLoop % 2 == 1)
						easedIntercept = 1f - easedIntercept;
					break;
				case LoopType.Incremental:
					easedIntercept = intercept + currentLoop;
					break;
			}
			
			lerpAction?.Invoke(easedIntercept);
		}

		/// <summary>
		/// Sets the update type for this tweener.
		/// </summary>
		public ITweener SetUpdateType(UpdateType updateType)
		{
			this.updateType = updateType;
			return this;
		}
	}
}