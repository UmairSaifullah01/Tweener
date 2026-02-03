using System;
using System.Collections;
using THEBADDEST.Tweening2.Core;
using UnityEngine;
using UnityEngine.Events;


namespace THEBADDEST.Tweening2
{
    [DisallowMultipleComponent]
    public class TweenSequencer : MonoBehaviour
    {
        public enum PlayType
        {
            Forward,
            Backward
        }

        public enum AutoplayType
        {
            Awake,
            OnEnable,
            Nothing
        }
        
        [SerializeReference] 
        private TweenStepBase[] tweenSteps = Array.Empty<TweenStepBase>();
        public TweenStepBase[] TweenSteps => tweenSteps;

        [SerializeField] 
        private UpdateType updateType = UpdateType.Normal;
        [SerializeField] 
        private bool timeScaleIndependent = false;
        [SerializeField] 
        private AutoplayType autoplayMode = AutoplayType.Awake;
        [SerializeField] 
        protected bool startPaused;
        [SerializeField] 
        private float playbackSpeed = 1f;
        public float PlaybackSpeed => playbackSpeed;
        [SerializeField] 
        protected PlayType playType = PlayType.Forward;
        [SerializeField] 
        private int loops = 1;

        [SerializeField] 
        private LoopType loopType = LoopType.Restart;
        [SerializeField] 
        private bool autoKill = true;

        [SerializeField] 
        private UnityEvent onStartEvent = new UnityEvent();

        public UnityEvent OnStartEvent
        {
            get => onStartEvent;
            protected set => onStartEvent = value;
        }

        [SerializeField] 
        private UnityEvent onFinishedEvent = new UnityEvent();

        public UnityEvent OnFinishedEvent
        {
            get => onFinishedEvent;
            protected set => onFinishedEvent = value;
        }

        [SerializeField] 
        private UnityEvent onProgressEvent = new UnityEvent();
        public UnityEvent OnProgressEvent => onProgressEvent;

        private Sequence playingSequence;
        public ISequence PlayingSequence => playingSequence;
        private PlayType playTypeInternal = PlayType.Forward;
#if UNITY_EDITOR
        private bool requiresReset = false;
#endif

        public bool IsPlaying => playingSequence != null && playingSequence.IsActive() && playingSequence.IsPlaying();
        public bool IsPaused => playingSequence != null && playingSequence.IsActive() && !playingSequence.IsPlaying();

        protected virtual void Awake()
        {
            if (!Application.isPlaying)
                return;
            if (autoplayMode != AutoplayType.Awake)
                return;

            Autoplay();
        }

        protected virtual void OnEnable()
        {
            if (!Application.isPlaying)
                return;
            if (autoplayMode != AutoplayType.OnEnable)
                return;

            Autoplay();
        }

        private void Autoplay()
        {
            if (!Application.isPlaying)
                return;
            Play();
            if (startPaused)
                playingSequence.Pause();
        }

        protected virtual void OnDisable()
        {
            if (autoplayMode != AutoplayType.OnEnable)
                return;

            if (playingSequence == null)
                return;

            ClearPlayingSequence();
            // Reset the object to its initial state so that if it is re-enabled the start values are correct for
            // regenerating the Sequence.
            ResetToInitialState();
        }

        protected virtual void OnDestroy()
        {
            ClearPlayingSequence();
        }

        public virtual void Play()
        {
            Play(null);
        }

        public virtual void Play(Action onCompleteCallback)
        {
            if (!Application.isPlaying)
                return;
            playTypeInternal = playType;

            ClearPlayingSequence();

            onFinishedEvent.RemoveAllListeners();

            if (onCompleteCallback != null)
                onFinishedEvent.AddListener(onCompleteCallback.Invoke);

            playingSequence = (Sequence)GenerateSequence();
            if (playingSequence == null)
                return;

            switch (playTypeInternal)
            {
                case PlayType.Backward:
                    playingSequence.PlayBackwards();
                    break;

                case PlayType.Forward:
                    playingSequence.PlayForward();
                    break;

                default:
                    playingSequence.Play();
                    break;
            }
        }

        public virtual void PlayForward(bool resetFirst = true, Action onCompleteCallback = null)
        {
            if (!Application.isPlaying)
                return;
            if (playingSequence == null)
                Play();

            playTypeInternal = PlayType.Forward;
            onFinishedEvent.RemoveAllListeners();

            if (onCompleteCallback != null)
                onFinishedEvent.AddListener(onCompleteCallback.Invoke);

            if (resetFirst)
                SetProgress(0);

            playingSequence.PlayForward();
        }

        public virtual void PlayBackwards(bool completeFirst = true, Action onCompleteCallback = null)
        {
            if (!Application.isPlaying)
                return;
            if (playingSequence == null)
                Play();

            playTypeInternal = PlayType.Backward;
            onFinishedEvent.RemoveAllListeners();

            if (onCompleteCallback != null)
                onFinishedEvent.AddListener(onCompleteCallback.Invoke);

            if (completeFirst)
                SetProgress(1);

            playingSequence.PlayBackwards();
        }

        public virtual void SetTime(float seconds, bool andPlay = true)
        {
            if (!Application.isPlaying)
                return;
            if (playingSequence == null)
                Play();

            float duration = playingSequence.Duration();
            float finalProgress = Mathf.Clamp01(seconds / duration);
            SetProgress(finalProgress, andPlay);
        }

        public virtual void SetProgress(float targetProgress, bool andPlay = true)
        {
            if (!Application.isPlaying)
                return;
            targetProgress = Mathf.Clamp01(targetProgress);

            if (playingSequence == null)
                Play();

            if (playingSequence != null && playingSequence.duration > 0)
            {
                playingSequence.Goto(targetProgress * playingSequence.duration, andPlay);
            }
        }

        public virtual void TogglePause()
        {
            if (!Application.isPlaying || playingSequence == null)
                return;

            playingSequence.TogglePause();
        }

        public virtual void Pause()
        {
            if (!Application.isPlaying || !IsPlaying)
                return;

            playingSequence.Pause();
        }

        public virtual void Resume()
        {
            if (!Application.isPlaying || playingSequence == null)
                return;

            playingSequence.Play();
        }

        public virtual void Complete(bool withCallbacks = true)
        {
            if (!Application.isPlaying || playingSequence == null)
                return;

            playingSequence.Complete(withCallbacks);
        }

        public virtual void Rewind(bool includeDelay = true)
        {
            if (!Application.isPlaying || playingSequence == null)
                return;

            playingSequence.Rewind(includeDelay);
        }

        public virtual void Kill(bool complete = false)
        {
            if (!Application.isPlaying || !IsPlaying)
                return;

            playingSequence.Kill(complete);
        }

        public virtual IEnumerator PlayEnumerator()
        {
            if (!Application.isPlaying)
                yield break;
            Play();
            if (playingSequence != null)
                yield return playingSequence.WaitForCompletion();
        }

        public virtual ISequence GenerateSequence()
        {
            if (!Application.isPlaying)
                return null;
            Sequence sequence = (Sequence)TweenCore.Sequence();

            // Various edge cases exists with OnStart() and OnComplete(), some of which can be solved with OnRewind(),
            // but it still leaves callbacks unfired when reversing direction after natural completion of the animation.
            // Rather than using the in-built callbacks, we simply bookend the Sequence with AppendCallback to ensure
            // a Start and Finish callback is always fired.
            sequence.AppendCallback(() =>
            {
                if (playTypeInternal == PlayType.Forward)
                {
                    onStartEvent.Invoke();
                }
                else
                {
                    onFinishedEvent.Invoke();
                }
            });

            for (int i = 0; i < tweenSteps.Length; i++)
            {
                TweenStepBase tweenStepBase = tweenSteps[i];
                if (tweenStepBase != null)
                {
                    tweenStepBase.AddTweenToSequence(sequence);
                }
            }

            sequence.SetTarget(this);
            sequence.SetAutoKill(autoKill);
            sequence.SetUpdate(updateType, timeScaleIndependent);
            sequence.onUpdate = () => { onProgressEvent.Invoke(); };
            
            // See comment above regarding bookending via AppendCallback.
            sequence.AppendCallback(() =>
            {
                if (playTypeInternal == PlayType.Forward)
                {
                    onFinishedEvent.Invoke();
                }
                else
                {
                    onStartEvent.Invoke();
                }
            });

            int targetLoops = loops;
            if (targetLoops == 0) targetLoops = 1;

            if (!Application.isPlaying)
            {
                if (loops == -1)
                {
                    targetLoops = 10;
                    Debug.LogWarning("Infinity sequences on editor can cause issues, using 10 loops while on editor.");
                }
            }

            sequence.SetLoops(targetLoops, loopType);
            sequence.timeScale = playbackSpeed;
            return sequence;
        }

        public virtual void ResetToInitialState()
        {
            for (int i = tweenSteps.Length - 1; i >= 0; i--)
            {
                if (tweenSteps[i] != null)
                    tweenSteps[i].ResetToInitialState();
            }
        }

        public void ClearPlayingSequence()
        {
            if (playingSequence != null)
            {
                playingSequence.Kill(false);
            }
            playingSequence = null;
        }

        public void SetAutoplayMode(AutoplayType autoplayType)
        {
            autoplayMode = autoplayType;
        }

        public void SetPlayOnAwake(bool targetPlayOnAwake)
        {
        }

        public void SetPauseOnAwake(bool targetPauseOnAwake)
        {
            startPaused = targetPauseOnAwake;
        }

        public void SetTimeScaleIndependent(bool targetTimeScaleIndependent)
        {
            timeScaleIndependent = targetTimeScaleIndependent;
        }

        public void SetPlayType(PlayType targetPlayType)
        {
            playType = targetPlayType;
        }

        public void SetUpdateType(UpdateType targetUpdateType)
        {
            updateType = targetUpdateType;
        }

        public void SetAutoKill(bool targetAutoKill)
        {
            autoKill = targetAutoKill;
        }

        public void SetLoops(int targetLoops)
        {
            loops = targetLoops;
        }

#if UNITY_EDITOR
        // Unity Event Function called when component is added or reset.
        private void Reset()
        {
            loops = 1;
            requiresReset = true;
        }

        private void OnValidate()
        {
            if (loops == 0) loops = 1;
        }

        // Used by the CustomEditor so it knows when to reset to the defaults.
        public bool IsResetRequired()
        {
            return requiresReset;
        }

        // Called by the CustomEditor once the reset has been completed 
        public void ResetComplete()
        {
            requiresReset = false;
        }
#endif
        public bool TryGetStepAtIndex<T>(int index, out T result) where T : TweenStepBase
        {
            if (index < 0 || index > tweenSteps.Length - 1)
            {
                result = null;
                return false;
            }

            result = tweenSteps[index] as T;
            return result != null;
        }

        public void ReplaceTarget<T>(GameObject targetGameObject) where T : GameObjectTweenStep
        {
            for (int i = tweenSteps.Length - 1; i >= 0; i--)
            {
                TweenStepBase tweenStepBase = tweenSteps[i];
                if (tweenStepBase == null)
                    continue;

                if (tweenStepBase is not T gameObjectAnimationStep)
                    continue;

                gameObjectAnimationStep.SetTarget(targetGameObject);
            }
        }

        public void ReplaceTargets(params (GameObject original, GameObject target)[] replacements)
        {
            for (int i = 0; i < replacements.Length; i++)
            {
                (GameObject original, GameObject target) replacement = replacements[i];
                ReplaceTargets(replacement.original, replacement.target);
            }
        }

        public void ReplaceTargets(GameObject originalTarget, GameObject newTarget)
        {
            for (int i = tweenSteps.Length - 1; i >= 0; i--)
            {
                TweenStepBase tweenStepBase = tweenSteps[i];
                if (tweenStepBase == null)
                    continue;
                
                if(tweenStepBase is not GameObjectTweenStep gameObjectAnimationStep)
                    continue;

                if (gameObjectAnimationStep.Target == originalTarget)
                    gameObjectAnimationStep.SetTarget(newTarget);
            }
        }
    }
}
