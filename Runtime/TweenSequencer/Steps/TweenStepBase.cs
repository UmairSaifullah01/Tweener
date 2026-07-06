using System;
using THEBADDEST.Tweening.Core;
using UnityEngine;

namespace THEBADDEST.Tweening
{
    [Serializable]
    public abstract class TweenStepBase
    {
        [SerializeField]
        private float delay;
        public float Delay => delay;

        [SerializeField]
        private FlowType flowType;
        public FlowType FlowType => flowType;

        public abstract string DisplayName { get; }
        
        public abstract void AddTweenToSequence(Sequence animationSequence);

        public abstract void ResetToInitialState();

        public virtual string GetDisplayNameForEditor(int index)
        {
            return $"{index}. {this}";
        }
    }
}
