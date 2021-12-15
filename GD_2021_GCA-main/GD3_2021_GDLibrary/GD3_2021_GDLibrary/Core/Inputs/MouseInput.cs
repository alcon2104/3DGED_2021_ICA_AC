using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GDLibrary.Inputs
{
    public enum MouseButton : sbyte
    {
        Left, Middle, Right, Any
    }

    public class MouseComponent : GameComponent
    {
        #region Fields

        private MouseState currentState;
        private MouseState previousState;
        protected Vector2 mouseDelta;

        #endregion Fields

        #region Properties

        public int X
        {
            get { return currentState.X; }
        }

        public int Y
        {
            get { return currentState.Y; }
        }

        public Vector2 Position
        {
            get { return new Vector2(currentState.X, currentState.Y); }
            set { Mouse.SetPosition((int)value.X, (int)value.Y); }
        }

        public Vector2 PreviousPosition
        {
            get { return new Vector2(previousState.X, previousState.Y); }
        }

        public int Wheel
        {
            get { return currentState.ScrollWheelValue - previousState.ScrollWheelValue; }
        }

        public Vector2 Delta
        {
            get { return mouseDelta; }
        }

        public bool IsMoving
        {
            get { return (currentState.X != previousState.X) || (currentState.Y != previousState.Y); }
        }

        public bool IsDragging(MouseButton button = MouseButton.Left)
        {
            return IsDown(button) && IsMoving;
        }

        #endregion Properties

        #region Constructors

        public MouseComponent(Game game)
         : base(game)
        {
            currentState = Mouse.GetState();
            previousState = currentState;
            mouseDelta = Vector2.Zero;
        }

        #endregion Constructors

        #region Update

        public override void Update(GameTime gameTime)
        {
            previousState = currentState;
            currentState = Mouse.GetState();
            base.Update(gameTime);
        }

        #endregion Update

        #region Actions

        public virtual bool IsDown(MouseButton button)
        {
            return MouseButtonState(button, ButtonState.Pressed);
        }

        public virtual bool IsUp(MouseButton button = MouseButton.Left)
        {
            return MouseButtonState(button, ButtonState.Released);
        }

        protected virtual bool MouseButtonState(MouseButton button, ButtonState state)
        {
            bool result = false;

            switch (button)
            {
                case MouseButton.Left: result = currentState.LeftButton == state; break;
                case MouseButton.Middle: result = currentState.MiddleButton == state; break;
                case MouseButton.Right: result = currentState.RightButton == state; break;
            }

            return result;
        }

        public virtual bool WasJustClicked(MouseButton button = MouseButton.Left)
        {
            bool clicked = false;

            if (button == MouseButton.Left)
                clicked = currentState.LeftButton == ButtonState.Released && previousState.LeftButton == ButtonState.Pressed;
            else if (button == MouseButton.Middle)
                clicked = currentState.MiddleButton == ButtonState.Released && previousState.MiddleButton == ButtonState.Pressed;
            else if (button == MouseButton.Right)
                clicked = currentState.RightButton == ButtonState.Released && previousState.RightButton == ButtonState.Pressed;

            return clicked;
        }

        #endregion Actions
    }
}