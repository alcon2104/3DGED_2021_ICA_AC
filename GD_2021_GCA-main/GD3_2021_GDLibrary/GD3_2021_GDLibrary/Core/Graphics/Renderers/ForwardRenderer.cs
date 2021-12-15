using GDLibrary.Components;
using GDLibrary.Graphics;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace GDLibrary.Renderers
{
    /// <summary>
    /// Renders the scene using a forward lighting technique and related effects
    /// </summary>
    public class ForwardRenderer : IRenderScene
    {
        //temps used in Render
        private Renderer renderer;

        private Material material;
        private Shader shader;
        private Scene scene;

        private RasterizerState rasterizerStateOpaque;
        private RasterizerState rasterizerStateTransparent;

        public ForwardRenderer()
        {
            Initialize();
        }

        public virtual void Initialize()
        {
            //set the graphics card to repeat the end pixel value for any UV value outside 0-1
            //See http://what-when-how.com/xna-game-studio-4-0-programmingdeveloping-for-windows-phone-7-and-xbox-360/samplerstates-xna-game-studio-4-0-programming/
            SamplerState samplerState = new SamplerState();
            samplerState.AddressU = TextureAddressMode.Mirror;
            samplerState.AddressV = TextureAddressMode.Mirror;
            Application.GraphicsDevice.SamplerStates[0] = samplerState;

            //opaque objects
            rasterizerStateOpaque = new RasterizerState();
            rasterizerStateOpaque.CullMode = CullMode.CullCounterClockwiseFace;

            //transparent objects
            rasterizerStateTransparent = new RasterizerState();
            rasterizerStateTransparent.CullMode = CullMode.None;

            //Remember this code from our initial aliasing problems with the Sky box?
            //enable anti-aliasing along the edges of the quad i.e. to remove jagged edges to the primitive
            Application.GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;

            //set depth and blend state
            SetGraphicsStates(false);
        }

        public virtual void SetGraphicsStates(bool isOpaque)
        {
            if (isOpaque)
            {
                //set the appropriate state for opaque objects
                Application.GraphicsDevice.RasterizerState = rasterizerStateOpaque;

                //disable to see what happens when we disable depth buffering - look at the boxes
                Application.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            }
            else
            {
                //set the appropriate state for transparent objects
                Application.GraphicsDevice.RasterizerState = rasterizerStateTransparent;

                //enable alpha blending for transparent objects i.e. trees
                Application.GraphicsDevice.BlendState = BlendState.AlphaBlend;

                //disable to see what happens when we disable depth buffering - look at the boxes
                Application.GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
            }
        }

        public virtual void Render(GraphicsDevice graphicsDevice, Camera camera)
        {
            //until first update this will be null - then in SceneManager the activeScene will be set
            if (scene == null)
                scene = Application.SceneManager.ActiveScene;

            //set viewport
            graphicsDevice.Viewport = camera.Viewport;

            //render game objects
            var length = scene.Renderers.Count;

            //sort by alpha
            //   scene.Renderers.Sort((x, y) => y.Material.Alpha.CompareTo(x.Material.Alpha));

            for (var i = 0; i < length; i++)
            {
                renderer = scene.Renderers[i];
                material = renderer.Material;

                if (material == null || renderer == null)
                    throw new NullReferenceException("This game object has no material and/or renderer!");

                //set transparent or opaque based on object alpha
                if (material.Alpha >= 1)
                {
                    //set the appropriate state for opaque objects
                    Application.GraphicsDevice.RasterizerState = rasterizerStateOpaque;

                    //disable to see what happens when we disable depth buffering - look at the boxes
                    Application.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
                }
                else
                {
                    //set the appropriate state for transparent objects
                    Application.GraphicsDevice.RasterizerState = rasterizerStateTransparent;

                    //enable alpha blending for transparent objects i.e. trees
                    Application.GraphicsDevice.BlendState = BlendState.AlphaBlend;

                    //disable to see what happens when we disable depth buffering - look at the boxes
                    Application.GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
                }

                //   SetGraphicsStates(material.Alpha < 1);

                //access the shader (e.g. BasicEffect) for this rendered game object
                shader = material.Shader;

                //Set View and Projection
                shader.PrePass(camera);

                //Set World matrix
                shader.Pass(renderer);

                //draw scene contents
                renderer.Draw(graphicsDevice, shader.Effect);
            }
        }
    }
}