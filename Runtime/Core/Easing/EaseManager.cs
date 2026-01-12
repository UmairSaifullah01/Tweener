using System;
using UnityEngine;

namespace THEBADDEST.Tweening2.Core.Easing
{
    public static class EaseManager
    {
        const float _PiOver2 = Mathf.PI * 0.5f;
        const float _TwoPi = Mathf.PI * 2;

        /// <summary>
        /// Returns a value between 0 and 1 (inclusive) based on the elapsed time and ease selected
        /// </summary>
        public static float Evaluate(EaseType easeType, EaseFunction customEase, float time, float duration, float overshootOrAmplitude, float period)
        {
            if (customEase != null)
            {
                return customEase(time, duration, overshootOrAmplitude, period);
            }

            switch (easeType)
            {
                case EaseType.Linear:
                    return time / duration;
                case EaseType.InSine:
                    return -(float)Math.Cos(time / duration * _PiOver2) + 1;
                case EaseType.OutSine:
                    return (float)Math.Sin(time / duration * _PiOver2);
                case EaseType.InOutSine:
                    return -0.5f * ((float)Math.Cos(Mathf.PI * time / duration) - 1);
                case EaseType.InQuad:
                    return (time /= duration) * time;
                case EaseType.OutQuad:
                    return -(time /= duration) * (time - 2);
                case EaseType.InOutQuad:
                    if ((time /= duration * 0.5f) < 1) return 0.5f * time * time;
                    return -0.5f * ((--time) * (time - 2) - 1);
                case EaseType.InCubic:
                    return (time /= duration) * time * time;
                case EaseType.OutCubic:
                    return ((time = time / duration - 1) * time * time + 1);
                case EaseType.InOutCubic:
                    if ((time /= duration * 0.5f) < 1) return 0.5f * time * time * time;
                    return 0.5f * ((time -= 2) * time * time + 2);
                case EaseType.InQuart:
                    return (time /= duration) * time * time * time;
                case EaseType.OutQuart:
                    return -((time = time / duration - 1) * time * time * time - 1);
                case EaseType.InOutQuart:
                    if ((time /= duration * 0.5f) < 1) return 0.5f * time * time * time * time;
                    return -0.5f * ((time -= 2) * time * time * time - 2);
                case EaseType.InQuint:
                    return (time /= duration) * time * time * time * time;
                case EaseType.OutQuint:
                    return ((time = time / duration - 1) * time * time * time * time + 1);
                case EaseType.InOutQuint:
                    if ((time /= duration * 0.5f) < 1) return 0.5f * time * time * time * time * time;
                    return 0.5f * ((time -= 2) * time * time * time * time + 2);
                case EaseType.InExpo:
                    return (time == 0) ? 0 : (float)Math.Pow(2, 10 * (time / duration - 1));
                case EaseType.OutExpo:
                    if (time == duration) return 1;
                    return (-(float)Math.Pow(2, -10 * time / duration) + 1);
                case EaseType.InOutExpo:
                    if (time == 0) return 0;
                    if (time == duration) return 1;
                    if ((time /= duration * 0.5f) < 1) return 0.5f * (float)Math.Pow(2, 10 * (time - 1));
                    return 0.5f * (-(float)Math.Pow(2, -10 * --time) + 2);
                case EaseType.InCirc:
                    return -((float)Math.Sqrt(1 - (time /= duration) * time) - 1);
                case EaseType.OutCirc:
                    return (float)Math.Sqrt(1 - (time = time / duration - 1) * time);
                case EaseType.InOutCirc:
                    if ((time /= duration * 0.5f) < 1) return -0.5f * ((float)Math.Sqrt(1 - time * time) - 1);
                    return 0.5f * ((float)Math.Sqrt(1 - (time -= 2) * time) + 1);
                case EaseType.InElastic:
                    float s0;
                    if (time == 0) return 0;
                    if ((time /= duration) == 1) return 1;
                    if (period == 0) period = duration * 0.3f;
                    if (overshootOrAmplitude < 1)
                    {
                        overshootOrAmplitude = 1;
                        s0 = period / 4;
                    }
                    else s0 = period / _TwoPi * (float)Math.Asin(1 / overshootOrAmplitude);
                    return -(overshootOrAmplitude * (float)Math.Pow(2, 10 * (time -= 1)) * (float)Math.Sin((time * duration - s0) * _TwoPi / period));
                case EaseType.OutElastic:
                    float s1;
                    if (time == 0) return 0;
                    if ((time /= duration) == 1) return 1;
                    if (period == 0) period = duration * 0.3f;
                    if (overshootOrAmplitude < 1)
                    {
                        overshootOrAmplitude = 1;
                        s1 = period / 4;
                    }
                    else s1 = period / _TwoPi * (float)Math.Asin(1 / overshootOrAmplitude);
                    return overshootOrAmplitude * (float)Math.Pow(2, -10 * time) * (float)Math.Sin((time * duration - s1) * _TwoPi / period) + 1;
                case EaseType.InOutElastic:
                    float s2;
                    if (time == 0) return 0;
                    if ((time /= duration * 0.5f) == 2) return 1;
                    if (period == 0) period = duration * 0.3f;
                    if (overshootOrAmplitude < 1)
                    {
                        overshootOrAmplitude = 1;
                        s2 = period / 4;
                    }
                    else s2 = period / _TwoPi * (float)Math.Asin(1 / overshootOrAmplitude);
                    if (time < 1) return -0.5f * overshootOrAmplitude * (float)Math.Pow(2, 10 * (time -= 1)) * (float)Math.Sin((time * duration - s2) * _TwoPi / period);
                    return overshootOrAmplitude * (float)Math.Pow(2, -10 * (time -= 1)) * (float)Math.Sin((time * duration - s2) * _TwoPi / period) * 0.5f + 1;
                case EaseType.InBack:
                    return (time /= duration) * time * ((overshootOrAmplitude + 1) * time - overshootOrAmplitude);
                case EaseType.OutBack:
                    return ((time = time / duration - 1) * time * ((overshootOrAmplitude + 1) * time + overshootOrAmplitude) + 1);
                case EaseType.InOutBack:
                    if ((time /= duration * 0.5f) < 1) return 0.5f * time * time * (((overshootOrAmplitude *= 1.525f) + 1) * time - overshootOrAmplitude);
                    return 0.5f * ((time -= 2) * time * (((overshootOrAmplitude *= 1.525f) + 1) * time + overshootOrAmplitude) + 2);
                case EaseType.InBounce:
                    return 1 - Evaluate(EaseType.OutBounce, null, duration - time, duration, overshootOrAmplitude, period);
                case EaseType.OutBounce:
                    if ((time /= duration) < (1 / 2.75f))
                    {
                        return 7.5625f * time * time;
                    }
                    else if (time < (2 / 2.75f))
                    {
                        return 7.5625f * (time -= (1.5f / 2.75f)) * time + 0.75f;
                    }
                    else if (time < (2.5 / 2.75))
                    {
                        return 7.5625f * (time -= (2.25f / 2.75f)) * time + 0.9375f;
                    }
                    else
                    {
                        return 7.5625f * (time -= (2.625f / 2.75f)) * time + 0.984375f;
                    }
                case EaseType.InOutBounce:
                    if (time < duration * 0.5f) return Evaluate(EaseType.InBounce, null, time * 2, duration, overshootOrAmplitude, period) * 0.5f;
                    return Evaluate(EaseType.OutBounce, null, time * 2 - duration, duration, overshootOrAmplitude, period) * 0.5f + 0.5f;
                case EaseType.INTERNAL_Zero:
                    return 1;
                default:
                    return time / duration;
            }
        }
    }
}
