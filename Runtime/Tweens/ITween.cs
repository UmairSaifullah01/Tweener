using System.Collections;
using UnityEngine;


namespace THEBADDEST.Tweening
{


	public interface ITween
	{

		ITweener tweener  { get; }
		int     priority { get; }

		float delay { get; set; }

		
		IEnumerator Play(Transform target);

	}


}