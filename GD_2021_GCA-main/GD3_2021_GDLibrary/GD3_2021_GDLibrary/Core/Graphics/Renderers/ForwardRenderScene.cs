using GDLibrary.Components;
using GDLibrary.Graphics;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace GDLibrary.Renderers
{
    public class ForwardRenderScene : IRenderScene
    {
        private Renderer renderer;
        private Material material;
        private Shader shader;

        public virtual void Render(GraphicsDevice graphicsDevice, Camera camera, Scene scene)
        {
            //set depth and blend state

            //render game objects
            var length = scene.Renderers.Count;

            for (var i = 0; i < length; i++)
            {
                renderer = scene.Renderers[i];
                material = renderer.Material;

                if (material == null)
                    throw new NullReferenceException("This game object has no material set for its renderer!");

                //access the shader (e.g. BasicEffect) for this rendered game object
                shader = material.Shader;

                //Set View and Projection
                shader.PrePass(camera);

                //Set World matrix
                shader.Pass(renderer);

                //draw scene contents
                renderer.Draw(graphicsDevice);
            }

            //render post processing

            //render ui
        }
    }
}