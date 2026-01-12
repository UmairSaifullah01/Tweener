namespace THEBADDEST.Tweening2
{
    /// <summary>
    /// Used for tween callbacks
    /// </summary>
    public delegate void TweenCallback();
    /// <summary>
    /// Used for tween callbacks
    /// </summary>
    public delegate void TweenCallback<in T>(T value);

    /// <summary>
    /// Used for custom and animationCurve-based ease functions. Must return a value between 0 and 1.
    /// </summary>
    public delegate float EaseFunction(float time, float duration, float overshootOrAmplitude, float period);
}

namespace THEBADDEST.Tweening2.Core
{
    /// <summary>
    /// Used in place of System.Func, which is not available in mscorlib.
    /// </summary>
    public delegate T DOGetter<out T>();

    /// <summary>
    /// Used in place of System.Action.
    /// </summary>
    public delegate void DOSetter<in T>(T pNewValue);
}
