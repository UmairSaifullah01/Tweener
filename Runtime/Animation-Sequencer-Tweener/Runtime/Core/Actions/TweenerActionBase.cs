using System;
using THEBADDEST.Tweening2;
using THEBADDEST.Tweening2.Core;
using UnityEngine;

namespace THEBADDEST.Tweening2
{
    [Serializable]
    public abstract class TweenerActionBase
    {
        public enum AnimationDirection
        {
            To, 
            From
        }
        
        [SerializeField]
        protected AnimationDirection direction;
        public AnimationDirection Direction
        {
            get => direction;
            set => direction = value;
        }

        [SerializeField]
        protected EaseType ease = EaseType.InOutCirc;
        public EaseType Ease
        {
            get => ease;
            set => ease = value;
        }

        [SerializeField]
        protected AnimationCurve customEaseCurve;
        public AnimationCurve CustomEaseCurve
        {
            get => customEaseCurve;
            set => customEaseCurve = value;
        }

        [SerializeField]
        protected bool isRelative;
        public bool IsRelative
        {
            get => isRelative;
            set => isRelative = value;
        }

        public virtual Type TargetComponentType { get; }
        public abstract string DisplayName { get; }

        protected abstract Tween GenerateTween_Internal(GameObject target, float duration);

        public Tween GenerateTween(GameObject target, float duration)
        {
            Tween tween = GenerateTween_Internal(target, duration);
            
            if (tween == null) return null;

            // Apply ease
            if (customEaseCurve != null && customEaseCurve.keys.Length > 0)
            {
                tween.easeType = EaseType.INTERNAL_Custom;
                tween.customEase = (time, dur, overshoot, period) =>
                {
                    return customEaseCurve.Evaluate(time / dur);
                };
            }
            else
            {
                tween.easeType = ease;
            }

            // Apply relative
            if (isRelative && tween is Tweener tweener)
            {
                tweener.isRelative = true;
            }

            // Apply From direction - this needs to be handled by the specific action
            // as it requires knowing the start value
            if (direction == AnimationDirection.From)
            {
                // Note: From functionality will need to be implemented in each action
                // as it requires setting the start value explicitly
            }

            return tween;
        }

        public abstract void ResetToInitialState();
    }
}
