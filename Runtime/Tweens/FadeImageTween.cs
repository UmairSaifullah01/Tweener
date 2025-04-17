using System.Collections;
using UnityEngine;
using UnityEngine.UI;


namespace THEBADDEST.Tweening
{


	[CreateAssetMenu(menuName = "THEBADDEST/Tweening/FadeImageTween", fileName = "FadeImageTween", order = 0)]
	public class FadeImageTween : Tween
	{

		[SerializeField] float alphaStartValue, alphaEndValue;
		Image                  grapic;
		public override IEnumerator Play(Transform target)
		{
			PlayWithTarget(target);
			yield return base.Play(target);
		}

		public override void PlayWithTarget(Transform target)
		{
			if (grapic ==null)
			{
				grapic = target.GetComponent<Image>();
			}
			tweener=grapic.FadeImage(alphaStartValue, alphaEndValue, duration);
			base.PlayWithTarget(target);
		}
	}


}