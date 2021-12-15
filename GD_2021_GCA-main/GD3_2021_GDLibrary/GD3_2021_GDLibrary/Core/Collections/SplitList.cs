using System;
using System.Collections.Generic;

namespace GDLibrary.Core.Collections
{
    /// <summary>
    /// Provides list storage for a gameobject in either a static or dynamic list
    /// The list itself is not static or dynamic, rather the game object may be static (e.g. a wall) or dynamic (e.g. a pickup spawned in game)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <see cref="https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/generics/constraints-on-type-parameters"/>
    public class GameObjectList
    {
        /// <summary>
        /// Indicate the likely number of static objects in your game scene
        /// </summary>
        private static readonly int STATIC_LIST_DEFAULT_SIZE = 20;

        /// <summary>
        /// Indicate the likely number of dynamic objects in your game scene
        /// </summary>
        private static readonly int DYNAMIC_LIST_DEFAULT_SIZE = 10;

        protected List<GameObject> staticList;
        protected List<GameObject> dynamicList;

        public GameObjectList()
        {
            staticList = new List<GameObject>(STATIC_LIST_DEFAULT_SIZE);
            dynamicList = new List<GameObject>(DYNAMIC_LIST_DEFAULT_SIZE);
        }

        public void Add(GameObject gameObject)
        {
            if (obj.IsStatic())
                staticList.Add(gameObject);
            else
                dynamicList.Add(gameObject);
        }

        public void Remove(GameObject gameObject)
        {
            if (obj.IsStatic())
                staticList.Add(gameObject);
            else
                dynamicList.Add(gameObject);
        }

        public GameObject Find(Predicate<GameObject> predicate)
        {
            GameObject found = staticList.Find(predicate);
            if (found == null)
                found = dynamicList.Find(predicate);

            return found;
        }

        public List<GameObject> FindAll(Predicate<T> predicate)
        {
            List<GameObject> found = staticList.FindAll(predicate);
            if (found == null)
                found = dynamicList.FindAll(predicate);

            return found;
        }

        public IEnumerator<GameObject> GetStaticListEnumerator()
        {
            return staticList.GetEnumerator();
        }

        public IEnumerator<GameObject> GetDynamicListEnumerator()
        {
            return dynamicList.GetEnumerator();
        }

        protected void Clear()
        {
            staticList.Clear();
            dynamicList.Clear();
        }
    }
}