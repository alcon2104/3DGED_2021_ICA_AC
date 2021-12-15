using GDLibrary.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace GDLibrary.Components
{
    public class ModelRenderer : Renderer
    {
        #region Fields

        /// <summary>
        /// Stores vertex, normal, uv data for the model
        /// </summary>
        protected Model model;

        /// <summary>
        /// Stores bone transforms for the model (e.g. each mesh will normally have one bone)
        /// </summary>
        protected Matrix[] boneTransforms;

        #endregion Fields

        #region Properties

        public Model Model
        {
            get
            {
                return model;
            }
            set
            {
                if (value != model)
                {
                    model = value;

                    if (model != null)
                    {
                        boneTransforms = new Matrix[model.Bones.Count];
                        model.CopyAbsoluteBoneTransformsTo(boneTransforms);

                        //TODO - BUG - on clone
                        //SetBoundingVolume();
                    }
                }
            }
        }

        #endregion Properties

        public ModelRenderer(Model model, Material material) : base(material)
        {
            Model = model;
        }

        public override void SetBoundingVolume()
        {
            if (model != null)
            {
                foreach (ModelMesh mesh in model.Meshes)
                    boundingSphere = BoundingSphere.CreateMerged(BoundingSphere, mesh.BoundingSphere);

                boundingSphere.Center = gameObject.Transform.LocalTranslation;
                boundingSphere.Transform(gameObject.Transform.WorldMatrix);
                boundingSphere.Radius *= Math.Max(
                    Math.Max(transform.LocalScale.X, transform.LocalScale.Y),
                    transform.LocalScale.Z);
            }
        }

        public override void Draw(GraphicsDevice device, Effect effect)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                var basicEffect = effect as BasicEffect;

                //set world for game object
                basicEffect.World = boneTransforms[mesh.ParentBone.Index] * transform.WorldMatrix;

                //set pass
                basicEffect.CurrentTechnique.Passes[0].Apply();

                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    device.SetVertexBuffer(meshPart.VertexBuffer);
                    device.Indices = meshPart.IndexBuffer;
                    device.DrawIndexedPrimitives(PrimitiveType.TriangleList, meshPart.VertexOffset, meshPart.StartIndex, meshPart.PrimitiveCount);
                }
            }

            ////  Alternatively we can draw using the BasicEffect
            //foreach (ModelMesh mesh in model.Meshes)
            //{
            //    foreach (BasicEffect effect in mesh.Effects)
            //    {
            //        effect.World = boneTransforms[mesh.ParentBone.Index] * transform.WorldMatrix;
            //        effect.View = Camera.Main.ViewMatrix;
            //        effect.Projection = Camera.Main.ProjectionMatrix;
            //        effect.EnableDefaultLighting();
            //        effect.CurrentTechnique.Passes[0].Apply();
            //    }
            //    mesh.Draw();
            //}
        }

        #region Actions - Housekeeping

        //TODO - Dispose

        public override object Clone()
        {
            return new ModelRenderer(model, material.Clone() as Material);
        }

        #endregion Actions - Housekeeping
    }
}