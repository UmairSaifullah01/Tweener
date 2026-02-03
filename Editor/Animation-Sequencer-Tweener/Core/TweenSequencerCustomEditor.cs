using System;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditorInternal;
using UnityEngine;

namespace THEBADDEST.Tweening2
{
    [CustomEditor(typeof(TweenSequencer), true)]
    public class TweenSequencerCustomEditor : Editor
    {
        private static readonly GUIContent CollapseAllAnimationStepsContent = new GUIContent("▸◂", "Collapse all animation steps");
        private static readonly GUIContent ExpandAllAnimationStepsContent   = new GUIContent("◂▸", "Expand all animation steps");

        private ReorderableList reorderableList;
        
        private TweenSequencer sequencer;

        private static TweenStepAdvancedDropdown cachedTweenStepsDropdown;
        private static TweenStepAdvancedDropdown tweenStepAdvancedDropdown
        {
            get
            {
                if (cachedTweenStepsDropdown == null)
                    cachedTweenStepsDropdown = new TweenStepAdvancedDropdown(new AdvancedDropdownState());
                return cachedTweenStepsDropdown;
            }
        }

        private bool showPreviewPanel = true;
        private bool showSettingsPanel;
        private bool showCallbacksPanel;
        private bool showStepsPanel = true;
        private float tweenTimeScale = 1f;
        private bool wasShowingStepsPanel;
        private bool justStartPreviewing;
        private bool isPreviewing = false;

        private (float start, float end)[] previewingTimings;

        private void OnEnable()
        {
            sequencer = target as TweenSequencer;
            reorderableList = new ReorderableList(serializedObject, serializedObject.FindProperty("tweenSteps"), true, false, true, true);
            reorderableList.drawElementCallback += OnDrawAnimationStep;
            reorderableList.drawElementBackgroundCallback += OnDrawAnimationStepBackground;
            reorderableList.elementHeightCallback += GetAnimationStepHeight;
            reorderableList.onAddDropdownCallback += OnClickToAddNew;
            reorderableList.onRemoveCallback += OnClickToRemove;
            reorderableList.onReorderCallback += OnListOrderChanged;
            reorderableList.drawHeaderCallback += OnDrawerHeader;
            EditorApplication.update += EditorUpdate;
            EditorApplication.playModeStateChanged += OnEditorPlayModeChanged;
            
#if UNITY_2021_1_OR_NEWER
            UnityEditor.SceneManagement.PrefabStage.prefabSaving += PrefabSaving;
#else
            UnityEditor.Experimental.SceneManagement.PrefabStage.prefabSaving += PrefabSaving;
#endif
            
            Repaint();
        }

        public override bool RequiresConstantRepaint()
        {
            return isPreviewing;
        }

        public override bool UseDefaultMargins()
        {
            return false;
        }

        private void OnDisable()
        {
            reorderableList.drawElementCallback -= OnDrawAnimationStep;
            reorderableList.drawElementBackgroundCallback -= OnDrawAnimationStepBackground;
            reorderableList.elementHeightCallback -= GetAnimationStepHeight;
            reorderableList.onAddDropdownCallback -= OnClickToAddNew;
            reorderableList.onRemoveCallback -= OnClickToRemove;
            reorderableList.onReorderCallback -= OnListOrderChanged;
            reorderableList.drawHeaderCallback -= OnDrawerHeader;
            EditorApplication.playModeStateChanged -= OnEditorPlayModeChanged;
            EditorApplication.update -= EditorUpdate;

#if UNITY_2021_1_OR_NEWER
            UnityEditor.SceneManagement.PrefabStage.prefabSaving -= PrefabSaving;
#else
            UnityEditor.Experimental.SceneManagement.PrefabStage.prefabSaving -= PrefabSaving;
#endif

            if (!Application.isPlaying)
            {
                if (isPreviewing)
                {
                    sequencer.ResetToInitialState();
                    StopPreview();
                }
            }
            
            tweenTimeScale = 1f;
        }

        private void EditorUpdate()
        {
            if (Application.isPlaying)
                return;

            SerializedProperty progressSP = serializedObject.FindProperty("progress");
            if (progressSP == null || Mathf.Approximately(progressSP.floatValue, -1))
                return;
            
            SetProgress(progressSP.floatValue);
        }

        private void OnEditorPlayModeChanged(PlayModeStateChange playModeState)
        {
            if (playModeState == PlayModeStateChange.ExitingEditMode)
            {
                if (isPreviewing)
                {
                    sequencer.ResetToInitialState();
                    StopPreview();
                }
            }
        }
        
        private void PrefabSaving(GameObject gameObject)
        {
            if (isPreviewing)
            {
                sequencer.ResetToInitialState();
                StopPreview();
            }
        }
        
        private void OnDrawerHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Animation Steps");
        }
        
        private void AddNewAnimationStepOfType(Type targetAnimationType)
        {
            SerializedProperty animationStepsProperty = reorderableList.serializedProperty;
            int targetIndex = animationStepsProperty.arraySize;
            animationStepsProperty.InsertArrayElementAtIndex(targetIndex);
            SerializedProperty arrayElementAtIndex = animationStepsProperty.GetArrayElementAtIndex(targetIndex);
            object managedReferenceValue = Activator.CreateInstance(targetAnimationType);
            arrayElementAtIndex.managedReferenceValue = managedReferenceValue;
        
            SerializedProperty targetSerializedProperty = arrayElementAtIndex.FindPropertyRelative("target");
            if (targetSerializedProperty != null)
                targetSerializedProperty.objectReferenceValue = (serializedObject.targetObject as TweenSequencer)?.gameObject;
            
            serializedObject.ApplyModifiedProperties();
        }
        
        private void OnClickToRemove(ReorderableList list)
        {
            SerializedProperty element = reorderableList.serializedProperty.GetArrayElementAtIndex(list.index);
            SerializedPropertyExtensions.ClearPropertyCache(element.propertyPath);
            reorderableList.serializedProperty.DeleteArrayElementAtIndex(list.index);
            reorderableList.serializedProperty.serializedObject.ApplyModifiedProperties();
        }
        
        private void OnListOrderChanged(ReorderableList list)
        {
            SerializedPropertyExtensions.ClearPropertyCache(list.serializedProperty.propertyPath);
            list.serializedProperty.serializedObject.ApplyModifiedProperties();
        }
        
        private void OnClickToAddNew(Rect buttonRect, ReorderableList list)
        {
            tweenStepAdvancedDropdown.Show(buttonRect, OnNewAnimationStepTypeSelected);
        }

        private void OnNewAnimationStepTypeSelected(TweenStepAdvancedDropdownItem tweenStepAdvancedDropdownItem)
        {
            AddNewAnimationStepOfType(tweenStepAdvancedDropdownItem.AnimationStepType);
        }

        public override void OnInspectorGUI()
        {
            if (sequencer.IsResetRequired())
            {
                SetDefaults();
            }

            DrawFoldoutArea("Settings", ref showSettingsPanel, DrawSettings, DrawSettingsHeader);
            DrawFoldoutArea("Callback", ref showCallbacksPanel, DrawCallbacks);
            DrawFoldoutArea("Preview", ref showPreviewPanel, DrawPreviewControls);
            DrawFoldoutArea("Steps", ref showStepsPanel, DrawAnimationSteps, DrawAnimationStepsHeader, 50);
        }

        private void DrawAnimationStepsHeader(Rect rect, bool foldout)
        {
            if (!foldout)
                return;
            
            var collapseAllRect = new Rect(rect)
            {
                xMin = rect.xMax - 50,
                xMax = rect.xMax - 25,
            };

            var expandAllRect = new Rect(rect)
            {
                xMin = rect.xMax - 25,
                xMax = rect.xMax - 0,
            };

            if (GUI.Button(collapseAllRect, CollapseAllAnimationStepsContent, EditorStyles.miniButtonLeft))
            {
                SetStepsExpanded(false);
            }

            if (GUI.Button(expandAllRect, ExpandAllAnimationStepsContent, EditorStyles.miniButtonRight))
            {
                SetStepsExpanded(true);
            }
        }

        private void DrawAnimationSteps()
        {
            bool wasGUIEnabled = GUI.enabled;
            if (isPreviewing)
                GUI.enabled = false;

            reorderableList.DoLayoutList();
                        
            GUI.enabled = wasGUIEnabled;
        }

        protected virtual void DrawCallbacks()
        {
            bool wasGUIEnabled = GUI.enabled;
            if (isPreviewing)
                GUI.enabled = false;
            SerializedProperty onStartEventSerializedProperty = serializedObject.FindProperty("onStartEvent");
            SerializedProperty onFinishedEventSerializedProperty = serializedObject.FindProperty("onFinishedEvent");
            SerializedProperty onProgressEventSerializedProperty = serializedObject.FindProperty("onProgressEvent");

            
            using (EditorGUI.ChangeCheckScope changedCheck = new EditorGUI.ChangeCheckScope())
            {
                EditorGUILayout.PropertyField(onStartEventSerializedProperty);
                EditorGUILayout.PropertyField(onFinishedEventSerializedProperty);
                EditorGUILayout.PropertyField(onProgressEventSerializedProperty);
                
                if (changedCheck.changed)
                    serializedObject.ApplyModifiedProperties();
            }
            
            GUI.enabled = wasGUIEnabled;
        }

        private void DrawSettingsHeader(Rect rect, bool foldout)
        {
            var autoPlayModeSerializedProperty = serializedObject.FindProperty("autoplayMode");
            var autoKillSerializedProperty = serializedObject.FindProperty("autoKill");

            var autoplayMode = (TweenSequencer.AutoplayType) autoPlayModeSerializedProperty.enumValueIndex;
            var autoKill = autoKillSerializedProperty.boolValue;

            if (autoKill)
                rect = DrawAutoSizedBadgeRight(rect, "Auto Kill", new Color(1f, 0.2f, 0f, 0.6f));

            if (autoplayMode == TweenSequencer.AutoplayType.Awake)
                rect = DrawAutoSizedBadgeRight(rect, "AutoPlay on Awake", new Color(1f, 0.7f, 0f, 0.6f));
            else if (autoplayMode == TweenSequencer.AutoplayType.OnEnable)
                rect = DrawAutoSizedBadgeRight(rect, "AutoPlay on Enable", new Color(1f, 0.7f, 0f, 0.6f));
        }

        private void DrawSettings()
        {
            SerializedProperty autoPlayModeSerializedProperty = serializedObject.FindProperty("autoplayMode");
            SerializedProperty pauseOnAwakeSerializedProperty = serializedObject.FindProperty("startPaused");

            using (EditorGUI.ChangeCheckScope changedCheck = new EditorGUI.ChangeCheckScope())
            {
                TweenSequencer.AutoplayType autoplayMode = (TweenSequencer.AutoplayType)autoPlayModeSerializedProperty.enumValueIndex;
                EditorGUILayout.PropertyField(autoPlayModeSerializedProperty);

                if (autoplayMode != TweenSequencer.AutoplayType.Nothing)
                    EditorGUILayout.PropertyField(pauseOnAwakeSerializedProperty);
				
                DrawPlaybackSpeedSlider();
                
                if (changedCheck.changed)
                    serializedObject.ApplyModifiedProperties();
            }
            
            bool wasEnabled = GUI.enabled; 
            if (isPreviewing)
                GUI.enabled = false;
            
            SerializedProperty updateTypeSerializedProperty = serializedObject.FindProperty("updateType");
            SerializedProperty timeScaleIndependentSerializedProperty = serializedObject.FindProperty("timeScaleIndependent");
            SerializedProperty sequenceDirectionSerializedProperty = serializedObject.FindProperty("playType");
            SerializedProperty loopsSerializedProperty = serializedObject.FindProperty("loops");
            SerializedProperty loopTypeSerializedProperty = serializedObject.FindProperty("loopType");
            SerializedProperty autoKillSerializedProperty = serializedObject.FindProperty("autoKill");

            using (EditorGUI.ChangeCheckScope changedCheck = new EditorGUI.ChangeCheckScope())
            {
                EditorGUILayout.PropertyField(timeScaleIndependentSerializedProperty);
                EditorGUILayout.PropertyField(sequenceDirectionSerializedProperty);
                EditorGUILayout.PropertyField(updateTypeSerializedProperty);
                EditorGUILayout.PropertyField(autoKillSerializedProperty);

                EditorGUILayout.PropertyField(loopsSerializedProperty);

                if (loopsSerializedProperty.intValue != 0)
                {
                    EditorGUILayout.PropertyField(loopTypeSerializedProperty);
                }
 
                if (changedCheck.changed)
                {
                    loopsSerializedProperty.intValue = Mathf.Clamp(loopsSerializedProperty.intValue, -1, int.MaxValue);
                    serializedObject.ApplyModifiedProperties();
                }
                
            }
            GUI.enabled = wasEnabled;
        }
		
        private void DrawPlaybackSpeedSlider()
        {
            GUILayout.FlexibleSpace();
            EditorGUI.BeginChangeCheck();
            
            var playbackSpeedProperty = serializedObject.FindProperty("playbackSpeed");
            playbackSpeedProperty.floatValue = EditorGUILayout.Slider("Playback Speed", playbackSpeedProperty.floatValue, 0, 2);

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                UpdateSequenceTimeScale();
            }

            GUILayout.FlexibleSpace();
        }
        
        private void UpdateSequenceTimeScale()
        {
            if (sequencer.PlayingSequence == null)
                return;
            
            sequencer.PlayingSequence.timeScale = sequencer.PlaybackSpeed * tweenTimeScale;
        }
        

        private void DrawPreviewControls()
        {
            EditorGUILayout.BeginHorizontal();

            GUILayout.FlexibleSpace();
            
            bool guiEnabled = GUI.enabled;

            GUIStyle previewButtonStyle = new GUIStyle(GUI.skin.button);
            previewButtonStyle.fixedWidth = previewButtonStyle.fixedHeight = 40;
            
            GUIContent backButton = EditorGUIUtility.IconContent("d_beginButton");
            backButton.tooltip = "Rewind";
            if (GUILayout.Button(backButton, previewButtonStyle))
            {
                if (!sequencer.IsPlaying)
                    PlaySequence();

                sequencer.Rewind();
            }

            GUIContent stepBack = EditorGUIUtility.IconContent("Animation.PrevKey");
            stepBack.tooltip = "Step Back";
            if (GUILayout.Button(stepBack, previewButtonStyle))
            {
                if(!sequencer.IsPlaying)
                    PlaySequence();

                StepBack();
            }

            if (sequencer.IsPlaying)
            {
                GUIContent pauseButton = EditorGUIUtility.IconContent("PauseButton On");
                pauseButton.tooltip = "Pause";
                if (GUILayout.Button(pauseButton, previewButtonStyle))
                {
                    sequencer.Pause();
                }
            }
            else
            {
                GUIContent playButton = EditorGUIUtility.IconContent("PlayButton On");
                playButton.tooltip = "Play";
                if (GUILayout.Button(playButton, previewButtonStyle))
                {
                    PlaySequence();
                }
            }

            GUIContent stepNext = EditorGUIUtility.IconContent("Animation.NextKey");
            stepNext.tooltip = "Step Next";
            if (GUILayout.Button(stepNext, previewButtonStyle))
            {
                if(!sequencer.IsPlaying)
                    PlaySequence();

                StepNext();
            }
            
            GUIContent forwardButton = EditorGUIUtility.IconContent("d_endButton");
            forwardButton.tooltip = "Fast Forward";
            if (GUILayout.Button(forwardButton, previewButtonStyle))
            {
                if (!sequencer.IsPlaying)
                    PlaySequence();

                sequencer.Complete();
            }

            if (!Application.isPlaying)
            {
                GUI.enabled = isPreviewing;
                GUIContent stopButton = EditorGUIUtility.IconContent("animationdopesheetkeyframe");
                stopButton.tooltip = "Stop";
                if (GUILayout.Button(stopButton, previewButtonStyle))
                {
                    sequencer.Rewind();
                    sequencer.PlayingSequence?.Kill(false);
                    StopPreview();
                    sequencer.ResetToInitialState();
                    sequencer.ClearPlayingSequence();
                }
            }

            GUI.enabled = guiEnabled;
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            DrawTimeScaleSlider();
            DrawProgressSlider();
        }

        private void StepBack()
        {
            if (!sequencer.IsPlaying)
                PlaySequence();
            
            if (sequencer.PlayingSequence != null)
            {
                float currentProgress = sequencer.PlayingSequence.ElapsedPercentage();
                float duration = sequencer.PlayingSequence.Duration();
                sequencer.PlayingSequence.Goto((currentProgress - 0.01f) * duration, true);
            }
        }

        private void StepNext()
        {
            if (!sequencer.IsPlaying)
                PlaySequence();

            if (sequencer.PlayingSequence != null)
            {
                float currentProgress = sequencer.PlayingSequence.ElapsedPercentage();
                float duration = sequencer.PlayingSequence.Duration();
                sequencer.PlayingSequence.Goto((currentProgress + 0.01f) * duration, true);
            }
        }

        private void PlaySequence()
        {
            justStartPreviewing = false;
            if (!Application.isPlaying)
            {
                if (!isPreviewing)
                {
                    justStartPreviewing = true;
                    isPreviewing = true;

                    sequencer.Play();
                    
                    previewingTimings = TweenerProxy.GetTimings(sequencer.PlayingSequence,
                        sequencer.TweenSteps);
                }
                else
                {
                    if (sequencer.PlayingSequence == null)
                    {
                        sequencer.Play();
                    }
                    else
                    {
                        if (!sequencer.PlayingSequence.IsBackwards() &&
                            sequencer.PlayingSequence.fullPosition >= sequencer.PlayingSequence.Duration())
                        {
                            sequencer.Rewind();
                        }
                        else if (sequencer.PlayingSequence.IsBackwards() &&
                                 sequencer.PlayingSequence.fullPosition <= 0f)
                        {
                            sequencer.Complete();
                        }

                        sequencer.TogglePause();
                    }
                }
            }
            else
            {
                if (sequencer.PlayingSequence == null)
                    sequencer.Play();
                else
                {
                    if (sequencer.PlayingSequence.IsActive())
                        sequencer.TogglePause();
                    else
                        sequencer.Play();
                }
            }
        }

        private void StopPreview()
        {
            isPreviewing = false;
        }

        private void DrawProgressSlider()
        {
            GUILayout.FlexibleSpace();

            EditorGUI.BeginChangeCheck();
            float tweenProgress = 0;

            tweenProgress = GetCurrentSequencerProgress();

            var oldLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 65;
            tweenProgress = EditorGUILayout.Slider("Progress", tweenProgress, 0, 1);
            EditorGUIUtility.labelWidth = oldLabelWidth;

            if (EditorGUI.EndChangeCheck())
            {
                SetProgress(tweenProgress);

                if (!Application.isPlaying)
                {
                    serializedObject.FindProperty("progress").floatValue = tweenProgress;
                    serializedObject.ApplyModifiedProperties();
                }
            }

            GUILayout.FlexibleSpace();
        }

        private void SetProgress(float tweenProgress)
        {
            if (!sequencer.IsPlaying)
                PlaySequence();

            if (sequencer.PlayingSequence != null)
            {
                float duration = sequencer.PlayingSequence.Duration();
                sequencer.PlayingSequence.Goto(tweenProgress * duration, true);
            }
        }

        private float GetCurrentSequencerProgress()
        {
            float tweenProgress;
            if (sequencer.PlayingSequence != null && sequencer.PlayingSequence.IsActive())
                tweenProgress = sequencer.PlayingSequence.ElapsedPercentage();
            else
                tweenProgress = 0;
            return tweenProgress;
        }

        private void DrawTimeScaleSlider()
        {
            GUILayout.FlexibleSpace();
            EditorGUI.BeginChangeCheck();
            
            var oldLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 65;
            tweenTimeScale = EditorGUILayout.Slider("TimeScale", tweenTimeScale, 0, 2);
            EditorGUIUtility.labelWidth = oldLabelWidth;
			
            UpdateSequenceTimeScale();

            GUILayout.FlexibleSpace();
        }

        private void DrawFoldoutArea(string title, ref bool foldout, Action additionalInspectorGUI,
            Action<Rect, bool> additionalHeaderGUI = null, float additionalHeaderWidth = 0)
        {
            Rect rect = EditorGUILayout.GetControlRect();

            if (Event.current.type == EventType.Repaint)
            {
                GUI.skin.box.Draw(rect, false, false, false, false);
            }

            using (new EditorGUILayout.VerticalScope(TweenSequencerStyles.InspectorSideMargins))
            {
                Rect rectWithMargins = new Rect(rect)
                {
                    xMin = rect.xMin + TweenSequencerStyles.InspectorSideMargins.padding.left,
                    xMax = rect.xMax - TweenSequencerStyles.InspectorSideMargins.padding.right,
                };

                var foldoutRect = new Rect(rectWithMargins)
                {
                    xMax = rectWithMargins.xMax - additionalHeaderWidth,
                };

                var additionalHeaderRect = new Rect(rectWithMargins)
                {
                    xMin = foldoutRect.xMax,
                };

                foldout = EditorGUI.Foldout(foldoutRect, foldout, title, true);

                additionalHeaderGUI?.Invoke(additionalHeaderRect, foldout);

                if (foldout)
                {
                    additionalInspectorGUI.Invoke();
                    GUILayout.Space(10);
                }
            }
        }

        private void OnDrawAnimationStepBackground(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (Event.current.type == EventType.Repaint)
            {
                var titlebarRect = new Rect(rect)
                {
                    height = EditorGUIUtility.singleLineHeight,
                };

                if (isActive)
                    ReorderableList.defaultBehaviours.DrawElementBackground(rect, index, true, isFocused, false);
                else
                    TweenSequencerStyles.InspectorTitlebar.Draw(titlebarRect, false, false, false, false);
            }

            if (Event.current.type == EventType.Repaint &&
                isPreviewing &&
                previewingTimings != null &&
                index >= 0 && index < previewingTimings.Length)
            {
                var (start, end) = previewingTimings[index];

                var progress = GetCurrentSequencerProgress();

                var progressRect = new Rect(rect)
                {
                    xMin = Mathf.Lerp(rect.xMin, rect.xMax, start) - 1,
                    xMax = Mathf.Lerp(rect.xMin, rect.xMax, end) + 1,
                    height = EditorGUIUtility.singleLineHeight,
                };

                var markerRect = new Rect(rect)
                {
                    xMin = Mathf.Lerp(rect.xMin, rect.xMax, progress) - 1,
                    xMax = Mathf.Lerp(rect.xMin, rect.xMax, progress) + 1,
                    height = EditorGUIUtility.singleLineHeight,
                };

                var oldColor = GUI.color;

                GUI.color = new Color(0f, 0.5f, 0f, 0.45f);
                GUI.DrawTexture(progressRect, EditorGUIUtility.whiteTexture);

                GUI.color = Color.black;
                GUI.DrawTexture(markerRect, EditorGUIUtility.whiteTexture);

                GUI.color = oldColor;
            }
        }

        private void OnDrawAnimationStep(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty element = reorderableList.serializedProperty.GetArrayElementAtIndex(index);
            SerializedProperty flowTypeSerializedProperty = element.FindPropertyRelative("flowType");

            if (!element.TryGetTargetObjectOfProperty(out TweenStepBase animationStepBase))
                return;

            FlowType flowType = (FlowType)flowTypeSerializedProperty.enumValueIndex;

            int baseIdentLevel = EditorGUI.indentLevel;
            
            GUIContent guiContent = new GUIContent(element.displayName);
            if (animationStepBase != null)
                guiContent = new GUIContent(animationStepBase.GetDisplayNameForEditor(index + 1));

            if (flowType == FlowType.Join)
                EditorGUI.indentLevel = baseIdentLevel + 1;
            
            rect.height = EditorGUIUtility.singleLineHeight;
            rect.x += 10;
            rect.width -= 20;

            EditorGUI.PropertyField(
                rect,
                element,
                guiContent,
                false
            );

            EditorGUI.indentLevel = baseIdentLevel;
        }

        private float GetAnimationStepHeight(int index)
        {
            if (index > reorderableList.serializedProperty.arraySize - 1)
                return EditorGUIUtility.singleLineHeight;
            
            SerializedProperty element = reorderableList.serializedProperty.GetArrayElementAtIndex(index);
            return element.GetPropertyDrawerHeight();
        }

        private void SetStepsExpanded(bool expanded)
        {
            SerializedProperty animationStepsProperty = reorderableList.serializedProperty;
            for (int i = 0; i < animationStepsProperty.arraySize; i++)
            {
                animationStepsProperty.GetArrayElementAtIndex(i).isExpanded = expanded;
            }
        }

        private void SetDefaults()
        {
            sequencer = target as TweenSequencer;
            if (sequencer != null)
            {
                sequencer.SetAutoplayMode(TweenSequencer.AutoplayType.Awake);
                sequencer.SetPlayOnAwake(false);
                sequencer.SetPauseOnAwake(false);
                sequencer.SetTimeScaleIndependent(false);
                sequencer.SetPlayType(TweenSequencer.PlayType.Forward);
                sequencer.SetUpdateType(THEBADDEST.Tweening2.Core.UpdateType.Normal);
                sequencer.SetAutoKill(true);
                sequencer.SetLoops(0);
                sequencer.ResetComplete();
            }
        }

        private static Rect DrawAutoSizedBadgeRight(Rect rect, string text, Color color)
        {
            var style = TweenSequencerStyles.Badge;
            var size = style.CalcSize(EditorGUIUtility.TrTempContent(text));
            var buttonRect = new Rect(rect)
            {
                xMin = rect.xMax - size.x,
            };

            if (Event.current.type == EventType.Repaint)
            {
                var oldColor = GUI.backgroundColor;
                GUI.backgroundColor = color;
                style.Draw(buttonRect, text, false, false, true, false);
                GUI.backgroundColor = oldColor;
            }

            return new Rect(rect)
            {
                xMax = rect.xMax - size.x - style.margin.left,
            };
        }
    }
}
