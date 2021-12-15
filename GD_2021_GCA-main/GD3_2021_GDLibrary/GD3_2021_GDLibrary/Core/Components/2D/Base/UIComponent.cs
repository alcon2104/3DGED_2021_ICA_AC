using System;

namespace GDLibrary.Components.UI
{
    /// <summary>
    /// A part of a ui object e.g. Change Texture on mouse over
    /// </summary>
    public abstract class UIComponent : IDisposable, IComparable, ICloneable
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
        /// Reference to the container ui object for all components
        /// </summary>
        protected internal UIObject uiObject;

        /// <summary>
        /// Reference to the object transform stored in the container game object
        /// </summary>
        protected internal Transform2D transform2D;

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

        public UIObject UIObject
        {
            get { return uiObject; }
            set { uiObject = value; }
        }

        public Transform2D Transform2D
        {
            get { return transform2D; }
            protected set { transform2D = value; }
        }

        #endregion Properties

        #region Constructors

        public UIComponent()
        {
            IsRunning = false;
            IsEnabled = true;
            ID = "UI_CMP-" + Guid.NewGuid();
        }

        #endregion Constructors

        #region Update

        public virtual void Update()
        {
            //Overridden in child class
        }

        #endregion Update

        #region Actions - Activation

        /// <summary>
        /// Called when the component is first instanciated
        /// </summary>
        public virtual void Awake()
        {
            if (uiObject == null)
                throw new NullReferenceException("This component is not attached to a game object!");

            //Cache the transform so that we can access in child components without double de-reference e.g. transform.LocalTranslation not gameObject.Transform.LocalTranslation
            if (transform2D == null)
                transform2D = uiObject.Transform;
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

        #region Actions - Components

        ///// <summary>
        ///// Adds a component to the parent UIObject
        ///// </summary>
        ///// <typeparam name="T">Instance of type UIComponent</typeparam>
        ///// <returns>Instance of the new component of type T</returns>
        ///// <seealso cref="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/new-constraint"/>
        //public T AddComponent<T>() where T : UIComponent, new()
        //{
        //    return uiObject.AddComponent<T>();
        //}

        ///// <summary>
        ///// Gets the first component in the parent UIObject of type T
        ///// </summary>
        ///// <typeparam name="T">Instance of type UIComponent</typeparam>
        ///// <returns>First instance of the component  of type T</returns>
        //public T GetComponent<T>() where T : UIComponent
        //{
        //    return uiObject.GetComponent<T>();
        //}

        ///// <summary>
        ///// Gets an array of all components in the parent UIObject of type T
        ///// </summary>
        ///// <typeparam name="T">Instance of type UIComponent</typeparam>
        ///// <returns>Array of instanced of the component of type T</returns>
        //public T[] GetComponents<T>() where T : UIComponent
        //{
        //    return uiObject.GetComponents<T>();
        //}

        #endregion Actions - Components

        #region Actions - Sorting

        public virtual int CompareTo(object obj)
        {
            var component = obj as UIComponent;

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