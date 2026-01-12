using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using THEBADDEST.Tweening2;
using THEBADDEST.Tweening2.Core;
using THEBADDEST.Tweening2.Bridge;


namespace THEBADDEST.Tweening2.Examples
{
    /// <summary>
    /// Comprehensive example demonstrating all features of THEBADDEST.Tweening2
    /// </summary>
    public class Tweening2Example : MonoBehaviour
    {
        [Header("Transform References")]
        [SerializeField] private Transform targetTransform;
        [SerializeField] private RectTransform targetRectTransform;
        [SerializeField] private Camera targetCamera;
        [SerializeField] private Image targetImage;
        [SerializeField] private Text targetText;

        [Header("Tween Settings")]
        [SerializeField] private float duration = 1f;
        [SerializeField] private int loops = 1;
        [SerializeField] private LoopType loopType = LoopType.Restart;
        [SerializeField] private EaseType easeType = EaseType.OutQuad;
        [SerializeField] private float delay = 0f;

        private ITweener currentTween;

        void Start()
        {
            // Initialize the tweening system
            TweenCore.Init();
            
            // Uncomment any method below to see it in action
            Example1_BasicTransformTween();
            // Example2_ChainedTween();
            // Example3_RectTransformTween();
            // Example4_CameraTween();
            // Example5_GraphicTween();
            // Example6_CustomPropertyTween();
            // Example7_SequenceExample();
            // Example8_CallbacksExample();
            // Example9_ControlMethods();
            // Example10_LoopTypes();
        }

        #region Example 1: Basic Transform Tween

        /// <summary>
        /// Basic transform movement using extension methods
        /// </summary>
        void Example1_BasicTransformTween()
        {
            if (targetTransform == null) return;

            // Move to a position
            targetTransform.Move(new Vector3(5, 0, 0), duration)
                .SetEase(easeType)
                .SetLoops(loops, loopType)
                .SetDelay(delay);
        }

        #endregion

        #region Example 2: Chained Tween Methods

        /// <summary>
        /// Demonstrates method chaining with multiple settings
        /// </summary>
        void Example2_ChainedTween()
        {
            if (targetTransform == null) return;

            currentTween = targetTransform.Move(new Vector3(0, 5, 0), duration)
                .SetEase(EaseType.InOutBounce)
                .SetLoops(3, LoopType.Yoyo)
                .SetDelay(0.5f)
                .SetTime(true) // Independent time
                .OnComplete(() => Debug.Log("Tween completed!"), false);
        }

        #endregion

        #region Example 3: RectTransform Tween

        /// <summary>
        /// UI RectTransform animations
        /// </summary>
        void Example3_RectTransformTween()
        {
            if (targetRectTransform == null) return;

            // Animate anchored position
            targetRectTransform.AnchoredPosition(new Vector2(200, 100), duration)
                .SetEase(EaseType.OutElastic)
                .SetLoops(2, LoopType.Restart);

            // Animate size
            targetRectTransform.SizeDelta(new Vector2(300, 200), duration)
                .SetEase(EaseType.InOutQuad)
                .SetDelay(0.5f);
        }

        #endregion

        #region Example 4: Camera Tween

        /// <summary>
        /// Camera property animations
        /// </summary>
        void Example4_CameraTween()
        {
            if (targetCamera == null) return;

            // Animate field of view
            targetCamera.FieldOfView(60f, duration)
                .SetEase(EaseType.InOutSine)
                .SetLoops(2, LoopType.Yoyo);

            // Animate background color
            targetCamera.BackgroundColor(Color.red, duration)
                .SetEase(EaseType.Linear)
                .SetDelay(1f);
        }

        #endregion

        #region Example 5: Graphic Tween

        /// <summary>
        /// UI Graphic (Image, Text) animations
        /// </summary>
        void Example5_GraphicTween()
        {
            if (targetImage == null) return;

            // Fade out
            targetImage.Fade(0f, duration)
                .SetEase(EaseType.OutQuad)
                .OnComplete(() => Debug.Log("Image faded out!"), false);

            // Change color
            targetImage.Color(Color.blue, duration)
                .SetEase(EaseType.InOutSine)
                .SetDelay(0.5f);

            // Fill amount (for Image component)
            if (targetImage.type == Image.Type.Filled)
            {
                targetImage.FillAmount(1f, duration)
                    .SetEase(EaseType.OutQuad);
            }
        }

        #endregion

        #region Example 6: Custom Property Tween

        /// <summary>
        /// Tweening custom properties using TweenCore.To()
        /// Use AsITweener() extension method to get ITweener interface for method chaining
        /// </summary>
        void Example6_CustomPropertyTween()
        {
            // Tween a float value - use AsITweener() extension method
            float myValue = 0f;
            TweenCore.To(
                () => myValue,
                (x) => myValue = x,
                100f,
                duration
            )
            .AsITweener()
            .SetEase(EaseType.OutQuad)
            .OnComplete(() => Debug.Log($"Final value: {myValue}"), false);

            // Tween a Vector3 - use AsITweener() extension method
            Vector3 myPosition = Vector3.zero;
            TweenCore.To(
                () => myPosition,
                (x) => myPosition = x,
                new Vector3(10, 20, 30),
                duration
            )
            .AsITweener()
            .SetEase(EaseType.OutElastic);

            // Tween a Color - use AsITweener() extension method
            Color myColor = Color.white;
            TweenCore.To(
                () => myColor,
                (x) => myColor = x,
                Color.red,
                duration
            )
            .AsITweener()
            .SetEase(EaseType.Linear);
        }

        #endregion

        #region Example 7: Sequence

        /// <summary>
        /// Creating sequences of tweens
        /// Note: Sequence extension methods need to be implemented for full functionality
        /// </summary>
        void Example7_SequenceExample()
        {
            if (targetTransform == null) return;

            // Create a sequence (basic usage)
            var sequence = TweenCore.Sequence();
            
            // Note: For full sequence functionality, you would need extension methods like:
            // sequence.Append(tween), sequence.AppendInterval(time), etc.
            // For now, sequences can be created but adding tweens requires extension methods
            
            Debug.Log("Sequence created. Extension methods for Append/Insert needed for full functionality.");
        }

        #endregion

        #region Example 8: Callbacks

        /// <summary>
        /// Using callbacks with tweens
        /// </summary>
        void Example8_CallbacksExample()
        {
            if (targetTransform == null) return;

            currentTween = targetTransform.Move(new Vector3(5, 0, 0), duration)
                .SetEase(EaseType.OutQuad)
                .SetLoops(3, LoopType.Restart)
                .OnComplete(() => 
                {
                    Debug.Log("All loops completed!");
                }, false) // false = called when all loops complete
                .OnComplete(() => 
                {
                    Debug.Log("Single iteration completed!");
                }, true); // true = called after each loop iteration

            // Subscribe to events
            currentTween.OnCompleteAllLoops += () => Debug.Log("Event: All loops done!");
            currentTween.OnCompleteIteration += () => Debug.Log("Event: Iteration done!");
        }

        #endregion

        #region Example 9: Control Methods

        /// <summary>
        /// Controlling tweens (pause, resume, kill, reverse)
        /// </summary>
        void Example9_ControlMethods()
        {
            if (targetTransform == null) return;

            currentTween = targetTransform.Move(new Vector3(5, 0, 0), duration)
                .SetEase(EaseType.OutQuad);

            // Control the tween
            StartCoroutine(ControlTweenCoroutine());
        }

        IEnumerator ControlTweenCoroutine()
        {
            yield return new WaitForSeconds(0.5f);
            currentTween.Pause();
            Debug.Log("Tween paused");

            yield return new WaitForSeconds(1f);
            currentTween.Resume();
            Debug.Log("Tween resumed");

            yield return new WaitForSeconds(0.5f);
            currentTween.Reverse();
            Debug.Log("Tween reversed");

            yield return new WaitForSeconds(1f);
            currentTween.Kill();
            Debug.Log("Tween killed");
        }

        #endregion

        #region Example 10: Loop Types

        /// <summary>
        /// Different loop types demonstration
        /// </summary>
        void Example10_LoopTypes()
        {
            if (targetTransform == null) return;

            // Restart: Starts from beginning each loop
            targetTransform.Move(new Vector3(5, 0, 0), 1f)
                .SetLoops(3, LoopType.Restart)
                .SetEase(EaseType.OutQuad);

            // Yoyo: Goes back and forth
            targetTransform.Move(new Vector3(0, 5, 0), 1f)
                .SetLoops(3, LoopType.Yoyo)
                .SetEase(EaseType.InOutSine);

            // Incremental: Adds to end value each loop
            targetTransform.Move(new Vector3(5, 0, 0), 1f)
                .SetLoops(3, LoopType.Incremental)
                .SetEase(EaseType.Linear);
        }

        #endregion

        #region Example 11: Ease Types

        /// <summary>
        /// Different easing functions
        /// </summary>
        void Example11_EaseTypes()
        {
            if (targetTransform == null) return;

            // Built-in ease types
            targetTransform.Move(new Vector3(5, 0, 0), duration)
                .SetEase(EaseType.InOutBounce);

            // Custom AnimationCurve
            AnimationCurve customCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
            customCurve.AddKey(0.5f, 0.8f); // Add a keyframe

            targetTransform.Move(new Vector3(0, 5, 0), duration)
                .SetEase(customCurve);
        }

        #endregion

        #region Example 12: Lerp Delegate

        /// <summary>
        /// Using Lerp delegate for custom interpolation
        /// </summary>
        void Example12_LerpDelegate()
        {
            currentTween = TweenCore.Create();
            currentTween.Lerp((t) =>
            {
                // Custom interpolation logic
                if (targetTransform != null)
                {
                    targetTransform.position = Vector3.Lerp(
                        Vector3.zero,
                        new Vector3(10, 0, 0),
                        t
                    );
                }
            }, duration);
            currentTween.SetEase(EaseType.OutQuad)
            .SetLoops(2, LoopType.Yoyo);
        }

        #endregion

        #region Example 13: Wait for Completion

        /// <summary>
        /// Waiting for tween completion using coroutines
        /// </summary>
        void Example13_WaitForCompletion()
        {
            if (targetTransform == null) return;

            currentTween = targetTransform.Move(new Vector3(5, 0, 0), duration)
                .SetEase(EaseType.OutQuad);

            StartCoroutine(WaitForTweenCoroutine());
        }

        IEnumerator WaitForTweenCoroutine()
        {
            yield return currentTween.WaitForCompletion();
            Debug.Log("Tween finished! Doing something else...");
            
            // Do something after tween completes
            if (targetTransform != null)
            {
                targetTransform.Move(Vector3.zero, duration);
            }
        }

        #endregion

        #region Example 14: Global Controls

        /// <summary>
        /// Global tween control methods
        /// </summary>
        void Example14_GlobalControls()
        {
            // Pause all tweens
            TweenCore.PauseAll();

            // Resume all tweens
            TweenCore.ResumesAll();

            // Kill all tweens
            TweenCore.KillAll();

            // Set global time scale
            TweenCore.timeScale = 0.5f; // Slow motion

            // Set default ease for all new tweens
            TweenCore.defaultEaseType = EaseType.OutQuad;
        }

        #endregion

        #region UI Button Handlers (for testing)

        public void OnMoveButton()
        {
            Example1_BasicTransformTween();
        }

        public void OnScaleButton()
        {
            if (targetTransform != null)
            {
                targetTransform.Scale(new Vector3(2, 2, 2), duration)
                    .SetEase(EaseType.OutBounce)
                    .SetLoops(2, LoopType.Yoyo);
            }
        }

        public void OnRotateButton()
        {
            if (targetTransform != null)
            {
                // targetTransform.Rotate(new Vector3(0, 360, 0), duration)
                //     .SetEase(EaseType.OutQuad);
            }
        }

        public void OnFadeButton()
        {
            if (targetImage != null)
            {
                targetImage.Fade(0f, duration)
                    .SetEase(EaseType.OutQuad)
                    .OnComplete(() => 
                    {
                        targetImage.Fade(1f, duration)
                            .SetEase(EaseType.InQuad);
                    }, false);
            }
        }

        public void OnPauseButton()
        {
            if (currentTween != null)
            {
                currentTween.Pause();
            }
        }

        public void OnResumeButton()
        {
            if (currentTween != null)
            {
                currentTween.Resume();
            }
        }

        public void OnKillButton()
        {
            if (currentTween != null)
            {
                currentTween.Kill();
            }
        }

        #endregion

        void OnDestroy()
        {
            // Clean up tweens when object is destroyed
            if (currentTween != null)
            {
                currentTween.Kill();
            }
        }
    }
}
