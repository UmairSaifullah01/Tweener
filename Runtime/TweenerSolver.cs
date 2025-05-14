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

        private Dictionary<ITweener, Coroutine> _tweens = new Dictionary<ITweener, Coroutine>();

        TweenerFactory factory;

        public static ITweener Create()
        {
            return Solver.factory.Create();
        }
        public static void Dispose(ITweener tweener)
        {
            Solver.factory.Dispose(tweener);
        }

        public static void PlayTweener(ITweener tweener, IEnumerator coroutine)
        {
            Solver._tweens[tweener] = Solver.StartCoroutine(coroutine);
        }

        public static void PlayTweener(IEnumerator coroutine)
        {
            Solver.StartCoroutine(coroutine);
        }
        public static void StopTweener(ITweener tweener)
        {
            if (tweener == null) return;
            if (Solver._tweens.TryGetValue(tweener, out Coroutine coroutine))
            {
                Solver.StopCoroutine(coroutine);
                Solver._tweens.Remove(tweener);
                Dispose(tweener);
            }
        }

        public static void StopAllTweeners()
        {
            foreach (var tween in Solver._tweens)
            {
                Solver.StopCoroutine(tween.Value);
                Dispose(tween.Key);
            }
            Solver._tweens.Clear();
        }

        void OnApplicationQuit()
        {
            StopAllCoroutines();
        }

    }
}
