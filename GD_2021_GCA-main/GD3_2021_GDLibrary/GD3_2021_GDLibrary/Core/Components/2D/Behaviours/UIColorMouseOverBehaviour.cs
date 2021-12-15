using Microsoft.Xna.Framework;

namespace GDLibrary.Components.UI
{
    /// <summary>
    /// Changes button color on mouse over
    /// </summary>
    public class UIColorMouseOverBehaviour : UIBehaviour
    {
        private Color colorActive, colorInactive;

        public UIColorMouseOverBehaviour(Color colorActive, Color colorInactive)
            : base()
        {
            this.colorActive = colorActive;
            this.colorInactive = colorInactive;
        }

        public override void Update()
        {
            var uiButtonObject = uiObject as UIButtonObject;

            if (uiButtonObject != null)
            {
                if (uiButtonObject.Bounds.Contains(Input.Mouse.Bounds))
                {
                    uiButtonObject.Color = colorActive;

                    //EventDispatcher.Raise();
                }
                else
                    uiButtonObject.Color = colorInactive;
            }
        }

        //to do...Equals, GetHashCode, Clone
    }
}