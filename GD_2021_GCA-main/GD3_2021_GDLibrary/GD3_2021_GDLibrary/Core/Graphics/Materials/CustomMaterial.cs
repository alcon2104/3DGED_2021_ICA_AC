using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDLibrary.Graphics
{
    public class CustomMaterial : BasicMaterial
    {
        #region Fields

        protected Vector2 tiling;

        protected Texture2D normalTexture;

        #endregion Fields

        #region Properties

        public Vector2 Tiling { get => tiling; set => tiling = value; }
        public Texture2D NormalTexture { get => normalTexture; set => normalTexture = value; }

        #endregion Properties

        #region Constructors

        public CustomMaterial(string name, Shader shader, Texture2D texture)
           : this(name, shader, new Color(255, 255, 255, 255), 1, texture)
        {
        }

        public CustomMaterial(string name, Shader shader, Color color, float alpha, Texture2D texture)
            : base(name, shader, color, alpha, texture)
        {
        }

        #endregion Constructors

        public override object Clone()
        {
            var clone = new CustomMaterial($"Clone - {name}", shader, new Color(diffuseColor), alpha, texture);
            clone.normalTexture = normalTexture;
            clone.shader = shader;
            return clone;
        }
    }
}