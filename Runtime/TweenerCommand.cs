using UnityEngine;
using UnityEngine.UI;


namespace THEBADDEST.Tweening
{


	public abstract class TweenerStartEndCommand : ITweenerCommand
	{

		protected readonly ITweener  tweener;
		protected readonly Transform target;
		protected readonly Vector3   start;
		protected readonly Vector3   end;
		protected readonly float     duration;

		protected TweenerStartEndCommand(ITweener tweener, Transform target, Vector3 start, Vector3 end, float duration)
		{
			this.tweener  = tweener;
			this.start    = start;
			this.end      = end;
			this.duration = duration;
			this.target   = target;
		}

		public abstract void Execute();

	}

	public class MoveStartEndCommand : TweenerStartEndCommand
	{

		public MoveStartEndCommand(ITweener tweener, Transform target, Vector3 start, Vector3 end, float duration) : base(tweener, target, start, end, duration)
		{
		}

		public override void Execute()
		{
			tweener.Lerp(t => { target.position = Vector3.Lerp(start, end, t); }, duration);
		}

	}

	public class MoveLocalStartEndCommand : TweenerStartEndCommand
	{

		public MoveLocalStartEndCommand(ITweener tweener, Transform target, Vector3 start, Vector3 end, float duration) : base(tweener, target, start, end, duration)
		{
		}

		public override void Execute()
		{
			tweener.Lerp(t => { target.localPosition = Vector3.Lerp(start, end, t); }, duration);
		}

	}

	public class ScaleStartEndCommand : TweenerStartEndCommand
	{

		public ScaleStartEndCommand(ITweener tweener, Transform target, Vector3 start, Vector3 end, float duration) : base(tweener, target, start, end, duration)
		{
		}

		public override void Execute()
		{
			tweener.Lerp(t => { target.localScale = Vector3.Lerp(start, end, t); }, duration);
		}

	}

	public class RotateStartEndCommand : TweenerStartEndCommand
	{

		public RotateStartEndCommand(ITweener tweener, Transform target, Vector3 start, Vector3 end, float duration) : base(tweener, target, start, end, duration)
		{
		}

		public override void Execute()
		{
			tweener.Lerp(t => { target.eulerAngles = Vector3.Lerp(start, end, t); }, duration);
		}

	}

	public class RotateLocalStartEndCommand : TweenerStartEndCommand
	{

		public RotateLocalStartEndCommand(ITweener tweener, Transform target, Vector3 start, Vector3 end, float duration) : base(tweener, target, start, end, duration)
		{
		}

		public override void Execute()
		{
			//tweener.Lerp(t => { target.localRotation=Quaternion.Slerp(Quaternion.Euler(start),Quaternion.Euler(end),t); }, duration);
			//tweener.Lerp(t => { target.localRotation=Quaternion.LerpUnclamped(Quaternion.Euler(start),Quaternion.Euler(end),t); }, duration);
			tweener.Lerp(t =>  target.localEulerAngles = Vector3.LerpUnclamped(start, end, t), duration);
		
		}

	}


	public class FadeImageCommand : ITweenerCommand
	{

		ITweener tweener;
		Graphic    target;
		float    start;
		float    end;
		float    duration;

		public FadeImageCommand(ITweener tweener, Graphic target, float start, float end, float duration)
		{
			this.tweener  = tweener;
			this.target   = target;
			this.start    = start;
			this.end      = end;
			this.duration = duration;
		}

		public void Execute()
		{
			Color targetColor = target.color;
			tweener.Lerp(intercept =>
			{
				targetColor.a = Mathf.Lerp(start, end, intercept);
				target.color  = targetColor;
			}, duration);
		}

	}


}