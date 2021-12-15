using GDLibrary.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GDLibrary.Graphics
{
    public abstract class Shader
    {
        #region Statics

        protected static Vector3 ambientLightColor = Color.Black.ToVector3();
        protected static Vector3 AmbientLightColor { get => ambientLightColor; set => ambientLightColor = value; }

        #endregion Statics

        #region Fields

        private Effect effect;
        private EffectPass effectPass;

        #endregion Fields

        #region Properties

        public Effect Effect { get => effect; protected set => effect = value; }
        public EffectPass EffectPass { get => effectPass; protected set => effectPass = value; }

        #endregion Properties

        public Shader(ContentManager content)
        {
            LoadEffect(content);
        }

        #region Actions

        public abstract void LoadEffect(ContentManager content);

        /// <summary>
        /// Called in RenderManager::Render to set View and Projection
        /// </summary>
        /// <param name="camera"></param>
        public abstract void PrePass(Camera camera);

        /// <summary>
        /// Called in RenderManager::Render to set World matrix for the game object
        /// </summary>
        /// <param name="camera"></param>
        public abstract void Pass(Renderer renderer);

        #endregion Actions
    }
}