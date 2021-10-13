namespace OpenNefia.Core.Rendering
{
    public interface IDrawable
    {
        public void Update(float dt);
        public void Draw();
    }
}