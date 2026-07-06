namespace THEBADDEST.Tweening2
{
    /// <summary>
    /// Types of loop
    /// </summary>
    public enum LoopType
    {
        /// <summary>Each loop cycle restarts from the beginning</summary>
        Restart,
        /// <summary>The tween moves forward and backwards at alternate cycles</summary>
        Yoyo,
        /// <summary>Continuously increments the tween at the end of each loop cycle (A to B, B to B+(A-B), and so on), thus always moving "onward".</summary>
        Incremental
    }
}
