using Microsoft.Xna.Framework;
using System;

namespace GDLibrary
{
    /// <summary>
    /// Replaces GameTime for any game objects that want to slow down, or speed up, time in the game
    /// </summary>
    /// <example>
    ///     var time = Time.GetInstance(this);  //In Main.cs we create the first and only instance
    ///     Components.Add(time);               //Add to the component list so that it will be updated
    /// </example>
    /// <seealso cref="https://refactoring.guru/design-patterns/singleton/csharp/example"/>
    public class Time : GameComponent
    {
        #region Statics

        /// <summary>
        ///  Singleton instance to allow global accessibility
        /// </summary>
        private static Time instance;

        #endregion Statics

        #region Fields

        /// <summary>
        /// Time between updates
        /// </summary>
        private float deltaTimeMs;

        /// <summary>
        /// Total elapsed time since start
        /// </summary>
        private float totalGameTimeMs;

        /// <summary>
        /// 0-1 scale factor used to slow down time (e.g. slo-mo effects)
        /// </summary>
        private float timeScale = 1;

        /// <summary>
        /// Total frames since game start
        /// </summary>
        private long totalFrames = 0;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Scale at which time passes (0-1)
        /// </summary>
        public float TimeScale { get => timeScale; set => timeScale = value >= 0 ? value : 1; }

        /// <summary>
        /// Unscaled interval in time from the last frame to the current one
        /// </summary>
        public float UnscaledDeltaTimeMs => deltaTimeMs;

        /// <summary>
        /// Scaled interval in time from the last frame to the current one
        /// </summary>
        public float DeltaTimeMs => deltaTimeMs * timeScale;

        /// <summary>
        /// Unscaled time since the game started
        /// </summary>
        public float UnscaledTotalTimeMs => totalGameTimeMs;

        /// <summary>
        /// Scaled time since the game started
        /// </summary>
        public float TotalGameTimeMs => totalGameTimeMs * timeScale;

        /// <summary>
        /// Count of frames since the game started
        /// </summary>
        public long TotalFrames => totalFrames;

        public static Time Instance
        {
            get
            {
                if (instance == null)
                    throw new NullReferenceException("Instance is null. Has GetInstance been called at startup?");

                return instance;
            }
        }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Constructs a Singleton instance - usually called in Main to initialize the entity once
        /// </summary>
        /// <param name="game"></param>
        /// <returns></returns>
        public static Time GetInstance(Game game)
        {
            if (instance == null)
                instance = new Time(game);

            return instance;
        }

        private Time(Game game) : base(game)
        {
        }

        #endregion Constructors

        #region Update

        public override void Update(GameTime gameTime)
        {
            totalFrames++;
            deltaTimeMs = gameTime.ElapsedGameTime.Milliseconds;
            totalGameTimeMs = (float)gameTime.TotalGameTime.TotalMilliseconds;
        }

        #endregion Update
    }
}