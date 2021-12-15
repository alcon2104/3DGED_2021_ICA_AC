namespace GDLibrary.Components
{
    /// <summary>
    /// Parent class for any controller component that
    /// accepts input and modifies a game object's transform
    /// </summary>
    public abstract class Controller : Component
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