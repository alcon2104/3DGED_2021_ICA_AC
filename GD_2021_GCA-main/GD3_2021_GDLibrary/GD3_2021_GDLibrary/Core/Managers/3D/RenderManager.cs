using GDLibrary.Components;
using GDLibrary.Core;
using GDLibrary.Renderers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace GDLibrary.Managers
{
    /// <summary>
    /// Renders the game objects in the currently active scene using the IRenderScene and single, or multi cameras, available
    /// </summary>
    public class RenderManager : PausableDrawableGameComponent
    {
        #region Fields

        protected GraphicsDevice graphicsDevice;
        protected IRenderScene sceneRenderer;
        protected bool isMultiCamera;
        private bool isPausableOverridden;
        protected List<Camera> activeSceneCameras;

        #endregion Fields

        #region Properties

        protected IRenderScene SceneRenderer
        {
            get => sceneRenderer;
            set
            {
                if (value == null)
                    throw new ArgumentNullException("You must set a valid renderer (e.g. ForwardRenderer) for the render manager!");

                //if we're setting a different renderer then set it!
                if (sceneRenderer != value)
                    sceneRenderer = value;
            }
        }

        protected bool IsMultiCamera
        {
            get => isMultiCamera;
            set => isMultiCamera = value;
        }

        #endregion Properties

        #region Constructors

        public RenderManager(Game game, IRenderScene sceneRenderer) : this(game, sceneRenderer, false, false)
        {
        }

        public RenderManager(Game game, IRenderScene sceneRenderer, bool isMultiCamera = false, bool isPausableOverridden = false) : base(game)
        {
            //cache this de-reference to save us some CPU cycles since its called often in Render below
            graphicsDevice = Application.GraphicsDevice;

            //set the render used to render/draw the scene
            SceneRenderer = sceneRenderer;

            //sets whether we are drawing multiple cameras
            this.isMultiCamera = isMultiCamera;

            //this overrides pausing so that render manager is drawn during menu scene
            this.isPausableOverridden = isPausableOverridden;
        }

        #endregion Constructors

        #region Actions - Draw

        public override void Draw(GameTime gameTime)
        {
            //is this component paused because of the menu?
            if (IsDrawn || isPausableOverridden)
            {
                if (isMultiCamera)
                {
                    //get a reference to the list of all cameras for the current scene
                    activeSceneCameras = Application.SceneManager.ActiveScene.GetAllActiveSceneCameras();

                    //draw scene with each camera
                    foreach (var camera in activeSceneCameras)
                    {
                        sceneRenderer.Render(graphicsDevice, camera);
                    }
                }
                else
                {
                    //draw scene with main camera
                    sceneRenderer.Render(graphicsDevice, Camera.Main);
                }
            }
        }

        #endregion Actions - Draw
    }
}