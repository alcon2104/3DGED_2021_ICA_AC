using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDLibrary.Graphics
{
    public class BasicMaterial : Material
    {
        #region Fields

        protected Vector3 diffuseColor;
        protected Texture2D texture;

        #endregion Fields

        #region Properties

        public Vector3 DiffuseColor { get => diffuseColor; set => diffuseColor = value; }
        public Texture2D Texture { get => texture; set => texture = value; }

        #endregion Properties

        #region Constructors

        public BasicMaterial(string name, Shader shader, Texture2D texture)
            : this(name, shader, new Color(255, 255, 255, 255), 1, texture)
        {
        }

        public BasicMaterial(string name, Shader shader, Color diffuseColor, float alpha, Texture2D texture)
            : base(name, shader, alpha)
        {
            this.diffuseColor = diffuseColor.ToVector3();
            this.texture = texture;
        }

        #endregion Constructors

        public override object Clone()
        {
            return new BasicMaterial($"Clone - {name}", shader, new Color(diffuseColor), alpha, texture);
        }
    }
}