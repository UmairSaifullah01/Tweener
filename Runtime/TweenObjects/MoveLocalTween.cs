using UnityEngine;


namespace THEBADDEST.Tweening2
{


	[CreateAssetMenu(menuName = "THEBADDEST/Tweening/MoveLocalTween", fileName = "MoveLocalTween", order = 0)]
	public class MoveLocalTween : FromToTween
	{
		

		public override void PlayWithTarget(Transform target)
		{
			tweener=target.MoveLocal(from, to, duration);
			base.PlayWithTarget(target);
		}
		
	}


}