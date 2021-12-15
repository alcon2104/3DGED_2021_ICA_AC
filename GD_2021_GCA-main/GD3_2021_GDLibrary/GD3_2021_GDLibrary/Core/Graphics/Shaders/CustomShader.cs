using GDLibrary.Components;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GDLibrary.Graphics
{
    public class CustomShader : Shader
    {
        #region Fields

        protected EffectParameter m_EPView;
        protected EffectParameter m_EPProjection;
        protected EffectParameter m_EPEyePosition;
        protected EffectParameter m_EPWorld;
        protected EffectParameter m_EPTextureTilling;
        protected EffectParameter m_EPDiffuseColor;
        protected EffectParameter m_EPMainTexture;
        protected EffectParameter m_EPNormalTexture;
        protected EffectParameter m_EPNormalTextureEnabled;

        #endregion Fields

        #region Constructors

        public CustomShader(ContentManager content) : base(content)
        {
        }

        #endregion Constructors

        #region Initialization

        public override void LoadEffect(ContentManager content)
        {
            Effect = content.Load<Effect>("Assets/Shaders/Deferred/Standard");
            EffectPass = Effect.CurrentTechnique.Passes[0];

            m_EPView = Effect.Parameters["View"];
            m_EPProjection = Effect.Parameters["Projection"];
            m_EPEyePosition = Effect.Parameters["EyePosition"];
            m_EPWorld = Effect.Parameters["World"];
            m_EPTextureTilling = Effect.Parameters["TextureTiling"];
            m_EPDiffuseColor = Effect.Parameters["DiffuseColor"];
            m_EPMainTexture = Effect.Parameters["MainTexture"];
            m_EPNormalTexture = Effect.Parameters["NormalMap"];
            m_EPNormalTextureEnabled = Effect.Parameters["NormalTextureEnabled"];
        }

        #endregion Initialization

        #region Actions - Pass

        public override void PrePass(Camera camera)
        {
            m_EPView.SetValue(camera.ViewMatrix);
            m_EPProjection.SetValue(camera.ProjectionMatrix);
            m_EPEyePosition.SetValue(camera.Transform.LocalTranslation);
        }

        public override void Pass(Renderer renderer)
        {
            //access the game objects material (i.e. diffuse color, alpha, texture)
            var material = renderer.Material as CustomMaterial;

            m_EPWorld.SetValue(renderer.Transform.WorldMatrix);
            m_EPTextureTilling.SetValue(material.Tiling);
            //TODO - offset
            m_EPDiffuseColor.SetValue(material.DiffuseColor);
            m_EPMainTexture.SetValue(material.Texture);
            m_EPNormalTexture.SetValue(material.NormalTexture);
            m_EPNormalTextureEnabled.SetValue(material.NormalTexture != null);
            EffectPass.Apply();
        }

        #endregion Actions - Pass
    }
}