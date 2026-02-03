using THEBADDEST.Tweening2.Core;

namespace THEBADDEST.Tweening2
{
    /// <summary>
    /// Extension methods for ISequence to provide a DOTween-like API.
    /// Methods that only need the interface are implemented on Sequence; these extensions provide overloads that take Tween or need internal access.
    /// </summary>
    public static class SequenceExtensions
    {
        /// <summary>Appends the given tween to the end of the sequence</summary>
        public static ISequence Append(this ISequence sequence, Tween tween)
        {
            if (sequence == null || tween == null) return sequence;
            var s = sequence as Sequence;
            if (s == null) return sequence;
            Sequence.Insert(s, tween, s.duration);
            return sequence;
        }

        /// <summary>Appends the given tweener to the end of the sequence</summary>
        public static ISequence Append(this ISequence sequence, ITweener tweener)
        {
            if (sequence == null || tweener == null) return sequence;
            return sequence.Append(tweener.Tween);
        }

        /// <summary>Appends the given nested sequence to the end of the sequence</summary>
        public static ISequence Append(this ISequence sequence, ISequence nestedSequence)
        {
            if (sequence == null || nestedSequence == null) return sequence;
            var s = sequence as Sequence;
            var nested = nestedSequence as Sequence;
            if (s == null || nested == null) return sequence;
            Sequence.Insert(s, nested, s.duration);
            return sequence;
        }

        /// <summary>Inserts the given tween at the same position as the last tween added to the sequence (for joining)</summary>
        public static ISequence Join(this ISequence sequence, Tween tween)
        {
            if (sequence == null || tween == null) return sequence;
            var s = sequence as Sequence;
            if (s == null) return sequence;
            Sequence.Insert(s, tween, s.lastTweenInsertTime);
            return sequence;
        }

        /// <summary>Inserts the given tweener at the same position as the last tween added to the sequence (for joining)</summary>
        public static ISequence Join(this ISequence sequence, ITweener tweener)
        {
            if (sequence == null || tweener == null) return sequence;
            return sequence.Join(tweener.Tween);
        }

        /// <summary>Inserts the given nested sequence at the same position as the last tween added (for joining)</summary>
        public static ISequence Join(this ISequence sequence, ISequence nestedSequence)
        {
            if (sequence == null || nestedSequence == null) return sequence;
            var s = sequence as Sequence;
            var nested = nestedSequence as Sequence;
            if (s == null || nested == null) return sequence;
            Sequence.Insert(s, nested, s.lastTweenInsertTime);
            return sequence;
        }

        /// <summary>Inserts the given tween at the specified time position in the sequence</summary>
        public static ISequence Insert(this ISequence sequence, Tween tween, float atPosition)
        {
            if (sequence == null || tween == null) return sequence;
            var s = sequence as Sequence;
            if (s == null) return sequence;
            Sequence.Insert(s, tween, atPosition);
            return sequence;
        }

        /// <summary>Inserts the given tweener at the specified time position in the sequence</summary>
        public static ISequence Insert(this ISequence sequence, ITweener tweener, float atPosition)
        {
            if (sequence == null || tweener == null) return sequence;
            return sequence.Insert(tweener.Tween, atPosition);
        }

        /// <summary>Inserts the given nested sequence at the specified time position in the sequence</summary>
        public static ISequence Insert(this ISequence sequence, ISequence nestedSequence, float atPosition)
        {
            if (sequence == null || nestedSequence == null) return sequence;
            var s = sequence as Sequence;
            var nested = nestedSequence as Sequence;
            if (s == null || nested == null) return sequence;
            Sequence.Insert(s, nested, atPosition);
            return sequence;
        }

        /// <summary>Appends an interval (empty space) to the sequence</summary>
        public static ISequence AppendInterval(this ISequence sequence, float interval)
        {
            if (sequence == null) return sequence;
            var s = sequence as Sequence;
            if (s == null) return sequence;
            Sequence.AppendInterval(s, interval);
            return sequence;
        }

        /// <summary>Appends a callback to the sequence</summary>
        public static ISequence AppendCallback(this ISequence sequence, TweenCallback callback)
        {
            if (sequence == null || callback == null) return sequence;
            var s = sequence as Sequence;
            if (s == null) return sequence;
            Sequence.InsertCallback(s, callback, s.duration);
            return sequence;
        }

        /// <summary>Returns the elapsed time as a percentage (0-1)</summary>
        public static float ElapsedPercentage(this ISequence sequence)
        {
            if (sequence == null) return 0f;
            return sequence.ElapsedPercentage();
        }

        /// <summary>Returns the total duration of the sequence</summary>
        public static float Duration(this ISequence sequence)
        {
            if (sequence == null) return 0f;
            return sequence.Duration();
        }

        /// <summary>Returns TRUE if the sequence is active</summary>
        public static bool IsActive(this ISequence sequence)
        {
            return sequence != null && sequence.IsActive();
        }

        /// <summary>Returns TRUE if the sequence is playing</summary>
        public static bool IsPlaying(this ISequence sequence)
        {
            return sequence != null && sequence.IsPlaying();
        }

        /// <summary>Returns TRUE if the sequence is playing backwards</summary>
        public static bool IsBackwards(this ISequence sequence)
        {
            return sequence != null && sequence.IsBackwards();
        }
    }
}
