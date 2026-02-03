using System;
using THEBADDEST.Tweening2;
using THEBADDEST.Tweening2.Core;
using THEBADDEST.Tweening2.Plugins.Options;
using UnityEngine;

namespace THEBADDEST.Tweening2
{
    [Serializable]
    public sealed class MoveToPositionTweenerAction : TweenerActionBase
    {
        public override Type TargetComponentType => typeof(Transform);
        public override string DisplayName => "Move to Position";

        [SerializeField]
        private Vector3 position;
        public Vector3 Position
        {
            get => position;
            set => position = value;
        }

        [SerializeField]
        private bool localMove;
        public bool LocalMove
        {
            get => localMove;
            set => localMove = value;
        }

        private Vector3 previousPosition;
        private GameObject previousTarget;

        protected override Tween GenerateTween_Internal(GameObject target, float duration)
        {
            if (target == null || target.transform == null)
                return null;

            previousTarget = target;
            TweenerCore<Vector3, Vector3, VectorOptions> moveTween;

            if (localMove)
            {
                previousPosition = target.transform.localPosition;
                moveTween = TweenCore.To(
                    () => target.transform.localPosition,
                    x => target.transform.localPosition = x,
                    position,
                    duration
                );
            }
            else
            {
                previousPosition = target.transform.position;
                moveTween = TweenCore.To(
                    () => target.transform.position,
                    x => target.transform.position = x,
                    position,
                    duration
                );
            }

            if (moveTween != null)
            {
                moveTween.target = target.transform;
            }

            return moveTween;
        }

        public override void ResetToInitialState()
        {
            if (previousTarget == null || previousTarget.transform == null)
                return;
            
            if (localMove)
                previousTarget.transform.localPosition = previousPosition;
            else
                previousTarget.transform.position = previousPosition;
        }
    }
}
