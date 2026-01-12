using UnityEngine;
using UnityEngine.UI;
using THEBADDEST.Tweening2;
using THEBADDEST.Tweening2.Core;
using THEBADDEST.Tweening2.Plugins.Options;

namespace THEBADDEST.Tweening2
{
    /// <summary>
    /// Methods that extend Graphic (Image, Text, etc.) and allow to directly create and control tweens
    /// </summary>
    public static class GraphicExtensions
    {
        /// <summary>Tweens a Graphic's color alpha to the given value.
        /// Also stores the graphic as the tween's target</summary>
        public static ITweener Fade(this Graphic target, float endValue, float duration)
        {
            Color endColor = target.color;
            endColor.a = endValue;
            TweenerCore<Color, Color, ColorOptions> t = TweenCore.To(() => target.color, x => target.color = x, endColor, duration);
            if (t != null) t.target = target;
            return new Bridge.TweenerWrapper(t);
        }

        /// <summary>Tweens a Graphic's color to the given value.
        /// Also stores the graphic as the tween's target</summary>
        public static ITweener Color(this Graphic target, Color endValue, float duration)
        {
            TweenerCore<Color, Color, ColorOptions> t = TweenCore.To(() => target.color, x => target.color = x, endValue, duration);
            if (t != null) t.target = target;
            return new Bridge.TweenerWrapper(t);
        }

        /// <summary>Tweens an Image's fillAmount to the given value.
        /// Also stores the image as the tween's target</summary>
        public static ITweener FillAmount(this Image target, float endValue, float duration)
        {
            TweenerCore<float, float, FloatOptions> t = TweenCore.To(() => target.fillAmount, x => target.fillAmount = x, endValue, duration);
            if (t != null) t.target = target;
            return new Bridge.TweenerWrapper(t);
        }
    }
}
