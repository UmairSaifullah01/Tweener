using UnityEngine;
using THEBADDEST.Tweening2;
using THEBADDEST.Tweening2.Core;
using THEBADDEST.Tweening2.Plugins.Options;

namespace THEBADDEST.Tweening2
{
    /// <summary>
    /// Methods that extend Transform and allow to directly create and control tweens
    /// </summary>
    public static class TransformExtensions
    {
        /// <summary>Tweens a Transform's position to the given value.
        /// Also stores the transform as the tween's target</summary>
        public static ITweener Move(this Transform target, Vector3 endValue, float duration)
        {
            TweenerCore<Vector3, Vector3, VectorOptions> t = TweenCore.To(() => target.position, x => target.position = x, endValue, duration);
            if (t != null) t.target = target;
            return new Bridge.TweenerWrapper(t);
        }

        /// <summary>Tweens a Transform's localPosition to the given value.
        /// Also stores the transform as the tween's target</summary>
        public static ITweener MoveLocal(this Transform target, Vector3 endValue, float duration)
        {
            TweenerCore<Vector3, Vector3, VectorOptions> t = TweenCore.To(() => target.localPosition, x => target.localPosition = x, endValue, duration);
            if (t != null) t.target = target;
            return new Bridge.TweenerWrapper(t);
        }

        /// <summary>Tweens a Transform's localScale to the given value.
        /// Also stores the transform as the tween's target</summary>
        public static ITweener Scale(this Transform target, Vector3 endValue, float duration)
        {
            TweenerCore<Vector3, Vector3, VectorOptions> t = TweenCore.To(() => target.localScale, x => target.localScale = x, endValue, duration);
            if (t != null) t.target = target;
            return new Bridge.TweenerWrapper(t);
        }

        /// <summary>Tweens a Transform's rotation to the given value.
        /// Also stores the transform as the tween's target</summary>
        public static ITweener Rotate(this Transform target, Vector3 endValue, float duration)
        {
            TweenerCore<Vector3, Vector3, VectorOptions> t = TweenCore.To(() => target.eulerAngles, x => target.eulerAngles = x, endValue, duration);
            if (t != null) t.target = target;
            return new Bridge.TweenerWrapper(t);
        }

        /// <summary>Tweens a Transform's localRotation to the given value.
        /// Also stores the transform as the tween's target</summary>
        public static ITweener RotateLocal(this Transform target, Vector3 endValue, float duration)
        {
            TweenerCore<Vector3, Vector3, VectorOptions> t = TweenCore.To(() => target.localEulerAngles, x => target.localEulerAngles = x, endValue, duration);
            if (t != null) t.target = target;
            return new Bridge.TweenerWrapper(t);
        }
    }
}
