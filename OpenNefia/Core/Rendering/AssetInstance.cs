namespace OpenNefia.Core.Rendering
{
    public class AssetInstance
    {
        public AssetDrawable AssetDrawable { get; }
        public int Width { get; }
        public int Height { get; }

        public AssetInstance(AssetDrawable assetDrawable, int width, int height)
        {
            this.AssetDrawable = assetDrawable;
            this.Width = width;
            this.Height = height;
        }
    }
}