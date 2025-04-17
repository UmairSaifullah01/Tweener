using THEBADDEST.Tweening;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;


namespace THEBADDEST.Tweening
{



    public static class VirtualTween
    {

        /// <summary>
        /// Creates a typewriter effect that types out text character by character.
        /// </summary>
        /// <param name="onTextUpdated">Callback called when text should be updated</param>
        /// <param name="text">The text to type out</param>
        /// <param name="duration">Duration of the typing animation</param>
        /// <param name="typeSpeed">Speed of typing in characters per second (default: 10)</param>
        /// <param name="startDelay">Delay before starting the typing (default: 0)</param>
        /// <returns>The tweener instance</returns>
        public static ITweener Typewriter(System.Action<string> onTextUpdated, string text, float duration, float typeSpeed = 10f, float startDelay = 0f)
        {
            if (onTextUpdated == null) return null;

            var tweener = TweenerSolver.Create();
            onTextUpdated(string.Empty);
            int   totalCharacters   = text.Length;
            float delayPerCharacter = 1f / typeSpeed;

            tweener.Lerp(t =>
            {
                // Apply start delay
                float adjustedT = Mathf.Max(0, t - startDelay);
                if (adjustedT <= 0) return;

                // Calculate how many characters should be shown
                int charactersToShow = Mathf.Min(totalCharacters, Mathf.FloorToInt(adjustedT / delayPerCharacter));
                onTextUpdated(text.Substring(0, charactersToShow));
            }, duration);
            return tweener;
        }

        /// <summary>
        /// Creates a linear float tween from `start` to `end` over `duration` seconds.
        /// </summary>
        /// <param name="lerpDelegate">Callback called when the value should be updated</param>
        /// <param name="duration">Duration of the tween animation</param>
        /// <param name="start">Starting value of the tween (default: 0f)</param>
        /// <param name="end">Ending value of the tween (default: 1f)</param>
        /// <returns>The tweener instance</returns>
        public static ITweener Float(LerpDelegate lerpDelegate, float duration, float start = 0f, float end = 1f)
        {
            var tweener = TweenerSolver.Create();
            tweener.Lerp(t => { lerpDelegate?.Invoke(Mathf.LerpUnclamped(start, end, t)); }, duration);
            return tweener;
        }

        /// <summary>
        /// Creates a typewriter effect that types out text character by character.
        /// </summary>
        /// <param name="onTextUpdated">Callback called when text should be updated</param>
        /// <param name="text">The text to type out</param>
        /// <param name="duration">Duration of the typing animation</param>
        /// <param name="typeSpeed">Speed of typing in characters per second (default: 10)</param>
        /// <param name="startDelay">Delay before starting the typing (default: 0)</param>
        /// <param name="onCharacterTyped">Callback called for each character typed (default: null)</param>
        /// <returns>The tweener instance</returns>
        public static ITweener Typewriter(System.Action<string> onTextUpdated, string text, float duration, float typeSpeed, float startDelay, System.Action<char> onCharacterTyped)
        {
            if (onTextUpdated == null) return null;

            var tweener = TweenerSolver.Create();
            onTextUpdated(string.Empty);
            int   totalCharacters    = text.Length;
            float delayPerCharacter  = 1f / typeSpeed;
            int   lastCharacterCount = 0;

            tweener.Lerp(t =>
            {
                // Apply start delay
                float adjustedT = Mathf.Max(0, t - startDelay);
                if (adjustedT <= 0) return;

                // Calculate how many characters should be shown
                int charactersToShow = Mathf.Min(totalCharacters, Mathf.FloorToInt(adjustedT / delayPerCharacter));

                // Call callback for new characters
                if (onCharacterTyped != null && charactersToShow > lastCharacterCount)
                {
                    for (int i = lastCharacterCount; i < charactersToShow; i++)
                    {
                        onCharacterTyped(text[i]);
                    }
                }

                lastCharacterCount = charactersToShow;
                onTextUpdated(text.Substring(0, charactersToShow));
            }, duration);
            return tweener;
        }

        /// <summary>
        /// Creates a counter effect that counts up from a start value to an end value.
        /// </summary>
        /// <param name="onValueUpdated">Callback called when the value should be updated</param>
        /// <param name="startValue">Starting value</param>
        /// <param name="endValue">Ending value</param>
        /// <param name="duration">Duration of the counting animation</param>
        /// <returns>The tweener instance</returns>
        public static ITweener Counter(System.Action<int> onValueUpdated, int startValue, int endValue, float duration)
        {
            if (onValueUpdated == null) return null;

            var tweener = TweenerSolver.Create();
            tweener.Lerp(t =>
            {
                int currentValue = Mathf.RoundToInt(Mathf.Lerp(startValue, endValue, t));
                onValueUpdated(currentValue);
            }, duration);
            return tweener;
        }

        /// <summary>
        /// Creates a counter effect that counts up from a start value to an end value.
        /// </summary>
        /// <param name="onValueUpdated">Callback called when the value should be updated</param>
        /// <param name="startValue">Starting value</param>
        /// <param name="endValue">Ending value</param>
        /// <param name="duration">Duration of the counting animation</param>
        /// <param name="onValueChanged">Callback called when the value changes (default: null)</param>
        /// <returns>The tweener instance</returns>
        public static ITweener Counter(System.Action<int> onValueUpdated, int startValue, int endValue, float duration, System.Action<int> onValueChanged)
        {
            if (onValueUpdated == null) return null;

            var tweener   = TweenerSolver.Create();
            int lastValue = startValue;
            tweener.Lerp(t =>
            {
                int currentValue = Mathf.RoundToInt(Mathf.Lerp(startValue, endValue, t));

                // Call callback if value has changed
                if (onValueChanged != null && currentValue != lastValue)
                {
                    onValueChanged(currentValue);
                }

                lastValue = currentValue;
                onValueUpdated(currentValue);
            }, duration);
            return tweener;
        }

        /// <summary>
        /// Shuffles a list using the Fisher-Yates shuffle algorithm.
        /// </summary>
        private static List<T> Shuffle<T>(List<T> list)
        {
            if (list == null) return null;

            for (int i = list.Count - 1; i > 0; i--)
            {
                int j    = Random.Range(0, i + 1);
                T   temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }

            return list;
        }

        /// <summary>
        /// Returns a list of random unique indices within a range.
        /// </summary>
        private static List<int> GetRandomIndices(int count, int maxValue)
        {
            if (count > maxValue) count = maxValue;

            List<int> indices = new List<int>();
            for (int i = 0; i < maxValue; i++)
            {
                indices.Add(i);
            }

            return Shuffle(indices).Take(count).ToList();
        }

        /// <summary>
        /// Returns a random element from a list.
        /// </summary>
        private static T GetRandomElement<T>(List<T> list)
        {
            if (list == null || list.Count == 0) return default;
            return list[Random.Range(0, list.Count)];
        }

        /// <summary>
        /// Returns a list of random elements from a source list.
        /// </summary>
        private static List<T> GetRandomElements<T>(List<T> sourceList, int count)
        {
            if (sourceList == null || sourceList.Count == 0) return new List<T>();
            if (count > sourceList.Count) count = sourceList.Count;

            List<int> indices = GetRandomIndices(count, sourceList.Count);
            return indices.Select(i => sourceList[i]).ToList();
        }

        /// <summary>
        /// Returns a random float value between min and max.
        /// </summary>
        private static float GetRandomFloat(float min, float max)
        {
            return Random.Range(min, max);
        }

        /// <summary>
        /// Returns a random integer value between min and max.
        /// </summary>
        private static int GetRandomInt(int min, int max)
        {
            return Random.Range(min, max + 1);
        }

        /// <summary>
        /// Returns a random Vector3 within the specified range.
        /// </summary>
        private static Vector3 GetRandomVector3(Vector3 min, Vector3 max)
        {
            return new Vector3(Random.Range(min.x, max.x), Random.Range(min.y, max.y), Random.Range(min.z, max.z));
        }

        /// <summary>
        /// Returns a random color.
        /// </summary>
        private static Color GetRandomColor(bool includeAlpha = false)
        {
            if (includeAlpha)
            {
                return new Color(Random.value, Random.value, Random.value, Random.value);
            }

            return new Color(Random.value, Random.value, Random.value, 1f);
        }

        /// <summary>
        /// Returns a random rotation in euler angles.
        /// </summary>
        private static Vector3 GetRandomRotation(Vector3 minAngle, Vector3 maxAngle)
        {
            return new Vector3(Random.Range(minAngle.x, maxAngle.x), Random.Range(minAngle.y, maxAngle.y), Random.Range(minAngle.z, maxAngle.z));
        }

        /// <summary>
        /// Returns a random point within a circle.
        /// </summary>
        private static Vector2 GetRandomPointInCircle(Vector2 center, float radius)
        {
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            float r     = Random.Range(0f, radius);
            return center + new Vector2(r * Mathf.Cos(angle), r * Mathf.Sin(angle));
        }

        /// <summary>
        /// Returns a random point within a sphere.
        /// </summary>
        private static Vector3 GetRandomPointInSphere(Vector3 center, float radius)
        {
            float theta = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            float phi   = Random.Range(0f, 180f) * Mathf.Deg2Rad;
            float r     = Random.Range(0f, radius);

            return center + new Vector3(r * Mathf.Sin(phi) * Mathf.Cos(theta), r * Mathf.Sin(phi) * Mathf.Sin(theta), r * Mathf.Cos(phi));
        }

        /// <summary>
        /// Creates a tweening animation that continuously shuffles a list over time.
        /// </summary>
        /// <typeparam name="T">Type of elements in the list</typeparam>
        /// <param name="list">List to shuffle</param>
        /// <param name="onListUpdated">Callback called when the list is updated</param>
        /// <param name="duration">Duration of the shuffling animation</param>
        /// <param name="shuffleCount">Number of times to shuffle the list (default: 5)</param>
        /// <returns>The tweener instance</returns>
        public static ITweener ShuffleList<T>(List<T> list, System.Action<List<T>> onListUpdated, float duration, int shuffleCount = 5)
        {
            if (list == null || onListUpdated == null) return null;

            var     tweener     = TweenerSolver.Create();
            List<T> workingList = new List<T>(list);
            float   interval    = duration / shuffleCount;

            tweener.Lerp(t =>
            {
                int currentShuffle = Mathf.FloorToInt(t * shuffleCount);
                if (currentShuffle > 0)
                {
                    // Perform one shuffle
                    for (int i = workingList.Count - 1; i > 0; i--)
                    {
                        int j    = Random.Range(0, i + 1);
                        T   temp = workingList[i];
                        workingList[i] = workingList[j];
                        workingList[j] = temp;
                    }

                    onListUpdated(workingList);
                }
            }, duration);
            return tweener;
        }

        /// <summary>
        /// Creates a tweening animation that generates random indices over time.
        /// </summary>
        /// <param name="onIndicesUpdated">Callback called when new indices are generated</param>
        /// <param name="count">Number of indices to generate each time</param>
        /// <param name="maxValue">Maximum value (exclusive)</param>
        /// <param name="duration">Duration of the animation</param>
        /// <param name="updateCount">Number of times to update the indices (default: 5)</param>
        /// <returns>The tweener instance</returns>
        public static ITweener RandomIndicesSequence(System.Action<List<int>> onIndicesUpdated, int count, int maxValue, float duration, int updateCount = 5)
        {
            if (onIndicesUpdated == null) return null;

            var   tweener  = TweenerSolver.Create();
            float interval = duration / updateCount;

            tweener.Lerp(t =>
            {
                int currentUpdate = Mathf.FloorToInt(t * updateCount);
                if (currentUpdate > 0)
                {
                    List<int> indices = GetRandomIndices(count, maxValue);
                    onIndicesUpdated(indices);
                }
            }, duration);
            return tweener;
        }

        /// <summary>
        /// Creates a tweening animation that generates a sequence of random colors over time.
        /// </summary>
        /// <param name="onColorsUpdated">Callback called when new colors are generated</param>
        /// <param name="colorCount">Number of colors to generate each time</param>
        /// <param name="duration">Duration of the animation</param>
        /// <param name="updateCount">Number of times to update the colors (default: 5)</param>
        /// <param name="includeAlpha">Whether to include random alpha values (default: false)</param>
        /// <returns>The tweener instance</returns>
        public static ITweener RandomColorsSequence(System.Action<List<Color>> onColorsUpdated, int colorCount, float duration, int updateCount = 5, bool includeAlpha = false)
        {
            if (onColorsUpdated == null) return null;

            var   tweener  = TweenerSolver.Create();

            tweener.Lerp(t =>
            {
                int currentUpdate = Mathf.FloorToInt(t * updateCount);
                if (currentUpdate > 0)
                {
                    List<Color> colors = new List<Color>();
                    for (int i = 0; i < colorCount; i++)
                    {
                        colors.Add(GetRandomColor(includeAlpha));
                    }

                    onColorsUpdated(colors);
                }
            }, duration);
            return tweener;
        }

        /// <summary>
        /// Creates a tweening animation that generates a sequence of random Vector3 positions over time.
        /// </summary>
        /// <param name="onPositionsUpdated">Callback called when new positions are generated</param>
        /// <param name="positionCount">Number of positions to generate each time</param>
        /// <param name="min">Minimum values for each component</param>
        /// <param name="max">Maximum values for each component</param>
        /// <param name="duration">Duration of the animation</param>
        /// <param name="updateCount">Number of times to update the positions (default: 5)</param>
        /// <returns>The tweener instance</returns>
        /// <summary>
        /// Creates a tweening animation that generates a sequence of random Vector3 positions over time.
        /// </summary>
        /// <param name="onPositionsUpdated">Callback called when new positions are generated</param>
        /// <param name="positionCount">Number of positions to generate each time</param>
        /// <param name="min">Minimum values for each component</param>
        /// <param name="max">Maximum values for each component</param>
        /// <param name="duration">Duration of the animation</param>
        /// <param name="updateCount">Number of times to update the positions (default: 5)</param>
        /// <returns>The tweener instance</returns>
        public static ITweener RandomPositionsSequence(System.Action<List<Vector3>> onPositionsUpdated, int positionCount, Vector3 min, Vector3 max, float duration, int updateCount = 5)
        {
            if (onPositionsUpdated == null) return null;

            var tweener = TweenerSolver.Create();

            tweener.Lerp(t =>
            {
                int currentUpdate = Mathf.FloorToInt(t * updateCount);
                if (currentUpdate > 0)
                {
                    onPositionsUpdated(Enumerable.Range(0, positionCount).Select(i => GetRandomVector3(min, max)).ToList());
                }
            }, duration);
            return tweener;
        }

        /// <summary>
        /// Creates a tweening animation that generates a sequence of random rotations over time.
        /// </summary>
        /// <param name="onRotationsUpdated">Callback called when new rotations are generated</param>
        /// <param name="rotationCount">Number of rotations to generate each time</param>
        /// <param name="minAngle">Minimum angle for each axis</param>
        /// <param name="maxAngle">Maximum angle for each axis</param>
        /// <param name="duration">Duration of the animation</param>
        /// <param name="updateCount">Number of times to update the rotations (default: 5)</param>
        /// <returns>The tweener instance</returns>
        public static ITweener RandomRotationsSequence(System.Action<List<Vector3>> onRotationsUpdated, int rotationCount, Vector3 minAngle, Vector3 maxAngle, float duration, int updateCount = 5)
        {
            if (onRotationsUpdated == null) return null;

            var   tweener  = TweenerSolver.Create();
            float interval = duration / updateCount;

            tweener.Lerp(t =>
            {
                int currentUpdate = Mathf.FloorToInt(t * updateCount);
                if (currentUpdate > 0)
                {
                    List<Vector3> rotations = new List<Vector3>();
                    for (int i = 0; i < rotationCount; i++)
                    {
                        rotations.Add(GetRandomRotation(minAngle, maxAngle));
                    }

                    onRotationsUpdated(rotations);
                }
            }, duration);
            return tweener;
        }

        /// <summary>
        /// Creates a zigzag path between two points.
        /// </summary>
        /// <param name="start">Starting point</param>
        /// <param name="end">Ending point</param>
        /// <param name="zigzagCount">Number of zigzags in the path</param>
        /// <param name="amplitude">Maximum distance from the direct path</param>
        /// <returns>List of points forming the zigzag path</returns>
        public static List<Vector3> CreateZigzagPath(Vector3 start, Vector3 end, int zigzagCount, float amplitude)
        {
            List<Vector3> path          = new List<Vector3>();
            Vector3       direction     = (end - start).normalized;
            Vector3       perpendicular = Vector3.Cross(direction, Vector3.up).normalized;
            float         totalDistance = Vector3.Distance(start, end);

            // Add start point
            path.Add(start);

            // Calculate points for each zigzag
            for (int i = 0; i < zigzagCount; i++)
            {
                float   t         = (i + 1) / (float)(zigzagCount + 1);
                Vector3 basePoint = Vector3.Lerp(start, end, t);

                // Alternate between positive and negative amplitude
                float   currentAmplitude = amplitude     * (i % 2 == 0 ? 1 : -1);
                Vector3 offset           = perpendicular * currentAmplitude;

                path.Add(basePoint + offset);
            }

            // Add end point
            path.Add(end);
            return path;
        }

    }


}