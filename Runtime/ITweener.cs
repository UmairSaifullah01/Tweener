﻿using System;
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
		ITweener                SetEase(TweenerEasing.Ease ease);

		ITweener SetEase(AnimationCurve easeCurve);

		ITweener SetLoops(int loops, LoopType loopType);

		Tweener  SetDelay(float seconds);
		Tweener  SetTime(bool independent=false);
		void Lerp(LerpDelegate      lerp, float duration);

		void         Reverse();
		 IEnumerator WaitForCompletion();

	}


}