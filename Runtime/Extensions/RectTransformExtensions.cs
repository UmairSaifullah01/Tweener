using UnityEngine;
using THEBADDEST.Tweening2;
using THEBADDEST.Tweening2.Core;
using THEBADDEST.Tweening2.Plugins.Options;

namespace THEBADDEST.Tweening2
{
    /// <summary>
    /// Methods that extend RectTransform and allow to directly create and control tweens
    /// </summary>
    public static class RectTransformExtensions
    {
        /// <summary>Tweens a RectTransform's anchoredPosition to the given value.
        /// Also stores the RectTransform as the tween's target</summary>
        public static ITweener AnchoredPosition(this RectTransform target, Vector2 endValue, float duration)
        {
            TweenerCore<Vector2, Vector2, VectorOptions> t = TweenCore.To(() => target.anchoredPosition, x => target.anchoredPosition = x, endValue, duration);
            if (t != null) t.target = target;
            return new Bridge.TweenerWrapper(t);
        }

        /// <summary>Tweens a RectTransform's sizeDelta to the given value.
        /// Also stores the RectTransform as the tween's target</summary>
        public static ITweener SizeDelta(this RectTransform target, Vector2 endValue, float duration)
        {
            TweenerCore<Vector2, Vector2, VectorOptions> t = TweenCore.To(() => target.sizeDelta, x => target.sizeDelta = x, endValue, duration);
            if (t != null) t.target = target;
            return new Bridge.TweenerWrapper(t);
        }

        /// <summary>Tweens a RectTransform's anchorMin to the given value.
        /// Also stores the RectTransform as the tween's target</summary>
        public static ITweener AnchorMin(this RectTransform target, Vector2 endValue, float duration)
        {
            TweenerCore<Vector2, Vector2, VectorOptions> t = TweenCore.To(() => target.anchorMin, x => target.anchorMin = x, endValue, duration);
            if (t != null) t.target = target;
            return new Bridge.TweenerWrapper(t);
        }

        /// <summary>Tweens a RectTransform's anchorMax to the given value.
        /// Also stores the RectTransform as the tween's target</summary>
        public static ITweener AnchorMax(this RectTransform target, Vector2 endValue, float duration)
        {
            TweenerCore<Vector2, Vector2, VectorOptions> t = TweenCore.To(() => target.anchorMax, x => target.anchorMax = x, endValue, duration);
            if (t != null) t.target = target;
            return new Bridge.TweenerWrapper(t);
        }

        /// <summary>Tweens a RectTransform's pivot to the given value.
        /// Also stores the RectTransform as the tween's target</summary>
        public static ITweener Pivot(this RectTransform target, Vector2 endValue, float duration)
        {
            TweenerCore<Vector2, Vector2, VectorOptions> t = TweenCore.To(() => target.pivot, x => target.pivot = x, endValue, duration);
            if (t != null) t.target = target;
            return new Bridge.TweenerWrapper(t);
        }
    }
}
