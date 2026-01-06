using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace THEBADDEST.Tweening
{
    [CreateAssetMenu(menuName = "THEBADDEST/Tweening/AnimationTween", fileName = "AnimationTween", order = 0)]
    public class AnimationTween : Tween
    {

        [SerializeField] AnimationClip animClip;
        public override void PlayWithTarget(Transform target)
        {
            tweener=VirtualTween.Float(t =>
            {
                if (target) 
                    animClip.SampleAnimation(target.gameObject, t * animClip.length);
            }, duration);
            base.PlayWithTarget(target);
        }
    }
}