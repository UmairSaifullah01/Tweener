using System;
using THEBADDEST.Tweening2.Core;
using THEBADDEST.Tweening2.Core.Easing;
using THEBADDEST.Tweening2.Plugins.Core;
using THEBADDEST.Tweening2.Plugins.Options;

namespace THEBADDEST.Tweening2.Plugins
{
    public class FloatPlugin : ABSTweenPlugin<float, float, FloatOptions>
    {
        public override void Reset(TweenerCore<float, float, FloatOptions> t) { }

        public override void SetFrom(TweenerCore<float, float, FloatOptions> t, bool isRelative)
        {
            float prevEndVal = t.endValue;
            t.endValue = t.getter();
            t.startValue = isRelative ? t.endValue + prevEndVal : prevEndVal;
            t.setter(!t.plugOptions.snapping ? t.startValue : (float)Math.Round(t.startValue));
        }
        
        public override void SetFrom(TweenerCore<float, float, FloatOptions> t, float fromValue, bool setImmediately, bool isRelative)
        {
            if (isRelative)
            {
                float currVal = t.getter();
                t.endValue += currVal;
                fromValue += currVal;
            }
            t.startValue = fromValue;
            if (setImmediately) t.setter(!t.plugOptions.snapping ? fromValue : (float)Math.Round(fromValue));
        }

        public override float ConvertToStartValue(TweenerCore<float, float, FloatOptions> t, float value)
        {
            return value;
        }

        public override void SetRelativeEndValue(TweenerCore<float, float, FloatOptions> t)
        {
            t.endValue += t.startValue;
        }

        public override void SetChangeValue(TweenerCore<float, float, FloatOptions> t)
        {
            t.changeValue = t.endValue - t.startValue;
        }

        public override float GetSpeedBasedDuration(FloatOptions options, float unitsXSecond, float changeValue)
        {
            float res = changeValue / unitsXSecond;
            if (res < 0) res = -res;
            return res;
        }

        public override void EvaluateAndApply(
            FloatOptions options, Tween t, bool isRelative, Getter<float> getter, Setter<float> setter,
            float elapsed, float startValue, float changeValue, float duration, bool usingInversePosition, int newCompletedSteps,
            UpdateNotice updateNotice
        )
        {
            if (t.loopType == LoopType.Incremental) startValue += changeValue * (t.isComplete ? t.completedLoops - 1 : t.completedLoops);
            if (t.isSequenced && t.sequenceParent.loopType == LoopType.Incremental)
            {
                startValue += changeValue * (t.loopType == LoopType.Incremental ? t.loops : 1)
                    * (t.sequenceParent.isComplete ? t.sequenceParent.completedLoops - 1 : t.sequenceParent.completedLoops);
            }

            setter(
                !options.snapping
                ? startValue + changeValue * EaseManager.Evaluate(t.easeType, t.customEase, elapsed, duration, t.easeOvershootOrAmplitude, t.easePeriod)
                : (float)Math.Round(startValue + changeValue * EaseManager.Evaluate(t.easeType, t.customEase, elapsed, duration, t.easeOvershootOrAmplitude, t.easePeriod))
            );
        }
    }
}
