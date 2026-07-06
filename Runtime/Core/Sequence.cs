using System.Collections.Generic;
using UnityEngine;
using THEBADDEST.Tweening2;
using THEBADDEST.Tweening2.Core.Easing;

namespace THEBADDEST.Tweening2.Core
{
    /// <summary>
    /// Controls other tweens as a group
    /// </summary>
    public sealed class Sequence : Tween, ISequence
    {
        // SETUP DATA ////////////////////////////////////////////////

        internal readonly List<Tween> sequencedTweens = new List<Tween>(); // Only Tweens (used for despawning and validation)
        readonly List<ABSSequentiable> _sequencedObjs = new List<ABSSequentiable>(); // Tweens plus SequenceCallbacks
        internal float lastTweenInsertTime; // Used to insert a tween at the position of the previous one

        #region Constructor

        internal Sequence()
        {
            tweenType = TweenType.Sequence;
            Reset();
        }

        #endregion

        #region Creation Methods

        internal static Sequence Prepend(Sequence inSequence, Tween t)
        {
            if (t.loops == -1)
            {
                t.loops = int.MaxValue;
                TweenLog.LogWarning("Infinite loops aren't allowed inside a Sequence (only on the Sequence itself) and will be changed to int.MaxValue");
            }
            float tFullTime = t.delay + (t.duration * t.loops);
            inSequence.duration += tFullTime;
            int len = inSequence._sequencedObjs.Count;
            for (int i = 0; i < len; ++i)
            {
                ABSSequentiable sequentiable = inSequence._sequencedObjs[i];
                sequentiable.sequencedPosition += tFullTime;
                sequentiable.sequencedEndPosition += tFullTime;
            }

            return Insert(inSequence, t, 0);
        }

        internal static Sequence Insert(Sequence inSequence, Tween t, float atPosition)
        {
            TweenManager.AddActiveTweenToSequence(t);

            // If t has a delay add it as an interval
            atPosition += t.delay;
            inSequence.lastTweenInsertTime = atPosition;

            t.isSequenced = t.creationLocked = true;
            t.sequenceParent = inSequence;
            if (t.loops == -1)
            {
                t.loops = int.MaxValue;
                TweenLog.LogWarning("Infinite loops aren't allowed inside a Sequence (only on the Sequence itself) and will be changed to int.MaxValue");
            }
            float tFullTime = t.duration * t.loops;
            t.autoKill = false;
            t.delay = t.elapsedDelay = 0;
            t.delayComplete = true;
            if (t.isSpeedBased)
            {
                t.isSpeedBased = false;
                TweenLog.LogWarning("SpeedBased tweens are not allowed inside a Sequence: interpreting speed as duration");
            }
            t.sequencedPosition = atPosition;
            t.sequencedEndPosition = atPosition + tFullTime;

            if (t.sequencedEndPosition > inSequence.duration) inSequence.duration = t.sequencedEndPosition;
            inSequence._sequencedObjs.Add(t);
            inSequence.sequencedTweens.Add(t);

            return inSequence;
        }

        internal static Sequence AppendInterval(Sequence inSequence, float interval)
        {
            inSequence.lastTweenInsertTime = inSequence.duration;
            inSequence.duration += interval;
            return inSequence;
        }

        internal static Sequence PrependInterval(Sequence inSequence, float interval)
        {
            inSequence.lastTweenInsertTime = 0;
            inSequence.duration += interval;
            int len = inSequence._sequencedObjs.Count;
            for (int i = 0; i < len; ++i)
            {
                ABSSequentiable sequentiable = inSequence._sequencedObjs[i];
                sequentiable.sequencedPosition += interval;
                sequentiable.sequencedEndPosition += interval;
            }

            return inSequence;
        }

        internal static Sequence InsertCallback(Sequence inSequence, TweenCallback callback, float atPosition)
        {
            inSequence.lastTweenInsertTime = atPosition;
            SequenceCallback c = new SequenceCallback(atPosition, callback);
            c.sequencedPosition = c.sequencedEndPosition = atPosition;
            inSequence._sequencedObjs.Add(c);
            if (inSequence.duration < atPosition) inSequence.duration = atPosition;
            return inSequence;
        }

        #endregion

        // NOTE: up to v1.2.340 Sequences didn't implement this method and delays were always included as prepended intervals
        internal override float UpdateDelay(float elapsed)
        {
            float tweenDelay = delay;
            if (elapsed > tweenDelay)
            {
                // Delay complete
                elapsedDelay = tweenDelay;
                delayComplete = true;
                return elapsed - tweenDelay;
            }
            elapsedDelay = elapsed;
            return 0;
        }

        internal override void Reset()
        {
            base.Reset();

            sequencedTweens.Clear();
            _sequencedObjs.Clear();
            lastTweenInsertTime = 0;
        }

        // Called by TweenManager.Validate.
        // Returns TRUE if the tween is valid
        internal override bool Validate()
        {
            int len = sequencedTweens.Count;
            for (int i = len - 1; i > -1; --i)
            {
                if (!sequencedTweens[i].Validate())
                {
                    // Remove invalid tween
                    sequencedTweens.RemoveAt(i);
                }
            }
            return sequencedTweens.Count > 0;
        }

        // Called the moment the tween starts.
        // Returns TRUE in case of success, FALSE if there are missing references and the tween needs to be killed
        internal override bool Startup()
        {
            return Startup(this);
        }

        internal static bool Startup(Sequence s)
        {
            if (s._sequencedObjs.Count == 0 && s.sequencedTweens.Count == 0 && !IsAnyCallbackSet(s))
                return false;

            s.startupDone = true;
            s.fullDuration = s.loops > -1 ? s.duration * s.loops : float.PositiveInfinity;
            StableSortSequencedObjs(s._sequencedObjs);
            return true;
        }

        static bool IsAnyCallbackSet(Sequence s)
        {
            return s.onComplete != null || s.onKill != null || s.onPause != null || s.onPlay != null
                || s.onRewind != null || s.onStart != null || s.onStepComplete != null || s.onUpdate != null;
        }

        static void StableSortSequencedObjs(List<ABSSequentiable> list)
        {
            int len = list.Count;
            for (int i = 1; i < len; i++)
            {
                int j = i;
                ABSSequentiable temp = list[i];
                while (j > 0 && list[j - 1].sequencedPosition > temp.sequencedPosition)
                {
                    list[j] = list[j - 1];
                    j = j - 1;
                }
                list[j] = temp;
            }
        }

        // Applies the tween set by DoGoto.
        // Returns TRUE if the tween needs to be killed
        internal override bool ApplyTween(float prevPosition, int prevCompletedLoops, int newCompletedSteps, bool useInversePosition, UpdateMode updateMode, UpdateNotice updateNotice)
        {
            return ApplyTween(this, prevPosition, prevCompletedLoops, newCompletedSteps, useInversePosition, updateMode);
        }

        // Simplified version of ApplyTween for Sequence
        internal static bool ApplyTween(Sequence s, float prevPosition, int prevCompletedLoops, int newCompletedSteps, bool useInversePosition, UpdateMode updateMode)
        {
            float newPos = s.position;
            if (s.easeType != EaseType.Linear)
            {
                newPos = s.duration * EaseManager.Evaluate(s.easeType, s.customEase, newPos, s.duration, s.easeOvershootOrAmplitude, s.easePeriod);
            }

            bool isBackwardsUpdate = newPos < prevPosition;

            if (isBackwardsUpdate)
                ApplyInternalCycleBackwards(s, prevPosition, newPos, updateMode);
            else
                ApplyInternalCycleForward(s, prevPosition, newPos, updateMode);

            return false;
        }

        static void ApplyInternalCycleForward(Sequence s, float fromPos, float toPos, UpdateMode updateMode)
        {
            int len = s._sequencedObjs.Count;
            for (int i = 0; i < len; ++i)
            {
                // Nested sequences have active=false (removed from active list) but are driven by parent - must still run
                if (!s.active && !s.isSequenced) return;
                ABSSequentiable sequentiable = s._sequencedObjs[i];
                if (sequentiable.sequencedPosition > toPos || sequentiable.sequencedEndPosition < fromPos) continue;

                if (sequentiable.tweenType == TweenType.Callback)
                {
                    if (updateMode == UpdateMode.Update && fromPos < sequentiable.sequencedPosition && toPos >= sequentiable.sequencedPosition)
                        Tween.OnTweenCallback(sequentiable.onStart, s);
                }
                else
                {
                    Tween t = (Tween)sequentiable;
                    if (!t.startupDone && !t.Startup()) continue;

                    float localTime = toPos - sequentiable.sequencedPosition;
                    if (localTime < 0) localTime = 0;
                    int toCompletedLoops;
                    float toPosition;
                    if (t.duration <= 0)
                    {
                        toCompletedLoops = 0;
                        toPosition = 0;
                    }
                    else
                    {
                        toCompletedLoops = t.loops == -1 ? (int)(localTime / t.duration) : Mathf.Min((int)(localTime / t.duration), t.loops);
                        toPosition = localTime - toCompletedLoops * t.duration;
                        if (t.loops != -1 && toCompletedLoops >= t.loops)
                        {
                            toCompletedLoops = t.loops;
                            toPosition = t.duration;
                        }
                    }
                    t.isBackwards = false;
                    if (Tween.Goto(t, toPosition, toCompletedLoops, updateMode))
                    {
                        s._sequencedObjs.RemoveAt(i);
                        s.sequencedTweens.Remove(t);
                        --i; --len;
                    }
                }
            }
        }

        static void ApplyInternalCycleBackwards(Sequence s, float fromPos, float toPos, UpdateMode updateMode)
        {
            int len = s._sequencedObjs.Count;
            for (int i = len - 1; i >= 0; --i)
            {
                // Nested sequences have active=false (removed from active list) but are driven by parent - must still run
                if (!s.active && !s.isSequenced) return;
                ABSSequentiable sequentiable = s._sequencedObjs[i];
                if (sequentiable.sequencedEndPosition < toPos || sequentiable.sequencedPosition > fromPos) continue;

                if (sequentiable.tweenType == TweenType.Callback)
                {
                    if (updateMode == UpdateMode.Update && fromPos >= sequentiable.sequencedPosition && toPos < sequentiable.sequencedPosition)
                        Tween.OnTweenCallback(sequentiable.onStart, s);
                }
                else
                {
                    Tween t = (Tween)sequentiable;
                    if (!t.startupDone) continue;

                    float localTime = toPos - sequentiable.sequencedPosition;
                    if (localTime < 0) localTime = 0;
                    int toCompletedLoops;
                    float toPosition;
                    if (t.duration <= 0)
                    {
                        toCompletedLoops = 0;
                        toPosition = 0;
                    }
                    else
                    {
                        toCompletedLoops = t.loops == -1 ? (int)(localTime / t.duration) : Mathf.Min((int)(localTime / t.duration), t.loops);
                        toPosition = localTime - toCompletedLoops * t.duration;
                        if (t.loops != -1 && toCompletedLoops >= t.loops)
                        {
                            toCompletedLoops = t.loops;
                            toPosition = t.duration;
                        }
                    }
                    t.isBackwards = true;
                    if (Tween.Goto(t, toPosition, toCompletedLoops, updateMode))
                    {
                        s._sequencedObjs.RemoveAt(i);
                        s.sequencedTweens.Remove(t);
                    }
                }
            }
        }

        #region ISequence

        public ISequence Append(ITweener tweener)
        {
            if (tweener == null) return this;
            Sequence.Insert(this, tweener.Tween, duration);
            return this;
        }

        public ISequence Join(ITweener tweener)
        {
            if (tweener == null) return this;
            Sequence.Insert(this, tweener.Tween, lastTweenInsertTime);
            return this;
        }

        public ISequence Insert(ITweener tweener, float atPosition)
        {
            if (tweener == null) return this;
            Sequence.Insert(this, tweener.Tween, atPosition);
            return this;
        }

        public ISequence AppendInterval(float interval)
        {
            Sequence.AppendInterval(this, interval);
            return this;
        }

        public ISequence AppendCallback(TweenCallback callback)
        {
            if (callback == null) return this;
            Sequence.InsertCallback(this, callback, duration);
            return this;
        }

        public ISequence SetDelay(float delay)
        {
            this.delay = delay;
            return this;
        }

        public ISequence SetLoops(int loops, LoopType loopType = LoopType.Restart)
        {
            this.loops = loops;
            this.loopType = loopType;
            return this;
        }

        public ISequence SetUpdate(UpdateType updateType, bool isIndependentUpdate = false)
        {
            TweenManager.SetUpdateType(this, updateType, isIndependentUpdate);
            return this;
        }

        public ISequence SetTarget(object target)
        {
            this.target = target;
            return this;
        }

        public ISequence SetAutoKill(bool autoKill)
        {
            this.autoKill = autoKill;
            return this;
        }

        public ISequence SetTimeScale(float timeScale)
        {
            this.timeScale = timeScale;
            return this;
        }

        public ISequence Goto(float toPosition, bool andPlay = false)
        {
            TweenManager.Goto(this, toPosition, andPlay);
            return this;
        }

        public ISequence Play()
        {
            TweenManager.Play(this);
            return this;
        }

        public ISequence PlayForward()
        {
            isBackwards = false;
            TweenManager.Play(this);
            return this;
        }

        public ISequence PlayBackwards()
        {
            isBackwards = true;
            TweenManager.Play(this);
            return this;
        }

        public ISequence Pause()
        {
            TweenManager.Pause(this);
            return this;
        }

        public ISequence TogglePause()
        {
            if (isPlaying)
                TweenManager.Pause(this);
            else
                TweenManager.Play(this);
            return this;
        }

        public ISequence Rewind(bool includeDelay = true)
        {
            Goto(0, false);
            if (includeDelay)
            {
                elapsedDelay = 0;
                delayComplete = false;
            }
            return this;
        }

        public ISequence Complete(bool withCallbacks = true)
        {
            Goto(duration, false);
            if (withCallbacks && onComplete != null)
                Tween.OnTweenCallback(onComplete, this);
            return this;
        }

        public void Kill(bool complete = false)
        {
            if (complete)
                Complete(true);
            TweenManager.MarkForKilling(this);
        }

        public float Duration()
        {
            return duration;
        }

        public float ElapsedPercentage()
        {
            if (duration <= 0) return 0f;
            return position / duration;
        }

        public bool IsActive()
        {
            return active;
        }

        public bool IsPlaying()
        {
            return isPlaying;
        }

        public bool IsBackwards()
        {
            return isBackwards;
        }

        public System.Collections.IEnumerator WaitForCompletion()
        {
            while (active && isPlaying && !isComplete)
                yield return null;
        }

        #endregion
    }

    // Internal class for Sequence callbacks
    internal class SequenceCallback : ABSSequentiable
    {
        internal SequenceCallback(float sequencedPosition, TweenCallback callback)
        {
            this.sequencedPosition = sequencedPosition;
            this.onStart = callback;
            tweenType = TweenType.Callback;
        }
    }
}
