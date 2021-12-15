using GDLibrary.Components;
using Microsoft.Xna.Framework.Graphics;

namespace GDLibrary.Renderers
{
    public interface IRenderScene
    {
        public void Initialize();

        public void SetGraphicsStates(bool isOpaque);

        public void Render(GraphicsDevice graphicsDevice, Camera camera);
    }
}