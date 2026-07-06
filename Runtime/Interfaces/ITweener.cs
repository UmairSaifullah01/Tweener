using System.Collections;
using UnityEngine;
using THEBADDEST.Tweening2.Core;

namespace THEBADDEST.Tweening2
{
    public delegate void LerpDelegate(float intercept);
    public delegate void CallbackDelegate();

    public interface ITweener
    {
        /// <summary>Underlying Tween for use in sequences and engine.</summary>
        Tween Tween { get; }

        event CallbackDelegate OnCompleteAllLoops;
        event CallbackDelegate OnCompleteIteration;
        
        ITweener SetEase(EaseType ease);
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
        float Duration { get; }
    }
}
