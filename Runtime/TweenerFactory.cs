using System.Collections.Generic;
using UnityEngine.Pool;


namespace THEBADDEST.Tweening
{


	public class TweenerFactory
	{

		ObjectPool<ITweener> pool;
		List<ITweener>       tweeners;

		public TweenerFactory(bool isCoroutineBased = false)
		{
			pool = isCoroutineBased ? new ObjectPool<ITweener>(() => new CorotineTweener(), Get, Release, OnDestroy) : new ObjectPool<ITweener>(() => new FrameBasedTweener(), Get, Release, OnDestroy);
			tweeners = new List<ITweener>();
		}

		public ITweener Create()
		{
			ITweener tweener = pool.Get();
			tweeners.Add(tweener);
			return tweener;
		}

		public void Dispose(ITweener tweener)
		{
			pool.Release(tweener);
			tweeners.Remove(tweener);
		}

		void Get(ITweener tweener)
		{
			tweener.Reset();
		}

		void Release(ITweener tweener)
		{
			tweener.Reset();
		}

		void OnDestroy(ITweener tweener)
		{
			tweener.Reset();
		}

	}


}