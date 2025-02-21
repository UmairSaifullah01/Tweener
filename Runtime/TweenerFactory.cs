using System.Collections.Generic;
using UnityEngine.Pool;


namespace THEBADDEST.Tweening
{


	public class TweenerFactory
	{

		ObjectPool<ITweener> pool;
		List<ITweener>       tweeners;

		public TweenerFactory(bool isCoroutineBased = true)
		{
			if (isCoroutineBased)
				pool = new ObjectPool<ITweener>(() => new CorotineTweener(), Get, Release, OnDestroy);
			else
				pool = new ObjectPool<ITweener>(() => new AsyncTweener(), Get, Release, OnDestroy);
			tweeners = new List<ITweener>();
		}

		public ITweener Create()
		{
			return pool.Get();
		}

		public void Dispose(ITweener tweener)
		{
			pool.Release(tweener);
		}

		void Get(ITweener tweener)
		{
		}

		void Release(ITweener tweener)
		{
		}

		void OnDestroy(ITweener tweener)
		{
		}

	}


}