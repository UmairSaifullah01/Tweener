using System;
using System.Collections;
using UnityEngine;


namespace THEBADDEST.Tweening
{
	
	public abstract class Tween : ScriptableObject, ITween
	{
		[SerializeField] int m_priority = 0;
		[SerializeField] protected float m_delay = 0;
		[SerializeField] protected float duration = 1;
		[SerializeField] protected TweenerEasing.Ease ease = TweenerEasing.Ease.Linear;
		[SerializeField] protected AnimationCurve easeCurve = AnimationCurve.Linear(0, 0, 1, 1);
		[SerializeField][Range(-1, 100)] protected int loops = 1;
		[SerializeField] protected LoopType loopType = LoopType.Linear;
		
		public event Action onComplete;
		public ITweener tweener { get; protected set; }

		public int priority
		{
			get => m_priority;
			protected set => m_priority = value;
		}
		public float delay
		{
			get => m_delay;
			set => m_delay = value;
		}
		protected void Init()
		{
			tweener.SetLoops(loops, loopType).SetEase(easeCurve).SetEase(ease).SetDelay(delay);
		}
		public virtual IEnumerator Play(Transform target)
		{
			yield return tweener.WaitForCompletion();
			onComplete?.Invoke();
		}

	}
	public abstract class FromToTween : Tween
	{

		[SerializeField] protected Vector3 from, to;

	}


}