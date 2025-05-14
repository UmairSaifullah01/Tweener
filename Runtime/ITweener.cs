using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;


namespace THEBADDEST.Tweening
{


	public delegate void LerpDelegate(float intercept);

	public delegate void CallbackDelegate();

	public interface ITweener
	{
		public event CallbackDelegate OnCompleteAllLoops;
		public event CallbackDelegate OnCompleteIteration;
		ITweener SetEase(TweenerEasing.Ease ease);

		ITweener SetEase(AnimationCurve easeCurve);

		ITweener SetLoops(int loops, LoopType loopType);

		ITweener SetDelay(float seconds);
		ITweener SetTime(bool independent = false);
		void Lerp(LerpDelegate lerp, float duration);

		void Reverse();
		void Kill();

		void Pause();

		void Resume();

		 void Reset();

		IEnumerator WaitForCompletion();

		ITweener OnComplete(CallbackDelegate onComplete, bool singleIteration = false);
		
		float Duration { get;  }

	}


}