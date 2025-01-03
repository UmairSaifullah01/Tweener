using System.Collections;
using UnityEngine;


namespace THEBADDEST.Tweening
{


	public interface ITween
	{

		Tweener tweener  { get; }
		int     priority { get; }

		float delay { get; set; }

		
		IEnumerator Play(Transform target);

	}


}