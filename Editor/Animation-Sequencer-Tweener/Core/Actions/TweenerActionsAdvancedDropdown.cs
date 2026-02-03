using System;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using Object = UnityEngine.Object;

namespace THEBADDEST.Tweening2
{
    public sealed class TweenerActionAdvancedDropdownItem : AdvancedDropdownItem
    {
        private Type baseTweenerActionType;
        public Type BaseTweenerActionType => baseTweenerActionType;

        public TweenerActionAdvancedDropdownItem(Type baseTweenerActionType, string displayName) : base(displayName)
        {
            this.baseTweenerActionType = baseTweenerActionType;
        }
    }
    
    public sealed class TweenerActionsAdvancedDropdown : AdvancedDropdown
    {
        private Action<TweenerActionAdvancedDropdownItem> callback;
        private SerializedProperty actionsList;
        private GameObject targetGameObject;

        public TweenerActionsAdvancedDropdown(AdvancedDropdownState state) : base(state)
        {
            this.minimumSize = new Vector2(200, 300);
        }

        protected override AdvancedDropdownItem BuildRoot()
        {
            AdvancedDropdownItem root = new AdvancedDropdownItem("Tweener Actions");

            foreach (var typeToDisplayGUI in SequenceEditorGUIUtility.TypeToDisplayName)
            {
                Type baseTweenerActionType = typeToDisplayGUI.Key;

                AdvancedDropdownItem targetFolder = root;

                if (SequenceEditorGUIUtility.TypeToParentDisplay.TryGetValue(baseTweenerActionType, out GUIContent parent))
                {
                    AdvancedDropdownItem item = targetFolder.children.FirstOrDefault(dropdownItem =>
                        dropdownItem.name.Equals(parent.text, StringComparison.Ordinal));
                    
                    if (item == null)
                    {
                        item = new AdvancedDropdownItem(parent.text)
                        {
                            icon = (Texture2D) parent.image
                        };
                        targetFolder.AddChild(item);
                    }
                    
                    targetFolder = item;
                }

                TweenerActionAdvancedDropdownItem tweenerActionAdvancedDropdownItem = 
                    new TweenerActionAdvancedDropdownItem(baseTweenerActionType, typeToDisplayGUI.Value.text)
                {
                    enabled = !IsTypeAlreadyInUse(actionsList, baseTweenerActionType) && SequenceEditorGUIUtility.CanActionBeAppliedToTarget(baseTweenerActionType, targetGameObject)
                };
                
                if (typeToDisplayGUI.Value.image != null)
                {
                    tweenerActionAdvancedDropdownItem.icon = (Texture2D) typeToDisplayGUI.Value.image;
                }
                
                targetFolder.AddChild(tweenerActionAdvancedDropdownItem);
            }
            
            return root;
        }

        protected override void ItemSelected(AdvancedDropdownItem item)
        {
            base.ItemSelected(item);
            callback?.Invoke(item as TweenerActionAdvancedDropdownItem);
        }

        public void Show(Rect rect, SerializedProperty actionsListSerializedProperty, Object targetGameObject, Action<TweenerActionAdvancedDropdownItem> 
        onActionSelectedCallback)
        {
            callback = onActionSelectedCallback;
            this.actionsList = actionsListSerializedProperty;
            if (targetGameObject is GameObject target)
                this.targetGameObject = target;
            base.Show(rect);
        }

        private bool IsTypeAlreadyInUse(SerializedProperty actionsSerializedProperty, Type targetType)
        {
            if (actionsSerializedProperty == null || string.IsNullOrEmpty(targetType.FullName))
                return false;
            for (int i = 0; i < actionsSerializedProperty.arraySize; i++)
            {
                SerializedProperty actionElement = actionsSerializedProperty.GetArrayElementAtIndex(i);
                if (actionElement.managedReferenceFullTypename.IndexOf(targetType.FullName, StringComparison.Ordinal) > -1)
                    return true;
            }

            return false;
        }
    }
}
