using GDApp;
using GDLibrary.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace GDLibrary
{
    /// <summary>
    /// Static class that contains global objects used in the engine.
    /// </summary>
    public class Application : IDisposable
    {
        /// <summary>
        /// Gets or sets the main game
        /// </summary>
        public static Game Main { get; set; }

        /// <summary>
        /// Gets or sets the content manager.
        /// </summary>
        public static ContentManager Content { get; set; }

        /// <summary>
        /// Gets or sets the graphics device.
        /// </summary>
        public static GraphicsDevice GraphicsDevice { get; set; }

        /// <summary>
        /// Gets or sets the graphics device manager.
        /// </summary>
        public static GraphicsDeviceManager GraphicsDeviceManager { get; set; }

        /// <summary>
        /// Gets or sets the scene manager.
        /// </summary>
        public static SceneManager SceneManager { get; internal set; }

        /// <summary>
        /// Gets or sets the state manager.
        /// </summary>
        public static MyStateManager StateManager { get; internal set; }

        /// <summary>
        /// Gets the physics manager.
        /// </summary>
        public static PhysicsManager PhysicsManager { get; internal set; }

        /// <summary>
        /// Called when we exit the application.
        /// </summary>
        public void Dispose()
        {
            //TODO - do we need to do anything here that isnt done in SceneManager, Scene, GameObject, or Component Dispose() method calls?
            throw new NotImplementedException();
        }
    }
}