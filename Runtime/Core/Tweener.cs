using System;
using THEBADDEST.Tweening2.Plugins.Core;
using THEBADDEST.Tweening2.Plugins.Options;

namespace THEBADDEST.Tweening2.Core
{
    /// <summary>
    /// Animates a single value
    /// </summary>
    public abstract class Tweener : Tween
    {
        // TRUE when start value has been changed via From or ChangeStart/Values
        internal bool hasManuallySetStartValue;
        internal bool isFromAllowed = true;

        internal Tweener() { }

        // ===================================================================================
        // ABSTRACT METHODS ------------------------------------------------------------------

        /// <summary>Changes the start value of a tween and rewinds it (without pausing it).
        /// Has no effect with tweens that are inside Sequences</summary>
        public abstract Tweener ChangeStartValue(object newStartValue, float newDuration = -1);

        /// <summary>Changes the end value of a tween and rewinds it (without pausing it).
        /// Has no effect with tweens that are inside Sequences</summary>
        public abstract Tweener ChangeEndValue(object newEndValue, float newDuration = -1, bool snapStartValue = false);
        
        /// <summary>Changes the end value of a tween and rewinds it (without pausing it).
        /// Has no effect with tweens that are inside Sequences</summary>
        public abstract Tweener ChangeEndValue(object newEndValue, bool snapStartValue);

        /// <summary>Changes the start and end value of a tween and rewinds it (without pausing it).
        /// Has no effect with tweens that are inside Sequences</summary>
        public abstract Tweener ChangeValues(object newStartValue, object newEndValue, float newDuration = -1);

        internal abstract Tweener SetFrom(bool relative);

        // ===================================================================================
        // INTERNAL METHODS ------------------------------------------------------------------

        // CALLED BY Tween static API when spawning/creating a new Tweener.
        // Returns TRUE if the setup is successful
        internal static bool Setup<T1, T2, TPlugOptions>(
            TweenerCore<T1, T2, TPlugOptions> t, Getter<T1> getter, Setter<T1> setter, T2 endValue, float duration, ABSTweenPlugin<T1, T2, TPlugOptions> plugin = null
        )
            where TPlugOptions : struct, IPlugOptions
        {
            if (plugin != null) t.tweenPlugin = plugin;
            else
            {
                if (t.tweenPlugin == null) t.tweenPlugin = PluginsManager.GetDefaultPlugin<T1, T2, TPlugOptions>();
                if (t.tweenPlugin == null)
                {
                    TweenLog.LogError("No suitable plugin found for this type");
                    return false;
                }
            }

            t.getter = getter;
            t.setter = setter;
            t.endValue = endValue;
            t.duration = duration;
            
            // Defaults - will be set by Tween static API
            t.autoKill = true;
            t.isRecyclable = false;
            t.easeType = EaseType.OutQuad;
            t.easeOvershootOrAmplitude = 1.70158f;
            t.easePeriod = 0f;
            t.loopType = LoopType.Restart;
            t.isPlaying = true;
            return true;
        }

        // CALLED BY TweenerCore
        // Returns the elapsed time minus delay in case of success,
        // -1 if there are missing references and the tween needs to be killed
        internal static float DoUpdateDelay<T1, T2, TPlugOptions>(TweenerCore<T1, T2, TPlugOptions> t, float elapsed) where TPlugOptions : struct, IPlugOptions
        {
            float tweenDelay = t.delay;
            if (elapsed > tweenDelay)
            {
                // Delay complete
                t.elapsedDelay = tweenDelay;
                t.delayComplete = true;
                return elapsed - tweenDelay;
            }
            t.elapsedDelay = elapsed;
            return 0;
        }

        // CALLED VIA Tween the moment the tween starts, AFTER any delay has elapsed
        // Returns TRUE in case of success,
        // FALSE if there are missing references and the tween needs to be killed
        internal static bool DoStartup<T1, T2, TPlugOptions>(TweenerCore<T1, T2, TPlugOptions> t) where TPlugOptions : struct, IPlugOptions
        {
            t.startupDone = true;

            // Special startup operations
            if (t.specialStartupMode != SpecialStartupMode.None)
            {
                // For now, skip special startup modes
                // if (!DOStartupSpecials(t)) return false;
            }

            if (!t.hasManuallySetStartValue)
            {
                // Take start value from current target value
                try
                {
                    if (t.isFrom)
                    {
                        // From tween without forced From value
                        t.SetFrom(t.isRelative && !t.isBlendable);
                        t.isRelative = false;
                    }
                    else t.startValue = t.tweenPlugin.ConvertToStartValue(t, t.getter());
                }
                catch (Exception e)
                {
                    TweenLog.LogError($"Tween startup failed (NULL target/property): the tween will now be killed ► {e.Message}");
                    return false; // Target/field doesn't exist: kill tween
                }
            }

            if (t.isRelative) t.tweenPlugin.SetRelativeEndValue(t);

            t.tweenPlugin.SetChangeValue(t);

            // Duration based startup operations
            DOStartupDurationBased(t);

            // Applied here so that the eventual duration derived from a speedBased tween has been set
            if (t.duration <= 0) t.easeType = EaseType.INTERNAL_Zero;
            
            return true;
        }

        // CALLED BY TweenerCore
        internal static TweenerCore<T1, T2, TPlugOptions> DoChangeStartValue<T1, T2, TPlugOptions>(
            TweenerCore<T1, T2, TPlugOptions> t, T2 newStartValue, float newDuration
        ) where TPlugOptions : struct, IPlugOptions
        {
            t.hasManuallySetStartValue = true;
            t.startValue = newStartValue;

            if (t.startupDone)
            {
                if (t.specialStartupMode != SpecialStartupMode.None)
                {
                    // Skip special startup for now
                    // if (!DOStartupSpecials(t)) return null;
                }
                t.tweenPlugin.SetChangeValue(t);
            }

            if (newDuration > 0)
            {
                t.duration = newDuration;
                if (t.startupDone) DOStartupDurationBased(t);
            }

            // Force rewind
            Tween.Goto(t, 0, 0, UpdateMode.IgnoreOnUpdate);

            return t;
        }

        // CALLED BY TweenerCore
        internal static TweenerCore<T1, T2, TPlugOptions> DoChangeEndValue<T1, T2, TPlugOptions>(
            TweenerCore<T1, T2, TPlugOptions> t, T2 newEndValue, float newDuration, bool snapStartValue
        ) where TPlugOptions : struct, IPlugOptions
        {
            t.endValue = newEndValue;
            t.isRelative = false;

            if (t.startupDone)
            {
                if (t.specialStartupMode != SpecialStartupMode.None)
                {
                    // Skip special startup for now
                    // if (!DOStartupSpecials(t)) return null;
                }
                if (snapStartValue)
                {
                    // Reassign startValue with current target's value
                    try
                    {
                        t.startValue = t.tweenPlugin.ConvertToStartValue(t, t.getter());
                    }
                    catch (Exception e)
                    {
                        TweenLog.LogError($"Target or field is missing/null ► {e.Message}");
                        TweenManager.Despawn(t);
                        return null;
                    }
                }
                t.tweenPlugin.SetChangeValue(t);
            }

            if (newDuration > 0)
            {
                t.duration = newDuration;
                if (t.startupDone) DOStartupDurationBased(t);
            }

            // Force rewind
            Tween.Goto(t, 0, 0, UpdateMode.IgnoreOnUpdate);

            return t;
        }

        internal static TweenerCore<T1, T2, TPlugOptions> DoChangeValues<T1, T2, TPlugOptions>(
            TweenerCore<T1, T2, TPlugOptions> t, T2 newStartValue, T2 newEndValue, float newDuration
        ) where TPlugOptions : struct, IPlugOptions
        {
            t.hasManuallySetStartValue = true;
            t.isRelative = t.isFrom = false;
            t.startValue = newStartValue;
            t.endValue = newEndValue;

            if (t.startupDone)
            {
                if (t.specialStartupMode != SpecialStartupMode.None)
                {
                    // Skip special startup for now
                    // if (!DOStartupSpecials(t)) return null;
                }
                t.tweenPlugin.SetChangeValue(t);
            }

            if (newDuration > 0)
            {
                t.duration = newDuration;
                if (t.startupDone) DOStartupDurationBased(t);
            }

            // Force rewind
            Tween.Goto(t, 0, 0, UpdateMode.IgnoreOnUpdate);

            return t;
        }

        static void DOStartupDurationBased<T1, T2, TPlugOptions>(TweenerCore<T1, T2, TPlugOptions> t) where TPlugOptions : struct, IPlugOptions
        {
            if (t.isSpeedBased) t.duration = t.tweenPlugin.GetSpeedBasedDuration(t.plugOptions, t.duration, t.changeValue);
            t.fullDuration = t.loops > -1 ? t.duration * t.loops : float.PositiveInfinity;
        }
    }
}
