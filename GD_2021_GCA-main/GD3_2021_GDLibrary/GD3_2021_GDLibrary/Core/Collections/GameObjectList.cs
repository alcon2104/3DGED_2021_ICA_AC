using GDLibrary.Components;
using GDLibrary.Graphics;
using System;
using System.Collections.Generic;

namespace GDLibrary.Collections
{
    /// <summary>
    /// Actions that we can apply to a game object in a list
    /// </summary>
    public enum ComponentChangeType
    {
        Add, Update, Remove
    }

    /// <summary>
    /// Provides list storage for a gameobject in either a static or dynamic list
    /// The list itself is not static or dynamic, rather the game object may be static (e.g. a wall) or dynamic (e.g. a pickup spawned in game)
    /// </summary>
    public class GameObjectList : IDisposable
    {
        #region Fields

        /// <summary>
        /// Indicate the likely number of static objects in your game scene
        /// </summary>
        private static readonly int STATIC_LIST_DEFAULT_SIZE = 20;

        /// <summary>
        /// Indicate the likely number of dynamic objects in your game scene
        /// </summary>
        private static readonly int DYNAMIC_LIST_DEFAULT_SIZE = 10;

        protected List<GameObject> presistentList;
        protected List<GameObject> dynamicList;
        protected List<Renderer> renderers;
        protected List<Controller> controllers;
        protected List<Behaviour> behaviours;
        protected List<Material> materials;
        protected List<Camera> cameras;
        protected List<Collider> colliders;
        private bool isAlphaChanged;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets a list of all renderers attached to game objects in this scene
        /// </summary>
        public List<Renderer> Renderers => renderers;

        /// <summary>
        /// Gets  a list of all materials attached to renderers in this scene
        /// </summary>
        public List<Material> Materials => materials;

        /// <summary>
        /// Gets a list of all cameras in this scene
        /// </summary>
        public List<Camera> Cameras => cameras;

        /// <summary>
        /// Gets a list of all colliders in this scene
        /// </summary>
        public List<Collider> Colliders => colliders;

        #endregion Properties

        #region Constructors

        public GameObjectList()
        {
            presistentList = new List<GameObject>(STATIC_LIST_DEFAULT_SIZE);
            dynamicList = new List<GameObject>(DYNAMIC_LIST_DEFAULT_SIZE);
            renderers = new List<Renderer>(STATIC_LIST_DEFAULT_SIZE);
            controllers = new List<Controller>(STATIC_LIST_DEFAULT_SIZE);
            behaviours = new List<Behaviour>(STATIC_LIST_DEFAULT_SIZE);
            colliders = new List<Collider>(STATIC_LIST_DEFAULT_SIZE);

            //TODO - replace hardcoded with some reasonable constants
            materials = new List<Material>(5);
            cameras = new List<Camera>(5);

            Material.AlphaPropertyChanged += HandleAlphaPropertyChanged;
        }

        private void HandleAlphaPropertyChanged()
        {
            isAlphaChanged = true;
        }

        #endregion Constructors

        #region Actions - Update, Unload, Clear

        public virtual void Update()
        {
            if (isAlphaChanged)
            {
                isAlphaChanged = false;
                renderers.Sort((x, y) => y.Material.Alpha.CompareTo(x.Material.Alpha));
            }

            for (int i = 0; i < presistentList.Count; i++)
            {
                if (presistentList[i].IsEnabled)
                    presistentList[i].Update();
            }

            for (int i = 0; i < dynamicList.Count; i++)
            {
                if (dynamicList[i].IsEnabled)
                    dynamicList[i].Update();
            }
        }

        public virtual void Unload()
        {
            foreach (GameObject gameObject in presistentList)
                gameObject.Dispose();

            foreach (GameObject gameObject in dynamicList)
                gameObject.Dispose();

            //TODO - add Dispose for materials, behaviours etc

            Clear();
        }

        protected void Clear()
        {
            presistentList.Clear();
            dynamicList.Clear();
            controllers.Clear();
            behaviours.Clear();
            colliders.Clear();
            renderers.Clear();
            materials.Clear();
            cameras.Clear();
        }

        #endregion Actions - Update, Unload, Clear

        #region Actions - Add, Remove, Find, CheckComponents

        public void Add(Scene scene, GameObject gameObject)
        {
            //add to the appropriate list of objects in the scene
            if (gameObject.IsPersistent)
                presistentList.Add(gameObject);
            else
                dynamicList.Add(gameObject);

            //set the scene that object belongs to
            gameObject.Scene = scene;

            //TODO - add root transform set and notify change
            //gameObject.Transform.Root = transform;

            //if object enabled then add to appropriate list
            if (gameObject.IsEnabled)
                CheckComponents(gameObject, ComponentChangeType.Add);

            //if object isn't initialized then init
            if (!gameObject.IsRunning)
                gameObject.Initialize();
        }

        public void Remove(GameObject obj)
        {
            //remove the game object
            if (obj.IsPersistent)
                presistentList.Remove(obj);
            else
                dynamicList.Remove(obj);

            //remove the objects components e.g. Renderer etc
            CheckComponents(obj, ComponentChangeType.Remove);
        }

        public GameObject Find(Predicate<GameObject> predicate)
        {
            GameObject found = presistentList.Find(predicate);
            if (found == null)
                found = dynamicList.Find(predicate);

            return found;
        }

        public List<GameObject> FindAll(Predicate<GameObject> predicate)
        {
            List<GameObject> found = presistentList.FindAll(predicate);
            if (found == null)
                found = dynamicList.FindAll(predicate);

            return found;
        }

        protected void CheckComponents(GameObject gameObject, ComponentChangeType type)
        {
            for (int i = 0; i < gameObject.Components.Count; i++)
            {
                var component = gameObject.Components[i];

                if (component is Renderer renderer)
                {
                    if (type == ComponentChangeType.Add)
                        AddRenderer(renderer);
                    else if (type == ComponentChangeType.Remove)
                        RemoveRenderer(renderer);
                }
                else if (component is Collider collider)
                {
                    if (type == ComponentChangeType.Add)
                        AddCollider(collider);
                    else if (type == ComponentChangeType.Remove)
                        RemoveCollider(collider);
                }
                else if (component is Behaviour behaviour)
                {
                    if (type == ComponentChangeType.Add)
                        AddBehaviour(behaviour);
                    else if (type == ComponentChangeType.Remove)
                        RemoveBehaviour(behaviour);
                }
                else if (component is Controller controller)
                {
                    if (type == ComponentChangeType.Add)
                        AddController(controller);
                    else if (type == ComponentChangeType.Remove)
                        RemoveController(controller);
                }
                else if (component is Camera camera)
                {
                    if (type == ComponentChangeType.Add)
                        AddCamera(camera);
                    else if (type == ComponentChangeType.Remove)
                        RemoveCamera(camera);
                }
            }
        }

        protected int AddCamera(Camera camera)
        {
            var index = cameras.IndexOf(camera);

            if (index == -1)
            {
                cameras.Add(camera);
                cameras.Sort();
                index = cameras.Count - 1;

                if (Camera.Main == null)
                    Camera.Main = camera;
            }

            return index;
        }

        protected void Add<E>(List<E> list, E obj, bool sortEnabled)
        {
            if (list.Contains(obj))
                return;

            list.Add(obj);

            if (sortEnabled)
                list.Sort();
        }

        protected bool Remove<E>(List<E> list, E obj)
        {
            if (list.Contains(obj))
                return list.Remove(obj);

            return false;
        }

        protected void RemoveCamera(Camera camera)
        {
            if (cameras.Contains(camera))
                cameras.Remove(camera);
        }

        protected void AddRenderer(Renderer renderer)
        {
            if (renderers.Contains(renderer))
                return;

            renderers.Add(renderer);
            renderers.Sort((x, y) => y.Material.Alpha.CompareTo(x.Material.Alpha));
        }

        protected void RemoveRenderer(Renderer renderer)
        {
            if (renderers.Contains(renderer))
                renderers.Remove(renderer);
        }

        protected void AddCollider(Collider collider)
        {
            if (colliders.Contains(collider))
                return;

            colliders.Add(collider);
            colliders.Sort();
        }

        protected void RemoveCollider(Collider collider)
        {
            if (colliders.Contains(collider))
                colliders.Remove(collider);
        }

        protected void AddController(Controller controller)
        {
            if (controllers.Contains(controller))
                return;

            controllers.Add(controller);
            controllers.Sort();
        }

        protected void RemoveController(Controller controller)
        {
            if (controllers.Contains(controller))
                controllers.Remove(controller);
        }

        protected void AddBehaviour(Behaviour behaviour)
        {
            if (behaviours.Contains(behaviour))
                return;

            behaviours.Add(behaviour);
            behaviours.Sort();
        }

        protected void RemoveBehaviour(Behaviour behaviour)
        {
            if (behaviours.Contains(behaviour))
                behaviours.Remove(behaviour);
        }

        public void Dispose()
        {
            Clear();
        }

        #endregion Actions - Add, Remove, Find, CheckComponents
    }
}