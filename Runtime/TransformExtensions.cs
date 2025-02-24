using UnityEngine;
using UnityEngine.UI;


namespace THEBADDEST.Tweening
{


	public static class TransformExtensions
	{

		public static ITweener Move(this Transform target, Vector3 start, Vector3 end, float duration)
		{
			var tweener = TweenerSolver.Create();
			tweener.Lerp(t => target.position = Vector3.Lerp(start, end, t), duration);
			return tweener;
		}

		public static ITweener MoveLocal(this Transform target, Vector3 start, Vector3 end, float duration)
		{
			var tweener = TweenerSolver.Create();
			tweener.Lerp(t =>
			{
				if (target == null) return;
				target.localPosition = Vector3.Lerp(start, end, t);
			}, duration);
			return tweener;
		}

		public static ITweener Scale(this Transform target, Vector3 start, Vector3 end, float duration)
		{
			var tweener = TweenerSolver.Create();
			tweener.Lerp(t =>
			{
				if (target == null) return;
				target.localScale = Vector3.Lerp(start, end, t);
			}, duration);
			return tweener;
		}

		public static ITweener Rotate(this Transform target, Vector3 start, Vector3 end, float duration)
		{
			var tweener = TweenerSolver.Create();
			tweener.Lerp(t =>
			{
				if (target == null) return;
				target.eulerAngles = Vector3.Lerp(start, end, t);
			}, duration);
			return tweener;
		}

		public static ITweener RotateLocal(this Transform target, Vector3 start, Vector3 end, float duration)
		{
			var tweener = TweenerSolver.Create();
			tweener.Lerp(t =>
			{
				if (target == null) return;
				target.localEulerAngles = Vector3.Lerp(start, end, t);
			}, duration);
			return tweener;
		}
		

		public static ITweener Jump(this Transform target, Vector3 start, Vector3 peak, Vector3 end, float duration)
		{
			var tweener = TweenerSolver.Create();
			tweener.Lerp(t =>
			{
				if (target == null) return;
				float   progress        = Mathf.PingPong(t * 2, 1);
				Vector3 currentPosition = Vector3.Lerp(Vector3.Lerp(start, peak, progress), Vector3.Lerp(peak, end, progress), t);
				target.position = currentPosition;
			}, duration);
			return tweener;
		}

		public static ITweener Shake(this Transform target, float intensity, float duration)
		{
			var     tweener          = TweenerSolver.Create();
			Vector3 originalPosition = target.position;
			tweener.Lerp(t =>
			{
				if (target == null) return;
				float   shakeAmount  = intensity               * (1 - t);
				Vector3 randomOffset = Random.insideUnitSphere * shakeAmount;
				target.position = originalPosition + randomOffset;
			}, duration);
			return tweener;
		}

		public static ITweener RotateAround(this Transform target, Vector3 point, Vector3 axis, float start, float end, float duration)
		{
			var tweener = TweenerSolver.Create();
			tweener.Lerp(t =>
			{
				if (target == null) return;
				float angle = Mathf.Lerp(start, end, t);
				target.RotateAround(point, axis, angle);
			}, duration);
			return tweener;
		}

		public static ITweener LookAt(this Transform target, Vector3 point, float duration)
		{
			var tweener = TweenerSolver.Create();
			tweener.Lerp(t =>
			{
				if (target == null) return;
				target.rotation = Quaternion.Lerp(target.rotation, Quaternion.LookRotation(point - target.position, target.up), t);
			}, duration);
			return tweener;
		}

		public static ITweener LookAt(this Transform target, Transform point, float duration)
		{
			return LookAt(target, point.position, duration);
		}
		
		
		public static ITweener Punch(this Transform target, Vector3 amount, float duration, float elasticity = 0.5f)
		{
			var tweener = TweenerSolver.Create();
			var originalPosition = target.position;
			tweener.Lerp(t =>
			{
				if (target == null) return;
				float   progress        = Mathf.PingPong(t * 2, 1);
				Vector3 currentPosition = originalPosition + amount * Mathf.Pow(elasticity, progress);
				target.position = currentPosition;
			}, duration);
			return tweener;
		}

		public static ITweener PunchScale(this Transform target, Vector3 amount, float duration, float elasticity = 0.5f)
		{
			var originalScale = target.localScale;
			var tweener = TweenerSolver.Create();
			tweener.Lerp(t =>
			{
				if (target == null) return;
				float   progress        = Mathf.PingPong(t * 2, 1);
				Vector3 currentPosition = originalScale + amount * Mathf.Pow(elasticity, progress);
				target.localScale = currentPosition;
			}, duration);
			return tweener;
		}
		
		
		public static ITweener PunchRotation(this Transform target, Vector3 amount, float duration, float elasticity = 0.5f)
		{
			var originalRotation = target.rotation;
			var tweener = TweenerSolver.Create();
			tweener.Lerp(t =>
			{
				if (target == null) return;
				float   progress        = Mathf.PingPong(t * 2, 1);
				Quaternion currentPosition = Quaternion.Euler(originalRotation.eulerAngles + amount * Mathf.Pow(elasticity, progress));
				target.rotation = currentPosition;
			}, duration);
			return tweener;
		}
		
		
		public static ITweener PunchRotationLocal(this Transform target, Vector3 amount, float duration, float elasticity = 0.5f)
		{
			var originalRotation = target.localRotation;
			var tweener = TweenerSolver.Create();
			tweener.Lerp(t =>
			{
				if (target == null) return;
				float   progress        = Mathf.PingPong(t * 2, 1);
				Quaternion currentPosition = Quaternion.Euler(originalRotation.eulerAngles + amount * Mathf.Pow(elasticity, progress));
				target.localRotation = currentPosition;
			}, duration);
			return tweener;
		}
		
		
		public static ITweener Swing(this Transform target, Vector3 amount, float duration, float oscillations = 3)
		{
			var tweener = TweenerSolver.Create();
			var originalPosition = target.position;
			tweener.Lerp(t =>
			{
				if (target == null) return;
				float   progress        = t * oscillations * 2;
				Vector3 currentPosition = originalPosition + amount * Mathf.Sin(progress * Mathf.PI);
				target.position = currentPosition;
			}, duration);
			return tweener;
		}
		
		
		public static ITweener ZingZag(this Transform target, Vector3 amount, float duration, float oscillations = 3)
		{
			var tweener = TweenerSolver.Create();
			var originalPosition = target.position;
			tweener.Lerp(t =>
			{
				if (target == null) return;
				float   progress        = t * oscillations * 2;
				Vector3 currentPosition = originalPosition + amount * (Mathf.Sin(progress * Mathf.PI) + Mathf.Cos(progress * 2 * Mathf.PI) / 2);
				target.position = currentPosition;
			}, duration);
			return tweener;
		}

	}


}