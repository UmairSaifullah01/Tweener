using System;
using System.Collections.Generic;
using UnityEngine;
using THEBADDEST.Tweening2.Plugins.Options;

namespace THEBADDEST.Tweening2.Core
{
    internal static class TweenManager
    {
        const int _DefaultMaxTweeners = 200;
        const int _DefaultMaxSequences = 50;
        const float _EpsilonVsTimeCheck = 0.000001f;

        internal static bool isUnityEditor;
        internal static bool isDebugBuild;
        internal static int maxActive = _DefaultMaxTweeners + _DefaultMaxSequences;
        internal static int maxTweeners = _DefaultMaxTweeners;
        internal static int maxSequences = _DefaultMaxSequences;
        internal static bool hasActiveTweens, hasActiveDefaultTweens, hasActiveLateTweens, hasActiveFixedTweens, hasActiveManualTweens;
        internal static int totActiveTweens, totActiveDefaultTweens, totActiveLateTweens, totActiveFixedTweens, totActiveManualTweens;
        internal static int totActiveTweeners, totActiveSequences;
        internal static int totPooledTweeners, totPooledSequences;
        internal static int totTweeners, totSequences;
        internal static bool isUpdateLoop;

        // Arrays are organized so that existing elements are next to each other from 0 to (totActiveTweens - 1)
        internal static Tween[] _activeTweens = new Tween[_DefaultMaxTweeners + _DefaultMaxSequences];
        static Tween[] _pooledTweeners = new Tween[_DefaultMaxTweeners];
        static readonly Stack<Tween> _PooledSequences = new Stack<Tween>();

        static readonly List<Tween> _KillList = new List<Tween>(_DefaultMaxTweeners + _DefaultMaxSequences);
        static int _maxActiveLookupId = -1;
        static bool _requiresActiveReorganization;
        static int _reorganizeFromId = -1;
        static int _minPooledTweenerId = -1;
        static int _maxPooledTweenerId = -1;

        static bool _despawnAllCalledFromUpdateLoopCallback;

        #region Static Constructor

        static TweenManager()
        {
            isUnityEditor = Application.isEditor;
#if DEBUG
            isDebugBuild = true;
#endif
        }

        #endregion

        #region Main

        // Returns a new Tweener, from the pool if there's one available,
        // otherwise by instantiating a new one
        internal static TweenerCore<T1, T2, TPlugOptions> GetTweener<T1, T2, TPlugOptions>()
            where TPlugOptions : struct, IPlugOptions
        {
            TweenerCore<T1, T2, TPlugOptions> t;
            // Search inside pool
            if (totPooledTweeners > 0)
            {
                Type typeofT1 = typeof(T1);
                Type typeofT2 = typeof(T2);
                Type typeofTPlugOptions = typeof(TPlugOptions);
                for (int i = _maxPooledTweenerId; i > _minPooledTweenerId - 1; --i)
                {
                    Tween tween = _pooledTweeners[i];
                    if (tween != null && tween.typeofT1 == typeofT1 && tween.typeofT2 == typeofT2 && tween.typeofTPlugOptions == typeofTPlugOptions)
                    {
                        // Pooled Tweener exists: spawn it
                        t = (TweenerCore<T1, T2, TPlugOptions>)tween;
                        AddActiveTween(t);
                        _pooledTweeners[i] = null;
                        if (_maxPooledTweenerId != _minPooledTweenerId)
                        {
                            if (i == _maxPooledTweenerId) _maxPooledTweenerId--;
                            else if (i == _minPooledTweenerId) _minPooledTweenerId++;
                        }
                        totPooledTweeners--;
                        return t;
                    }
                }
                // Not found: remove a tween from the pool in case it's full
                if (totTweeners >= maxTweeners)
                {
                    _pooledTweeners[_maxPooledTweenerId] = null;
                    _maxPooledTweenerId--;
                    totPooledTweeners--;
                    totTweeners--;
                }
            }
            else
            {
                // Increase capacity in case max number of Tweeners has already been reached
                if (totTweeners >= maxTweeners - 1)
                {
                    int prevMaxTweeners = maxTweeners;
                    int prevMaxSequences = maxSequences;
                    IncreaseCapacities(CapacityIncreaseMode.TweenersOnly);
                }
            }
            // Not found: create new TweenerCore
            t = new TweenerCore<T1, T2, TPlugOptions>();
            totTweeners++;
            AddActiveTween(t);
            return t;
        }

        // Returns a new Sequence, from the pool if there's one available,
        // otherwise by instantiating a new one
        internal static Sequence GetSequence()
        {
            Sequence s;
            if (totPooledSequences > 0)
            {
                s = (Sequence)_PooledSequences.Pop();
                AddActiveTween(s);
                totPooledSequences--;
                return s;
            }
            // Increase capacity in case max number of Sequences has already been reached
            if (totSequences >= maxSequences - 1)
            {
                int prevMaxTweeners = maxTweeners;
                int prevMaxSequences = maxSequences;
                IncreaseCapacities(CapacityIncreaseMode.SequencesOnly);
            }
            // Not found: create new Sequence
            s = new Sequence();
            totSequences++;
            AddActiveTween(s);
            return s;
        }

        internal static void SetUpdateType(Tween t, UpdateType updateType, bool isIndependentUpdate)
        {
            if (!t.active || t.updateType == updateType)
            {
                t.updateType = updateType;
                t.isIndependentUpdate = isIndependentUpdate;
                return;
            }
            // Remove previous update type
            if (t.updateType == UpdateType.Normal)
            {
                totActiveDefaultTweens--;
                hasActiveDefaultTweens = totActiveDefaultTweens > 0;
            }
            else
            {
                switch (t.updateType)
                {
                    case UpdateType.Fixed:
                        totActiveFixedTweens--;
                        hasActiveFixedTweens = totActiveFixedTweens > 0;
                        break;
                    case UpdateType.Late:
                        totActiveLateTweens--;
                        hasActiveLateTweens = totActiveLateTweens > 0;
                        break;
                    default: // Manual
                        totActiveManualTweens--;
                        hasActiveManualTweens = totActiveManualTweens > 0;
                        break;
                }
            }
            // Assign new one
            t.updateType = updateType;
            t.isIndependentUpdate = isIndependentUpdate;
            if (updateType == UpdateType.Normal)
            {
                totActiveDefaultTweens++;
                hasActiveDefaultTweens = true;
            }
            else
            {
                switch (updateType)
                {
                    case UpdateType.Fixed:
                        totActiveFixedTweens++;
                        hasActiveFixedTweens = true;
                        break;
                    case UpdateType.Late:
                        totActiveLateTweens++;
                        hasActiveLateTweens = true;
                        break;
                    default: // Manual
                        totActiveManualTweens++;
                        hasActiveManualTweens = true;
                        break;
                }
            }
        }

        // Adds a tween to the active list
        internal static void AddActiveTween(Tween t)
        {
            if (t.activeId != -1) return; // Already added

            if (_requiresActiveReorganization) ReorganizeActiveTweens();

            // Find first available slot
            int id = -1;
            for (int i = 0; i < maxActive; ++i)
            {
                if (_activeTweens[i] == null)
                {
                    id = i;
                    break;
                }
            }

            if (id == -1)
            {
                // No available slot, increase capacity
                IncreaseCapacities(CapacityIncreaseMode.TweenersAndSequences);
                id = totActiveTweens;
            }

            _activeTweens[id] = t;
            t.activeId = id;
            t.active = true;
            if (id > _maxActiveLookupId) _maxActiveLookupId = id;

            totActiveTweens++;
            hasActiveTweens = true;

            if (t.tweenType == TweenType.Tweener)
            {
                totActiveTweeners++;
            }
            else
            {
                totActiveSequences++;
            }

            // Update type tracking
            if (t.updateType == UpdateType.Normal)
            {
                totActiveDefaultTweens++;
                hasActiveDefaultTweens = true;
            }
            else
            {
                switch (t.updateType)
                {
                    case UpdateType.Fixed:
                        totActiveFixedTweens++;
                        hasActiveFixedTweens = true;
                        break;
                    case UpdateType.Late:
                        totActiveLateTweens++;
                        hasActiveLateTweens = true;
                        break;
                    default: // Manual
                        totActiveManualTweens++;
                        hasActiveManualTweens = true;
                        break;
                }
            }
        }

        // Removes the given tween from the active tweens list
        internal static void RemoveActiveTween(Tween t)
        {
            if (t.activeId == -1) return;

            int id = t.activeId;
            _activeTweens[id] = null;
            t.activeId = -1;
            t.active = false;

            totActiveTweens--;
            hasActiveTweens = totActiveTweens > 0;

            if (t.tweenType == TweenType.Tweener)
            {
                totActiveTweeners--;
            }
            else
            {
                totActiveSequences--;
            }

            // Update type tracking
            if (t.updateType == UpdateType.Normal)
            {
                totActiveDefaultTweens--;
                hasActiveDefaultTweens = totActiveDefaultTweens > 0;
            }
            else
            {
                switch (t.updateType)
                {
                    case UpdateType.Fixed:
                        totActiveFixedTweens--;
                        hasActiveFixedTweens = totActiveFixedTweens > 0;
                        break;
                    case UpdateType.Late:
                        totActiveLateTweens--;
                        hasActiveLateTweens = totActiveLateTweens > 0;
                        break;
                    default: // Manual
                        totActiveManualTweens--;
                        hasActiveManualTweens = totActiveManualTweens > 0;
                        break;
                }
            }

            if (id == _maxActiveLookupId)
            {
                // Find new max
                _maxActiveLookupId = -1;
                for (int i = id - 1; i >= 0; --i)
                {
                    if (_activeTweens[i] != null)
                    {
                        _maxActiveLookupId = i;
                        break;
                    }
                }
            }

            if (id < _reorganizeFromId || _reorganizeFromId == -1) _reorganizeFromId = id;
            _requiresActiveReorganization = true;
        }

        // Removes the given tween from the active tweens list (for sequences)
        internal static void AddActiveTweenToSequence(Tween t)
        {
            RemoveActiveTween(t);
        }

        // Marks a tween for killing
        internal static void MarkForKilling(Tween t, bool isSingleTweenManualUpdate = false)
        {
            if (isUpdateLoop && !isSingleTweenManualUpdate)
            {
                _KillList.Add(t);
            }
            else
            {
                Despawn(t);
            }
        }

        // Despawn all
        internal static int DespawnAll()
        {
            int totDespawned = totActiveTweens;
            for (int i = 0; i < _maxActiveLookupId + 1; ++i)
            {
                Tween t = _activeTweens[i];
                if (t != null) Despawn(t, false);
            }
            ClearTweenArray(_activeTweens);
            hasActiveTweens = hasActiveDefaultTweens = hasActiveLateTweens = hasActiveFixedTweens = hasActiveManualTweens = false;
            totActiveTweens = totActiveDefaultTweens = totActiveLateTweens = totActiveFixedTweens = totActiveManualTweens = 0;
            totActiveTweeners = totActiveSequences = 0;
            _maxActiveLookupId = _reorganizeFromId = -1;
            _requiresActiveReorganization = false;

            if (isUpdateLoop) _despawnAllCalledFromUpdateLoopCallback = true;

            return totDespawned;
        }

        internal static void Despawn(Tween t, bool modifyActiveLists = true)
        {
            // Callbacks
            if (t.onKill != null) Tween.OnTweenCallback(t.onKill, t);

            if (modifyActiveLists)
            {
                // Remove tween from active list
                RemoveActiveTween(t);
            }
            if (t.isRecyclable)
            {
                // Put the tween inside a pool
                switch (t.tweenType)
                {
                    case TweenType.Sequence:
                        _PooledSequences.Push(t);
                        totPooledSequences++;
                        // Despawn sequenced tweens
                        Sequence s = (Sequence)t;
                        int len = s.sequencedTweens.Count;
                        for (int i = 0; i < len; ++i) Despawn(s.sequencedTweens[i], false);
                        break;
                    case TweenType.Tweener:
                        if (_maxPooledTweenerId == -1)
                        {
                            _maxPooledTweenerId = maxTweeners - 1;
                            _minPooledTweenerId = maxTweeners - 1;
                        }
                        if (_maxPooledTweenerId < maxTweeners - 1)
                        {
                            _pooledTweeners[_maxPooledTweenerId + 1] = t;
                            _maxPooledTweenerId++;
                            if (_minPooledTweenerId > _maxPooledTweenerId) _minPooledTweenerId = _maxPooledTweenerId;
                        }
                        else
                        {
                            for (int i = _maxPooledTweenerId; i > -1; --i)
                            {
                                if (_pooledTweeners[i] != null) continue;
                                _pooledTweeners[i] = t;
                                if (i < _minPooledTweenerId) _minPooledTweenerId = i;
                                if (_maxPooledTweenerId < _minPooledTweenerId) _maxPooledTweenerId = _minPooledTweenerId;
                                break;
                            }
                        }
                        totPooledTweeners++;
                        break;
                }
            }
            else
            {
                // Remove
                switch (t.tweenType)
                {
                    case TweenType.Sequence:
                        totSequences--;
                        // Despawn sequenced tweens
                        Sequence s = (Sequence)t;
                        int len = s.sequencedTweens.Count;
                        for (int i = 0; i < len; ++i) Despawn(s.sequencedTweens[i], false);
                        break;
                    case TweenType.Tweener:
                        totTweeners--;
                        break;
                }
            }
            t.active = false;
            t.Reset();
        }

        // deltaTime will be passed as fixedDeltaTime in case of UpdateType.Fixed
        internal static void Update(UpdateType updateType, float deltaTime, float independentTime)
        {
            if (_requiresActiveReorganization) ReorganizeActiveTweens();

            isUpdateLoop = true;
            bool willKill = false;
            int len = _maxActiveLookupId + 1;
            for (int i = 0; i < len; ++i)
            {
                Tween t = _activeTweens[i];
                if (t == null || t.updateType != updateType) continue;
                if (Update(t, deltaTime, independentTime, false)) willKill = true;
            }
            // Kill all eventually marked tweens
            if (willKill)
            {
                if (_despawnAllCalledFromUpdateLoopCallback)
                {
                    _despawnAllCalledFromUpdateLoopCallback = false;
                }
                else
                {
                    DespawnActiveTweens(_KillList);
                }
                _KillList.Clear();
            }
            isUpdateLoop = false;
        }

        // deltaTime will be passed as fixedDeltaTime in case of UpdateType.Fixed
        // Returns TRUE if the tween should be killed
        internal static bool Update(Tween t, float deltaTime, float independentTime, bool isSingleTweenManualUpdate)
        {
            if (!t.active) return false;
            if (!t.isPlaying) return false;
            t.creationLocked = true;
            float tDeltaTime = (t.isIndependentUpdate ? independentTime : deltaTime) * t.timeScale;
            if (tDeltaTime < _EpsilonVsTimeCheck && tDeltaTime > -_EpsilonVsTimeCheck) return false;
            if (!t.delayComplete)
            {
                tDeltaTime = t.UpdateDelay(t.elapsedDelay + tDeltaTime);
                if (tDeltaTime <= -1)
                {
                    MarkForKilling(t, isSingleTweenManualUpdate);
                    return true;
                }
                if (tDeltaTime <= 0) return false;
                if (t.playedOnce && t.onPlay != null)
                {
                    Tween.OnTweenCallback(t.onPlay, t);
                }
            }
            // Startup
            if (!t.startupDone)
            {
                if (!t.Startup())
                {
                    MarkForKilling(t, isSingleTweenManualUpdate);
                    return true;
                }
            }
            // Find update data
            float toPosition = t.position;
            bool wasEndPosition = toPosition >= t.duration;
            int toCompletedLoops = t.completedLoops;
            if (t.duration <= 0)
            {
                toPosition = 0;
                toCompletedLoops = t.loops == -1 ? t.completedLoops + 1 : t.loops;
            }
            else
            {
                if (t.isBackwards)
                {
                    toPosition -= tDeltaTime;
                    while (toPosition < 0 && toCompletedLoops > -1)
                    {
                        toPosition += t.duration;
                        toCompletedLoops--;
                    }
                    if (toCompletedLoops < 0 || wasEndPosition && toCompletedLoops < 1)
                    {
                        toPosition = 0;
                        toCompletedLoops = wasEndPosition ? 1 : 0;
                    }
                }
                else
                {
                    toPosition += tDeltaTime;
                    while (toPosition >= t.duration && (t.loops == -1 || toCompletedLoops < t.loops))
                    {
                        toPosition -= t.duration;
                        toCompletedLoops++;
                    }
                }
                if (wasEndPosition) toCompletedLoops--;
                if (t.loops != -1 && toCompletedLoops >= t.loops) toPosition = t.duration;
            }
            // Goto
            bool needsKilling = Tween.DoGoto(t, toPosition, toCompletedLoops, UpdateMode.Update);
            if (needsKilling)
            {
                MarkForKilling(t, isSingleTweenManualUpdate);
                return true;
            }
            return false;
        }

        internal static void Goto(Tween t, float to, bool andPlay = false, UpdateMode updateMode = UpdateMode.Goto)
        {
            bool wasPlaying = t.isPlaying;
            t.isPlaying = andPlay;
            t.delayComplete = true;
            t.elapsedDelay = t.delay;
            int toCompletedLoops = t.duration <= 0 ? 1 : Mathf.FloorToInt(to / t.duration);
            float toPosition = to % t.duration;
            if (t.loops != -1 && toCompletedLoops >= t.loops)
            {
                toCompletedLoops = t.loops;
                toPosition = t.duration;
            }
            Tween.DoGoto(t, toPosition, toCompletedLoops, updateMode);
        }

        internal static bool Play(Tween t)
        {
            if (t.isComplete && t.loops != -1) return false;
            t.isPlaying = true;
            return true;
        }

        internal static bool Pause(Tween t)
        {
            if (!t.isPlaying) return false;
            t.isPlaying = false;
            return true;
        }

        internal static void PauseAll()
        {
            for (int i = 0; i <= TweenManager._maxActiveLookupId; ++i)
            {
                Core.Tween t = TweenManager._activeTweens[i];
                if (t != null && t.active) TweenManager.Pause(t);
            }
        }

        internal static void PlayAll()
        {
            for (int i = 0; i <= TweenManager._maxActiveLookupId; ++i)
            {
                Core.Tween t = TweenManager._activeTweens[i];
                if (t != null && t.active) TweenManager.Play(t);
            }
        }
        static void DespawnActiveTweens(List<Tween> tweens)
        {
            int len = tweens.Count;
            for (int i = 0; i < len; ++i)
            {
                Tween t = tweens[i];
                if (t != null && t.activeId != -1) Despawn(t, false);
            }
        }

        static void ReorganizeActiveTweens()
        {
            if (_reorganizeFromId == -1) return;
            int len = _maxActiveLookupId + 1;
            for (int i = _reorganizeFromId; i < len; ++i)
            {
                Tween t = _activeTweens[i];
                if (t == null) continue;
                if (i != t.activeId)
                {
                    _activeTweens[i] = null;
                    _activeTweens[t.activeId] = t;
                }
            }
            _requiresActiveReorganization = false;
            _reorganizeFromId = -1;
        }

        static void ClearTweenArray(Tween[] array)
        {
            int len = array.Length;
            for (int i = 0; i < len; ++i) array[i] = null;
        }

        enum CapacityIncreaseMode
        {
            TweenersAndSequences,
            TweenersOnly,
            SequencesOnly
        }

        static void IncreaseCapacities(CapacityIncreaseMode increaseMode)
        {
            int increase = increaseMode == CapacityIncreaseMode.SequencesOnly ? 50 : 200;
            maxTweeners += increase;
            if (increaseMode != CapacityIncreaseMode.TweenersOnly) maxSequences += increase / 4;
            maxActive = maxTweeners + maxSequences;
            Array.Resize(ref _activeTweens, maxActive);
            Array.Resize(ref _pooledTweeners, maxTweeners);
            _KillList.Capacity = maxActive;
        }
        #endregion
    }
}
