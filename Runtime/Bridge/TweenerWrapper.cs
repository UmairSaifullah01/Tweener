using System.Collections;
using UnityEngine;
using THEBADDEST.Tweening2.Core;
using THEBADDEST.Tweening2.Plugins.Options;
using CoreTween = THEBADDEST.Tweening2.Core.Tween;

namespace THEBADDEST.Tweening2.Bridge
{
    /// <summary>
    /// Wraps TweenerCore to provide ITweener interface compatibility
    /// </summary>
    internal class TweenerWrapper : ITweener
    {
        private CoreTween _core;
        private LerpDelegate _lerpDelegate;

        public event CallbackDelegate OnCompleteAllLoops;
        public event CallbackDelegate OnCompleteIteration;

        public float Duration => _core?.duration ?? 0f;

        internal TweenerWrapper(CoreTween core)
        {
            _core = core;
            if (_core != null)
            {
                _core.onComplete += () => OnCompleteAllLoops?.Invoke();
                _core.onStepComplete += () => OnCompleteIteration?.Invoke();
            }
        }

        public ITweener SetEase(EaseType ease)
        {
            if (_core != null)
            {
                _core.easeType = ease;
            }
            return this;
        }

        public ITweener SetEase(AnimationCurve easeCurve)
        {
            if (_core != null)
            {
                _core.easeType = EaseType.INTERNAL_Custom;
                _core.customEase = (time, duration, overshootOrAmplitude, period) => easeCurve.Evaluate(time / duration);
            }
            return this;
        }

        public ITweener SetLoops(int loops, LoopType loopType)
        {
            if (_core != null)
            {
                _core.loops = loops;
                _core.loopType = loopType;
            }
            return this;
        }

        public ITweener SetDelay(float seconds)
        {
            if (_core != null)
            {
                _core.delay = seconds;
            }
            return this;
        }

        public ITweener SetTime(bool independent = false)
        {
            if (_core != null)
            {
                _core.isIndependentUpdate = independent;
                TweenManager.SetUpdateType(_core, _core.updateType, independent);
            }
            return this;
        }

        public void Lerp(LerpDelegate lerp, float duration)
        {
            _lerpDelegate = lerp;
            if (lerp != null)
            {
                // Create a float tween that calls the lerp delegate
                TweenerCore<float, float, FloatOptions> floatCore = TweenManager.GetTweener<float, float, FloatOptions>();
                float startValue = 0f;
                float endValue = 1f;
                
                floatCore.getter = () => startValue;
                floatCore.setter = (value) =>
                {
                    _lerpDelegate?.Invoke(value);
                };
                floatCore.endValue = endValue;
                floatCore.duration = duration;
                
                // Setup and start
                if (!Tweener.Setup(floatCore, floatCore.getter, floatCore.setter, endValue, duration))
                {
                    TweenLog.LogError("Failed to setup lerp tween");
                    TweenManager.Despawn(floatCore);
                    return;
                }
                
                // Replace core
                if (_core != null) TweenManager.Despawn(_core);
                _core = floatCore;
                _core.onComplete += () => OnCompleteAllLoops?.Invoke();
                _core.onStepComplete += () => OnCompleteIteration?.Invoke();
                
                TweenManager.AddActiveTween(_core);
            }
        }

        public void Reverse()
        {
            if (_core != null)
            {
                _core.isBackwards = !_core.isBackwards;
            }
        }

        public void Kill()
        {
            if (_core != null)
            {
                TweenManager.MarkForKilling(_core);
            }
            OnCompleteAllLoops?.Invoke();
        }

        public void Pause()
        {
            if (_core != null)
            {
                TweenManager.Pause(_core);
            }
        }

        public void Resume()
        {
            if (_core != null)
            {
                TweenManager.Play(_core);
            }
        }

        public void Reset()
        {
            if (_core != null)
            {
                _core.Reset();
            }
            OnCompleteAllLoops = null;
            OnCompleteIteration = null;
            _lerpDelegate = null;
        }

        public IEnumerator WaitForCompletion()
        {
            if (_core != null)
            {
                yield return new WaitUntil(() => !_core.isPlaying || !_core.active);
            }
        }

        public ITweener OnComplete(CallbackDelegate onComplete, bool singleIteration = false)
        {
            if (singleIteration)
            {
                OnCompleteIteration += onComplete;
            }
            else
            {
                OnCompleteAllLoops += onComplete;
            }
            return this;
        }
    }
}
