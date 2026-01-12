using THEBADDEST.Tweening2;
using UnityEngine;


namespace THEBADDEST.Tweening
{


	public class TweenPlayer : MonoBehaviour
	{

		[SerializeField] bool autoPlay = false;
		[SerializeField] TweenerObject tween;
		ITweener tweener;

		void OnEnable()
		{
			if (autoPlay)
			{
				Play();
			}
		}

		void OnDisable()
		{
			tweener?.Kill();
		}

		void Play()
		{
			tween.PlayWithTarget(transform);
			tweener = tween.tweener;
		}

	}


}