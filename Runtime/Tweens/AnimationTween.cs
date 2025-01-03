using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace THEBADDEST.Tweening
{
    [CreateAssetMenu(menuName = "THEBADDEST/Tweening/AnimationTween", fileName = "AnimationTween", order = 0)]
    public class AnimationTween : Tween
    {

        [SerializeField] AnimationClip animClip;
        public override IEnumerator Play(Transform target)
        {
            Init();
            tweener.Lerp(t => animClip.SampleAnimation(target.gameObject, t * animClip.length), duration);
            yield return base.Play(target);
        }
    }
}