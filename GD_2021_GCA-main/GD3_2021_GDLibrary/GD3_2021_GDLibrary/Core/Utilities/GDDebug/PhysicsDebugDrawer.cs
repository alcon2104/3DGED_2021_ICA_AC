using GDLibrary.Components;
using GDLibrary.Core;
using JigLibX.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace GDLibrary.Utilities.GDDebug
{
    public class PhysicsDebugDrawer : PausableDrawableGameComponent
    {
        private Color collisionSkinColor = Color.Red;
        private BasicEffect effect;

        //temps
        private List<VertexPositionColor> vertexData;

        private VertexPositionColor[] wf;

        public PhysicsDebugDrawer(Game game, Color collisionSkinColor)
            : base(game)
        {
            this.collisionSkinColor = collisionSkinColor;

            vertexData = new List<VertexPositionColor>();
            effect = new BasicEffect(Application.GraphicsDevice);
            effect.AmbientLightColor = Vector3.One;
            effect.VertexColorEnabled = true;
        }

        public override void Draw(GameTime gameTime)
        {
            if (IsDrawn)
            {
                //add the vertices for each and every drawn object (opaque or transparent) to the vertexData array for drawing
                foreach (Collider collider in Application.SceneManager.ActiveScene.Colliders)
                    AddCollisionSkinVertexData(collider);

                //no vertices to draw - would happen if we forget to call DrawCollisionSkins() above or there were no drawn objects to see!
                if (vertexData.Count == 0)
                    return;

                //draw skin
                Game.GraphicsDevice.Viewport = Camera.Main.Viewport;
                effect.View = Camera.Main.ViewMatrix;
                effect.Projection = Camera.Main.ProjectionMatrix;
                effect.CurrentTechnique.Passes[0].Apply();
                GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineStrip, vertexData.ToArray(), 0, vertexData.Count - 1);

                //reset data
                vertexData.Clear();
            }
        }

        public void AddVertexDataForShape(List<Vector3> shape, Color color)
        {
            if (vertexData.Count > 0)
            {
                Vector3 v = vertexData[vertexData.Count - 1].Position;
                vertexData.Add(new VertexPositionColor(v, color));
                vertexData.Add(new VertexPositionColor(shape[0], color));
            }

            foreach (Vector3 p in shape)
            {
                vertexData.Add(new VertexPositionColor(p, color));
            }
        }

        public void AddVertexDataForShape(List<Vector3> shape, Color color, bool closed)
        {
            AddVertexDataForShape(shape, color);

            Vector3 v = shape[0];
            vertexData.Add(new VertexPositionColor(v, color));
        }

        public void AddVertexDataForShape(List<VertexPositionColor> shape, Color color)
        {
            if (vertexData.Count > 0)
            {
                Vector3 v = vertexData[vertexData.Count - 1].Position;
                vertexData.Add(new VertexPositionColor(v, color));
                vertexData.Add(new VertexPositionColor(shape[0].Position, color));
            }

            foreach (VertexPositionColor vps in shape)
            {
                vertexData.Add(vps);
            }
        }

        public void AddVertexDataForShape(VertexPositionColor[] shape, Color color)
        {
            if (vertexData.Count > 0)
            {
                Vector3 v = vertexData[vertexData.Count - 1].Position;
                vertexData.Add(new VertexPositionColor(v, color));
                vertexData.Add(new VertexPositionColor(shape[0].Position, color));
            }

            foreach (VertexPositionColor vps in shape)
            {
                vertexData.Add(vps);
            }
        }

        public void AddVertexDataForShape(List<VertexPositionColor> shape, Color color, bool closed)
        {
            AddVertexDataForShape(shape, color);

            VertexPositionColor v = shape[0];
            vertexData.Add(v);
        }

        public void AddCollisionSkinVertexData(Collider collider)
        {
            if (collider.GameObject.GameObjectType != GameObjectType.Ground)
            {
                wf = collider.Collision.GetLocalSkinWireframe();

                // if the collision skin was also added to the body
                // we have to transform the skin wireframe to the body space
                if (collider.Body.CollisionSkin != null)
                {
                    collider.Body.TransformWireframe(wf);
                }

                AddVertexDataForShape(wf, collisionSkinColor);
            }
        }
    }
}