using THEBADDEST.Tweening2.Core;
using THEBADDEST.Tweening2.Plugins.Options;
using THEBADDEST.Tweening2.Bridge;

namespace THEBADDEST.Tweening2
{
    /// <summary>
    /// Extension methods for TweenerCore to convert to ITweener
    /// </summary>
    public static class TweenerCoreExtensions
    {
        /// <summary>
        /// Converts a TweenerCore to ITweener interface for method chaining
        /// </summary>
        public static ITweener AsITweener<T1, T2, TPlugOptions>(this TweenerCore<T1, T2, TPlugOptions> tweener)
            where TPlugOptions : struct, IPlugOptions
        {
            if (tweener == null) return null;
            return new TweenerWrapper(tweener);
        }
    }
}
