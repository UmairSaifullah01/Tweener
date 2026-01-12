using System;
using UnityEngine;

namespace THEBADDEST.Tweening2.Core
{
    /// <summary>
    /// Indicates either a Tweener or a Sequence
    /// </summary>
    public abstract class Tween : ABSSequentiable
    {
        // OPTIONS ///////////////////////////////////////////////////

        // Modifiable at runtime
        /// <summary>TimeScale for the tween</summary>
        public float timeScale = 1f;
        /// <summary>If TRUE the tween will play backwards</summary>
        public bool isBackwards;
        /// <summary>If TRUE the tween is completely inverted but without playing it backwards</summary>
        internal bool isInverted;
        /// <summary>Object ID (usable for filtering)</summary>
        public object id;
        /// <summary>String ID (usable for filtering)</summary>
        public string stringId;
        /// <summary>Int ID (usable for filtering). Default is -999</summary>
        public int intId = -999;
        /// <summary>Tween target (usable for filtering)</summary>
        public object target;
        
        // Update type and eventual independence
        internal UpdateType updateType;
        internal bool isIndependentUpdate;
        
        /// <summary>Called when the tween is set in a playing state, after any eventual delay</summary>
        public TweenCallback onPlay;
        /// <summary>Called when the tween state changes from playing to paused</summary>
        public TweenCallback onPause;
        /// <summary>Called when the tween is rewinded</summary>
        public TweenCallback onRewind;
        /// <summary>Called each time the tween updates</summary>
        public TweenCallback onUpdate;
        /// <summary>Called the moment the tween completes one loop cycle</summary>
        public TweenCallback onStepComplete;
        /// <summary>Called the moment the tween reaches completion (loops included)</summary>
        public TweenCallback onComplete;
        /// <summary>Called the moment the tween is killed</summary>
        public TweenCallback onKill;
        /// <summary>Called when a path tween's current waypoint changes</summary>
        public TweenCallback<int> onWaypointChange;
        
        // Fixed after creation
        internal bool isFrom;
        internal bool isBlendable;
        internal bool isRecyclable;
        internal bool isSpeedBased;
        internal bool autoKill;
        internal float duration;
        internal int loops;
        internal LoopType loopType;
        internal float delay;
        /// <summary>Tweeners-only (ignored by Sequences), returns TRUE if the tween was set as relative</summary>
        public bool isRelative { get; internal set; }
        internal EaseType easeType;
        internal EaseFunction customEase;
        public float easeOvershootOrAmplitude = 1.70158f;
        public float easePeriod = 0f;

        // SPECIAL DEBUG DATA
        public string debugTargetId;

        // SETUP DATA
        internal Type typeofT1;
        internal Type typeofT2;
        internal Type typeofTPlugOptions;
        /// <summary>FALSE when tween is (or should be) despawned - set only by TweenManager</summary>
        public bool active { get; internal set; }
        internal bool isSequenced;
        internal Sequence sequenceParent;
        internal int activeId = -1;
        internal SpecialStartupMode specialStartupMode;

        // PLAY DATA
        /// <summary>Gets and sets the time position (loops included, delays excluded) of the tween</summary>
        public float fullPosition { get { return this.Elapsed(true); } set { this.Goto(value, this.isPlaying); } }
        /// <summary>Returns TRUE if the tween is set to loop (either a set number of times or infinitely)</summary>
        public bool hasLoops { get { return loops == -1 || loops > 1; } }

        internal bool creationLocked;
        internal bool startupDone;
        /// <summary>TRUE after the tween was set in a play state at least once, AFTER any delay is elapsed</summary>
        public bool playedOnce { get; private set; }
        /// <summary>Time position within a single loop cycle</summary>
        public float position { get; internal set; }
        internal float fullDuration;
        internal int completedLoops;
        internal bool isPlaying;
        internal bool isComplete;
        internal float elapsedDelay;
        internal bool delayComplete = true;
        
        internal int miscInt = -1;

        #region Abstracts + Overrideables

        internal virtual void Reset()
        {
            timeScale = 1;
            isBackwards = false;
            id = null;
            stringId = null;
            intId = -999;
            isIndependentUpdate = false;
            onStart = onPlay = onRewind = onUpdate = onComplete = onStepComplete = onKill = null;
            onWaypointChange = null;

            debugTargetId = null;

            target = null;
            isFrom = false;
            isBlendable = false;
            isSpeedBased = false;
            duration = 0;
            loops = 1;
            delay = 0;
            isRelative = false;
            customEase = null;
            isSequenced = false;
            sequenceParent = null;
            specialStartupMode = SpecialStartupMode.None;
            creationLocked = startupDone = playedOnce = false;
            position = fullDuration = completedLoops = 0;
            isPlaying = isComplete = false;
            elapsedDelay = 0;
            delayComplete = true;

            miscInt = -1;
        }

        // Called by TweenManager.Validate.
        // Returns TRUE if the tween is valid
        internal abstract bool Validate();

        // Called by TweenManager in case a tween has a delay that needs to be updated.
        // Returns the eventual time in excess compared to the tween's delay time.
        internal virtual float UpdateDelay(float elapsed)
        {
            float tweenDelay = delay;
            if (elapsed > tweenDelay)
            {
                elapsedDelay = tweenDelay;
                delayComplete = true;
                return elapsed - tweenDelay;
            }
            elapsedDelay = elapsed;
            return 0;
        }

        // Called the moment the tween starts.
        // Returns TRUE in case of success, FALSE if there are missing references and the tween needs to be killed
        internal abstract bool Startup();

        // Applies the tween set by DoGoto.
        // Returns TRUE if the tween needs to be killed.
        internal abstract bool ApplyTween(float prevPosition, int prevCompletedLoops, int newCompletedSteps, bool useInversePosition, UpdateMode updateMode, UpdateNotice updateNotice);

        #endregion

        #region Goto and Callbacks

        // Instead of advancing the tween from the previous position each time,
        // uses the given position to calculate running time since startup, and places the tween there like a Goto.
        internal static bool DoGoto(Tween t, float toPosition, int toCompletedLoops, UpdateMode updateMode)
        {
            // Startup
            if (!t.startupDone)
            {
                if (!t.Startup()) return true;
            }
            
            // OnStart and first OnPlay callbacks
            if (!t.playedOnce && updateMode == UpdateMode.Update)
            {
                t.playedOnce = true;
                if (t.onStart != null)
                {
                    OnTweenCallback(t.onStart, t);
                    if (!t.active) return true;
                }
                if (t.onPlay != null)
                {
                    OnTweenCallback(t.onPlay, t);
                    if (!t.active) return true;
                }
            }

            float prevPosition = t.position;
            int prevCompletedLoops = t.completedLoops;
            t.completedLoops = toCompletedLoops;
            bool wasRewinded = t.position <= 0 && prevCompletedLoops <= 0;
            bool wasComplete = t.isComplete;
            
            // Determine if it will be complete after update
            if (t.loops != -1) t.isComplete = t.completedLoops == t.loops;
            
            // Calculate newCompletedSteps
            int newCompletedSteps = 0;
            if (updateMode == UpdateMode.Update)
            {
                if (t.isBackwards)
                {
                    newCompletedSteps = t.completedLoops < prevCompletedLoops ? prevCompletedLoops - t.completedLoops : (toPosition <= 0 && !wasRewinded ? 1 : 0);
                    if (wasComplete) newCompletedSteps--;
                }
                else newCompletedSteps = t.completedLoops > prevCompletedLoops ? t.completedLoops - prevCompletedLoops : 0;
            }
            else if (t.tweenType == TweenType.Sequence)
            {
                newCompletedSteps = prevCompletedLoops - toCompletedLoops;
                if (newCompletedSteps < 0) newCompletedSteps = -newCompletedSteps;
            }

            // Set position
            t.position = toPosition;
            if (t.position > t.duration) t.position = t.duration;
            else if (t.position <= 0)
            {
                if (t.completedLoops > 0 || t.isComplete) t.position = t.duration;
                else t.position = 0;
            }
            
            // Set playing state after update
            bool wasPlaying = t.isPlaying;
            if (t.isPlaying)
            {
                if (!t.isBackwards) t.isPlaying = !t.isComplete;
                else t.isPlaying = !(t.completedLoops == 0 && t.position <= 0);
            }

            // updatePosition is different in case of Yoyo loop
            bool useInversePosition = t.hasLoops && t.loopType == LoopType.Yoyo
                && (t.position < t.duration ? t.completedLoops % 2 != 0 : t.completedLoops % 2 == 0);

            // Get values from plugin and set them
            bool isRewindStep = !wasRewinded && (
                                    t.loopType == LoopType.Restart && t.completedLoops != prevCompletedLoops && (t.loops == -1 || t.completedLoops < t.loops)
                                    || t.position <= 0 && t.completedLoops <= 0
                                );
            UpdateNotice updateNotice = isRewindStep ? UpdateNotice.RewindStep : UpdateNotice.None;
            if (t.ApplyTween(prevPosition, prevCompletedLoops, newCompletedSteps, useInversePosition, updateMode, updateNotice)) return true;

            // Additional callbacks
            if (t.onUpdate != null && updateMode != UpdateMode.IgnoreOnUpdate)
            {
                OnTweenCallback(t.onUpdate, t);
            }
            if (t.position <= 0 && t.completedLoops <= 0 && !wasRewinded && t.onRewind != null)
            {
                OnTweenCallback(t.onRewind, t);
            }
            if (newCompletedSteps > 0 && updateMode == UpdateMode.Update && t.onStepComplete != null)
            {
                for (int i = 0; i < newCompletedSteps; ++i)
                {
                    OnTweenCallback(t.onStepComplete, t);
                    if (!t.active) break;
                }
            }
            if (t.isComplete && !wasComplete && updateMode != UpdateMode.IgnoreOnComplete && t.onComplete != null)
            {
                OnTweenCallback(t.onComplete, t);
            }
            if (!t.isPlaying && wasPlaying && (!t.isComplete || !t.autoKill) && t.onPause != null)
            {
                OnTweenCallback(t.onPause, t);
            }

            // Return
            return t.autoKill && t.isComplete;
        }

        // Assumes that the callback exists (because it was previously checked).
        internal static bool OnTweenCallback(TweenCallback callback, Tween t)
        {
            try
            {
                callback();
            }
            catch (Exception e)
            {
                TweenLog.LogError($"Error in tween callback: {e.Message}\n{e.StackTrace}");
                return false;
            }
            return true;
        }
        
        internal static bool OnTweenCallback<T>(TweenCallback<T> callback, Tween t, T param)
        {
            try
            {
                callback(param);
            }
            catch (Exception e)
            {
                TweenLog.LogError($"Error in tween callback: {e.Message}\n{e.StackTrace}");
                return false;
            }
            return true;
        }

        #endregion

        #region Helper Methods

        internal float Elapsed(bool includeLoops = false)
        {
            if (!includeLoops) return position;
            return duration * completedLoops + position;
        }

        internal void Goto(float to, bool andPlay = false)
        {
            if (duration <= 0)
            {
                completedLoops = loops == -1 ? 0 : loops;
                position = 0;
            }
            else
            {
                completedLoops = (int)(to / duration);
                if (loops != -1 && completedLoops >= loops)
                {
                    completedLoops = loops;
                    position = duration;
                }
                else
                {
                    position = to % duration;
                    if (position < 0) position = 0;
                }
            }
            if (andPlay) isPlaying = true;
            DoGoto(this, position, completedLoops, UpdateMode.Goto);
        }

        #endregion
    }
}
