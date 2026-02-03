using System;
using System.Linq;
using THEBADDEST.Tweening2;
using THEBADDEST.Tweening2.Core;
using UnityEngine;

namespace THEBADDEST.Tweening2
{
    [Serializable]
    public sealed class TweenerTweenStep : GameObjectTweenStep
    {
        public override string DisplayName => "Tween Target";
        [SerializeField]
        private int loopCount = 1;
        public int LoopCount
        {
            get => loopCount;
            set => loopCount = value;
        }

        [SerializeField]
        private LoopType loopType;
        public LoopType LoopType
        {
            get => loopType;
            set => loopType = value;
        }

        [SerializeReference]
        private TweenerActionBase[] actions;
        public TweenerActionBase[] Actions
        {
            get => actions;
            set => actions = value;
        }

        public override void AddTweenToSequence(Sequence animationSequence)
        {
            if (actions == null || actions.Length == 0)
                return;

            Sequence sequence = TweenCore.Sequence();

            for (int i = 0; i < actions.Length; i++)
            {
                if (actions[i] == null)
                    continue;

                Tween tween = actions[i].GenerateTween(target, duration);
                if (tween == null)
                {
                    Debug.LogWarning($"[TweenerTweenStep] Action {i} failed to generate tween. Target: {target}");
                    continue;
                }

                sequence.Join(tween);
            }

            int targetLoopCount = Mathf.Max(1,loopCount);
            sequence.SetLoops(targetLoopCount, loopType);
            sequence.SetDelay(Delay);
            
            if (FlowType == FlowType.Join)
                animationSequence.Join(sequence);
            else
                animationSequence.Append(sequence);
        }

        public override void ResetToInitialState()
        {
            if (actions == null)
                return;

            for (int i = actions.Length - 1; i >= 0; i--)
            {
                if (actions[i] != null)
                    actions[i].ResetToInitialState();
            }
        }

        public override string GetDisplayNameForEditor(int index)
        {
            string targetName = "NULL";
            if (target != null)
                targetName = target.name;
            
            string actionsList = "No Actions";
            if (actions is { Length: > 0 })
            {
                actionsList = string.Join(", ", actions.Where(a => a != null).Select(action => action.DisplayName));
                if (actionsList.Length > 45)
                    actionsList = actionsList.Substring(0, 42) + "...";
            }
            
            return $"{index}. {targetName}: {actionsList}";
        }

        public bool TryGetActionAtIndex<T>(int index, out T result) where T: TweenerActionBase
        {
            if (actions == null || index < 0 || index > actions.Length - 1)
            {
                result = null;
                return false;
            }

            result = actions[index] as T;
            return result != null;
        }
    }
}
