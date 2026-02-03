using System.Collections;
using THEBADDEST.Tweening2.Core;

namespace THEBADDEST.Tweening2
{
    /// <summary>
    /// Public API for sequence operations (build, config, control, query).
    /// </summary>
    public interface ISequence
    {
        // Build
        ISequence Append(ITweener tweener);
        ISequence Join(ITweener tweener);
        ISequence Insert(ITweener tweener, float atPosition);
        ISequence AppendInterval(float interval);
        ISequence AppendCallback(TweenCallback callback);

        // Config
        ISequence SetDelay(float delay);
        ISequence SetLoops(int loops, LoopType loopType = LoopType.Restart);
        ISequence SetUpdate(UpdateType updateType, bool isIndependentUpdate = false);
        ISequence SetTarget(object target);
        ISequence SetAutoKill(bool autoKill);
        ISequence SetTimeScale(float timeScale);

        // Control
        ISequence Goto(float toPosition, bool andPlay = false);
        ISequence Play();
        ISequence PlayForward();
        ISequence PlayBackwards();
        ISequence Pause();
        ISequence TogglePause();
        ISequence Rewind(bool includeDelay = true);
        ISequence Complete(bool withCallbacks = true);
        void Kill(bool complete = false);

        // Query
        float Duration();
        float ElapsedPercentage();
        bool IsActive();
        bool IsPlaying();
        bool IsBackwards();

        // Coroutine
        IEnumerator WaitForCompletion();
    }
}
