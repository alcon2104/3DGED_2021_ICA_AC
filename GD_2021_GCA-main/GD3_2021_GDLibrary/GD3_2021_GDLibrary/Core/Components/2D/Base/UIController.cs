namespace GDLibrary.Components.UI
{
    /// <summary>
    /// Parent class for any controller ui component that accepts input and modifies a ui object's transform2D, texture, text, origin, color etc
    /// </summary>
    public abstract class UIController : UIComponent
    {
        #region Input

        /// <summary>
        /// Calls handle methods for Keyboard/Mouse/Gamepad
        /// </summary>
        protected abstract void HandleInputs();

        /// <summary>
        /// Read and apply the keyboard input.
        /// </summary>
        protected abstract void HandleKeyboardInput();

        /// <summary>
        /// Read and apply the mouse input.
        /// </summary>
        protected abstract void HandleMouseInput();

        /// <summary>
        /// Read and apply the gamepad input.
        /// </summary>
        protected abstract void HandleGamepadInput();

        #endregion Input
    }
}