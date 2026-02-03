using System;
using UnityEditor.IMGUI.Controls;

namespace THEBADDEST.Tweening2
{
    public sealed class TweenStepAdvancedDropdownItem : AdvancedDropdownItem
    {
        private readonly Type animationStepType;
        public Type AnimationStepType => animationStepType;

        public TweenStepAdvancedDropdownItem(TweenStepBase tweenStepBase, string displayName) : base(displayName)
        {
            animationStepType = tweenStepBase.GetType();
        }
    }
}
