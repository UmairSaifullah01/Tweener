using System;
using THEBADDEST.Tweening2;
using THEBADDEST.Tweening2.Core;
using UnityEngine;

namespace THEBADDEST.Tweening2
{
    [Serializable]
    public sealed class WaitForIntervalStep : TweenStepBase
    {
        public override string DisplayName => "Wait for Interval";

        [SerializeField]
        private float interval;
        public float Interval
        {
            get => interval;
            set => interval = value;
        }

        public override void AddTweenToSequence(Sequence animationSequence)
        {
            Sequence sequence = (Sequence)TweenCore.Sequence();
            // Pause the nested sequence - it will be controlled by the parent sequence
            sequence.isPlaying = false;
            sequence.SetDelay(Delay);
            sequence.AppendInterval(interval);
            
            if (FlowType == FlowType.Join)
                animationSequence.Join(sequence);
            else
                animationSequence.Append(sequence);
        }

        public override void ResetToInitialState()
        {
        }

        public override string GetDisplayNameForEditor(int index)
        {
            return $"{index}. Wait {interval} seconds";
        }
    }
}
