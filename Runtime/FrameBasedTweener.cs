using System.Collections;
using UnityEngine;

namespace THEBADDEST.Tweening
{
	/// <summary>
	/// A frame-based implementation of the Tweener class.
	/// Uses Unity's Update loop for smooth tweening operations.
	/// </summary>
	internal class FrameBasedTweener : Tweener
	{
		public override IEnumerator WaitForCompletion()
		{
			yield return new WaitUntil(() => !isPlaying);
		}
	}

}