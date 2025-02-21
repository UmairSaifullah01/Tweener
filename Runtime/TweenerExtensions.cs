using UnityEngine;
using UnityEngine.UI;


namespace THEBADDEST.Tweening
{


	public static class TweenerExtensions
	{

		public static ITweener Move(this ITweener tweener, Transform target, Vector3 start, Vector3 end, float duration)
		{
			var command = new MoveStartEndCommand(tweener, target, start, end, duration);
			command.Execute();
			return tweener;
		}

		public static ITweener MoveLocal(this ITweener tweener, Transform target, Vector3 start, Vector3 end, float duration)
		{
			var command = new MoveLocalStartEndCommand(tweener, target, start, end, duration);
			command.Execute();
			return tweener;
		}

		public static ITweener Scale(this ITweener tweener, Transform target, Vector3 start, Vector3 end, float duration)
		{
			var command = new ScaleStartEndCommand(tweener, target, start, end, duration);
			command.Execute();
			return tweener;
		}

		public static ITweener Rotate(this ITweener tweener, Transform target, Vector3 start, Vector3 end, float duration)
		{
			var command = new RotateStartEndCommand(tweener, target, start, end, duration);
			command.Execute();
			return tweener;
		}

		public static ITweener RotateLocal(this ITweener tweener, Transform target, Vector3 start, Vector3 end, float duration)
		{
			var command = new RotateLocalStartEndCommand(tweener, target, start, end, duration);
			command.Execute();
			return tweener;
		}

		public static ITweener FadeImage(this ITweener tweener, Graphic target, float start, float end, float duration)
		{
			var command = new FadeImageCommand(tweener, target, start, end, duration);
			command.Execute();
			return tweener;
		}

	}


}