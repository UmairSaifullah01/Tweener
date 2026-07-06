using System;
using System.Collections.Generic;
using UnityEngine;
using THEBADDEST.Tweening2.Plugins.Options;

namespace THEBADDEST.Tweening2.Plugins.Core
{
    internal static class PluginsManager
    {
        // Default plugins
        static ITweenPlugin _floatPlugin;
        static ITweenPlugin _vector2Plugin;
        static ITweenPlugin _vector3Plugin;
        static ITweenPlugin _vector4Plugin;
        static ITweenPlugin _colorPlugin;

        // Advanced and custom plugins
        const int _MaxCustomPlugins = 20;
        static Dictionary<Type, ITweenPlugin> _customPlugins;

        // ===================================================================================
        // INTERNAL METHODS ------------------------------------------------------------------

        internal static ABSTweenPlugin<T1, T2, TPlugOptions> GetDefaultPlugin<T1, T2, TPlugOptions>() where TPlugOptions : struct, IPlugOptions
        {
            Type t1 = typeof(T1);
            Type t2 = typeof(T2);
            ITweenPlugin plugin = null;
            
            if (t1 == typeof(Vector3) && t1 == t2)
            {
                if (_vector3Plugin == null) _vector3Plugin = new Vector3Plugin();
                plugin = _vector3Plugin;
            }
            else if (t1 == typeof(Vector2) && t1 == t2)
            {
                if (_vector2Plugin == null) _vector2Plugin = new Vector2Plugin();
                plugin = _vector2Plugin;
            }
            else if (t1 == typeof(Vector4) && t1 == t2)
            {
                if (_vector4Plugin == null) _vector4Plugin = new Vector4Plugin();
                plugin = _vector4Plugin;
            }
            else if (t1 == typeof(float) && t1 == t2)
            {
                if (_floatPlugin == null) _floatPlugin = new FloatPlugin();
                plugin = _floatPlugin;
            }
            else if (t1 == typeof(Color) && t1 == t2)
            {
                if (_colorPlugin == null) _colorPlugin = new ColorPlugin();
                plugin = _colorPlugin;
            }

            if (plugin != null) return plugin as ABSTweenPlugin<T1, T2, TPlugOptions>;

            return null;
        }

        // Public so it can be used by custom plugins Get method
        public static ABSTweenPlugin<T1, T2, TPlugOptions> GetCustomPlugin<TPlugin, T1, T2, TPlugOptions>()
            where TPlugin : ITweenPlugin, new()
            where TPlugOptions : struct, IPlugOptions
        {
            Type t = typeof(TPlugin);
            ITweenPlugin plugin;

            if (_customPlugins == null) _customPlugins = new Dictionary<Type, ITweenPlugin>(_MaxCustomPlugins);
            else if (_customPlugins.TryGetValue(t, out plugin)) return plugin as ABSTweenPlugin<T1, T2, TPlugOptions>;

            plugin = new TPlugin();
            _customPlugins.Add(t, plugin);
            return plugin as ABSTweenPlugin<T1, T2, TPlugOptions>;
        }

        // Un-caches all plugins
        internal static void PurgeAll()
        {
            _floatPlugin = null;
            _vector2Plugin = null;
            _vector3Plugin = null;
            _vector4Plugin = null;
            _colorPlugin = null;

            if (_customPlugins != null) _customPlugins.Clear();
        }
    }
}
