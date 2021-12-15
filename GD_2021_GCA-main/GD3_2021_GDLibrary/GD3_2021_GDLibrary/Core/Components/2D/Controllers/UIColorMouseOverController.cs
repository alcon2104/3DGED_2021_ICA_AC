using Microsoft.Xna.Framework;
using System;

namespace GDLibrary.Components.UI
{
    public class UIColorMouseOverBehaviour : UIBehaviour
    {
        private Color colorActive, colorInactive;

        public UIColorMouseOverBehaviour(Color colorActive, Color colorInactive) : base(id, controllerType)
        {
            this.colorActive = colorActive;
            this.colorInactive = colorInactive;
        }

        public override void Update(GameTime gameTime, IActor actor)
        {
            DrawnActor2D drawnActor = actor as DrawnActor2D;

            if (drawnActor != null)
            {
                if (drawnActor.Transform2D.Bounds.Contains(mouseManager.Bounds))
                {
                    drawnActor.Color = colorActive;
                }
                else
                {
                    drawnActor.Color = colorInactive;
                }
            }

            base.Update(gameTime, actor);
        }

        //to do...Equals, GetHashCode, Clone
    }
}