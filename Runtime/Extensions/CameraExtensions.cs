using UnityEngine;
using THEBADDEST.Tweening2;
using THEBADDEST.Tweening2.Core;
using THEBADDEST.Tweening2.Plugins.Options;

namespace THEBADDEST.Tweening2
{
    /// <summary>
    /// Methods that extend Camera and allow to directly create and control tweens
    /// </summary>
    public static class CameraExtensions
    {
        /// <summary>Tweens a Camera's fieldOfView to the given value.
        /// Also stores the camera as the tween's target</summary>
        public static ITweener FieldOfView(this Camera target, float endValue, float duration)
        {
            TweenerCore<float, float, FloatOptions> t = TweenCore.To(() => target.fieldOfView, x => target.fieldOfView = x, endValue, duration);
            if (t != null) t.target = target;
            return new Bridge.TweenerWrapper(t);
        }

        /// <summary>Tweens a Camera's orthographicSize to the given value.
        /// Also stores the camera as the tween's target</summary>
        public static ITweener OrthographicSize(this Camera target, float endValue, float duration)
        {
            TweenerCore<float, float, FloatOptions> t = TweenCore.To(() => target.orthographicSize, x => target.orthographicSize = x, endValue, duration);
            if (t != null) t.target = target;
            return new Bridge.TweenerWrapper(t);
        }

        /// <summary>Tweens a Camera's backgroundColor to the given value.
        /// Also stores the camera as the tween's target</summary>
        public static ITweener BackgroundColor(this Camera target, Color endValue, float duration)
        {
            TweenerCore<Color, Color, ColorOptions> t = TweenCore.To(() => target.backgroundColor, x => target.backgroundColor = x, endValue, duration);
            if (t != null) t.target = target;
            return new Bridge.TweenerWrapper(t);
        }
    }
}
