using System.Collections;
using UnityEngine;


namespace THEBADDEST.Tweening
{


	public abstract class Tweener : ITweener
	{

		protected CallbackDelegate onCompleteAllLoopsDelegate;

		public event CallbackDelegate OnCompleteAllLoops
		{
			add { onCompleteAllLoopsDelegate    += value; }
			remove { onCompleteAllLoopsDelegate -= value; }
		}
		protected CallbackDelegate onCompleteIterationDelegate;

		public event CallbackDelegate OnCompleteIteration
		{
			add { onCompleteIterationDelegate    += value; }
			remove { onCompleteIterationDelegate -= value; }
		}

		protected int                loops           = 1;
		protected LoopType           loopType        = LoopType.Linear;
		protected TweenerEasing.Ease ease            = TweenerEasing.Ease.Linear;
		protected AnimationCurve     easeCurve       = AnimationCurve.Linear(0, 0, 1, 1);
		protected float              deltaTime       = Time.deltaTime;
		protected bool               independentTime = false;
		protected float              delay           = 0.0f;
		protected bool               isPlaying       = false;

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
			deltaTime = Time.deltaTime;
		}

		public ITweener SetEase(TweenerEasing.Ease ease)
		{
			this.ease = ease;
			return this;
		}

		public ITweener SetEase(AnimationCurve easeCurve)
		{
			this.easeCurve = easeCurve;
			return this;
		}

		public ITweener SetLoops(int loops = 1, LoopType loopType = LoopType.Linear)
		{
			this.loops    = loops;
			this.loopType = loopType;
			return this;
		}

		public Tweener SetDelay(float seconds)
		{
			delay = seconds;
			return this;
		}

		public Tweener SetTime(bool independent = false)
		{
			independentTime = independent;
			return this;
		}

		public abstract void Lerp(LerpDelegate lerp, float duration);

		public abstract IEnumerator WaitForCompletion();

	}


}