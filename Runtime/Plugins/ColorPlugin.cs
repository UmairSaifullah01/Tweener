using THEBADDEST.Tweening2.Core;
using THEBADDEST.Tweening2.Core.Easing;
using THEBADDEST.Tweening2.Plugins.Core;
using THEBADDEST.Tweening2.Plugins.Options;
using UnityEngine;

namespace THEBADDEST.Tweening2.Plugins
{
    public class ColorPlugin : ABSTweenPlugin<Color, Color, ColorOptions>
    {
        public override void Reset(TweenerCore<Color, Color, ColorOptions> t) { }

        public override void SetFrom(TweenerCore<Color, Color, ColorOptions> t, bool isRelative)
        {
            Color prevEndVal = t.endValue;
            t.endValue = t.getter();
            t.startValue = isRelative ? t.endValue + prevEndVal : prevEndVal;
            t.setter(t.startValue);
        }
        
        public override void SetFrom(TweenerCore<Color, Color, ColorOptions> t, Color fromValue, bool setImmediately, bool isRelative)
        {
            if (isRelative)
            {
                Color currVal = t.getter();
                t.endValue += currVal;
                fromValue += currVal;
            }
            t.startValue = fromValue;
            if (setImmediately) t.setter(fromValue);
        }

        public override Color ConvertToStartValue(TweenerCore<Color, Color, ColorOptions> t, Color value)
        {
            return value;
        }

        public override void SetRelativeEndValue(TweenerCore<Color, Color, ColorOptions> t)
        {
            t.endValue += t.startValue;
        }

        public override void SetChangeValue(TweenerCore<Color, Color, ColorOptions> t)
        {
            t.changeValue = t.endValue - t.startValue;
        }

        public override float GetSpeedBasedDuration(ColorOptions options, float unitsXSecond, Color changeValue)
        {
            return changeValue.a / unitsXSecond;
        }

        public override void EvaluateAndApply(
            ColorOptions options, Tween t, bool isRelative, DOGetter<Color> getter, DOSetter<Color> setter,
            float elapsed, Color startValue, Color changeValue, float duration, bool usingInversePosition, int newCompletedSteps,
            UpdateNotice updateNotice
        )
        {
            if (t.loopType == LoopType.Incremental) startValue += changeValue * (t.isComplete ? t.completedLoops - 1 : t.completedLoops);
            if (t.isSequenced && t.sequenceParent.loopType == LoopType.Incremental)
            {
                startValue += changeValue * (t.loopType == LoopType.Incremental ? t.loops : 1)
                    * (t.sequenceParent.isComplete ? t.sequenceParent.completedLoops - 1 : t.sequenceParent.completedLoops);
            }

            float easeVal = EaseManager.Evaluate(t.easeType, t.customEase, elapsed, duration, t.easeOvershootOrAmplitude, t.easePeriod);
            
            if (options.alphaOnly)
            {
                Color res = getter();
                res.a = startValue.a + changeValue.a * easeVal;
                setter(res);
            }
            else
            {
                startValue.r += changeValue.r * easeVal;
                startValue.g += changeValue.g * easeVal;
                startValue.b += changeValue.b * easeVal;
                startValue.a += changeValue.a * easeVal;
                setter(startValue);
            }
        }
    }
}
