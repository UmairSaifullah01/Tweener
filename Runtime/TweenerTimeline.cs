using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace THEBADDEST.Tweening
{
	/// <summary>
	/// A timeline that allows scheduling tweens and callbacks at specific times.
	/// </summary>
	public class TweenerTimeline 
	{
		private class TimelineEntry
		{
			public float Time { get; set; }
			public ITweener Tweener { get; set; }
			public Action Callback { get; set; }
			public bool IsExecuted { get; set; }
			public List<TimelineEntry> Dependencies { get; set; } = new List<TimelineEntry>();
			public List<TimelineEntry> Dependents { get; set; } = new List<TimelineEntry>();
		}

		private List<TimelineEntry> entries = new List<TimelineEntry>();
		private TimelineEntry lastEntry = null;
		private TimelineEntry currentGroup = null;
		private bool isPlaying = false;
		private bool isPaused = false;
		private float currentTime = 0f;
		private float totalDuration = 0f;
		private bool isReversed = false;
		private int loopCount = 1;
		private int currentLoop = 0;

		/// <summary>
		/// Event triggered when the timeline completes.
		/// </summary>
		public event Action OnComplete;

		/// <summary>
		/// Gets whether the timeline is currently playing.
		/// </summary>
		public bool IsPlaying => isPlaying;

		/// <summary>
		/// Gets whether the timeline is currently paused.
		/// </summary>
		public bool IsPaused => isPaused;

		/// <summary>
		/// Gets the total duration of the timeline.
		/// </summary>
		public float TotalDuration => totalDuration;

		/// <summary>
		/// Gets the current progress of the timeline (0 to 1).
		/// </summary>
		public float Progress => totalDuration > 0 ? currentTime / totalDuration : 0f;

		/// <summary>
		/// Gets the current time of the timeline.
		/// </summary>
		public float CurrentTime => currentTime;

		/// <summary>
		/// Sets the number of times the timeline should loop. Use -1 for infinite loops.
		/// </summary>
		public TweenerTimeline SetLoops(int count)
		{
			loopCount = count;
			return this;
		}

		/// <summary>
		/// Adds a tweener to the timeline at the specified time.
		/// </summary>
		/// <param name="time">Time in seconds when the tweener should start</param>
		/// <param name="tweener">The tweener to add</param>
		/// <returns>The timeline instance for method chaining</returns>
		public TweenerTimeline Insert(float time, ITweener tweener)
		{
			if (tweener == null) return this;
			
			tweener.Pause();
			var entry = new TimelineEntry
			{
				Time = time,
				Tweener = tweener,
				IsExecuted = false
			};

			entries.Add(entry);
			lastEntry = entry;
			currentGroup = null;

			// Update total duration
			totalDuration = Mathf.Max(totalDuration, time + tweener.Duration);
			
			return this;
		}

		/// <summary>
		/// Adds a callback to the timeline at the specified time.
		/// </summary>
		/// <param name="time">Time in seconds when the callback should be executed</param>
		/// <param name="callback">The callback to execute</param>
		/// <returns>The timeline instance for method chaining</returns>
		public TweenerTimeline Insert(float time, Action callback)
		{
			if (callback == null) return this;
			
			var entry = new TimelineEntry
			{
				Time = time,
				Callback = callback,
				IsExecuted = false
			};

			entries.Add(entry);
			lastEntry = entry;
			currentGroup = null;

			// Update total duration
			totalDuration = Mathf.Max(totalDuration, time);
			
			return this;
		}

		/// <summary>
		/// Adds a tweener that will execute after the last added tweener completes.
		/// </summary>
		public TweenerTimeline Append(ITweener tweener)
		{
			if (tweener == null || lastEntry == null) return this;
			
			tweener.Pause();
			var entry = new TimelineEntry
			{
				Time = lastEntry.Time + (lastEntry.Tweener?.Duration ?? 0f),
				Tweener = tweener,
				IsExecuted = false
			};

			// Add dependency
			entry.Dependencies.Add(lastEntry);
			lastEntry.Dependents.Add(entry);

			entries.Add(entry);
			lastEntry = entry;
			currentGroup = null;

			// Update total duration
			totalDuration = Mathf.Max(totalDuration, entry.Time + tweener.Duration);
			
			return this;
		}

		/// <summary>
		/// Adds a callback that will execute after the last added tweener completes.
		/// </summary>
		public TweenerTimeline Append(Action callback)
		{
			if (callback == null || lastEntry == null) return this;
			
			var entry = new TimelineEntry
			{
				Time = lastEntry.Time + (lastEntry.Tweener?.Duration ?? 0f),
				Callback = callback,
				IsExecuted = false
			};

			// Add dependency
			entry.Dependencies.Add(lastEntry);
			lastEntry.Dependents.Add(entry);

			entries.Add(entry);
			lastEntry = entry;
			currentGroup = null;

			// Update total duration
			totalDuration = Mathf.Max(totalDuration, entry.Time);
			
			return this;
		}

		/// <summary>
		/// Adds a tweener that will execute in parallel with the current group.
		/// </summary>
		public TweenerTimeline Join(ITweener tweener)
		{
			if (tweener == null) return this;
			
			tweener.Pause();
			var entry = new TimelineEntry
			{
				Time = lastEntry?.Time ?? 0f,
				Tweener = tweener,
				IsExecuted = false
			};

			if (currentGroup == null)
			{
				currentGroup = entry;
			}
			else
			{
				// Add to current group's dependencies
				entry.Dependencies.AddRange(currentGroup.Dependencies);
				foreach (var dep in entry.Dependencies)
				{
					dep.Dependents.Add(entry);
				}
			}

			entries.Add(entry);
			lastEntry = entry;

			// Update total duration
			totalDuration = Mathf.Max(totalDuration, entry.Time + tweener.Duration);
			
			return this;
		}

		/// <summary>
		/// Adds a callback that will execute in parallel with the current group.
		/// </summary>
		public TweenerTimeline Join(Action callback)
		{
			if (callback == null) return this;
			
			var entry = new TimelineEntry
			{
				Time = lastEntry?.Time ?? 0f,
				Callback = callback,
				IsExecuted = false
			};

			if (currentGroup == null)
			{
				currentGroup = entry;
			}
			else
			{
				// Add to current group's dependencies
				entry.Dependencies.AddRange(currentGroup.Dependencies);
				foreach (var dep in entry.Dependencies)
				{
					dep.Dependents.Add(entry);
				}
			}

			entries.Add(entry);
			lastEntry = entry;

			// Update total duration
			totalDuration = Mathf.Max(totalDuration, entry.Time);
			
			return this;
		}

		/// <summary>
		/// Starts playing the timeline.
		/// </summary>
		/// <returns>A coroutine that can be used to wait for the timeline to complete</returns>
		public void Play()
		{
			if (entries.Count == 0) return;
			
			if (isPlaying && !isPaused)
			{
				Stop();
			}

			TweenerSolver.PlayTweener(Execute());
		}

		/// <summary>
		/// Pauses the timeline.
		/// </summary>
		public void Pause()
		{
			if (!isPlaying || isPaused) return;
			
			isPaused = true;
			foreach (var entry in entries)
			{
				entry.Tweener?.Pause();
			}
		}

		/// <summary>
		/// Resumes the timeline from pause.
		/// </summary>
		public void Resume()
		{
			if (!isPlaying || !isPaused) return;
			
			isPaused = false;
			foreach (var entry in entries)
			{
				if (!entry.IsExecuted)
				{
					entry.Tweener?.Resume();
				}
			}
		}

		/// <summary>
		/// Reverses the timeline direction.
		/// </summary>
		public void Reverse()
		{
			isReversed = !isReversed;
			foreach (var entry in entries)
			{
				entry.Tweener?.Reverse();
			}
		}

		private IEnumerator Execute()
		{
			yield return null;
			
			if (isPlaying) yield break;
			
			isPlaying = true;
			isPaused = false;
			currentTime = 0f;
			currentLoop = 0;
			
			// Sort entries by time
			entries.Sort((a, b) => a.Time.CompareTo(b.Time));
			
			do
			{
				// Reset execution state
				foreach (var entry in entries)
				{
					entry.IsExecuted = false;
				}

				int currentIndex = 0;
				while (currentIndex < entries.Count)
				{
					if (isPaused)
					{
						yield return null;
						continue;
					}

					var entry = entries[currentIndex];
					if (currentTime >= entry.Time && !entry.IsExecuted)
					{
						// Check if all dependencies are executed
						bool canExecute = true;
						foreach (var dep in entry.Dependencies)
						{
							if (!dep.IsExecuted)
							{
								canExecute = false;
								break;
							}
						}

						if (canExecute)
						{
							if (entry.Tweener != null)
							{
								entry.Tweener.Resume();
							}
							entry.Callback?.Invoke();
							entry.IsExecuted = true;
							currentIndex++;
						}
						else
						{
							yield return null;
						}
					}
					else
					{
						currentTime += Time.deltaTime;
						yield return null;
					}
				}

				currentLoop++;
				if (loopCount > 0 && currentLoop >= loopCount)
				{
					break;
				}

				// Reset for next loop
				currentTime = 0f;
			} while (true);

			isPlaying = false;
			isPaused = false;
			OnComplete?.Invoke();
		}

		/// <summary>
		/// Stops the timeline and resets all tweens.
		/// </summary>
		public void Stop()
		{
			if (!isPlaying) return;
			
			isPlaying = false;
			isPaused = false;
			currentTime = 0f;
			currentLoop = 0;
			
			foreach (var entry in entries)
			{
				entry.IsExecuted = false;
				entry.Tweener?.Kill();
			}
		}

		/// <summary>
		/// Clears all entries from the timeline.
		/// </summary>
		public void Clear()
		{
			Stop();
			entries.Clear();
			lastEntry = null;
			currentGroup = null;
			totalDuration = 0f;
			OnComplete = null;
		}
	}
}