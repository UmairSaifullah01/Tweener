using UnityEngine;
using UnityEngine.UI;


namespace THEBADDEST.Tweening
{


	public static class TweenerTransformExtensions
	{
		
		public static ITweener Move(this Transform target, Vector3 start, Vector3 end, float duration)
		{
			var tweener = TweenerSolver.Create();
			var command = new MoveStartEndCommand(tweener, target, start, end, duration);
			command.Execute();
			return tweener;
		}

		public static ITweener MoveLocal(this Transform target, Vector3 start, Vector3 end, float duration)
		{
			var tweener = TweenerSolver.Create();
			var command = new MoveLocalStartEndCommand(tweener, target, start, end, duration);
			command.Execute();
			return tweener;
		}

		public static ITweener Scale(this  Transform target, Vector3 start, Vector3 end, float duration)
		{
			var tweener = TweenerSolver.Create();
			var command = new ScaleStartEndCommand(tweener, target, start, end, duration);
			command.Execute();
			return tweener;
		}

		public static ITweener Rotate(this Transform target, Vector3 start, Vector3 end, float duration)
		{
			var tweener = TweenerSolver.Create();
			var command = new RotateStartEndCommand(tweener, target, start, end, duration);
			command.Execute();
			return tweener;
		}

		public static ITweener RotateLocal(this Transform target, Vector3 start, Vector3 end, float duration)
		{
			var tweener = TweenerSolver.Create();
			var command = new RotateLocalStartEndCommand(tweener, target, start, end, duration);
			command.Execute();
			return tweener;
		}

		public static ITweener FadeImage(this Graphic target, float start, float end, float duration)
		{
			var tweener = TweenerSolver.Create();
			var command = new FadeImageCommand(tweener, target, start, end, duration);
			command.Execute();
			return tweener;
		}

	}


}