using Microsoft.Xna.Framework.Media;
using System;

namespace GDLibrary
{
    /// <summary>
    /// Encapsulates the properties of a playable video
    /// </summary>
    /// <see cref="http://rbwhitaker.wikidot.com/video-playback"/>
    public class VideoCue
    {
        #region Fields

        /// <summary>
        /// Unique auto generated id
        /// </summary>
        private string id;

        /// <summary>
        /// A user defined video file in supported format
        /// </summary>
        private Video video;

        /// <summary>
        /// Video volume [0-1]
        /// </summary>
        private float volume;

        /// <summary>
        /// Get/set looping
        /// </summary>
        private bool isLooped;

        /// <summary>
        /// Get/set is muted
        /// </summary>
        private bool isMuted;

        /// <summary>
        /// Time between each frame update in MS
        /// </summary>
        private int frameUpdateRateMS;

        #endregion Fields

        #region Properties

        public string Name { get => video.FileName; }
        public string Id { get => id; }
        public Video Video { get => video; set => video = value; }
        public bool IsLooped { get => isLooped; set => isLooped = value; }
        public bool IsMuted { get => isMuted; set => isMuted = value; }
        public TimeSpan Duration { get => video.Duration; }
        public float Volume { get => volume; set => volume = value >= 0 && value <= 1 ? value : 0.5f; }

        public int FrameUpdateRateMS
        {
            get
            {
                return frameUpdateRateMS;
            }
        }

        #endregion Properties

        public VideoCue(Video video)
           : this(video, 1, false, false)
        {
        }

        //REFACTOR - remove isMuted in next iteration
        public VideoCue(Video video, float volume = 1, bool isLooped = false)
            : this(video, volume, isLooped, volume == 0)
        {
        }

        public VideoCue(Video video, float volume = 1, bool isLooped = false, bool isMuted = false)
        {
            id = $"VC-" + Guid.NewGuid();
            this.video = video;
            frameUpdateRateMS = (int)Math.Ceiling(1000.0f / video.FramesPerSecond);
            Volume = volume;
            this.isLooped = isLooped;
            this.isMuted = isMuted;
        }

        //TODO - Clone, Equals, GetHashCode
    }
}