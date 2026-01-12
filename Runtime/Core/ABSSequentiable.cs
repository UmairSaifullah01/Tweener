namespace THEBADDEST.Tweening2.Core
{
    public abstract class ABSSequentiable
    {
        internal TweenType tweenType;
        internal float sequencedPosition; // position in Sequence
        internal float sequencedEndPosition; // end position in Sequence

        /// <summary>Called the first time the tween is set in a playing state, after any eventual delay</summary>
        internal TweenCallback onStart; // Used also by SequenceCallback as main callback
    }
}
