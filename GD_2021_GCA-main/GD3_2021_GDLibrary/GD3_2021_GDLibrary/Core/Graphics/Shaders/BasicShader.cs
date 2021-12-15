using GDLibrary.Components;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GDLibrary.Graphics
{
    public class BasicShader : Shader
    {
        #region Fields

        protected EffectParameter diffuseColorParameter;
        protected EffectParameter textureEnabledParameter;
        protected EffectParameter textureParameter;
        protected EffectParameter alphaParameter;

        //temp
        private BasicEffect basicEffect;
        private bool isLightingEnabled;
        private bool isTextureEnabled;

        #endregion Fields

        #region Constructors

        public BasicShader(ContentManager content, bool isLightingEnabled = true, bool isTextureEnabled = true)
            : base(content)
        {
            this.isLightingEnabled = isLightingEnabled;
            this.isTextureEnabled = isTextureEnabled;
        }

        #endregion Constructors

        #region Initialization

        public override void LoadEffect(ContentManager content)
        {
            Effect = new BasicEffect(Application.GraphicsDevice);

            if (isLightingEnabled)
            {
                (Effect as BasicEffect).LightingEnabled = true;
                (Effect as BasicEffect).EnableDefaultLighting();
            }
        }

        #endregion Initialization

        #region Actions - Pass

        public override void PrePass(Camera camera)
        {
            basicEffect = Effect as BasicEffect;
            basicEffect.View = camera.ViewMatrix;
            basicEffect.Projection = camera.ProjectionMatrix;
        }

        public override void Pass(Renderer renderer)
        {
            //access the game objects material (i.e. diffuse color, alpha, texture)
            var material = renderer.Material as BasicMaterial;

            //set color
            basicEffect.DiffuseColor = material.DiffuseColor;

            //set transparency
            basicEffect.Alpha = material.Alpha;

            //TODO - add bool to material to disable/enable texture
            basicEffect.TextureEnabled = isTextureEnabled;

            //set texture
            if (isTextureEnabled)
                basicEffect.Texture = material.Texture;

            //set ambient
            basicEffect.AmbientLightColor = ambientLightColor;

            ////set world for game object
            //basicEffect.World = renderer.Transform.WorldMatrix;

            ////set pass
            //effect.CurrentTechnique.Passes[0].Apply();
        }

        #endregion Actions - Pass
    }
}