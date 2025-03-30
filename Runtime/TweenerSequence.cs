using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace THEBADDEST.Tweening
{
    /// <summary>
    /// A class that manages a sequence of tweens with support for parallel and sequential execution.
    /// </summary>
    public class TweenerSequence
    {
        private class TweenGroup
        {
            public List<ITweener> Tweens = new List<ITweener>();
            public bool IsParallel { get; set; }
        }

        private List<TweenGroup> tweenGroups = new List<TweenGroup>();
        private TweenGroup currentGroup;
        private bool isPlaying = false;

        public TweenerSequence()
        {
            currentGroup = new TweenGroup { IsParallel = true };
            tweenGroups.Add(currentGroup);
        }

        /// <summary>
        /// Adds a tweener to the sequence that will run in parallel with the current tweens.
        /// </summary>
        /// <param name="tweener">The tweener to add</param>
        /// <returns>The sequence instance for method chaining</returns>
        public TweenerSequence Join(ITweener tweener)
        {
            if (tweener == null) return this;

            // If current group is not parallel, create a new parallel group
            if (!currentGroup.IsParallel)
            {
                currentGroup = new TweenGroup { IsParallel = true };
                tweenGroups.Add(currentGroup);
            }

            currentGroup.Tweens.Add(tweener);
            return this;
        }

        /// <summary>
        /// Adds a tweener to the sequence that will run after all current tweens complete.
        /// </summary>
        /// <param name="tweener">The tweener to append</param>
        /// <returns>The sequence instance for method chaining</returns>
        public TweenerSequence Append(ITweener tweener)
        {
            if (tweener == null) return this;

            // If current group is parallel, create a new sequential group
            if (currentGroup.IsParallel)
            {
                currentGroup = new TweenGroup { IsParallel = false };
                tweenGroups.Add(currentGroup);
            }

            currentGroup.Tweens.Add(tweener);
            return this;
        }

        /// <summary>
        /// Starts playing the sequence of tweens.
        /// </summary>
        /// <returns>A coroutine that can be used to wait for the sequence to complete</returns>
        public IEnumerator Play()
        {
            if (isPlaying) yield break;
            isPlaying = true;
            Debug.Log("TweenerSequence: Play started.");
            yield return null;
            // First, pause all tweens
            foreach (var group in tweenGroups)
            {
                foreach (var tweener in group.Tweens)
                {
                    if (tweener != null)
                    {
                        tweener.Pause();
                        Debug.Log("Tweener paused.");
                    }
                }
            }

           
            // Play each group in sequence
            foreach (var group in tweenGroups)
            {
                if (group.Tweens.Count == 0) continue;

                // Start all tweens in the current group
                foreach (var tweener in group.Tweens)
                {
                    if (tweener != null)
                    {
                        tweener.Resume();
                        Debug.Log("Tweener resumed.");
                    }
                }

                // Wait for all tweens in the current group to complete
                foreach (var tweener in group.Tweens)
                {
                    if (tweener != null)
                    {
                        yield return tweener.WaitForCompletion();
                        Debug.Log("Tweener completed.");
                    }
                }
            }

            isPlaying = false;
            Debug.Log("TweenerSequence: Play finished.");
        }

        /// <summary>
        /// Stops all tweens in the sequence.
        /// </summary>
        public void Kill()
        {
            foreach (var group in tweenGroups)
            {
                foreach (var tweener in group.Tweens)
                {
                    if (tweener != null)
                    {
                        tweener.Kill();
                    }
                }
            }
            isPlaying = false;
        }

        /// <summary>
        /// Pauses all tweens in the sequence.
        /// </summary>
        public void Pause()
        {
            foreach (var group in tweenGroups)
            {
                foreach (var tweener in group.Tweens)
                {
                    if (tweener != null)
                    {
                        tweener.Pause();
                    }
                }
            }
        }

        /// <summary>
        /// Resumes all tweens in the sequence.
        /// </summary>
        public void Resume()
        {
            foreach (var group in tweenGroups)
            {
                foreach (var tweener in group.Tweens)
                {
                    if (tweener != null)
                    {
                        tweener.Resume();
                    }
                }
            }
        }

        /// <summary>
        /// Clears all tweens from the sequence.
        /// </summary>
        public void Clear()
        {
            tweenGroups.Clear();
            currentGroup = new TweenGroup { IsParallel = true };
            tweenGroups.Add(currentGroup);
            isPlaying = false;
        }
    }
}