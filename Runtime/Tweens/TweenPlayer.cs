using System.Collections;
using UnityEngine;


namespace THEBADDEST.Tweening
{


	public class TweenPlayer : MonoBehaviour
	{

		[SerializeField] bool autoPlay = false;
		[SerializeField] Tween tween;

		void OnEnable()
		{
			if (autoPlay)
			{
				Play();
			}
		}

		void Play()
		{
			StopAllCoroutines();
			StartCoroutine(PlayCoroutine());
		}

		private IEnumerator PlayCoroutine()
		{
			yield return tween.Play(transform);
		}
	}


}