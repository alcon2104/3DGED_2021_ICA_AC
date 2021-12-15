using GDLibrary.Core;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System;
using System.IO;

namespace GDLibrary.Components.UI
{
    public class UIVideoTextureBehaviour : UIBehaviour, IDisposable
    {
        #region Fields

        private VideoCue videoCue;

        #endregion Fields

        #region Fields

        /// <summary>
        /// A player to play, pause, stop, resume, loop a user-defined video file
        /// </summary>
        private VideoPlayer videoPlayer;

        /// <summary>
        /// Cached reference to the UITextureObject for this controller
        /// </summary>
        private UITextureObject uiTextureObject;

        private float timeSinceLastFrameUpdateMS;

        #endregion Fields

        public UIVideoTextureBehaviour(VideoCue videoCue)
        {
            this.videoCue = videoCue;
            videoPlayer = new VideoPlayer(); ;
            videoPlayer.IsMuted = videoCue.IsMuted;
            videoPlayer.Volume = videoCue.IsMuted ? 0 : videoCue.Volume;

            //subscribe to events that will affect the state of this behaviour
            SubscribeToEvents();
        }

        #region Handle Events

        /// <summary>
        /// Subscribe to any events that will affect any child class (e.g. menu pause in ObjectManager)
        /// </summary>
        protected virtual void SubscribeToEvents()
        {
            EventDispatcher.Subscribe(EventCategoryType.Video, HandleEvent);
        }

        protected virtual void HandleEvent(EventData eventData)
        {
            var targetUIObjectName = eventData.Parameters[0] as string;

            //if no parameter or the event isnt meant for me then exit
            if (targetUIObjectName == null || !targetUIObjectName.Equals(uiObject.Name))
                return;

            switch (eventData.EventActionType)
            {
                case EventActionType.OnPlay:
                    videoPlayer?.Play(videoCue.Video);
                    break;

                case EventActionType.OnPause:
                    videoPlayer?.Pause();
                    break;

                case EventActionType.OnResume:
                    videoPlayer?.Resume();
                    break;

                case EventActionType.OnStop:
                    videoPlayer?.Stop();
                    break;

                case EventActionType.OnMute:
                    videoPlayer.Volume = 0;
                    break;

                case EventActionType.OnUnMute:
                    videoPlayer.Volume = videoCue.Volume;
                    break;

                case EventActionType.OnVolumeSet:
                    //make sure that we send volume in the object parameters!
                    videoPlayer.Volume = (float)eventData.Parameters[1];
                    break;

                default:
                    break;
            }
        }

        #endregion Handle Events

        public override void Awake()
        {
            //access the texture so we can update it!
            uiTextureObject = uiObject as UITextureObject;
        }

        public override void Update()
        {
            if (videoPlayer.State == MediaState.Playing)
            {
                timeSinceLastFrameUpdateMS += Time.Instance.DeltaTimeMs;

                //if time to update frame and we are playing
                if (timeSinceLastFrameUpdateMS >= videoCue.FrameUpdateRateMS)
                {
                    Texture2D texture = videoPlayer.GetTexture();
                    timeSinceLastFrameUpdateMS -= videoCue.FrameUpdateRateMS;
                    uiTextureObject.DefaultTexture = texture;
                }
            }

            //is finished and supposed to loop then restart
            if (videoCue.IsLooped && videoPlayer.State == MediaState.Stopped)
                videoPlayer.Play(videoCue.Video);
        }

        protected override void OnDisabled()
        {
            videoPlayer?.Pause();
            base.OnDisabled();
        }

        public override void Dispose()
        {
            videoPlayer?.Dispose();
        }

        //TODO - Clone
    }
}