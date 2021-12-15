using GDLibrary.Components;
using JigLibX.Geometry;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace GDLibrary.Utilities
{
    public class CollisionUtility
    {
        private static Vector2 leftBottom;
        private static Vector2 leftTop;
        private static Vector2 max;
        private static Vector2 min;
        private static Vector2 rightBottom;
        private static Vector2 rightTop;
        private static Matrix worldMatrix;

        /// <summary>
        /// Calculates an axis aligned rectangle which fully contains an arbitrarily transformed axis aligned rectangle.
        /// </summary>
        /// <param name="rectangle">Original bounding rectangle.</param>
        /// <param name="transform">World transform of the rectangle.</param>
        /// <returns>A new rectangle which contains the trasnformed rectangle.</returns>
        public static Microsoft.Xna.Framework.Rectangle CalculateTransformedBoundingRectangle(Microsoft.Xna.Framework.Rectangle rectangle, Matrix transform)
        {
            //   Matrix inverseMatrix = Matrix.Invert(transform);
            // Get all four corners in local space
            leftTop = new Vector2(rectangle.Left, rectangle.Top);
            rightTop = new Vector2(rectangle.Right, rectangle.Top);
            leftBottom = new Vector2(rectangle.Left, rectangle.Bottom);
            rightBottom = new Vector2(rectangle.Right, rectangle.Bottom);

            // Transform all four corners into work space
            Vector2.Transform(ref leftTop, ref transform, out leftTop);
            Vector2.Transform(ref rightTop, ref transform, out rightTop);
            Vector2.Transform(ref leftBottom, ref transform, out leftBottom);
            Vector2.Transform(ref rightBottom, ref transform, out rightBottom);

            // Find the minimum and maximum extents of the rectangle in world space
            min = Vector2.Min(Vector2.Min(leftTop, rightTop),
                                      Vector2.Min(leftBottom, rightBottom));
            max = Vector2.Max(Vector2.Max(leftTop, rightTop),
                                      Vector2.Max(leftBottom, rightBottom));

            // Return that as a rectangle
            return new Microsoft.Xna.Framework.Rectangle((int)Math.Round(min.X), (int)Math.Round(min.Y),
                                 (int)Math.Round(max.X - min.X), (int)Math.Round(max.Y - min.Y));
        }

        /// <summary>
        /// Generates a TriangleMesh (i.e. a complex collision surface) from a provided model
        /// </summary>
        /// <param name="model"></param>
        /// <param name="transform"></param>
        /// <returns></returns>
        public static TriangleMesh GetTriangleMesh(Model model,
            Vector3 translation, Vector3 rotation, Vector3 scale)
        {
            TriangleMesh triangleMesh = new TriangleMesh();
            List<Vector3> vertexList = new List<Vector3>();
            List<TriangleVertexIndices> indexList = new List<TriangleVertexIndices>();

            ExtractData(vertexList, indexList, model);

            worldMatrix = GetWorldMatrix(translation, rotation, scale);

            for (int i = 0; i < vertexList.Count; i++)
            {
                vertexList[i] = Vector3.Transform(vertexList[i], worldMatrix);
            }

            // create the collision mesh
            triangleMesh.CreateMesh(vertexList, indexList, 1, 1.0f);

            return triangleMesh;
        }

        protected static Matrix GetWorldMatrix(Vector3 translation,
            Vector3 rotation, Vector3 scale)
        {
            return Matrix.Identity *
                Matrix.CreateScale(scale) *
                        Matrix.CreateRotationX(MathHelper.ToRadians(rotation.X)) *
                        Matrix.CreateRotationY(MathHelper.ToRadians(rotation.Y)) *
                        Matrix.CreateRotationZ(MathHelper.ToRadians(rotation.Z)) *
                                    Matrix.CreateTranslation(translation);
        }

        protected static void ExtractData(List<Vector3> vertices, List<TriangleVertexIndices> indices, Model model)
        {
            Matrix[] bones_ = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(bones_);
            foreach (ModelMesh mm in model.Meshes)
            {
                int offset = vertices.Count;
                Matrix xform = bones_[mm.ParentBone.Index];
                foreach (ModelMeshPart mmp in mm.MeshParts)
                {
                    Vector3[] a = new Vector3[mmp.NumVertices];
                    int stride = mmp.VertexBuffer.VertexDeclaration.VertexStride;
                    mmp.VertexBuffer.GetData(mmp.VertexOffset * stride, a, 0, mmp.NumVertices, stride);

                    for (int i = 0; i != a.Length; ++i)
                        Vector3.Transform(ref a[i], ref xform, out a[i]);
                    vertices.AddRange(a);

                    if (mmp.IndexBuffer.IndexElementSize != IndexElementSize.SixteenBits)
                        throw new Exception(String.Format("Model uses 32-bit indices, which are not supported."));

                    short[] s = new short[mmp.PrimitiveCount * 3];
                    mmp.IndexBuffer.GetData(mmp.StartIndex * 2, s, 0, mmp.PrimitiveCount * 3);

                    JigLibX.Geometry.TriangleVertexIndices[] tvi = new JigLibX.Geometry.TriangleVertexIndices[mmp.PrimitiveCount];
                    for (int i = 0; i != tvi.Length; ++i)
                    {
                        tvi[i].I0 = s[i * 3 + 2] + offset;
                        tvi[i].I1 = s[i * 3 + 1] + offset;
                        tvi[i].I2 = s[i * 3 + 0] + offset;
                    }
                    indices.AddRange(tvi);
                }
            }
        }
    }
}