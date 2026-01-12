using System;
using THEBADDEST.Tweening2.Core;
using THEBADDEST.Tweening2.Core.Easing;
using THEBADDEST.Tweening2.Plugins.Core;
using THEBADDEST.Tweening2.Plugins.Options;
using UnityEngine;

namespace THEBADDEST.Tweening2.Plugins
{
    public class Vector3Plugin : ABSTweenPlugin<Vector3, Vector3, VectorOptions>
    {
        public override void Reset(TweenerCore<Vector3, Vector3, VectorOptions> t) { }

        public override void SetFrom(TweenerCore<Vector3, Vector3, VectorOptions> t, bool isRelative)
        {
            Vector3 prevEndVal = t.endValue;
            t.endValue = t.getter();
            t.startValue = isRelative ? t.endValue + prevEndVal : prevEndVal;
            t.setter(!t.plugOptions.snapping ? t.startValue : new Vector3((float)Math.Round(t.startValue.x), (float)Math.Round(t.startValue.y), (float)Math.Round(t.startValue.z)));
        }
        
        public override void SetFrom(TweenerCore<Vector3, Vector3, VectorOptions> t, Vector3 fromValue, bool setImmediately, bool isRelative)
        {
            if (isRelative)
            {
                Vector3 currVal = t.getter();
                t.endValue += currVal;
                fromValue += currVal;
            }
            t.startValue = fromValue;
            if (setImmediately) t.setter(!t.plugOptions.snapping ? fromValue : new Vector3((float)Math.Round(fromValue.x), (float)Math.Round(fromValue.y), (float)Math.Round(fromValue.z)));
        }

        public override Vector3 ConvertToStartValue(TweenerCore<Vector3, Vector3, VectorOptions> t, Vector3 value)
        {
            return value;
        }

        public override void SetRelativeEndValue(TweenerCore<Vector3, Vector3, VectorOptions> t)
        {
            t.endValue += t.startValue;
        }

        public override void SetChangeValue(TweenerCore<Vector3, Vector3, VectorOptions> t)
        {
            t.changeValue = t.endValue - t.startValue;
        }

        public override float GetSpeedBasedDuration(VectorOptions options, float unitsXSecond, Vector3 changeValue)
        {
            return changeValue.magnitude / unitsXSecond;
        }

        public override void EvaluateAndApply(
            VectorOptions options, Tween t, bool isRelative, DOGetter<Vector3> getter, DOSetter<Vector3> setter,
            float elapsed, Vector3 startValue, Vector3 changeValue, float duration, bool usingInversePosition, int newCompletedSteps,
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
            switch (options.axisConstraint)
            {
                case AxisConstraint.X:
                    Vector3 resX = getter();
                    resX.x = startValue.x + changeValue.x * easeVal;
                    if (options.snapping) resX.x = (float)Math.Round(resX.x);
                    setter(resX);
                    break;
                case AxisConstraint.Y:
                    Vector3 resY = getter();
                    resY.y = startValue.y + changeValue.y * easeVal;
                    if (options.snapping) resY.y = (float)Math.Round(resY.y);
                    setter(resY);
                    break;
                case AxisConstraint.Z:
                    Vector3 resZ = getter();
                    resZ.z = startValue.z + changeValue.z * easeVal;
                    if (options.snapping) resZ.z = (float)Math.Round(resZ.z);
                    setter(resZ);
                    break;
                default:
                    startValue.x += changeValue.x * easeVal;
                    startValue.y += changeValue.y * easeVal;
                    startValue.z += changeValue.z * easeVal;
                    if (options.snapping)
                    {
                        startValue.x = (float)Math.Round(startValue.x);
                        startValue.y = (float)Math.Round(startValue.y);
                        startValue.z = (float)Math.Round(startValue.z);
                    }
                    setter(startValue);
                    break;
            }
        }
    }
}
