using System;
using THEBADDEST.Tweening2.Core;
using UnityEngine;
using UnityEngine.Events;

namespace THEBADDEST.Tweening2
{
    [Serializable]
    public sealed class InvokeCallbackTweenStep : TweenStepBase
    {
        [SerializeField]
        private UnityEvent callback = new UnityEvent();
        public UnityEvent Callback
        {
            get => callback;
            set => callback = value;
        }

        public override string DisplayName => "Invoke Callback";
        

        public override void AddTweenToSequence(Sequence animationSequence)
        {
            Sequence sequence = (Sequence)TweenCore.Sequence();
            // Pause the nested sequence - it will be controlled by the parent sequence
            sequence.isPlaying = false;
            sequence.SetDelay(Delay);
            sequence.AppendCallback(() => callback.Invoke());
            
            if (FlowType == FlowType.Append)
                animationSequence.Append(sequence);
            else
                animationSequence.Join(sequence);
        }

        public override void ResetToInitialState()
        {
        }

        public override string GetDisplayNameForEditor(int index)
        {
            string[] persistentTargetNamesArray = new string[callback.GetPersistentEventCount()];
            for (int i = 0; i < callback.GetPersistentEventCount(); i++)
            {
                if (callback.GetPersistentTarget(i) == null)
                    continue;
                
                if (string.IsNullOrWhiteSpace(callback.GetPersistentMethodName(i)))
                    continue;
                
                persistentTargetNamesArray[i] = $"{callback.GetPersistentTarget(i).name}.{callback.GetPersistentMethodName(i)}()";
            }
            
            var persistentTargetNames = $"{string.Join(", ", persistentTargetNamesArray)}";
            if (persistentTargetNames.Length > 45)
                persistentTargetNames = persistentTargetNames.Substring(0, 42) + "...";
            
            return $"{index}. {DisplayName}: {persistentTargetNames}";
        }
    }
}
