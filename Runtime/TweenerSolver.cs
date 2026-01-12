using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace THEBADDEST.Tweening
{
    public class TweenerSolver : MonoBehaviour
    {
        private static TweenerSolver _SolverInstance;
        private static TweenerSolver Solver
        {
            get
            {
                if (_SolverInstance == null)
                {
                    GameObject go = new GameObject("TweenerSolver");
                    _SolverInstance = go.AddComponent<TweenerSolver>();
                    go.hideFlags = HideFlags.HideInHierarchy;
                    _SolverInstance.factory = new TweenerFactory();
                    DontDestroyOnLoad(go);
                }
                return _SolverInstance;
            }
        }

        // Active tweens management (replaces coroutine dictionary)
        private List<ITweener> _activeTweeners = new List<ITweener>();
        private List<ITweener> _activeLateTweeners = new List<ITweener>();
        private List<ITweener> _activeFixedTweeners = new List<ITweener>();
        private List<ITweener> _killList = new List<ITweener>();

        private float _unscaledTime;
        private float _unscaledDeltaTime;
        private bool _isUpdateLoop;

        TweenerFactory factory;

        void Awake()
        {
            _unscaledTime = Time.realtimeSinceStartup;
        }

        void Update()
        {
            _unscaledDeltaTime = Time.realtimeSinceStartup - _unscaledTime;
            _unscaledTime = Time.realtimeSinceStartup;

            _isUpdateLoop = true;
            UpdateTweeners(_activeTweeners, Time.deltaTime, _unscaledDeltaTime);
            _isUpdateLoop = false;

            // Kill marked tweens
            if (_killList.Count > 0)
            {
                foreach (var tweener in _killList)
                {
                    RemoveActiveTweener(tweener);
                    Dispose(tweener);
                }
                _killList.Clear();
            }
        }

        void LateUpdate()
        {
            if (_activeLateTweeners.Count > 0)
            {
                _isUpdateLoop = true;
                UpdateTweeners(_activeLateTweeners, Time.deltaTime, _unscaledDeltaTime);
                _isUpdateLoop = false;
            }
        }

        void FixedUpdate()
        {
            if (_activeFixedTweeners.Count > 0 && Time.timeScale > 0)
            {
                _isUpdateLoop = true;
                UpdateTweeners(_activeFixedTweeners, Time.fixedDeltaTime, Time.fixedUnscaledDeltaTime);
                _isUpdateLoop = false;
            }
        }

        private void UpdateTweeners(List<ITweener> tweeners, float deltaTime, float unscaledDeltaTime)
        {
            for (int i = tweeners.Count - 1; i >= 0; i--)
            {
                if (i >= tweeners.Count) continue; // Safety check
                
                var tweener = tweeners[i];
                if (tweener == null) continue;

                if (UpdateTweener(tweener, deltaTime, unscaledDeltaTime))
                {
                    _killList.Add(tweener);
                }
            }
        }

        private bool UpdateTweener(ITweener tweener, float deltaTime, float unscaledDeltaTime)
        {
            // Cast to Tweener to access internal methods
            if (tweener is Tweener t)
            {
                return t.UpdateTween(deltaTime, unscaledDeltaTime);
            }
            // Handle sequence
            else if (tweener is TweenerSequence seq)
            {
                return seq.UpdateTween(deltaTime, unscaledDeltaTime);
            }
            return false;
        }

        public static ITweener Create()
        {
            return Solver.factory.Create();
        }

        public static void Dispose(ITweener tweener)
        {
            Solver.factory.Dispose(tweener);
        }

        /// <summary>
        /// Plays a tweener with a coroutine (for coroutine-based tweens).
        /// </summary>
        public static void PlayTweener(ITweener tweener, IEnumerator coroutine)
        {
            if (tweener == null || coroutine == null) return;
            Solver.StartCoroutine(coroutine);
        }

        /// <summary>
        /// Plays a coroutine directly (for timeline and other coroutine-based systems).
        /// </summary>
        public static void PlayTweener(IEnumerator coroutine)
        {
            if (coroutine == null) return;
            Solver.StartCoroutine(coroutine);
        }

        public static void AddActiveTweener(ITweener tweener, UpdateType updateType = UpdateType.Normal)
        {
            if (tweener == null) return;

            RemoveActiveTweener(tweener); // Remove from any existing list first

            switch (updateType)
            {
                case UpdateType.Normal:
                    if (!Solver._activeTweeners.Contains(tweener))
                        Solver._activeTweeners.Add(tweener);
                    break;
                case UpdateType.Late:
                    if (!Solver._activeLateTweeners.Contains(tweener))
                        Solver._activeLateTweeners.Add(tweener);
                    break;
                case UpdateType.Fixed:
                    if (!Solver._activeFixedTweeners.Contains(tweener))
                        Solver._activeFixedTweeners.Add(tweener);
                    break;
            }
        }

        public static void RemoveActiveTweener(ITweener tweener)
        {
            if (tweener == null) return;

            Solver._activeTweeners.Remove(tweener);
            Solver._activeLateTweeners.Remove(tweener);
            Solver._activeFixedTweeners.Remove(tweener);
        }

        public static void MarkForKilling(ITweener tweener)
        {
            if (tweener == null) return;
            if (Solver._isUpdateLoop)
            {
                Solver._killList.Add(tweener);
            }
            else
            {
                RemoveActiveTweener(tweener);
                Dispose(tweener);
            }
        }

        public static void StopTweener(ITweener tweener)
        {
            if (tweener == null) return;
            RemoveActiveTweener(tweener);
            Dispose(tweener);
        }

        public static void StopAllTweeners()
        {
            foreach (var tweener in Solver._activeTweeners)
            {
                Dispose(tweener);
            }
            foreach (var tweener in Solver._activeLateTweeners)
            {
                Dispose(tweener);
            }
            foreach (var tweener in Solver._activeFixedTweeners)
            {
                Dispose(tweener);
            }

            Solver._activeTweeners.Clear();
            Solver._activeLateTweeners.Clear();
            Solver._activeFixedTweeners.Clear();
            Solver._killList.Clear();
        }

        void OnApplicationQuit()
        {
            StopAllTweeners();
        }
    }

    public enum UpdateType
    {
        Normal,
        Late,
        Fixed
    }
}
