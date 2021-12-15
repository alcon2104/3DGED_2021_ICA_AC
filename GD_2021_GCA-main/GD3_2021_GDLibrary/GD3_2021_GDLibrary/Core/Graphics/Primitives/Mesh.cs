using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace GDLibrary.Graphics
{
    public abstract class Mesh : ICloneable
    {
        #region Fields

        protected VertexPositionNormalTexture[] vertices;
        protected ushort[] indices;
        protected VertexBuffer vertexBuffer;
        protected IndexBuffer indexBuffer;

        #endregion Fields

        #region Constructors

        public VertexPositionNormalTexture[] Vertices
        {
            get { return vertices; }
            set { vertices = value; }
        }

        public ushort[] Indices
        {
            get { return indices; }
            set { indices = value; }
        }

        public VertexBuffer VertexBuffer
        {
            get { return vertexBuffer; }
            protected set { vertexBuffer = value; }
        }

        public IndexBuffer IndexBuffer
        {
            get { return indexBuffer; }
            protected set { indexBuffer = value; }
        }

        #endregion Constructors

        #region Constructors

        public Mesh()
        {
            //set up the position, normal, texture vertex array
            CreateGeometry();
            //set up the buffers on VRAM with the vertex array and index array
            CreateBuffers();
        }

        #endregion Constructors

        #region Actions

        protected abstract void CreateGeometry();

        private void CreateBuffers()
        {
            var graphicsDevice = Application.GraphicsDevice;

            vertexBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionNormalTexture), vertices.Length, BufferUsage.WriteOnly);
            vertexBuffer.SetData(vertices);

            indexBuffer = new IndexBuffer(graphicsDevice, typeof(ushort), indices.Length, BufferUsage.WriteOnly);
            indexBuffer.SetData(indices);
        }

        public void ComputeNormals()
        {
            for (int i = 0; i < vertices.Length; i++)
                vertices[i].Normal = Vector3.Zero;

            for (int i = 0; i < indices.Length / 3; i++)
            {
                int index0 = indices[i * 3];
                int index1 = indices[i * 3 + 1];
                int index2 = indices[i * 3 + 2];

                // Select the face
                Vector3 side1 = vertices[index0].Position - vertices[index2].Position;
                Vector3 side2 = vertices[index0].Position - vertices[index1].Position;
                Vector3 normal = Vector3.Cross(side1, side2);

                vertices[index0].Normal += normal;
                vertices[index1].Normal += normal;
                vertices[index2].Normal += normal;
            }

            for (int i = 0; i < vertices.Length; i++)
            {
                var normal = vertices[i].Normal;
                var len = normal.Length();
                if (len > 0.0f)
                    vertices[i].Normal = normal / len;
                else
                    vertices[i].Normal = Vector3.Zero;
            }
        }

        public Vector3[] GetVertices()
        {
            Vector3[] verts = new Vector3[vertices.Length];
            for (int i = 0; i < vertices.Length; i++)
                verts[i] = Vertices[i].Position;
            return verts;
        }

        public Vector3[] GetNormals()
        {
            Vector3[] normals = new Vector3[vertices.Length];
            for (int i = 0; i < vertices.Length; i++)
                normals[i] = Vertices[i].Normal;
            return normals;
        }

        #region Actions - Housekeeping

        public abstract object Clone();

        #endregion Actions - Housekeeping

        #endregion Actions
    }
}