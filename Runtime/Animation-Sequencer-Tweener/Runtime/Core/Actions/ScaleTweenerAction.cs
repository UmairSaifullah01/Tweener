using System;
using THEBADDEST.Tweening2;
using THEBADDEST.Tweening2.Core;
using THEBADDEST.Tweening2.Plugins.Options;
using UnityEngine;

namespace THEBADDEST.Tweening2
{
    [Serializable]
    public sealed class ScaleTweenerAction : TweenerActionBase
    {
        public override Type TargetComponentType => typeof(Transform);
        public override string DisplayName => "Scale to Size";

        [SerializeField]
        private Vector3 scale;
        public Vector3 Scale
        {
            get => scale;
            set => scale = value;
        }

        private Vector3? previousState;
        private GameObject previousTarget;

        protected override Tween GenerateTween_Internal(GameObject target, float duration)
        {
            if (target == null || target.transform == null)
                return null;

            previousState = target.transform.localScale;
            previousTarget = target;
            
            TweenerCore<Vector3, Vector3, VectorOptions> scaleTween = TweenCore.To(
                () => target.transform.localScale,
                x => target.transform.localScale = x,
                scale,
                duration
            );

            if (scaleTween != null)
            {
                scaleTween.target = target.transform;
            }

            return scaleTween;
        }

        public override void ResetToInitialState()
        {
            if (!previousState.HasValue || previousTarget == null || previousTarget.transform == null)
                return;

            previousTarget.transform.localScale = previousState.Value;
        }
    }
}
