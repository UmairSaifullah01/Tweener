using System;
using THEBADDEST.Tweening2.Plugins.Core;
using THEBADDEST.Tweening2.Plugins.Options;

namespace THEBADDEST.Tweening2.Core
{
    // Public so it can be used with SetOptions to show the correct overload
    // T1: type of value to tween
    // T2: format in which value is stored while tweening
    // TPlugOptions: options type
    public class TweenerCore<T1, T2, TPlugOptions> : Tweener where TPlugOptions : struct, IPlugOptions
    {
        // SETUP DATA ////////////////////////////////////////////////

        public T2 startValue, endValue, changeValue;
        public TPlugOptions plugOptions;
        public Getter<T1> getter;
        public Setter<T1> setter;
        internal ABSTweenPlugin<T1, T2, TPlugOptions> tweenPlugin;

        const string _TxtCantChangeSequencedValues = "You cannot change the values of a tween contained inside a Sequence";

        #region Constructor

        internal TweenerCore()
        {
            typeofT1 = typeof(T1);
            typeofT2 = typeof(T2);
            typeofTPlugOptions = typeof(TPlugOptions);
            tweenType = TweenType.Tweener;
            Reset();
        }

        #endregion

        #region Public Methods

        // No generics because T to T2 conversion isn't compatible with AOT
        public override Tweener ChangeStartValue(object newStartValue, float newDuration = -1)
        {
            if (isSequenced)
            {
                TweenLog.LogError(_TxtCantChangeSequencedValues);
                return this;
            }
            Type valT = newStartValue.GetType();
            if (valT != typeofT2)
            {
                TweenLog.LogError($"ChangeStartValue: incorrect newStartValue type (is {valT}, should be {typeofT2})");
                return this;
            }
            return DoChangeStartValue(this, (T2)newStartValue, newDuration);
        }

        // No generics because T to T2 conversion isn't compatible with AOT
        public override Tweener ChangeEndValue(object newEndValue, bool snapStartValue)
        { return ChangeEndValue(newEndValue, -1, snapStartValue); }
        
        // No generics because T to T2 conversion isn't compatible with AOT
        public override Tweener ChangeEndValue(object newEndValue, float newDuration = -1, bool snapStartValue = false)
        {
            if (isSequenced)
            {
                TweenLog.LogError(_TxtCantChangeSequencedValues);
                return this;
            }
            Type valT = newEndValue.GetType();
            if (valT != typeofT2)
            {
                TweenLog.LogError($"ChangeEndValue: incorrect newEndValue type (is {valT}, should be {typeofT2})");
                return this;
            }
            return DoChangeEndValue(this, (T2)newEndValue, newDuration, snapStartValue);
        }

        // No generics because T to T2 conversion isn't compatible with AOT
        public override Tweener ChangeValues(object newStartValue, object newEndValue, float newDuration = -1)
        {
            if (isSequenced)
            {
                TweenLog.LogError(_TxtCantChangeSequencedValues);
                return this;
            }
            Type valT0 = newStartValue.GetType();
            Type valT1 = newEndValue.GetType();
            if (valT0 != typeofT2 || valT1 != typeofT2)
            {
                TweenLog.LogError($"ChangeValues: incorrect value type (should be {typeofT2})");
                return this;
            }
            return DoChangeValues(this, (T2)newStartValue, (T2)newEndValue, newDuration);
        }

        #region Advanced Usage (direct from TweenerCore reference)

        /// <summary>NO-GC METHOD: changes the start value of a tween and rewinds it (without pausing it).
        /// Has no effect with tweens that are inside Sequences</summary>
        public TweenerCore<T1, T2, TPlugOptions> ChangeStartValue(T2 newStartValue, float newDuration = -1)
        {
            if (isSequenced)
            {
                TweenLog.LogError(_TxtCantChangeSequencedValues);
                return this;
            }
            return DoChangeStartValue(this, newStartValue, newDuration);
        }

        /// <summary>NO-GC METHOD: changes the end value of a tween and rewinds it (without pausing it).
        /// Has no effect with tweens that are inside Sequences</summary>
        public TweenerCore<T1, T2, TPlugOptions> ChangeEndValue(T2 newEndValue, bool snapStartValue)
        { return ChangeEndValue(newEndValue, -1, snapStartValue); }
        
        /// <summary>NO-GC METHOD: changes the end value of a tween and rewinds it (without pausing it).
        /// Has no effect with tweens that are inside Sequences</summary>
        public TweenerCore<T1, T2, TPlugOptions> ChangeEndValue(T2 newEndValue, float newDuration = -1, bool snapStartValue = false)
        {
            if (isSequenced)
            {
                TweenLog.LogError(_TxtCantChangeSequencedValues);
                return this;
            }
            return DoChangeEndValue(this, newEndValue, newDuration, snapStartValue);
        }

        /// <summary>NO-GC METHOD: changes the start and end value of a tween and rewinds it (without pausing it).
        /// Has no effect with tweens that are inside Sequences</summary>
        public TweenerCore<T1, T2, TPlugOptions> ChangeValues(T2 newStartValue, T2 newEndValue, float newDuration = -1)
        {
            if (isSequenced)
            {
                TweenLog.LogError(_TxtCantChangeSequencedValues);
                return this;
            }
            return DoChangeValues(this, newStartValue, newEndValue, newDuration);
        }

        #endregion

        #endregion

        // Sets From tweens, immediately sending the target to its endValue and assigning new start/endValues.
        internal override Tweener SetFrom(bool relative)
        {
            tweenPlugin.SetFrom(this, relative);
            hasManuallySetStartValue = true;
            return this;
        }
        
        // Sets From tweens in an alternate way where you can set the start value directly
        internal Tweener SetFrom(T2 fromValue, bool setImmediately, bool relative)
        {
            tweenPlugin.SetFrom(this, fromValue, setImmediately, relative);
            hasManuallySetStartValue = true;
            return this;
        }

        // _tweenPlugin is not reset since it's useful to keep it as a reference
        internal sealed override void Reset()
        {
            base.Reset();

            if (tweenPlugin != null) tweenPlugin.Reset(this);
            plugOptions.Reset();
            getter = null;
            setter = null;
            hasManuallySetStartValue = false;
            isFromAllowed = true;
        }

        // Called by TweenManager.Validate.
        // Returns TRUE if the tween is valid
        internal override bool Validate()
        {
            try
            {
                getter();
            }
            catch
            {
                return false;
            }
            return true;
        }

        // CALLED BY TweenManager at each update.
        // Returns the elapsed time minus delay in case of success,
        // -1 if there are missing references and the tween needs to be killed
        internal override float UpdateDelay(float elapsed)
        {
            return DoUpdateDelay(this, elapsed);
        }

        // CALLED BY Tween the moment the tween starts, AFTER any delay has elapsed
        // (unless it's a FROM tween, in which case it will be called BEFORE any eventual delay).
        // Returns TRUE in case of success,
        // FALSE if there are missing references and the tween needs to be killed
        internal override bool Startup()
        {
            return DoStartup(this);
        }

        // Applies the tween set by DoGoto.
        // Returns TRUE if the tween needs to be killed
        internal override bool ApplyTween(float prevPosition, int prevCompletedLoops, int newCompletedSteps, bool useInversePosition, UpdateMode updateMode, UpdateNotice updateNotice)
        {
            if (isInverted) useInversePosition = !useInversePosition;
            float updatePosition = useInversePosition ? duration - position : position;
            try
            {
                tweenPlugin.EvaluateAndApply(plugOptions, this, isRelative, getter, setter, updatePosition, startValue, changeValue, duration, useInversePosition, newCompletedSteps, updateNotice);
            }
            catch (Exception e)
            {
                // Target/field doesn't exist anymore: kill tween
                TweenLog.LogError($"Target or field is missing/null ► {e.Message}\n{e.StackTrace}");
                return true;
            }
            return false;
        }
    }
}
