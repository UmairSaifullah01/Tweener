using System;
using UnityEngine;

namespace THEBADDEST.Tweening2
{
    [Serializable]
    public abstract class GameObjectTweenStep : TweenStepBase
    {
        [SerializeField]
        protected GameObject target;
        public GameObject Target
        {
            get => target;
            set => target = value;
        }

        [SerializeField]
        protected float duration = 1;
        public float Duration
        {
            get => duration;
            set => duration = value;
        }

        public void SetTarget(GameObject newTarget)
        {
            target = newTarget;
        }
    }
}
