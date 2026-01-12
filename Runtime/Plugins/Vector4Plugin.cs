using System;
using THEBADDEST.Tweening2.Core;
using THEBADDEST.Tweening2.Core.Easing;
using THEBADDEST.Tweening2.Plugins.Core;
using THEBADDEST.Tweening2.Plugins.Options;
using UnityEngine;

namespace THEBADDEST.Tweening2.Plugins
{
    public class Vector4Plugin : ABSTweenPlugin<Vector4, Vector4, VectorOptions>
    {
        public override void Reset(TweenerCore<Vector4, Vector4, VectorOptions> t) { }

        public override void SetFrom(TweenerCore<Vector4, Vector4, VectorOptions> t, bool isRelative)
        {
            Vector4 prevEndVal = t.endValue;
            t.endValue = t.getter();
            t.startValue = isRelative ? t.endValue + prevEndVal : prevEndVal;
            t.setter(!t.plugOptions.snapping ? t.startValue : new Vector4((float)Math.Round(t.startValue.x), (float)Math.Round(t.startValue.y), (float)Math.Round(t.startValue.z), (float)Math.Round(t.startValue.w)));
        }
        
        public override void SetFrom(TweenerCore<Vector4, Vector4, VectorOptions> t, Vector4 fromValue, bool setImmediately, bool isRelative)
        {
            if (isRelative)
            {
                Vector4 currVal = t.getter();
                t.endValue += currVal;
                fromValue += currVal;
            }
            t.startValue = fromValue;
            if (setImmediately) t.setter(!t.plugOptions.snapping ? fromValue : new Vector4((float)Math.Round(fromValue.x), (float)Math.Round(fromValue.y), (float)Math.Round(fromValue.z), (float)Math.Round(fromValue.w)));
        }

        public override Vector4 ConvertToStartValue(TweenerCore<Vector4, Vector4, VectorOptions> t, Vector4 value)
        {
            return value;
        }

        public override void SetRelativeEndValue(TweenerCore<Vector4, Vector4, VectorOptions> t)
        {
            t.endValue += t.startValue;
        }

        public override void SetChangeValue(TweenerCore<Vector4, Vector4, VectorOptions> t)
        {
            t.changeValue = t.endValue - t.startValue;
        }

        public override float GetSpeedBasedDuration(VectorOptions options, float unitsXSecond, Vector4 changeValue)
        {
            return changeValue.magnitude / unitsXSecond;
        }

        public override void EvaluateAndApply(
            VectorOptions options, Tween t, bool isRelative, Getter<Vector4> getter, Setter<Vector4> setter,
            float elapsed, Vector4 startValue, Vector4 changeValue, float duration, bool usingInversePosition, int newCompletedSteps,
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
            startValue.x += changeValue.x * easeVal;
            startValue.y += changeValue.y * easeVal;
            startValue.z += changeValue.z * easeVal;
            startValue.w += changeValue.w * easeVal;
            if (options.snapping)
            {
                startValue.x = (float)Math.Round(startValue.x);
                startValue.y = (float)Math.Round(startValue.y);
                startValue.z = (float)Math.Round(startValue.z);
                startValue.w = (float)Math.Round(startValue.w);
            }
            setter(startValue);
        }
    }
}
