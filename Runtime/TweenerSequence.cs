using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace THEBADDEST.Tweening
{
	/// <summary>
	/// A sequence that can control multiple tweens and callbacks in order.
	/// Similar to DOTween's Sequence system.
	/// </summary>
	public class TweenerSequence : ITweener
	{
		private List<SequenceItem> _sequencedItems = new List<SequenceItem>();
		private float _duration = 0f;
		private float _position = 0f;
		private int _loops = 1;
		private LoopType _loopType = LoopType.Linear;
		private bool _isPlaying = false;
		private bool _isPaused = false;
		private int _currentLoop = 0;
		private bool _startupDone = false;
		private float _delay = 0f;
		private float _elapsedDelay = 0f;
		private bool _delayComplete = false;
		private UpdateType _updateType = UpdateType.Normal;

		public event CallbackDelegate OnCompleteAllLoops;
		public event CallbackDelegate OnCompleteIteration;

		public float Duration => _duration;
		public bool IsPlaying => _isPlaying;

		private class SequenceItem
		{
			public ITweener tweener;
			public CallbackDelegate callback;
			public float position;
			public float endPosition;
			public bool isCallback;
		}

		public TweenerSequence()
		{
		}

		public TweenerSequence Append(ITweener tweener)
		{
			if (tweener == null) return this;

			float insertPosition = _duration;
			float tweenerDuration = tweener.Duration;
			
			var item = new SequenceItem
			{
				tweener = tweener,
				position = insertPosition,
				endPosition = insertPosition + tweenerDuration,
				isCallback = false
			};

			_sequencedItems.Add(item);
			_duration = Mathf.Max(_duration, item.endPosition);

			// Remove tweener from active list (sequence will manage it)
			TweenerSolver.RemoveActiveTweener(tweener);

			return this;
		}

		public TweenerSequence AppendCallback(CallbackDelegate callback)
		{
			if (callback == null) return this;

			float insertPosition = _duration;
			
			var item = new SequenceItem
			{
				callback = callback,
				position = insertPosition,
				endPosition = insertPosition,
				isCallback = true
			};

			_sequencedItems.Add(item);
			_duration = Mathf.Max(_duration, insertPosition);

			return this;
		}

		public TweenerSequence AppendInterval(float interval)
		{
			_duration += interval;
			return this;
		}

		public TweenerSequence Insert(float atPosition, ITweener tweener)
		{
			if (tweener == null) return this;

			float tweenerDuration = tweener.Duration;
			
			var item = new SequenceItem
			{
				tweener = tweener,
				position = atPosition,
				endPosition = atPosition + tweenerDuration,
				isCallback = false
			};

			_sequencedItems.Add(item);
			_duration = Mathf.Max(_duration, item.endPosition);

			TweenerSolver.RemoveActiveTweener(tweener);

			return this;
		}

		public TweenerSequence InsertCallback(float atPosition, CallbackDelegate callback)
		{
			if (callback == null) return this;

			var item = new SequenceItem
			{
				callback = callback,
				position = atPosition,
				endPosition = atPosition,
				isCallback = true
			};

			_sequencedItems.Add(item);
			_duration = Mathf.Max(_duration, atPosition);

			return this;
		}

		public ITweener SetLoops(int loops, LoopType loopType = LoopType.Linear)
		{
			_loops = loops;
			_loopType = loopType;
			return this;
		}

		public ITweener SetDelay(float seconds)
		{
			_delay = seconds;
			_elapsedDelay = 0f;
			_delayComplete = seconds <= 0f;
			return this;
		}

		public TweenerSequence SetUpdateType(UpdateType updateType)
		{
			_updateType = updateType;
			return this;
		}

		public void Play()
		{
			if (_isPlaying) return;

			_isPlaying = true;
			_isPaused = false;
			_position = 0f;
			_currentLoop = 0;
			_startupDone = false;
			_elapsedDelay = 0f;
			_delayComplete = _delay <= 0f;

			// Sort items by position
			_sequencedItems.Sort((a, b) => a.position.CompareTo(b.position));

			TweenerSolver.AddActiveTweener(this, _updateType);
		}

		public void Pause()
		{
			_isPaused = true;
		}

		public void Resume()
		{
			_isPaused = false;
		}

		public void Kill()
		{
			_isPlaying = false;
			TweenerSolver.MarkForKilling(this);
			OnCompleteAllLoops?.Invoke();
		}

		public bool UpdateTween(float deltaTime, float unscaledDeltaTime)
		{
			if (!_isPlaying || _isPaused) return false;

			float tDeltaTime = unscaledDeltaTime; // Sequences use unscaled time by default
			
			// Handle delay
			if (!_delayComplete)
			{
				_elapsedDelay += tDeltaTime;
				if (_elapsedDelay >= _delay)
				{
					_delayComplete = true;
					tDeltaTime = _elapsedDelay - _delay;
				}
				else
				{
					return false; // Still in delay
				}
			}

			if (!_startupDone)
			{
				_startupDone = true;
				_position = 0f;
				_currentLoop = 0;
			}

			_position += tDeltaTime;

			// Handle loops
			while (_position >= _duration && (_loops == -1 || _currentLoop < _loops))
			{
				_position -= _duration;
				_currentLoop++;
				OnCompleteIteration?.Invoke();

				if (_loops != -1 && _currentLoop >= _loops)
				{
					_position = _duration;
					break;
				}
			}

			// Update sequenced items
			bool isBackwards = _loopType == LoopType.Yoyo && _currentLoop % 2 == 1;
			float currentPos = isBackwards ? _duration - _position : _position;

			foreach (var item in _sequencedItems)
			{
				if (item.isCallback)
				{
					// Handle callbacks - fire when we pass the position
					if (currentPos >= item.position && currentPos < item.position + 0.016f) // ~1 frame tolerance
					{
						item.callback?.Invoke();
					}
				}
				else if (item.tweener != null)
				{
					// Update nested tweener
					float tweenerPos = Mathf.Clamp(currentPos - item.position, 0f, item.endPosition - item.position);
					float tweenerProgress = (item.endPosition - item.position) > 0 
						? tweenerPos / (item.endPosition - item.position) 
						: 1f;

					// Manually update nested tweener position
					if (item.tweener is Tweener t)
					{
						t.SetPosition(tweenerProgress);
					}
				}
			}

			// Check if complete
			bool isComplete = (_loops != -1 && _currentLoop >= _loops && _position >= _duration);
			if (isComplete)
			{
				_isPlaying = false;
				OnCompleteAllLoops?.Invoke();
				return true; // Mark for killing
			}

			return false;
		}

		// ITweener interface implementation
		public ITweener SetEase(TweenerEasing.Ease ease) { return this; }
		public ITweener SetEase(AnimationCurve easeCurve) { return this; }
		// public ITweener SetLoops(int loops, LoopType loopType) 
		// { 
		// 	SetLoops(loops, loopType);
		// 	return this;
		// }
		// public ITweener SetDelay(float seconds) 
		// { 
		// 	SetDelay(seconds);
		// 	return this;
		// }
		public ITweener SetTime(bool independent = false) { return this; }
		public void Lerp(LerpDelegate lerp, float duration) { }
		public void Reverse() { }
		public void Reset() 
		{
			_sequencedItems.Clear();
			_duration = 0f;
			_position = 0f;
			_loops = 1;
			_loopType = LoopType.Linear;
			_isPlaying = false;
			_isPaused = false;
			_currentLoop = 0;
			_startupDone = false;
			_delay = 0f;
			_elapsedDelay = 0f;
			_delayComplete = false;
			OnCompleteAllLoops = null;
			OnCompleteIteration = null;
		}
		public IEnumerator WaitForCompletion() 
		{ 
			yield return new WaitUntil(() => !_isPlaying); 
		}
		public ITweener OnComplete(CallbackDelegate onComplete, bool singleIteration = false)
		{
			if (singleIteration) OnCompleteIteration += onComplete;
			else OnCompleteAllLoops += onComplete;
			return this;
		}
	}

	/// <summary>
	/// Extension methods for creating sequences.
	/// </summary>
	public static class TweenerSequenceExtensions
	{
		public static TweenerSequence CreateSequence()
		{
			return new TweenerSequence();
		}
	}
}