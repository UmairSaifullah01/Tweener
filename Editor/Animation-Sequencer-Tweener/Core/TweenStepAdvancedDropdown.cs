using System;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace THEBADDEST.Tweening2
{
    public sealed class TweenStepAdvancedDropdown : AdvancedDropdown
    {
        private Action<TweenStepAdvancedDropdownItem> callBack;

        public TweenStepAdvancedDropdown(AdvancedDropdownState state) : base(state)
        {
            this.minimumSize = new Vector2(200, 300);
        }

        protected override AdvancedDropdownItem BuildRoot()
        {
            AdvancedDropdownItem root = new AdvancedDropdownItem("Animation Step");

            TypeCache.TypeCollection availableTypesOfAnimationStep = TypeCache.GetTypesDerivedFrom(typeof(TweenStepBase));
            foreach (Type animatedItemType in availableTypesOfAnimationStep)
            {
                if (animatedItemType.IsAbstract)
                    continue;
                
                TweenStepBase tweenStepBase = Activator.CreateInstance(animatedItemType) as TweenStepBase;

                string displayName = tweenStepBase.GetType().Name;
                if (!string.IsNullOrEmpty(tweenStepBase.DisplayName))
                    displayName = tweenStepBase.DisplayName;
                
                root.AddChild(new TweenStepAdvancedDropdownItem(tweenStepBase, displayName));
            }

            return root;
        }

        protected override void ItemSelected(AdvancedDropdownItem item)
        {
            base.ItemSelected(item);
            callBack?.Invoke(item as TweenStepAdvancedDropdownItem);
        }

        public void Show(Rect rect, Action<TweenStepAdvancedDropdownItem> onItemSelectedCallback)
        {
            callBack = onItemSelectedCallback;
            base.Show(rect);
        }
    }
}
