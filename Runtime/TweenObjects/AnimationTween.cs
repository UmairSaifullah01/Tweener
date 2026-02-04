using UnityEngine;
namespace THEBADDEST.Tweening2
{
    [CreateAssetMenu(menuName = "THEBADDEST/Tweening/AnimationTween", fileName = "AnimationTween", order = 0)]
    public class AnimationTween : TweenerObject
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
