namespace THEBADDEST.Tweening2.Plugins.Options
{
    public struct FloatOptions : IPlugOptions
    {
        public bool snapping;

        public void Reset()
        {
            snapping = false;
        }
    }
}
