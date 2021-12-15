using GDLibrary.Components;
using GDLibrary.Core;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace GDLibrary.Managers
{
    /// <summary>
    /// SceneManager stores all scenes and calls update on currently active scene
    /// </summary>
    public class SceneManager : PausableGameComponent
    {
        #region Statics

        private static readonly int DEFAULT_SCENE_COUNT_AT_START = 4;
        private static readonly int DEFAULT_REMOVE_LIST_SIZE_AT_START = 20;

        #endregion Statics

        #region Fields

        private List<Scene> scenes;
        private List<GameObject> removeList;
        private int sceneToLoad;
        private int activeSceneIndex;
        private bool initialUpdate;

        #endregion Fields

        #region Properties & Indexer

        /// <summary>
        /// Allows us to access a scene in the SceneManager using [] notation e.g. sceneManager[0]
        /// </summary>
        /// <param name="index">Integer index of required scene</param>
        /// <returns>Instance of scene at the index specified</returns>
        public Scene this[int index]
        {
            get
            {
                return scenes[index];
            }
        }

        /// <summary>
        /// Returns the active scene
        /// </summary>
        public Scene ActiveScene
        {
            get { return activeSceneIndex >= 0 && activeSceneIndex < scenes.Count ? scenes[activeSceneIndex] : null; }
        }

        /// <summary>
        /// Count of scenes held in the manager scene list
        /// </summary>
        public int Count
        {
            get
            {
                return scenes.Count;
            }
        }

        #endregion Properties & Indexer

        #region Constructors

        public SceneManager(Game game) : base(game)
        {
            scenes = new List<Scene>(DEFAULT_SCENE_COUNT_AT_START);
            removeList = new List<GameObject>(DEFAULT_REMOVE_LIST_SIZE_AT_START);
            sceneToLoad = -1;
            activeSceneIndex = -1;
        }

        protected override void SubscribeToEvents()
        {
            //handle add/remove events
            EventDispatcher.Subscribe(EventCategoryType.GameObject, HandleGameObjectEvents);

            base.SubscribeToEvents();
        }

        protected void HandleGameObjectEvents(EventData eventData)
        {
            switch (eventData.EventActionType)
            {
                case EventActionType.OnRemoveObject:
                    Remove(eventData.Parameters[0] as GameObject);
                    break;

                case EventActionType.OnAddObject:
                    Add(eventData.Parameters[0] as GameObject);
                    break;

                default:
                    break;
                    //add more cases for each method that we want to support with events
            }

            //call base method because we want to participate in the pause/play events
            base.HandleEvent(eventData);
        }

        #endregion Constructors

        #region Actions - Update

        public override void Update(GameTime gameTime)
        {
            //is this component paused because of the menu?
            if (IsUpdated || !initialUpdate)
            {
                //apply batch remove
                PerformBatchRemove();

                initialUpdate = true;

                //if no active scene and no scene to load then exit
                if (activeSceneIndex == -1 && sceneToLoad == -1)
                    return;

                //if scene to load and its not the current scene
                if (sceneToLoad > -1)
                {
                    //unload current scene, if running
                    if (activeSceneIndex > -1)
                        scenes[activeSceneIndex].Unload();

                    //set scene to load as new scene
                    activeSceneIndex = sceneToLoad;

                    //set scene as current in globally accessible static in Scene
                    Scene.Current = scenes[activeSceneIndex];

                    //reset to -1 to show no scene is waiting to load
                    sceneToLoad = -1;

                    //initialize the new scene
                    scenes[activeSceneIndex].Initialize();
                }

                //update the scene (either new, or the same scene from last update)
                scenes[activeSceneIndex].Update();
            }
        }

        #endregion Actions - Update

        #region Actions - Add, Remove, Load, Find

        /// <summary>
        /// Add a new scene to the manager which by default is not the active scene
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="isActive"></param>
        public void Add(Scene scene, bool isActive = false)
        {
            if (!scenes.Contains(scene))
            {
                scenes.Add(scene);

                if (isActive)
                    sceneToLoad = scenes.Count - 1;
            }
        }

        public void Add(GameObject gameObject)
        {
            scenes[activeSceneIndex]?.Add(gameObject);
        }

        public void Remove(GameObject gameObject)
        {
            removeList.Add(gameObject);
        }

        private void PerformBatchRemove()
        {
            foreach (GameObject gameObject in removeList)
                scenes[activeSceneIndex]?.Remove(gameObject);

            removeList.Clear();
        }

        /// <summary>
        /// Remove a scene from the manager. Notice that you can't remove the default scene.
        /// </summary>
        /// <param name="scene">The scene to remove.</param>
        public void Remove(Scene scene)
        {
            var index = scenes.IndexOf(scene);

            if (index > 0)
            {
                //if we want to unload the active scene then set active to last scene in list, or zero
                if (activeSceneIndex == index)
                    activeSceneIndex = scenes.Count - 1;

                //call unload to remove any scene resources
                scenes[index].Unload();

                //remove from the list
                scenes.RemoveAt(index);
            }
        }

        public GameObject Find(Predicate<GameObject> predicate)
        {
            return ActiveScene.Find(predicate);
        }

        public List<GameObject> FindAll(Predicate<GameObject> predicate)
        {
            return ActiveScene.FindAll(predicate);
        }

        public void LoadScene(Scene scene)
        {
            if (scene != null)
            {
                var index = scenes.IndexOf(scene);

                if (index == -1)
                {
                    scenes.Add(scene);
                    index = scenes.Count - 1;
                }

                sceneToLoad = index;
            }
        }

        public void LoadScene(int index)
        {
            if (index >= 0 && index < scenes.Count)
            {
                sceneToLoad = index;
            }
            else
                throw new ArgumentOutOfRangeException("Index is out of range!");
        }

        public void LoadScene(string name)
        {
            var index = scenes.FindIndex(scene => scene.Name.Equals(name));
            if (index != -1)
                LoadScene(index);
            else
                throw new KeyNotFoundException($"No scene with name {name} exists in the scene manager!");
        }

        #endregion Actions - Add, Remove, Load, Find
    }
}