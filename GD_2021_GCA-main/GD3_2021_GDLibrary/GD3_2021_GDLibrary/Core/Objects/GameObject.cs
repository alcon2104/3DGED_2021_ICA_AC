using GDLibrary.Components;
using System;
using System.Collections.Generic;

namespace GDLibrary
{
    public enum GameObjectType : sbyte //0 - 255
    {
        Scene,
        Camera,
        Player,
        NPC,
        Interactable,
        Consumable,
        Architecture,
        Environment,
        Skybox,
        Editor,
        Prop,
        Ground
        //STU - add more for your game here...
    }

    //TODO - Add IComparable?
    /// <summary>
    /// Base object in the game scene
    /// </summary>
    public class GameObject : IDisposable, ICloneable
    {
        #region Statics

        private static readonly int DEFAULT_COMPONENT_LIST_SIZE = 4;    //typical minimum number of components added to a GameObject

        #endregion Statics

        #region Fields

        //Type, Tag, LayerMask, ID

        /// <summary>
        /// Indicates whether the game object will be removed during game play (e.g. any 3D object that persists during gameplay is there for entire game)
        /// </summary>
        protected bool isPersistent = true;

        /// <summary>
        /// Enumerated type indicating what category ths game object belongs to (e.g. Camera, Pickup, NPC, Interactable)
        /// </summary>
        protected GameObjectType gameObjectType;

        /// <summary>
        /// Unique identifier for each game object - may be used for search, sort later
        /// </summary>
        private string id;

        /// <summary>
        /// Friendly name for the current object
        /// </summary>
        protected string name;

        /// <summary>
        /// Set on first update of the component in SceneManager::Update
        /// </summary>
        private bool isRunning;

        /// <summary>
        /// Set in constructor to true. By default all components are enabled on instanciation
        /// </summary>
        private bool isEnabled;

        /// <summary>
        /// Scene to which this game object belongs
        /// </summary>
        protected Scene scene;

        /// <summary>
        /// Stores S, R, T of GameObject to generate the world matrix
        /// </summary>
        protected Transform transform;

        /// <summary>
        /// List of all attached components
        /// </summary>
        protected List<Component> components;

        #endregion Fields

        #region Properties

        public bool IsPersistent  //replaces IsStatic for clarity of terminology
        {
            get { return isPersistent; }
            set { isPersistent = value; }
        }

        /// <summary>
        /// Gets/sets the game object type
        /// </summary>
        public GameObjectType GameObjectType
        {
            get { return gameObjectType; }
            protected set { gameObjectType = value; }
        }

        /// <summary>
        /// Gets/sets the unique ID
        /// </summary>
        public string ID
        {
            get
            {
                return id;
            }
            protected set
            {
                id = value;
            }
        }

        /// <summary>
        /// Gets/sets the game object name
        /// </summary>
        public string Name { get => name; set => name = value.Trim(); }

        /// <summary>
        /// Gets boolean to determine if the object has run in first update cycle
        /// </summary>
        public bool IsRunning
        {
            get { return isRunning; }
        }

        /// <summary>
        /// Gets/sets boolean to allow object to be disabled (e.g. no update or draw) within a scene
        /// </summary>
        public bool IsEnabled
        {
            get
            {
                return isEnabled;
            }
            set
            {
                if (value != isEnabled)
                {
                    isEnabled = value;

                    //TODO - notify component has been enabled and enable all child components
                }
            }
        }

        /// <summary>
        /// Gets/sets the scene that the current object is used in
        /// </summary>
        public Scene Scene
        {
            get { return scene; }
            set { scene = value; }
        }

        /// <summary>
        /// Gets/sets the transform associated with the current game object
        /// </summary>
        public Transform Transform { get => transform; protected set => transform = value; }

        /// <summary>
        /// Gets a list of all components (e.g. controllers, behaviours, camera) of the current object
        /// </summary>
        public List<Component> Components { get => components; }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Instantiates a 3D game object
        /// </summary>
        /// <param name="name">String preferably a unique name but not essential</param>
        /// <param name="gameObjectType">GameObjectType one of an enum of types in case we ever want to search by type</param>
        public GameObject(string name, GameObjectType gameObjectType)
            : this(name, gameObjectType, true)
        {
        }

        /// <summary>
        /// Instantiates a 3D game object
        /// </summary>
        /// <param name="name">String preferably a unique name but not essential</param>
        /// <param name="gameObjectType">GameObjectType one of an enum of types in case we ever want to search by type</param>
        /// <param name="isPersistent">bool decides which list we put the game object in when added to the scene (i.e. persistent(lasts all the game) or dynamic (added/removed during gameplay)</param>
        public GameObject(string name,
           GameObjectType gameObjectType, bool isPersistent)
        {
            InternalConstructor(name, gameObjectType, isPersistent);
        }

        private void InternalConstructor(string name,
            GameObjectType gameObjectType, bool isPersistent)
        {
            this.gameObjectType = gameObjectType;
            if (transform == null)
            {
                components = new List<Component>(DEFAULT_COMPONENT_LIST_SIZE);          //instanciate list
                transform = new Transform();                                            //add default transform
                transform.transform = transform;
                transform.GameObject = this;                                            //tell transform who it belongs to
                transform.Awake(this);
                components.Add(transform);                                              //add transform to the list
            }

            isEnabled = true;
            isRunning = false;
            IsPersistent = isPersistent; //by default we will consider any new object static (i.e. belongs to a static list in GameObjectList used in Scene)

            ID = "GO-" + Guid.NewGuid();
            Name = string.IsNullOrEmpty(name) ? ID : name;
        }

        #endregion Constructors

        #region Initialization

        /// <summary>
        /// Called when the game object is run in the first update
        /// </summary>
        public virtual void Initialize()
        {
            if (!isRunning)
            {
                isRunning = true;

                //TODO - Add sort IComparable in each component
                components.Sort();

                for (int i = 0; i < components.Count; i++)
                    components[i].Start();
            }
        }

        #endregion Initialization

        #region Update

        /// <summary>
        /// Called each update to call an update on all components of the game object
        /// </summary>
        public virtual void Update()
        {
            for (int i = 0; i < components.Count; i++)
                components[i].Update();
        }

        #endregion Update

        #region Add & Get Components

        /// <summary>
        /// Adds a sppecific instance of a component
        /// Note - We cannot add a second Transform to any GameObject
        /// </summary>
        /// <typeparam name="T">Instance of a Component</typeparam>
        /// <returns>Instance of the component</returns>
        /// <seealso cref="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/new-constraint"/>
        public Component AddComponent(Component component)
        {
            if (component == null)
                return null;

            var transform = component as Transform;

            if (transform != null)
            {
                //TODO - check IsPersistent on components - is default true ok?
                InternalConstructor(name, gameObjectType, true);
                transform.SetTranslation(transform.LocalTranslation);
                transform.SetRotation(transform.LocalRotation);
                transform.SetScale(transform.LocalScale);
            }
            else
            {
                //set this as component's parent game object
                component.GameObject = this;
                //set components transform same as this component
                component.transform = this.transform;
                //perform any initial wake up operations
                component.Awake(this);
                //TODO - prevent duplicate components? Component::Equals and GetHashCode need to be implemented
                components.Add(component);
            }

            if (isRunning && !component.IsRunning)
            {
                component.Start();
                component.IsRunning = true;
                //TODO - check scene is running
                components.Sort();
            }

            //TODO - notify component change?

            return component;
        }

        /// <summary>
        /// Adds a component of type T
        /// </summary>
        /// <typeparam name="T">Instance of type Component</typeparam>
        /// <returns>Instance of the new component of type T</returns>
        /// <seealso cref="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/new-constraint"/>
        public T AddComponent<T>() where T : Component, new()
        {
            var component = new T();

            return (T)AddComponent(component);
        }

        /// <summary>
        /// Gets the first component of type T
        /// </summary>
        /// <typeparam name="T">Instance of type Component</typeparam>
        /// <returns>First instance of the component  of type T</returns>
        public T GetComponent<T>() where T : Component
        {
            for (int i = 0; i < components.Count; i++)
            {
                if (components[i] is T)
                    return components[i] as T;
            }

            return null;
        }

        /// <summary>
        /// Gets the first component of type T conforming to the supplied predicate
        /// </summary>
        /// <typeparam name="T">Instance of type Component</typeparam>
        /// <param name="pred">Predicate ot type T</param>
        /// <returns></returns>
        public T GetComponent<T>(Predicate<Component> pred) where T : Component
        {
            return components.Find(pred) as T;
        }

        /// <summary>
        /// Gets an array of all components of type T
        /// </summary>
        /// <typeparam name="T">Instance of type Component</typeparam>
        /// <returns>Array of instanced of the component of type T</returns>
        public T[] GetComponents<T>() where T : Component
        {
            List<T> componentList = new List<T>();

            for (int i = 0; i < components.Count; i++)
            {
                if (components[i] is T)
                    componentList.Add(components[i] as T);
            }

            return componentList.ToArray();
        }

        /// <summary>
        /// Gets all components of type T conforming to the supplied predicate
        /// </summary>
        /// <typeparam name="T">Instance of type Component</typeparam>
        /// <param name="pred">Predicate ot type T</param>
        /// <returns></returns>
        public T[] GetComponents<T>(Predicate<Component> pred) where T : Component
        {
            return components.FindAll(pred).ToArray() as T[];
        }

        #endregion Add & Get Components

        #region Housekeeping

        public virtual void Dispose()
        {
            foreach (Component component in components)
                component.Dispose();
        }

        public virtual object Clone()
        {
            var clone = new GameObject($"Clone - {Name}", gameObjectType);
            clone.ID = "GO-" + Guid.NewGuid();

            Component clonedComponent = null;
            Transform clonedTransform = null;

            foreach (Component component in components)
            {
                clonedComponent = clone.AddComponent((Component)component.Clone());
                clonedComponent.gameObject = clone;

                clonedTransform = clonedComponent as Transform;

                if (clonedTransform != null)
                    clonedComponent.transform = clonedTransform;
            }

            clone.IsPersistent = this.isPersistent;
            return clone;
        }

        #endregion Housekeeping
    }
}