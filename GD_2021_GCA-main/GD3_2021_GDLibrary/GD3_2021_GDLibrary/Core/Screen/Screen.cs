using Microsoft.Xna.Framework;

namespace GDLibrary.Core
{
    /// <summary>
    ///  A singleton class which provides access to methods relating to screen resolution, mouse visibility
    /// </summary>
    public sealed class Screen
    {
        #region Statics

        private static Screen instance;

        #endregion Statics

        #region Fields

        private Rectangle screenRectangle;
        private Vector2 screenCentre;

        #endregion Fields

        #region Properties

        public Rectangle ScreenRectangle
        {
            get => screenRectangle;
            set
            {
                screenRectangle = value;
                screenCentre = new Vector2(screenRectangle.Width / 2, screenRectangle.Height / 2);
            }
        }

        public Vector2 ScreenCentre { get => screenCentre; }

        //TODO - add code to show/hide mouse based on cursor lock
        public bool IsCursorLocked { get; set; }

        public bool IsMouseVisible
        {
            get { return Application.Main.IsMouseVisible; }
            set { Application.Main.IsMouseVisible = value; }
        }

        public static Screen Instance { get => instance; }

        #endregion Properties

        #region Constructors

        private Screen()
        {
        }

        /// <summary>
        /// Returns a singleton instance
        /// </summary>
        public static Screen GetInstance()
        {
            if (instance == null)
            {
                instance = new Screen();
            }

            return instance;
        }

        #endregion Constructors

        #region Actions

        /// <summary>
        /// Sets common screen parameters at any point in the game (e.g. resolution, mouse visibility)
        /// </summary>
        /// <param name="width">Screen width</param>
        /// <param name="height">Screen height</param>
        /// <param name="isMouseVisible"></param>
        /// <param name="isCursorLocked"></param>
        public void Set(int width, int height, bool? isMouseVisible, bool? isCursorLocked)
        {
            ScreenRectangle = new Rectangle(0, 0, width, height);

            if (isCursorLocked.HasValue)
                IsCursorLocked = isCursorLocked.Value;

            if (isMouseVisible.HasValue)
                IsMouseVisible = isMouseVisible.Value;

            Application.GraphicsDeviceManager.PreferredBackBufferWidth = width;
            Application.GraphicsDeviceManager.PreferredBackBufferHeight = height;
            Application.GraphicsDeviceManager.ApplyChanges();

            //TODO - raise event screen size changed
        }

        /// <summary>
        /// Allow us to toggle full screen on/off
        /// </summary>
        public void ToggleFullscreen()
        {
            Application.GraphicsDeviceManager.ToggleFullScreen();
        }

        #endregion Actions
    }
}