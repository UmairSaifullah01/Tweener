using UnityEngine;
using UnityEngine.UI;


namespace THEBADDEST.Tweening
{

	public static class TweenerExtensions
	{

		public static ITweener OnComplete(this ITweener tweener, CallbackDelegate onComplete)
		{
			tweener.OnCompleteAllLoops += onComplete.Invoke;
			return tweener;
		}
		
		public static ITweener ColorBlockTween(this Renderer target, Color start, Color end, float duration, string colorPropertyName = "_Color")
		{
			var tweener               = TweenerSolver.Create();
			var materialPropertyBlock = new MaterialPropertyBlock();
			target.GetPropertyBlock(materialPropertyBlock);
			tweener.Lerp(t =>
			{
				materialPropertyBlock.SetColor(colorPropertyName, Color.Lerp(start, end, t));
				target.SetPropertyBlock(materialPropertyBlock);
			}, duration);
			return tweener;
		}
		public static ITweener FieldOfViewTween(this Camera target, float start, float end, float duration)
		{
			var tweener = TweenerSolver.Create();
			tweener.Lerp(t =>
			{
				if (target == null) return;
				target.fieldOfView = Mathf.Lerp(start, end, t);
			}, duration);
			return tweener;
		}

		public static ITweener OrthographicSizeTween(this Camera target, float start, float end, float duration)
		{
			var tweener = TweenerSolver.Create();
			tweener.Lerp(t =>
			{
				if (target == null) return;
				target.orthographicSize = Mathf.Lerp(start, end, t);
			}, duration);
			return tweener;
		}
		public static ITweener FadeImage(this Graphic target, float start, float end, float duration)
		{
			var tweener = TweenerSolver.Create();
			var color   = target.color;
			tweener.Lerp(t =>
			{
				if (target == null) return;
				color.a      = Mathf.Lerp(start, end, t);
				target.color = color;
			}, duration);
			return tweener;
		}
		public static ITweener FillAmountImage(this Image target, float start, float end, float duration)
		{
			var tweener = TweenerSolver.Create();
			tweener.Lerp(t =>
			{
				if (target == null) return;
				target.fillAmount = Mathf.Lerp(start, end, t);
			}, duration);
			return tweener;
		}
		public static ITweener ColorTween(this Graphic target, Color start, Color end, float duration)
		{
			var tweener = TweenerSolver.Create();
			var color   = target.color;
			tweener.Lerp(t =>
			{
				if (target == null) return;
				color        = Color.Lerp(start, end, t);
				target.color = color;
			}, duration);
			return tweener;
		}

		public static ITweener SizeTween(this RectTransform target, Vector2 start, Vector2 end, float duration)
		{
			var tweener = TweenerSolver.Create();
			tweener.Lerp(t =>
			{
				if (target == null) return;
				target.sizeDelta = Vector2.Lerp(start, end, t);
			}, duration);
			return tweener;
		}

		public static ITweener AnchorTween(this RectTransform target, Vector2 start, Vector2 end, float duration)
		{
			var tweener = TweenerSolver.Create();
			tweener.Lerp(t =>
			{
				if (target == null) return;
				target.anchorMin = Vector2.Lerp(start, end, t);
				target.anchorMax = Vector2.Lerp(start, end, t);
			}, duration);
			return tweener;
		}

		public static ITweener PivotTween(this RectTransform target, Vector2 start, Vector2 end, float duration)
		{
			var tweener = TweenerSolver.Create();
			tweener.Lerp(t =>
			{
				if (target == null) return;
				target.pivot = Vector2.Lerp(start, end, t);
			}, duration);
			return tweener;
		}
		public static ITweener AnchoredPositionTween(this RectTransform target,Vector2 start, Vector2 end, float duration)
		{
			var tweener = TweenerSolver.Create();
			tweener.Lerp(t =>
			{
				if (target == null) return;
				target.anchoredPosition = Vector2.Lerp(start, end, t);
			}, duration);
			return tweener;
		}
		public static ITweener MaterialPropertyTween(this Renderer target, string propertyName, float start, float end, float duration)
		{
			var tweener  = TweenerSolver.Create();
			var material = target.material;
			tweener.Lerp(t =>
			{
				if (target == null) return;
				material.SetFloat(propertyName, Mathf.Lerp(start, end, t));
			}, duration);
			return tweener;
		}

	}


}