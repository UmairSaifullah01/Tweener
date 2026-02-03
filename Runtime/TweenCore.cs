using UnityEngine;
using THEBADDEST.Tweening2.Core;
using THEBADDEST.Tweening2.Plugins.Options;

namespace THEBADDEST.Tweening2
{
    /// <summary>
    /// Main Tween class. Contains static methods to create and control tweens
    /// </summary>
    public static class TweenCore
    {
        /// <summary>Tween's version</summary>
        public static readonly string Version = "2.0.0";

        ///////////////////////////////////////////////
        // Options ////////////////////////////////////

        /// <summary>Global timeScale (default: 1)</summary>
        public static float timeScale = 1f;
        /// <summary>Unscaled timeScale applied only to timeScaleIndependent tweens (default: 1)</summary>
        public static float unscaledTimeScale = 1f;

        ///////////////////////////////////////////////
        // Default options for Tweens /////////////////

        /// <summary>Default updateType for new tweens. Default: UpdateType.Normal</summary>
        public static UpdateType defaultUpdateType = UpdateType.Normal;
        /// <summary>Sets whether Unity's timeScale should be taken into account by default or not. Default: false</summary>
        public static bool defaultTimeScaleIndependent = false;
        /// <summary>Default autoKill behaviour for new tweens. Default: TRUE</summary>
        public static bool defaultAutoKill = true;
        /// <summary>Default loopType applied to all new tweens. Default: LoopType.Restart</summary>
        public static LoopType defaultLoopType = LoopType.Restart;
        /// <summary>If TRUE all newly created tweens are set as recyclable, otherwise not. Default: FALSE</summary>
        public static bool defaultRecyclable = false;
        /// <summary>Default ease applied to all new Tweeners. Default: EaseType.OutQuad</summary>
        public static EaseType defaultEaseType = EaseType.OutQuad;
        /// <summary>Default overshoot/amplitude used for eases. Default: 1.70158f</summary>
        public static float defaultEaseOvershootOrAmplitude = 1.70158f;
        /// <summary>Default period used for eases. Default: 0</summary>
        public static float defaultEasePeriod = 0f;

        /// <summary>Used internally. Assigned/removed by TweenComponent</summary>
        private static TweenComponent instance;

        internal static bool initialized;

        #region RuntimeInitMethods

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void RuntimeOnLoad()
        {
            initialized = false;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Initializes the Tween system. Called automatically on first use.
        /// </summary>
        public static void Init()
        {
            if (initialized) return;
            if (!Application.isPlaying) return;

            initialized = true;
            TweenComponent.GetInstance();
        }

        static void InitCheck()
        {
            if (!initialized) Init();
        }

        /// <summary>
        /// Kills all tweens
        /// </summary>
        public static void KillAll()
        {
            InitCheck();
            TweenManager.DespawnAll();
        }

        /// <summary>
        /// Pauses all tweens
        /// </summary>
        public static void PauseAll()
        {
            InitCheck();
            TweenManager.PauseAll();
        }

        /// <summary>
        /// Resumes all tweens
        /// </summary>
        public static void ResumesAll()
        {
            InitCheck();
            TweenManager.PlayAll();
        }

        #endregion

        #region Tween Creation Methods

        /// <summary>Tweens a property or field to the given value using default plugins</summary>
        public static TweenerCore<T1, T2, TPlugOptions> To<T1, T2, TPlugOptions>(
            Core.Getter<T1> getter, Core.Setter<T1> setter, T2 endValue, float duration)
            where TPlugOptions : struct, IPlugOptions
        {
            InitCheck();
            TweenerCore<T1, T2, TPlugOptions> t = TweenManager.GetTweener<T1, T2, TPlugOptions>();
            if (!Tweener.Setup(t, getter, setter, endValue, duration))
            {
                TweenManager.Despawn(t);
                return null;
            }
            t.autoKill = defaultAutoKill;
            t.isRecyclable = defaultRecyclable;
            t.easeType = defaultEaseType;
            t.easeOvershootOrAmplitude = defaultEaseOvershootOrAmplitude;
            t.easePeriod = defaultEasePeriod;
            t.loopType = defaultLoopType;
            t.isPlaying = true;
            TweenManager.SetUpdateType(t, defaultUpdateType, defaultTimeScaleIndependent);
            return t;
        }

        /// <summary>Tweens a float property or field to the given value</summary>
        public static TweenerCore<float, float, FloatOptions> To(Core.Getter<float> getter, Core.Setter<float> setter, float endValue, float duration)
        {
            return To<float, float, FloatOptions>(getter, setter, endValue, duration);
        }

        /// <summary>Tweens a Vector3 property or field to the given value</summary>
        public static TweenerCore<Vector3, Vector3, VectorOptions> To(Core.Getter<Vector3> getter, Core.Setter<Vector3> setter, Vector3 endValue, float duration)
        {
            return To<Vector3, Vector3, VectorOptions>(getter, setter, endValue, duration);
        }

        /// <summary>Tweens a Vector2 property or field to the given value</summary>
        public static TweenerCore<Vector2, Vector2, VectorOptions> To(Core.Getter<Vector2> getter, Core.Setter<Vector2> setter, Vector2 endValue, float duration)
        {
            return To<Vector2, Vector2, VectorOptions>(getter, setter, endValue, duration);
        }

        /// <summary>Tweens a Color property or field to the given value</summary>
        public static TweenerCore<Color, Color, ColorOptions> To(Core.Getter<Color> getter, Core.Setter<Color> setter, Color endValue, float duration)
        {
            return To<Color, Color, ColorOptions>(getter, setter, endValue, duration);
        }

        /// <summary>Creates a new ITweener for the old system flow</summary>
        public static ITweener Create()
        {
            InitCheck();
            TweenerCore<float, float, FloatOptions> core = To(() => 0f, x => { }, 1f, 1f);
            return new Bridge.TweenerWrapper(core);
        }

        /// <summary>Creates a new Sequence</summary>
        public static ISequence Sequence()
        {
            InitCheck();
            Sequence s = TweenManager.GetSequence();
            s.autoKill = defaultAutoKill;
            s.isRecyclable = defaultRecyclable;
            s.loopType = defaultLoopType;
            s.isPlaying = true;
            TweenManager.SetUpdateType(s, defaultUpdateType, defaultTimeScaleIndependent);
            return s;
        }

        #endregion
    }
}
