using UnityEngine;


namespace THEBADDEST.Tweening2.Core
{
    /// <summary>
    /// Used to separate Tween class from the MonoBehaviour instance.
    /// Contains all instance-based methods
    /// </summary>
    [AddComponentMenu("")]
    public class TweenComponent : MonoBehaviour
    {
        /// <summary>Used internally inside Unity Editor</summary>
        public int inspectorUpdater;

        float _unscaledTime;
        float _unscaledDeltaTime;

        bool _paused;
        float _pausedTime;
        bool _isQuitting;

        bool _duplicateToDestroy;

        static TweenComponent _instance;

        #region Unity Methods

        void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                inspectorUpdater = 0;
                _unscaledTime = Time.realtimeSinceStartup;
                DontDestroyOnLoad(gameObject);
                gameObject.hideFlags = HideFlags.HideInHierarchy;
            }
            else
            {
                if (Application.isEditor)
                {
                    TweenLog.LogWarning("Duplicate TweenComponent instance found in scene: destroying it");
                }
                Destroy(gameObject);
            }
        }

        void Start()
        {
            if (_instance != this)
            {
                _duplicateToDestroy = true;
                Destroy(gameObject);
            }
        }

        void Update()
        {
            _unscaledDeltaTime = Time.realtimeSinceStartup - _unscaledTime;
            if (TweenManager.hasActiveDefaultTweens)
            {
                TweenManager.Update(UpdateType.Normal, Time.deltaTime * TweenCore.timeScale, _unscaledDeltaTime * TweenCore.unscaledTimeScale * TweenCore.timeScale);
            }
            _unscaledTime = Time.realtimeSinceStartup;

            if (TweenManager.isUnityEditor)
            {
                inspectorUpdater++;
            }
        }

        void LateUpdate()
        {
            if (TweenManager.hasActiveLateTweens)
            {
                TweenManager.Update(UpdateType.Late, Time.deltaTime * TweenCore.timeScale, _unscaledDeltaTime * TweenCore.unscaledTimeScale * TweenCore.timeScale);
            }
        }

        void FixedUpdate()
        {
            if (TweenManager.hasActiveFixedTweens && Time.timeScale > 0)
            {
                TweenManager.Update(UpdateType.Fixed, Time.fixedDeltaTime * TweenCore.timeScale, (Time.fixedDeltaTime / Time.timeScale) * TweenCore.unscaledTimeScale * TweenCore.timeScale);
            }
        }

        void OnDestroy()
        {
            if (_duplicateToDestroy) return;

            if (_instance == this) _instance = null;
            TweenManager.DespawnAll();
        }

        public void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                _paused = true;
                _pausedTime = Time.realtimeSinceStartup;
            }
            else if (_paused)
            {
                _paused = false;
                _unscaledTime += Time.realtimeSinceStartup - _pausedTime;
            }
        }

        void OnApplicationQuit()
        {
            _isQuitting = true;
        }

        #endregion

        internal static TweenComponent GetInstance()
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("TweenComponent");
                _instance = go.AddComponent<TweenComponent>();
            }
            return _instance;
        }
    }
}
