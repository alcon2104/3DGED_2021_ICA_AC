using GDLibrary.Collections;
using GDLibrary.Graphics;
using System;
using System.Collections.Generic;

namespace GDLibrary.Components
{
    /// <summary>
    /// Stores and updates all game objects for a level or part of a level
    /// </summary>
    public class Scene : GameObject
    {
        #region Statics

        private static Scene current;
        public static Scene Current { get => current; set => current = value; }

        #endregion Statics

        #region Fields

        protected GameObjectList gameObjects;
        private int cycleIndex = 0;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the gameobjectlist of all game objects in this scene
        /// </summary>
        public GameObjectList GameObjectList => gameObjects;

        /// <summary>
        /// Gets a list of all renderers attached to game objects in this scene
        /// </summary>
        public List<Renderer> Renderers => gameObjects.Renderers;

        /// <summary>
        /// Gets  a list of all materials attached to renderers in this scene
        /// </summary>
        public List<Material> Materials => gameObjects.Materials;

        /// <summary>
        /// Gets a list of all cameras in this scene
        /// </summary>
        public List<Camera> Cameras => gameObjects.Cameras;

        /// <summary>
        /// Gets a list of all cameras in this scene
        /// </summary>
        public List<Collider> Colliders => gameObjects.Colliders;

        #endregion Properties

        #region Constructors

        public Scene(string name) : base(name, GameObjectType.Scene)
        {
            gameObjects = new GameObjectList();
        }

        #endregion Constructors

        #region Update, Unload, Clear

        public override void Update()
        {
            //TODO - is this done twice?
            //update any components attached to this scene
            base.Update();

            //update all the static and dynamic game objects in this scene
            gameObjects.Update();
        }

        public virtual void Unload()
        {
            gameObjects.Unload();
        }

        #endregion Update, Unload, Clear

        #region Actions - Add, Remove, Find, Check

        public void Add(GameObject gameObject)
        {
            gameObjects.Add(this, gameObject);
        }

        public void Remove(GameObject gameObject)
        {
            gameObjects.Remove(gameObject);
        }

        public GameObject Find(Predicate<GameObject> predicate)
        {
            return gameObjects.Find(predicate);
        }

        public List<GameObject> FindAll(Predicate<GameObject> predicate)
        {
            return gameObjects.FindAll(predicate);
        }

        /// <summary>
        /// Returns a list of all the cameras in this scene
        /// </summary>
        /// <returns></returns>
        /// <see cref="GDLibrary.Managers.RenderManager.Draw(Microsoft.Xna.Framework.GameTime)"/>
        public List<Camera> GetAllActiveSceneCameras()
        {
            return gameObjects.Cameras;
        }

        /// <summary>
        /// Sets the Main camera for the game using an appropriate predicate
        /// </summary>
        /// <param name="predicate">Predicate of type GameObject</param>
        /// <returns>True if set, otherwise false</returns>
        public bool SetMainCamera(Predicate<GameObject> predicate)
        {
            //look for the cameras game object
            var cameraObject = gameObjects.Find(predicate);

            //if not null then look for a camera component
            var camera = cameraObject?.GetComponent<Camera>();

            //if no camera then null
            if (camera == null)
                throw new ArgumentException("Predicate did not return a valid camera!");

            //set this camera as Main in the game
            Camera.Main = camera;

            return true;
        }

        /// <summary>
        /// Sets the Main camera for the game using the Name of the parent game object
        /// </summary>
        /// <param name="name">String name of the parent GameObject</param>
        /// <returns>True if set, otherwise false</returns>
        public bool SetMainCamera(string name)
        {
            return SetMainCamera(gameObject => gameObject.Name == name);
        }

        public void CycleCameras()
        {
            cycleIndex = cycleIndex < gameObjects.Cameras.Count - 1 ? ++cycleIndex : 0;
            Camera.Main = gameObjects.Cameras[cycleIndex];
        }

        #endregion Actions - Add, Remove, Find, Check
    }
}