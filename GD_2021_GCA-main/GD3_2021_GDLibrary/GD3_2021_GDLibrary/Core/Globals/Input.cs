using GDLibrary.Inputs;
using System;

namespace GDLibrary
{
    /// <summary>
    /// Static class that contains input objects used in the engine.
    /// </summary>
    public class Input : IDisposable
    {
        /// <summary>
        /// Gets or sets keyboard inputs
        /// </summary>
        public static KeyboardComponent Keys { get; set; }

        /// <summary>
        /// Gets or sets mouse inputs.
        /// </summary>
        public static MouseComponent Mouse { get; set; }

        /// <summary>
        /// Gets or sets gamepad inputs.
        /// </summary>
        public static GamepadComponent Gamepad { get; set; }

        /// <summary>
        /// Called when we exit the application.
        /// </summary>
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}