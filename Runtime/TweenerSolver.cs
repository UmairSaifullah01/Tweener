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
                    go.hideFlags    = HideFlags.HideInHierarchy;
                    DontDestroyOnLoad(go);
                }
                return _SolverInstance;
            }
        }

        private Dictionary<ITweener, Coroutine> _tweens = new Dictionary<ITweener, Coroutine>();

        public static void PlayTweener(ITweener tweener, IEnumerator coroutine)
        {
            Solver._tweens[tweener] = Solver.StartCoroutine(coroutine);
        }

        public static void StopTweener(ITweener tweener)
        {
            if(tweener==null) return;
            if (Solver._tweens.TryGetValue(tweener, out Coroutine coroutine))
            {
                Solver.StopCoroutine(coroutine);
                Solver._tweens.Remove(tweener);
            }
        }

        public static void StopAllTweeners()
        {
            foreach (Coroutine coroutine in Solver._tweens.Values)
            {
                Solver.StopCoroutine(coroutine);
            }
            Solver._tweens.Clear();
        }
    }
}
