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
		}

		public void Reverse()
		{
			if (lerpAction == null)
				throw new System.InvalidOperationException("Cannot reverse a tweener that hasn't been initialized with a lerp action");

			isPlaying = false;
			TweenerSolver.StopTweener(this);
			Lerp(t => lerpAction.Invoke(1 - t), duration);
		}

		public void Kill()
		{
			isPlaying = false;
			TweenerSolver.StopTweener(this);
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
	}
}