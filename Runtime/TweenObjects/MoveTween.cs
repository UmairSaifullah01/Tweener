using UnityEngine;


namespace THEBADDEST.Tweening2
{


	[CreateAssetMenu(menuName = "THEBADDEST/Tweening/MoveTween", fileName = "MoveTween", order = 0)]
	public class MoveTween : FromToTween
	{

		public override void PlayWithTarget(Transform target)
		{
			tweener=target.Move(from, to, duration);
			base.PlayWithTarget(target);
		}

	}


}