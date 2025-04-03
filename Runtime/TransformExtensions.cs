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

				// Create a parabolic arc using quadratic Bezier curve
				float t2 = t * t;
				Vector3 position = (1 - t) * (1 - t) * start +
								 2 * (1 - t) * t * peak +
								 t2 * end;

				target.position = position;
			}, duration);
			return tweener;
		}

		/// <summary>
		/// Creates a jumping motion with specified height and distance.
		/// </summary>
		/// <param name="target">The transform to animate</param>
		/// <param name="start">Starting position</param>
		/// <param name="end">Ending position</param>
		/// <param name="height">Maximum height of the jump</param>
		/// <param name="duration">Duration of the jump</param>
		/// <returns>The tweener instance</returns>
		public static ITweener Jump(this Transform target, Vector3 start, Vector3 end, float height, float duration)
		{
			var tweener = TweenerSolver.Create();

			// Calculate the peak point (highest point of the jump)
			Vector3 midPoint = Vector3.Lerp(start, end, 0.5f);
			Vector3 peak = midPoint + Vector3.up * height;

			tweener.Lerp(t =>
			{
				if (target == null) return;

				// Create a parabolic arc using quadratic Bezier curve
				float t2 = t * t;
				Vector3 position = (1 - t) * (1 - t) * start +
								 2 * (1 - t) * t * peak +
								 t2 * end;

				target.position = position;
			}, duration);
			return tweener;
		}

		/// <summary>
		/// Creates a vertical jump from the current position.
		/// </summary>
		/// <param name="target">The transform to animate</param>
		/// <param name="height">Maximum height of the jump</param>
		/// <param name="duration">Duration of the jump</param>
		/// <returns>The tweener instance</returns>
		public static ITweener Jump(this Transform target, float height, float duration)
		{
			if (target == null) return null;

			Vector3 start = target.position;
			Vector3 end = start;
			Vector3 peak = start + Vector3.up * height;

			var tweener = TweenerSolver.Create();
			tweener.Lerp(t =>
			{
				if (target == null) return;

				// Create a parabolic arc using quadratic Bezier curve
				float t2 = t * t;
				Vector3 position = (1 - t) * (1 - t) * start +
								 2 * (1 - t) * t * peak +
								 t2 * end;

				target.position = position;
			}, duration);
			return tweener;
		}
/// <summary>
/// Scales the transform's local scale to a peak value and back in a pulsing motion.
/// </summary>
/// <param name="target">The transform to animate</param>
/// <param name="scaleFactor">The factor by which the transform's scale is multiplied at the peak</param>
/// <param name="duration">Duration of the pulse animation</param>
/// <returns>The tweener instance</returns>
		public static ITweener PulseScale(this Transform target, float scaleFactor, float duration)
		{
			if (target == null) return null;

			Vector3 start = target.localScale;
			Vector3 peak  = start * scaleFactor;

			var tweener = TweenerSolver.Create();
			tweener.Lerp(t =>
			{
				if (target == null) return;

				// Create a parabolic arc using quadratic Bezier curve
				float t2 = t * t;
				Vector3 scale = (1 - t) * (1 - t) * start    +
				                2       * (1 - t) * t * peak +
				                t2      * start;

				target.localScale = scale;
			}, duration);
			return tweener;
		}
		/// <summary>
		/// Creates a vertical jump with multiple bounces.
		/// </summary>
		/// <param name="target">The transform to animate</param>
		/// <param name="height">Maximum height of the jump</param>
		/// <param name="duration">Duration of the jump</param>
		/// <param name="bounces">Number of bounces (default: 1)</param>
		/// <param name="bounceDecay">How much each bounce should decrease in height (default: 0.5f)</param>
		/// <returns>The tweener instance</returns>
		public static ITweener Bounce(this Transform target, float height, float duration, int bounces = 1, float bounceDecay = 0.5f)
		{
			if (target == null) return null;

			var tweener = TweenerSolver.Create();
			Vector3 start = target.position;
			Vector3 end = start;

			tweener.Lerp(t =>
			{
				if (target == null) return;

				// Calculate which bounce we're on and the progress within that bounce
				float bounceProgress = t * (bounces + 1);
				int currentBounce = Mathf.FloorToInt(bounceProgress);
				float bounceT = bounceProgress - currentBounce;

				// Calculate height for current bounce
				float currentHeight = height * Mathf.Pow(bounceDecay, currentBounce);
				Vector3 peak = start + Vector3.up * currentHeight;

				// Create a parabolic arc using quadratic Bezier curve
				float t2 = bounceT * bounceT;
				Vector3 position = (1 - bounceT) * (1 - bounceT) * start +
								 2 * (1 - bounceT) * bounceT * peak +
								 t2 * end;

				target.position = position;
			}, duration);
			return tweener;
		}

		public static ITweener Shake(this Transform target, float intensity, float duration)
		{
			var tweener = TweenerSolver.Create();
			Vector3 originalPosition = target.position;
			tweener.Lerp(t =>
			{
				if (target == null) return;
				float shakeAmount = intensity * (1 - t);
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


		/// <summary>
		/// Creates a punching effect that moves the transform in a direction and bounces back with elasticity.
		/// </summary>
		/// <param name="target">The transform to animate</param>
		/// <param name="direction">Direction of the punch (will be normalized)</param>
		/// <param name="strength">Strength of the punch</param>
		/// <param name="duration">Duration of the punch</param>
		/// <param name="elasticity">How bouncy the punch is (0-1)</param>
		/// <param name="vibrations">Number of vibrations after the punch (default: 3)</param>
		/// <returns>The tweener instance</returns>
		public static ITweener Punch(this Transform target, Vector3 direction, float strength, float duration, float elasticity = 0.5f, int vibrations = 3)
		{
			if (target == null) return null;

			var tweener = TweenerSolver.Create();
			Vector3 originalPosition = target.position;
			Vector3 normalizedDirection = direction.normalized;
			float totalDuration = duration * (1 + vibrations * 0.2f); // Adjust total duration based on vibrations

			tweener.Lerp(t =>
			{
				if (target == null) return;

				// Calculate the main punch progress (0-1)
				float punchProgress = Mathf.Clamp01(t * (1 + vibrations * 0.2f));

				// Create the main punch motion
				float punchMotion = Mathf.PingPong(punchProgress * 2, 1);

				// Add vibration effect
				float vibrationProgress = punchProgress * vibrations * 2 * Mathf.PI;
				float vibrationAmount = Mathf.Sin(vibrationProgress) * (1 - punchProgress) * elasticity;

				// Combine the motions
				float finalProgress = punchMotion + vibrationAmount;

				// Apply the motion with elasticity
				Vector3 offset = normalizedDirection * strength * finalProgress * Mathf.Pow(elasticity, punchProgress);
				target.position = originalPosition + offset;
			}, totalDuration);
			return tweener;
		}

		/// <summary>
		/// Creates a punching effect that moves the transform in a direction and bounces back with elasticity.
		/// </summary>
		/// <param name="target">The transform to animate</param>
		/// <param name="amount">Amount and direction of the punch</param>
		/// <param name="duration">Duration of the punch</param>
		/// <param name="elasticity">How bouncy the punch is (0-1)</param>
		/// <param name="vibrations">Number of vibrations after the punch (default: 3)</param>
		/// <returns>The tweener instance</returns>
		public static ITweener Punch(this Transform target, Vector3 amount, float duration, float elasticity = 0.5f, int vibrations = 3)
		{
			return Punch(target, amount, amount.magnitude, duration, elasticity, vibrations);
		}

		/// <summary>
		/// Creates a punching effect that scales the transform with elasticity and vibrations.
		/// </summary>
		/// <param name="target">The transform to animate</param>
		/// <param name="direction">Direction of the scale punch (will be normalized)</param>
		/// <param name="strength">Strength of the scale punch</param>
		/// <param name="duration">Duration of the punch</param>
		/// <param name="elasticity">How bouncy the punch is (0-1)</param>
		/// <param name="vibrations">Number of vibrations after the punch (default: 3)</param>
		/// <returns>The tweener instance</returns>
		public static ITweener PunchScale(this Transform target, Vector3 direction, float strength, float duration, float elasticity = 0.5f, int vibrations = 3)
		{
			if (target == null) return null;

			var tweener = TweenerSolver.Create();
			Vector3 originalScale = target.localScale;
			Vector3 normalizedDirection = direction.normalized;
			float totalDuration = duration * (1 + vibrations * 0.2f);

			tweener.Lerp(t =>
			{
				if (target == null) return;

				// Calculate the main punch progress (0-1)
				float punchProgress = Mathf.Clamp01(t * (1 + vibrations * 0.2f));

				// Create the main punch motion
				float punchMotion = Mathf.PingPong(punchProgress * 2, 1);

				// Add vibration effect
				float vibrationProgress = punchProgress * vibrations * 2 * Mathf.PI;
				float vibrationAmount = Mathf.Sin(vibrationProgress) * (1 - punchProgress) * elasticity;

				// Combine the motions
				float finalProgress = punchMotion + vibrationAmount;

				// Apply the motion with elasticity
				Vector3 offset = normalizedDirection * strength * finalProgress * Mathf.Pow(elasticity, punchProgress);
				target.localScale = originalScale + offset;
			}, totalDuration);
			return tweener;
		}

		/// <summary>
		/// Creates a punching effect that scales the transform with elasticity and vibrations.
		/// </summary>
		/// <param name="target">The transform to animate</param>
		/// <param name="amount">Amount and direction of the scale punch</param>
		/// <param name="duration">Duration of the punch</param>
		/// <param name="elasticity">How bouncy the punch is (0-1)</param>
		/// <param name="vibrations">Number of vibrations after the punch (default: 3)</param>
		/// <returns>The tweener instance</returns>
		public static ITweener PunchScale(this Transform target, Vector3 amount, float duration, float elasticity = 0.5f, int vibrations = 3)
		{
			return PunchScale(target, amount, amount.magnitude, duration, elasticity, vibrations);
		}

		/// <summary>
		/// Creates a punching effect that rotates the transform with elasticity and vibrations.
		/// </summary>
		/// <param name="target">The transform to animate</param>
		/// <param name="direction">Direction of the rotation punch (will be normalized)</param>
		/// <param name="strength">Strength of the rotation punch</param>
		/// <param name="duration">Duration of the punch</param>
		/// <param name="elasticity">How bouncy the punch is (0-1)</param>
		/// <param name="vibrations">Number of vibrations after the punch (default: 3)</param>
		/// <returns>The tweener instance</returns>
		public static ITweener PunchRotation(this Transform target, Vector3 direction, float strength, float duration, float elasticity = 0.5f, int vibrations = 3)
		{
			if (target == null) return null;

			var tweener = TweenerSolver.Create();
			Quaternion originalRotation = target.rotation;
			Vector3 normalizedDirection = direction.normalized;
			float totalDuration = duration * (1 + vibrations * 0.2f);

			tweener.Lerp(t =>
			{
				if (target == null) return;

				// Calculate the main punch progress (0-1)
				float punchProgress = Mathf.Clamp01(t * (1 + vibrations * 0.2f));

				// Create the main punch motion
				float punchMotion = Mathf.PingPong(punchProgress * 2, 1);

				// Add vibration effect
				float vibrationProgress = punchProgress * vibrations * 2 * Mathf.PI;
				float vibrationAmount = Mathf.Sin(vibrationProgress) * (1 - punchProgress) * elasticity;

				// Combine the motions
				float finalProgress = punchMotion + vibrationAmount;

				// Apply the motion with elasticity
				Vector3 offset = normalizedDirection * strength * finalProgress * Mathf.Pow(elasticity, punchProgress);
				target.rotation = Quaternion.Euler(originalRotation.eulerAngles + offset);
			}, totalDuration);
			return tweener;
		}

		/// <summary>
		/// Creates a punching effect that rotates the transform with elasticity and vibrations.
		/// </summary>
		/// <param name="target">The transform to animate</param>
		/// <param name="amount">Amount and direction of the rotation punch</param>
		/// <param name="duration">Duration of the punch</param>
		/// <param name="elasticity">How bouncy the punch is (0-1)</param>
		/// <param name="vibrations">Number of vibrations after the punch (default: 3)</param>
		/// <returns>The tweener instance</returns>
		public static ITweener PunchRotation(this Transform target, Vector3 amount, float duration, float elasticity = 0.5f, int vibrations = 3)
		{
			return PunchRotation(target, amount, amount.magnitude, duration, elasticity, vibrations);
		}

		/// <summary>
		/// Creates a punching effect that rotates the transform locally with elasticity and vibrations.
		/// </summary>
		/// <param name="target">The transform to animate</param>
		/// <param name="direction">Direction of the local rotation punch (will be normalized)</param>
		/// <param name="strength">Strength of the local rotation punch</param>
		/// <param name="duration">Duration of the punch</param>
		/// <param name="elasticity">How bouncy the punch is (0-1)</param>
		/// <param name="vibrations">Number of vibrations after the punch (default: 3)</param>
		/// <returns>The tweener instance</returns>
		public static ITweener PunchRotationLocal(this Transform target, Vector3 direction, float strength, float duration, float elasticity = 0.5f, int vibrations = 3)
		{
			if (target == null) return null;

			var tweener = TweenerSolver.Create();
			Quaternion originalRotation = target.localRotation;
			Vector3 normalizedDirection = direction.normalized;
			float totalDuration = duration * (1 + vibrations * 0.2f);

			tweener.Lerp(t =>
			{
				if (target == null) return;

				// Calculate the main punch progress (0-1)
				float punchProgress = Mathf.Clamp01(t * (1 + vibrations * 0.2f));

				// Create the main punch motion
				float punchMotion = Mathf.PingPong(punchProgress * 2, 1);

				// Add vibration effect
				float vibrationProgress = punchProgress * vibrations * 2 * Mathf.PI;
				float vibrationAmount = Mathf.Sin(vibrationProgress) * (1 - punchProgress) * elasticity;

				// Combine the motions
				float finalProgress = punchMotion + vibrationAmount;

				// Apply the motion with elasticity
				Vector3 offset = normalizedDirection * strength * finalProgress * Mathf.Pow(elasticity, punchProgress);
				target.localRotation = Quaternion.Euler(originalRotation.eulerAngles + offset);
			}, totalDuration);
			return tweener;
		}

		/// <summary>
		/// Creates a punching effect that rotates the transform locally with elasticity and vibrations.
		/// </summary>
		/// <param name="target">The transform to animate</param>
		/// <param name="amount">Amount and direction of the local rotation punch</param>
		/// <param name="duration">Duration of the punch</param>
		/// <param name="elasticity">How bouncy the punch is (0-1)</param>
		/// <param name="vibrations">Number of vibrations after the punch (default: 3)</param>
		/// <returns>The tweener instance</returns>
		public static ITweener PunchRotationLocal(this Transform target, Vector3 amount, float duration, float elasticity = 0.5f, int vibrations = 3)
		{
			return PunchRotationLocal(target, amount, amount.magnitude, duration, elasticity, vibrations);
		}

		public static ITweener Swing(this Transform target, Vector3 amount, float duration, float oscillations = 3)
		{
			var tweener = TweenerSolver.Create();
			var originalPosition = target.position;
			tweener.Lerp(t =>
			{
				if (target == null) return;
				float progress = t * oscillations * 2;
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
				float progress = t * oscillations * 2;
				Vector3 currentPosition = originalPosition + amount * (Mathf.Sin(progress * Mathf.PI) + Mathf.Cos(progress * 2 * Mathf.PI) / 2);
				target.position = currentPosition;
			}, duration);
			return tweener;
		}

		/// <summary>
		/// Moves a transform smoothly along a path defined by points using Catmull-Rom splines.
		/// </summary>
		/// <param name="target">The transform to animate</param>
		/// <param name="points">Array of points defining the path</param>
		/// <param name="duration">Duration of the movement</param>
		/// <param name="isClosedPath">Whether the path should loop back to the start (default: false)</param>
		/// <param name="smoothness">How smooth the path should be (1-10, default: 5)</param>
		/// <returns>The tweener instance</returns>
		public static ITweener MoveAlongPath(this Transform target, Vector3[] points, float duration, bool isClosedPath = false, int smoothness = 5)
		{
			if (target == null || points == null || points.Length < 2) return null;

			var tweener = TweenerSolver.Create();
			Vector3 originalPosition = target.position;

			// Calculate total path length for proper speed
			float totalLength = 0f;
			for (int i = 0; i < points.Length - 1; i++)
			{
				totalLength += Vector3.Distance(points[i], points[i + 1]);
			}
			if (isClosedPath)
			{
				totalLength += Vector3.Distance(points[points.Length - 1], points[0]);
			}

			tweener.Lerp(t =>
			{
				if (target == null) return;

				// Calculate the current position along the path
				float pathProgress = t * (isClosedPath ? points.Length : points.Length - 1);
				int segmentIndex = Mathf.FloorToInt(pathProgress);
				float segmentProgress = pathProgress - segmentIndex;

				// Get the four points needed for the Catmull-Rom spline
				Vector3 p0, p1, p2, p3;
				if (isClosedPath)
				{
					p0 = points[(segmentIndex - 1 + points.Length) % points.Length];
					p1 = points[segmentIndex % points.Length];
					p2 = points[(segmentIndex + 1) % points.Length];
					p3 = points[(segmentIndex + 2) % points.Length];
				}
				else
				{
					p0 = segmentIndex > 0 ? points[segmentIndex - 1] : points[0];
					p1 = points[segmentIndex];
					p2 = segmentIndex < points.Length - 1 ? points[segmentIndex + 1] : points[points.Length - 1];
					p3 = segmentIndex < points.Length - 2 ? points[segmentIndex + 2] : points[points.Length - 1];
				}

				// Calculate the Catmull-Rom spline position
				float t2 = segmentProgress * segmentProgress;
				float t3 = t2 * segmentProgress;

				Vector3 position = 0.5f * (
					(-p0 + 3f * p1 - 3f * p2 + p3) * t3 +
					(2f * p0 - 5f * p1 + 4f * p2 - p3) * t2 +
					(-p0 + p2) * segmentProgress +
					2f * p1
				);

				target.position = position;
			}, duration);
			return tweener;
		}

		/// <summary>
		/// Moves a transform smoothly along a path defined by points using Catmull-Rom splines.
		/// </summary>
		/// <param name="target">The transform to animate</param>
		/// <param name="points">List of points defining the path</param>
		/// <param name="duration">Duration of the movement</param>
		/// <param name="isClosedPath">Whether the path should loop back to the start (default: false)</param>
		/// <param name="smoothness">How smooth the path should be (1-10, default: 5)</param>
		/// <returns>The tweener instance</returns>
		public static ITweener MoveAlongPath(this Transform target, System.Collections.Generic.List<Vector3> points, float duration, bool isClosedPath = false, int smoothness = 5)
		{
			return MoveAlongPath(target, points.ToArray(), duration, isClosedPath, smoothness);
		}

		/// <summary>
		/// Moves a transform smoothly along a path defined by points using Catmull-Rom splines.
		/// </summary>
		/// <param name="target">The transform to animate</param>
		/// <param name="points">Array of points defining the path</param>
		/// <param name="duration">Duration of the movement</param>
		/// <param name="isClosedPath">Whether the path should loop back to the start (default: false)</param>
		/// <param name="smoothness">How smooth the path should be (1-10, default: 5)</param>
		/// <param name="lookAhead">Whether the transform should look ahead along the path (default: false)</param>
		/// <returns>The tweener instance</returns>
		public static ITweener MoveAlongPath(this Transform target, Vector3[] points, float duration, bool isClosedPath, int smoothness, bool lookAhead)
		{
			if (target == null || points == null || points.Length < 2) return null;

			var tweener = TweenerSolver.Create();
			Vector3 originalPosition = target.position;
			Quaternion originalRotation = target.rotation;

			// Calculate total path length for proper speed
			float totalLength = 0f;
			for (int i = 0; i < points.Length - 1; i++)
			{
				totalLength += Vector3.Distance(points[i], points[i + 1]);
			}
			if (isClosedPath)
			{
				totalLength += Vector3.Distance(points[points.Length - 1], points[0]);
			}

			tweener.Lerp(t =>
			{
				if (target == null) return;

				// Calculate the current position along the path
				float pathProgress = t * (isClosedPath ? points.Length : points.Length - 1);
				int segmentIndex = Mathf.FloorToInt(pathProgress);
				float segmentProgress = pathProgress - segmentIndex;

				// Get the four points needed for the Catmull-Rom spline
				Vector3 p0, p1, p2, p3;
				if (isClosedPath)
				{
					p0 = points[(segmentIndex - 1 + points.Length) % points.Length];
					p1 = points[segmentIndex % points.Length];
					p2 = points[(segmentIndex + 1) % points.Length];
					p3 = points[(segmentIndex + 2) % points.Length];
				}
				else
				{
					p0 = segmentIndex > 0 ? points[segmentIndex - 1] : points[0];
					p1 = points[segmentIndex];
					p2 = segmentIndex < points.Length - 1 ? points[segmentIndex + 1] : points[points.Length - 1];
					p3 = segmentIndex < points.Length - 2 ? points[segmentIndex + 2] : points[points.Length - 1];
				}

				// Calculate the Catmull-Rom spline position
				float t2 = segmentProgress * segmentProgress;
				float t3 = t2 * segmentProgress;

				Vector3 position = 0.5f * (
					(-p0 + 3f * p1 - 3f * p2 + p3) * t3 +
					(2f * p0 - 5f * p1 + 4f * p2 - p3) * t2 +
					(-p0 + p2) * segmentProgress +
					2f * p1
				);

				target.position = position;

				// Look ahead if requested
				if (lookAhead)
				{
					// Calculate the next position for look direction
					float nextProgress = Mathf.Min(1f, segmentProgress + 0.1f);
					float nextT2 = nextProgress * nextProgress;
					float nextT3 = nextT2 * nextProgress;

					Vector3 nextPosition = 0.5f * (
						(-p0 + 3f * p1 - 3f * p2 + p3) * nextT3 +
						(2f * p0 - 5f * p1 + 4f * p2 - p3) * nextT2 +
						(-p0 + p2) * nextProgress +
						2f * p1
					);

					// Calculate and apply the look direction
					Vector3 direction = (nextPosition - position).normalized;
					if (direction != Vector3.zero)
					{
						target.rotation = Quaternion.LookRotation(direction);
					}
				}
			}, duration);
			return tweener;
		}

		/// <summary>
		/// Moves a transform smoothly along a path defined by points using Catmull-Rom splines.
		/// </summary>
		/// <param name="target">The transform to animate</param>
		/// <param name="points">List of points defining the path</param>
		/// <param name="duration">Duration of the movement</param>
		/// <param name="isClosedPath">Whether the path should loop back to the start (default: false)</param>
		/// <param name="smoothness">How smooth the path should be (1-10, default: 5)</param>
		/// <param name="lookAhead">Whether the transform should look ahead along the path (default: false)</param>
		/// <returns>The tweener instance</returns>
		public static ITweener MoveAlongPath(this Transform target, System.Collections.Generic.List<Vector3> points, float duration, bool isClosedPath, int smoothness, bool lookAhead)
		{
			return MoveAlongPath(target, points.ToArray(), duration, isClosedPath, smoothness, lookAhead);
		}

		/// <summary>
		/// Moves a transform smoothly along a Bezier curve defined by control points.
		/// </summary>
		/// <param name="target">The transform to animate</param>
		/// <param name="controlPoints">Array of control points for the Bezier curve (minimum 2 points)</param>
		/// <param name="duration">Duration of the movement</param>
		/// <param name="lookAhead">Whether the transform should look ahead along the path (default: false)</param>
		/// <returns>The tweener instance</returns>
		public static ITweener MoveAlongBezier(this Transform target, Vector3[] controlPoints, float duration, bool lookAhead = false)
		{
			if (target == null || controlPoints == null || controlPoints.Length < 2) return null;

			var tweener = TweenerSolver.Create();
			Vector3 originalPosition = target.position;
			Quaternion originalRotation = target.rotation;

			// Calculate total path length for proper speed
			float totalLength = 0f;
			for (int i = 0; i < controlPoints.Length - 1; i++)
			{
				totalLength += Vector3.Distance(controlPoints[i], controlPoints[i + 1]);
			}

			tweener.Lerp(t =>
			{
				if (target == null) return;

				// Calculate the current position along the Bezier curve
				Vector3 position = CalculateBezierPoint(t, controlPoints);
				target.position = position;

				// Look ahead if requested
				if (lookAhead)
				{
					// Calculate the next position for look direction
					float nextT = Mathf.Min(1f, t + 0.1f);
					Vector3 nextPosition = CalculateBezierPoint(nextT, controlPoints);

					// Calculate and apply the look direction
					Vector3 direction = (nextPosition - position).normalized;
					if (direction != Vector3.zero)
					{
						target.rotation = Quaternion.LookRotation(direction);
					}
				}
			}, duration);
			return tweener;
		}

		/// <summary>
		/// Moves a transform smoothly along a Bezier curve defined by control points.
		/// </summary>
		/// <param name="target">The transform to animate</param>
		/// <param name="controlPoints">List of control points for the Bezier curve (minimum 2 points)</param>
		/// <param name="duration">Duration of the movement</param>
		/// <param name="lookAhead">Whether the transform should look ahead along the path (default: false)</param>
		/// <returns>The tweener instance</returns>
		public static ITweener MoveAlongBezier(this Transform target, System.Collections.Generic.List<Vector3> controlPoints, float duration, bool lookAhead = false)
		{
			return MoveAlongBezier(target, controlPoints.ToArray(), duration, lookAhead);
		}

		/// <summary>
		/// Moves a transform smoothly along a quadratic Bezier curve.
		/// </summary>
		/// <param name="target">The transform to animate</param>
		/// <param name="start">Starting point</param>
		/// <param name="control">Control point</param>
		/// <param name="end">Ending point</param>
		/// <param name="duration">Duration of the movement</param>
		/// <param name="lookAhead">Whether the transform should look ahead along the path (default: false)</param>
		/// <returns>The tweener instance</returns>
		public static ITweener MoveAlongQuadraticBezier(this Transform target, Vector3 start, Vector3 control, Vector3 end, float duration, bool lookAhead = false)
		{
			if (target == null) return null;

			var tweener = TweenerSolver.Create();
			Vector3 originalPosition = target.position;
			Quaternion originalRotation = target.rotation;

			tweener.Lerp(t =>
			{
				if (target == null) return;

				// Calculate the current position along the quadratic Bezier curve
				float t2 = t * t;
				Vector3 position = (1 - t) * (1 - t) * start +
								 2 * (1 - t) * t * control +
								 t2 * end;

				target.position = position;

				// Look ahead if requested
				if (lookAhead)
				{
					// Calculate the next position for look direction
					float nextT = Mathf.Min(1f, t + 0.1f);
					float nextT2 = nextT * nextT;
					Vector3 nextPosition = (1 - nextT) * (1 - nextT) * start +
										 2 * (1 - nextT) * nextT * control +
										 nextT2 * end;

					// Calculate and apply the look direction
					Vector3 direction = (nextPosition - position).normalized;
					if (direction != Vector3.zero)
					{
						target.rotation = Quaternion.LookRotation(direction);
					}
				}
			}, duration);
			return tweener;
		}

		/// <summary>
		/// Moves a transform smoothly along a cubic Bezier curve.
		/// </summary>
		/// <param name="target">The transform to animate</param>
		/// <param name="start">Starting point</param>
		/// <param name="control1">First control point</param>
		/// <param name="control2">Second control point</param>
		/// <param name="end">Ending point</param>
		/// <param name="duration">Duration of the movement</param>
		/// <param name="lookAhead">Whether the transform should look ahead along the path (default: false)</param>
		/// <returns>The tweener instance</returns>
		public static ITweener MoveAlongCubicBezier(this Transform target, Vector3 start, Vector3 control1, Vector3 control2, Vector3 end, float duration, bool lookAhead = false)
		{
			if (target == null) return null;

			var tweener = TweenerSolver.Create();
			Vector3 originalPosition = target.position;
			Quaternion originalRotation = target.rotation;

			tweener.Lerp(t =>
			{
				if (target == null) return;

				// Calculate the current position along the cubic Bezier curve
				float t2 = t * t;
				float t3 = t2 * t;
				Vector3 position = (1 - t) * (1 - t) * (1 - t) * start +
								 3 * (1 - t) * (1 - t) * t * control1 +
								 3 * (1 - t) * t * t * control2 +
								 t3 * end;

				target.position = position;

				// Look ahead if requested
				if (lookAhead)
				{
					// Calculate the next position for look direction
					float nextT = Mathf.Min(1f, t + 0.1f);
					float nextT2 = nextT * nextT;
					float nextT3 = nextT2 * nextT;
					Vector3 nextPosition = (1 - nextT) * (1 - nextT) * (1 - nextT) * start +
										 3 * (1 - nextT) * (1 - nextT) * nextT * control1 +
										 3 * (1 - nextT) * nextT * nextT * control2 +
										 nextT3 * end;

					// Calculate and apply the look direction
					Vector3 direction = (nextPosition - position).normalized;
					if (direction != Vector3.zero)
					{
						target.rotation = Quaternion.LookRotation(direction);
					}
				}
			}, duration);
			return tweener;
		}

		// Helper method to calculate a point on a Bezier curve
		private static Vector3 CalculateBezierPoint(float t, Vector3[] controlPoints)
		{
			if (controlPoints.Length == 2)
			{
				return Vector3.Lerp(controlPoints[0], controlPoints[1], t);
			}
			else if (controlPoints.Length == 3)
			{
				float t2 = t * t;
				return (1 - t) * (1 - t) * controlPoints[0] +
					2 * (1 - t) * t * controlPoints[1] +
					t2 * controlPoints[2];
			}
			else if (controlPoints.Length == 4)
			{
				float t2 = t * t;
				float t3 = t2 * t;
				return (1 - t) * (1 - t) * (1 - t) * controlPoints[0] +
					3 * (1 - t) * (1 - t) * t * controlPoints[1] +
					3 * (1 - t) * t * t * controlPoints[2] +
					t3 * controlPoints[3];
			}
			else
			{
				// For higher order Bezier curves, use De Casteljau's algorithm
				Vector3[] points = new Vector3[controlPoints.Length];
				System.Array.Copy(controlPoints, points, controlPoints.Length);

				for (int i = 0; i < controlPoints.Length - 1; i++)
				{
					for (int j = 0; j < controlPoints.Length - 1 - i; j++)
					{
						points[j] = Vector3.Lerp(points[j], points[j + 1], t);
					}
				}

				return points[0];
			}
		}

	}


}