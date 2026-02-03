using THEBADDEST.Tweening2.Core;

namespace THEBADDEST.Tweening2
{
    /// <summary>
    /// Extension methods for Sequence to provide a DOTween-like API
    /// </summary>
    public static class SequenceExtensions
    {
        /// <summary>Appends the given tween to the end of the sequence</summary>
        public static Sequence Append(this Sequence sequence, Tween tween)
        {
            if (sequence == null || tween == null) return sequence;
            Sequence.Insert(sequence, tween, sequence.duration);
            return sequence;
        }

        /// <summary>Inserts the given tween at the same position as the last tween added to the sequence (for joining)</summary>
        public static Sequence Join(this Sequence sequence, Tween tween)
        {
            if (sequence == null || tween == null) return sequence;
            Sequence.Insert(sequence, tween, sequence.lastTweenInsertTime);
            return sequence;
        }

        /// <summary>Inserts the given tween at the specified time position in the sequence</summary>
        public static Sequence Insert(this Sequence sequence, Tween tween, float atPosition)
        {
            if (sequence == null || tween == null) return sequence;
            Sequence.Insert(sequence, tween, atPosition);
            return sequence;
        }

        /// <summary>Appends an interval (empty space) to the sequence</summary>
        public static Sequence AppendInterval(this Sequence sequence, float interval)
        {
            if (sequence == null) return sequence;
            Sequence.AppendInterval(sequence, interval);
            return sequence;
        }

        /// <summary>Appends a callback to the sequence</summary>
        public static Sequence AppendCallback(this Sequence sequence, TweenCallback callback)
        {
            if (sequence == null || callback == null) return sequence;
            Sequence.InsertCallback(sequence, callback, sequence.duration);
            return sequence;
        }

        /// <summary>Sets the delay of the sequence</summary>
        public static Sequence SetDelay(this Sequence sequence, float delay)
        {
            if (sequence == null) return sequence;
            sequence.delay = delay;
            return sequence;
        }

        /// <summary>Sets the loops of the sequence</summary>
        public static Sequence SetLoops(this Sequence sequence, int loops, LoopType loopType = LoopType.Restart)
        {
            if (sequence == null) return sequence;
            sequence.loops = loops;
            sequence.loopType = loopType;
            return sequence;
        }

        /// <summary>Sets the update type of the sequence</summary>
        public static Sequence SetUpdate(this Sequence sequence, UpdateType updateType, bool isIndependentUpdate = false)
        {
            if (sequence == null) return sequence;
            TweenManager.SetUpdateType(sequence, updateType, isIndependentUpdate);
            return sequence;
        }

        /// <summary>Sets the target of the sequence (for filtering)</summary>
        public static Sequence SetTarget(this Sequence sequence, object target)
        {
            if (sequence == null) return sequence;
            sequence.target = target;
            return sequence;
        }

        /// <summary>Sets the auto kill behavior of the sequence</summary>
        public static Sequence SetAutoKill(this Sequence sequence, bool autoKill)
        {
            if (sequence == null) return sequence;
            sequence.autoKill = autoKill;
            return sequence;
        }

        /// <summary>Sets the time scale of the sequence</summary>
        public static Sequence SetTimeScale(this Sequence sequence, float timeScale)
        {
            if (sequence == null) return sequence;
            sequence.timeScale = timeScale;
            return sequence;
        }

        /// <summary>Goes to the given position in the sequence</summary>
        public static Sequence Goto(this Sequence sequence, float toPosition, bool andPlay = false)
        {
            if (sequence == null) return sequence;
            TweenManager.Goto(sequence, toPosition, andPlay);
            return sequence;
        }

        /// <summary>Returns the elapsed time as a percentage (0-1)</summary>
        public static float ElapsedPercentage(this Sequence sequence)
        {
            if (sequence == null || sequence.duration <= 0) return 0f;
            return sequence.position / sequence.duration;
        }

        /// <summary>Returns the total duration of the sequence</summary>
        public static float Duration(this Sequence sequence)
        {
            if (sequence == null) return 0f;
            return sequence.duration;
        }

        /// <summary>Returns TRUE if the sequence is active</summary>
        public static bool IsActive(this Sequence sequence)
        {
            return sequence != null && sequence.active;
        }

        /// <summary>Returns TRUE if the sequence is playing</summary>
        public static bool IsPlaying(this Sequence sequence)
        {
            return sequence != null && sequence.isPlaying;
        }

        /// <summary>Returns TRUE if the sequence is playing backwards</summary>
        public static bool IsBackwards(this Sequence sequence)
        {
            return sequence != null && sequence.isBackwards;
        }

        /// <summary>Plays the sequence</summary>
        public static Sequence Play(this Sequence sequence)
        {
            if (sequence == null) return sequence;
            TweenManager.Play(sequence);
            return sequence;
        }

        /// <summary>Plays the sequence forward</summary>
        public static Sequence PlayForward(this Sequence sequence)
        {
            if (sequence == null) return sequence;
            sequence.isBackwards = false;
            TweenManager.Play(sequence);
            return sequence;
        }

        /// <summary>Plays the sequence backwards</summary>
        public static Sequence PlayBackwards(this Sequence sequence)
        {
            if (sequence == null) return sequence;
            sequence.isBackwards = true;
            TweenManager.Play(sequence);
            return sequence;
        }

        /// <summary>Pauses the sequence</summary>
        public static Sequence Pause(this Sequence sequence)
        {
            if (sequence == null) return sequence;
            TweenManager.Pause(sequence);
            return sequence;
        }

        /// <summary>Toggles the pause state of the sequence</summary>
        public static Sequence TogglePause(this Sequence sequence)
        {
            if (sequence == null) return sequence;
            if (sequence.isPlaying)
                TweenManager.Pause(sequence);
            else
                TweenManager.Play(sequence);
            return sequence;
        }

        /// <summary>Rewinds the sequence</summary>
        public static Sequence Rewind(this Sequence sequence, bool includeDelay = true)
        {
            if (sequence == null) return sequence;
            sequence.Goto(0, false);
            if (includeDelay)
            {
                sequence.elapsedDelay = 0;
                sequence.delayComplete = false;
            }
            return sequence;
        }

        /// <summary>Completes the sequence</summary>
        public static Sequence Complete(this Sequence sequence, bool withCallbacks = true)
        {
            if (sequence == null) return sequence;
            sequence.Goto(sequence.duration, false);
            if (withCallbacks && sequence.onComplete != null)
            {
                Tween.OnTweenCallback(sequence.onComplete, sequence);
            }
            return sequence;
        }

        /// <summary>Kills the sequence</summary>
        public static void Kill(this Sequence sequence, bool complete = false)
        {
            if (sequence == null) return;
            if (complete)
            {
                sequence.Complete(true);
            }
            TweenManager.MarkForKilling(sequence);
        }

        /// <summary>Waits for the sequence to complete (coroutine-friendly)</summary>
        public static System.Collections.IEnumerator WaitForCompletion(this Sequence sequence)
        {
            if (sequence == null) yield break;
            while (sequence.active && sequence.isPlaying && !sequence.isComplete)
            {
                yield return null;
            }
        }
    }
}
