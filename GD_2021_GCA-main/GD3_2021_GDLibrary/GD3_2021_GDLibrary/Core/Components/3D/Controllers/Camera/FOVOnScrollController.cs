using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace GDLibrary.Components
{
    /// <summary>
    /// Increases/decreases camera FOV based on mouse scroll wheel direction
    /// </summary>
    public class FOVOnScrollController : Controller
    {
        #region Fields

        private Camera camera;
        private float originalFieldOfView;
        private float fovScrollDeltaInRadians;

        #endregion Fields

        public FOVOnScrollController(float fovScrollDeltaInRadians)
        {
            this.fovScrollDeltaInRadians = fovScrollDeltaInRadians;
        }

        public override void Awake(GameObject gameObject)
        {
            camera = GetComponent<Camera>();

            if (camera == null)
                throw new Exception("No camera attached to this game object.");

            originalFieldOfView = camera.FieldOfView;
        }

        public override void Update()
        {
            HandleInputs();
            HandleKeyboardInput();
            //    HandleGamepadInput(); //not using for this controller implementation
            //   base.Update(); //nothing happens so dont call this
        }

        protected override void HandleInputs()
        {
            HandleMouseInput();
        }

        protected override void HandleMouseInput()
        {
            var scrollDelta = Input.Mouse.GetDeltaFromScrollWheel();

            if (scrollDelta > 0)
                camera.FieldOfView -= fovScrollDeltaInRadians;
            else if (scrollDelta < 0)
                camera.FieldOfView += fovScrollDeltaInRadians;
            //notice we dont have a condition == 0 since we dont want any change on idle scroll wheel
        }

        protected override void HandleKeyboardInput()
        {
            if (Input.Keys.WasJustPressed(Keys.R))
            {
                camera.FieldOfView = originalFieldOfView;
            }
        }

        #region Unused

        protected override void HandleGamepadInput()
        {
        }

        #endregion Unused
    }
}