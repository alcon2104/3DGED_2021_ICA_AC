using System;

namespace GDLibrary.Components
{
    /// <summary>
    /// A part of a game object e.g. Mesh, MeshRenderer, Camera, FirstPersonController
    /// </summary>
    public abstract class Component : IDisposable, IComparable, ICloneable
    {
        #region Fields

        /// <summary>
        /// Unique identifier for each component - may be used for search, sort later
        /// </summary>
        private string id;

        /// <summary>
        /// Unique name for each component - may be used for search, sort later
        /// </summary>
        private string name;

        /// <summary>
        /// Set to true on the first update of the component
        /// </summary>
        private bool isRunning;

        /// <summary>
        /// Allows us to enable/disable a component during the game (e.g. enable component that opens a door, disable a component that re-generates health)
        /// </summary>
        private bool isEnabled;

        /// <summary>
        /// Sort order we can use to sort the update order of component in a game object
        /// </summary>
        private int sortOrder = 1;

        /// <summary>
        /// Reference to the container game object for all components
        /// </summary>
        protected internal GameObject gameObject;

        /// <summary>
        /// Reference to the object transform stored in the container game object
        /// </summary>
        protected internal Transform transform;

        #endregion Fields

        #region Properties

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

        public string Name
        {
            get
            {
                return name;
            }
            protected set
            {
                name = value;
            }
        }

        public bool IsRunning
        {
            get
            {
                return isRunning;
            }
            set
            {
                isRunning = value;
            }
        }

        public bool IsEnabled
        {
            get { return isEnabled; }
            set
            {
                if (value != isEnabled)
                {
                    isEnabled = value;

                    if (isEnabled)
                        OnEnabled();
                    else
                        OnDisabled();

                    //TODO - notify event enable/disable
                }
            }
        }

        public int SortOrder
        {
            get
            {
                return sortOrder;
            }
            protected set
            {
                if (value != sortOrder)
                {
                    sortOrder = value;
                    //TODO - notify property changed
                }
            }
        }

        public GameObject GameObject
        {
            get { return gameObject; }
            set { gameObject = value; }
        }

        public Transform Transform
        {
            get { return transform; }
            protected set { transform = value; }
        }

        #endregion Properties

        #region Constructors

        public Component()
        {
            IsRunning = false;
            IsEnabled = true;
            ID = "CMP-" + Guid.NewGuid();
        }

        #endregion Constructors

        #region Actions - Activation

        /// <summary>
        /// Called when the component is first instanciated
        /// </summary>
        public virtual void Awake(GameObject gameObject)
        {
            if (gameObject == null)
                throw new NullReferenceException("This component is not attached to a game object!");

            //Cache the transform so that we can access in child components without double de-reference e.g. transform.LocalTranslation not gameObject.Transform.LocalTranslation
            if (transform == null)
                transform = gameObject.Transform;
        }

        /// <summary>
        /// Called when the component runs on the first update
        /// </summary>
        public virtual void Start()
        {
            IsRunning = true;
        }

        /// <summary>
        /// Called when the component is enabled (e.g. enable animation on a crane arm in a lab)
        /// </summary>
        protected virtual void OnEnabled()
        {
            //Overridden in child class
        }

        /// <summary>
        /// Called when the component is disabled (e.g. disable rotation on a pickup)
        /// </summary>
        protected virtual void OnDisabled()
        {
            //Overridden in child class
        }

        #endregion Actions - Activation

        #region Update

        public virtual void Update()
        {
            //Overridden in child class
        }

        #endregion Update

        #region Actions - Components

        /// <summary>
        /// Adds a component to the parent GameObject
        /// </summary>
        /// <typeparam name="T">Instance of type Component</typeparam>
        /// <returns>Instance of the new component of type T</returns>
        /// <seealso cref="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/new-constraint"/>
        public T AddComponent<T>() where T : Component, new()
        {
            return gameObject.AddComponent<T>();
        }

        /// <summary>
        /// Gets the first component in the parent GameObject of type T
        /// </summary>
        /// <typeparam name="T">Instance of type Component</typeparam>
        /// <returns>First instance of the component  of type T</returns>
        public T GetComponent<T>() where T : Component
        {
            return gameObject.GetComponent<T>();
        }

        /// <summary>
        /// Gets an array of all components in the parent GameObject of type T
        /// </summary>
        /// <typeparam name="T">Instance of type Component</typeparam>
        /// <returns>Array of instanced of the component of type T</returns>
        public T[] GetComponents<T>() where T : Component
        {
            return gameObject.GetComponents<T>();
        }

        #endregion Actions - Components

        #region Actions - Sorting

        public virtual int CompareTo(object obj)
        {
            var component = obj as Component;

            if (component == null)
                return 1;

            if (this == component)
                return 0;

            if (sortOrder == component.sortOrder)
                return 0;
            else if (sortOrder > component.sortOrder)
                return 1;
            else
                return -1;
        }

        #endregion Actions - Sorting

        #region Actions - Housekeeping

        public virtual void Dispose()
        {
            //Overridden in child class
        }

        public virtual object Clone()
        {
            return MemberwiseClone();
        }

        #endregion Actions - Housekeeping

        //TODO - add virtual reset, clone(memberwise) with no code
    }
}