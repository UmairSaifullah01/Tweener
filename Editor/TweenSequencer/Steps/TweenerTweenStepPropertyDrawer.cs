using System;
using UnityEditor;
using UnityEngine;

namespace THEBADDEST.Tweening2
{
    [CustomPropertyDrawer(typeof(TweenerTweenStep))]
    public class TweenerTweenStepPropertyDrawer : TweenStepBasePropertyDrawer
    {
        public override bool CanCacheInspectorGUI(SerializedProperty property)
        {
            return false;
        }

        private void AddNewActionOfType(SerializedProperty actionsSerializedProperty, Type targetType)
        {
            actionsSerializedProperty.arraySize++;
            SerializedProperty arrayElement = actionsSerializedProperty.GetArrayElementAtIndex(actionsSerializedProperty.arraySize - 1);
            arrayElement.managedReferenceValue = Activator.CreateInstance(targetType);
            
            actionsSerializedProperty.serializedObject.ApplyModifiedProperties();
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            DrawBaseGUI(position, property, label, "actions", "loopCount", "loopType");

            float originHeight = position.y;
            if (property.isExpanded)
            {

                if (EditorGUI.indentLevel > 0)
                    position = EditorGUI.IndentedRect(position);

                EditorGUI.indentLevel++;
                position = EditorGUI.IndentedRect(position);
                EditorGUI.indentLevel--;

                EditorGUI.BeginChangeCheck();

                SerializedProperty actionsSerializedProperty = property.FindPropertyRelative("actions");
                SerializedProperty targetSerializedProperty = property.FindPropertyRelative("target");
                position.y += base.GetPropertyHeight(property, label) + EditorGUIUtility.standardVerticalSpacing;

                SerializedProperty loopCountSerializedProperty = property.FindPropertyRelative("loopCount");
                EditorGUI.PropertyField(position, loopCountSerializedProperty);
                position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                loopCountSerializedProperty.intValue = Mathf.Clamp(loopCountSerializedProperty.intValue, -1, int.MaxValue);
                if (loopCountSerializedProperty.intValue != 0)
                {
                    if (loopCountSerializedProperty.intValue == -1)
                    {
                        Debug.LogWarning("Infinity Loops doesn't work well with sequence, the best way of doing " +
                                         "that is setting to the int.MaxValue, will end eventually, but will take a really " +
                                         "long time.");
                        loopCountSerializedProperty.intValue = int.MaxValue;
                    }
                    SerializedProperty loopTypeSerializedProperty = property.FindPropertyRelative("loopType");
                    EditorGUI.PropertyField(position, loopTypeSerializedProperty);
                    position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                }

                position.height = EditorGUIUtility.singleLineHeight;
                if (GUI.Button(position, "Add Actions"))
                {
                    SequenceEditorGUIUtility.TweenerActionsDropdown.Show(position, actionsSerializedProperty, targetSerializedProperty.objectReferenceValue,
                        item =>
                        {
                            AddNewActionOfType(actionsSerializedProperty, item.BaseTweenerActionType);
                        });
                }

                position.y += 10;

                if (actionsSerializedProperty.arraySize > 0)
                    position.y += 26;
                
                for (int i = 0; i < actionsSerializedProperty.arraySize; i++)
                {
                    SerializedProperty actionSerializedProperty = actionsSerializedProperty.GetArrayElementAtIndex(i);

                    bool guiEnabled = GUI.enabled;
                    DrawDeleteActionButton(position, property, i);

                    if (GUI.enabled)
                    {
                        bool isValidTargetForRequiredComponent = IsValidTargetForRequiredComponent(targetSerializedProperty, actionSerializedProperty);
                        GUI.enabled = isValidTargetForRequiredComponent;
                    }
                    
                    EditorGUI.PropertyField(position, actionSerializedProperty);
                    
                    position.y += actionSerializedProperty.GetPropertyDrawerHeight();
                    
                    if (i < actionsSerializedProperty.arraySize - 1)
                        position.y += 30;

                    GUI.enabled = guiEnabled;
                }
                
                EditorGUI.indentLevel--;
                position = EditorGUI.IndentedRect(position);
                EditorGUI.indentLevel++;
                
                if (EditorGUI.EndChangeCheck())
                    property.serializedObject.ApplyModifiedProperties();
            }
            property.SetPropertyDrawerHeight(position.y - originHeight + EditorGUIUtility.singleLineHeight);
        }

        private static bool IsValidTargetForRequiredComponent(SerializedProperty targetSerializedProperty, SerializedProperty actionSerializedProperty)
        {
            if (targetSerializedProperty.objectReferenceValue == null)
                return false;

            Type type = actionSerializedProperty.GetTypeFromManagedFullTypeName();
            return SequenceEditorGUIUtility.CanActionBeAppliedToTarget(type, targetSerializedProperty.objectReferenceValue as GameObject); 
        }

        private static void DrawDeleteActionButton(Rect position, SerializedProperty property, int targetIndex)
        {
            Rect buttonPosition = position;
            buttonPosition.width = 24;
            buttonPosition.x += position.width - 34;
            if (GUI.Button(buttonPosition, "X", EditorStyles.miniButton))
            {
                EditorApplication.delayCall += () =>
                {
                    DeleteElementAtIndex(property, targetIndex);
                };
            }
        }

        private static void DeleteElementAtIndex(SerializedProperty serializedProperty, int targetIndex)
        {
            SerializedProperty actionsPropertyPath = serializedProperty.FindPropertyRelative("actions");
            actionsPropertyPath.DeleteArrayElementAtIndex(targetIndex);
            SerializedPropertyExtensions.ClearPropertyCache(actionsPropertyPath.propertyPath);
            actionsPropertyPath.serializedObject.ApplyModifiedProperties();
            actionsPropertyPath.serializedObject.Update();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return property.GetPropertyDrawerHeight();
        }
    }
}
