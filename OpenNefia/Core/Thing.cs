namespace OpenNefia
{
    public class Thing
    {
        public IThingData Data { get; set; }

        public int PosX { get; set; }
        public int PosY { get; set; }

        public Love.Texture Texture { get; set; }

        public Thing(IThingData data, int x, int y)
        {
            Data = data;
            PosX = x;
            PosY = y;
            Texture = Love.Graphics.NewImage(data.Image.Resolve());
        }
    }
}