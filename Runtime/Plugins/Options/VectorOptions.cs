namespace THEBADDEST.Tweening2.Plugins.Options
{
    public struct VectorOptions : IPlugOptions
    {
        public AxisConstraint axisConstraint;
        public bool snapping;

        public void Reset()
        {
            axisConstraint = AxisConstraint.None;
            snapping = false;
        }
    }
}
