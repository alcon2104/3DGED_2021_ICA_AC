using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace GDLibrary.Graphics
{
    /// <summary>
    /// Defines a textured 1x1 quad, centred on origin, facing +ve Z-axis
    /// </summary>
    public class QuadMesh : Mesh, ICloneable
    {
        protected override void CreateGeometry()
        {
            #region Positions

            var positions = new Vector3[4]
            {
                new Vector3(-0.5f, 0.5f, 0.0f),  //top left
                new Vector3(0.5f, 0.5f, 0.0f),   //top right
                new Vector3(0.5f, -0.5f, 0.0f),  //bottom right
                new Vector3(-0.5f, -0.5f, 0.0f)  //bottom left
            };

            #endregion Positions

            #region UVs

            var uvs = new Vector2[4]
             {
                new Vector2(0,0),
                new Vector2(1,0),
                new Vector2(1,1),
                new Vector2(0,1)
             };

            #endregion UVs

            vertices = new VertexPositionNormalTexture[4];

            for (int i = 0; i < 4; i++)
            {
                vertices[i].Position = positions[i];
                vertices[i].TextureCoordinate = uvs[i];
                vertices[i].Normal = Vector3.UnitZ; //facing toward the +ve Z-axis
            }

            indices = new ushort[] { 0, 1, 2, 0, 2, 3 };
        }

        #region Actions - Housekeeping

        public override object Clone()
        {
            return MemberwiseClone();
        }

        #endregion Actions - Housekeeping
    }
}