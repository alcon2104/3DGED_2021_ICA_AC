using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace GDLibrary.Components.UI
{
    /// <summary>
    /// Stores, updates and draws a collection (list) of ui objects
    /// </summary>
    public class UIScene
    {
        #region Statics

        private static readonly int DEFAULT_SIZE = 10;

        #endregion Statics

        #region Fields

        /// <summary>
        /// Unique identifier for each ui scene - may be used for search, sort later
        /// </summary>
        private string id;

        /// <summary>
        /// Friendly name for the current ui scene
        /// </summary>
        private string name;

        /// <summary>
        /// List of all ui objects drawn and update in this scene
        /// </summary>
        private List<UIObject> uiObjects;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Instanciate a scene to store ui objects for a particular view in the game (e.g. a menu screen, a ui layout)
        /// </summary>
        /// <param name="name">string</param>
        public UIScene(string name)
        {
            Name = name;
            ID = "UIS-" + Guid.NewGuid();
            uiObjects = new List<UIObject>(DEFAULT_SIZE);
        }

        #endregion Constructors

        #region Properties

        public string ID { get => id; set => id = value; }

        public string Name { get => name; set => name = value.Trim(); }

        public List<UIObject> UiObjects { get => uiObjects; set => uiObjects = value; }

        #endregion Properties

        #region Actions - Add, Remove, Find, Check

        public void Add(UIObject uiObject)
        {
            uiObjects.Add(uiObject);
        }

        public bool Remove(UIObject uiObject)
        {
            return uiObjects.Remove(uiObject);
        }

        public UIObject Find(Predicate<UIObject> predicate)
        {
            return uiObjects.Find(predicate);
        }

        public List<UIObject> FindAll(Predicate<UIObject> predicate)
        {
            return uiObjects.FindAll(predicate);
        }

        #endregion Actions - Add, Remove, Find, Check

        #region Actions - Update & Draw

        public virtual void Update()
        {
            foreach (UIObject uiObject in uiObjects)
            {
                if (uiObject.IsEnabled)
                    uiObject.Update();
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            foreach (UIObject uiObject in uiObjects)
            {
                //TODO - add IsRunning also?
                if (uiObject.IsEnabled)
                    uiObject.Draw(spriteBatch);
            }
        }

        #endregion Actions - Update & Draw
    }
}