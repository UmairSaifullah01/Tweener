using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace THEBADDEST.Tweening2
{
    public static class SequenceEditorGUIUtility
    {
        private static Dictionary<Type, GUIContent> cachedTypeToDisplayName;
        public static Dictionary<Type, GUIContent> TypeToDisplayName
        {
            get
            {
                CacheDisplayTypes();
                return cachedTypeToDisplayName;
            }
        }
        
        private static Dictionary<Type, GUIContent> cachedTypeToInstance;
        public static Dictionary<Type, GUIContent> TypeToParentDisplay
        {
            get
            {
                CacheDisplayTypes();
                return cachedTypeToInstance;
            }
        }

        
        private static Dictionary<Type, TweenerActionBase> typeToInstanceCache;
        public static Dictionary<Type, TweenerActionBase> TypeToInstanceCache
        {
            get
            {
                CacheDisplayTypes();
                return typeToInstanceCache;
            }
        }
        
        private static TweenerActionsAdvancedDropdown cachedTweenerActionsDropdown;
        public static TweenerActionsAdvancedDropdown TweenerActionsDropdown
        {
            get
            {
                if (cachedTweenerActionsDropdown == null)
                    cachedTweenerActionsDropdown = new TweenerActionsAdvancedDropdown(new AdvancedDropdownState());
                return cachedTweenerActionsDropdown;
            }
        }
        

        public static GUIContent GetTypeDisplayName(Type targetBaseTweenerType)
        {
            if (TypeToDisplayName.TryGetValue(targetBaseTweenerType, out GUIContent result))
                return result;

            return new GUIContent(targetBaseTweenerType.Name);
        }

        private static void CacheDisplayTypes()
        {
            if (cachedTypeToDisplayName != null)
                return;

            cachedTypeToDisplayName = new Dictionary<Type, GUIContent>();
            cachedTypeToInstance = new Dictionary<Type, GUIContent>();
            typeToInstanceCache = new Dictionary<Type, TweenerActionBase>();
            
            TypeCache.TypeCollection types = TypeCache.GetTypesDerivedFrom(typeof(TweenerActionBase));
            for (int i = 0; i < types.Count; i++)
            {
                Type type = types[i];
                if (type.IsAbstract)
                    continue;
                
                TweenerActionBase tweenerActionBaseInstance = Activator.CreateInstance(type) as TweenerActionBase;
                if (tweenerActionBaseInstance == null)
                    continue;
                GUIContent guiContent = new GUIContent(tweenerActionBaseInstance.DisplayName);
                if (tweenerActionBaseInstance.TargetComponentType != null)
                {
                    GUIContent targetComponentGUIContent = EditorGUIUtility.ObjectContent(null, tweenerActionBaseInstance.TargetComponentType);
                    guiContent.image = targetComponentGUIContent.image;
                    GUIContent parentGUIContent = new GUIContent(tweenerActionBaseInstance.TargetComponentType.Name)
                    {
                        image = targetComponentGUIContent.image
                    };
                    cachedTypeToInstance.Add(type, parentGUIContent);
                }
                
                cachedTypeToDisplayName.Add(type, guiContent);
                typeToInstanceCache.Add(type, tweenerActionBaseInstance);
            }
        }
        
        public static bool CanActionBeAppliedToTarget(Type targetActionType, GameObject targetGameObject)
        {
            if (targetGameObject == null)
                return false;

            if (TypeToInstanceCache.TryGetValue(targetActionType, out TweenerActionBase actionBaseInstance))
            {
                Type requiredComponent = actionBaseInstance.TargetComponentType;
                
                if (requiredComponent == null)
                    return true;
                
                if (requiredComponent == typeof(Transform))
                    return true;
                    
                if (requiredComponent == typeof(RectTransform))
                    return targetGameObject.transform is RectTransform;

                return targetGameObject.GetComponent(requiredComponent) != null;
            }
            return false;
        }
    }
}
