﻿using GDLibrary.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;

namespace GDLibrary.Managers
{
    /// <summary>
    /// Stores sound effect and sound properties for a particular sound.
    /// This class demonstrates the use of the sealed keyword to prevent inheritance.
    /// </summary>
    public sealed class Cue : IDisposable
    {
        #region Fields

        private string id;
        private SoundEffect soundEffect;
        private SoundCategoryType soundCategoryType;
        private Vector3 volumePitchPan;
        private bool isLooped;

        //private int maxPlayCount; //-1, 10, 1
        //private int timeToLiveInMs;  //Kashmir - 45000
        //private int minTimeSinceLastPlayedInMs; //1000, 60000

        #endregion Fields

        #region Properties

        public string ID
        {
            get
            {
                return id;
            }
        }

        public SoundEffect SoundEffect
        {
            get
            {
                return soundEffect;
            }
        }

        public bool IsLooped
        {
            get
            {
                return isLooped;
            }
            set
            {
                isLooped = value;
            }
        }

        public float Volume
        {
            get
            {
                return volumePitchPan.X;
            }
            set
            {
                volumePitchPan.X = (value >= 0 && value <= 1) ? value : 1;
            }
        }

        public float Pitch
        {
            get
            {
                return volumePitchPan.Y;
            }
            set
            {
                volumePitchPan.Y = (value >= -1 && value <= 1) ? value : 0;
            }
        }

        public float Pan
        {
            get
            {
                return volumePitchPan.Z;
            }
            set
            {
                volumePitchPan.Z = (value >= -1 && value <= 1) ? value : 0;
            }
        }

        public SoundCategoryType SoundCategoryType
        {
            get
            {
                return soundCategoryType;
            }
            set
            {
                soundCategoryType = value;
            }
        }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Cue is a sound that comes from a specified SoundEffect file, has a category, characteristics (pitch, pan, volume) and is looped or not
        /// </summary>
        /// <param name="id"></param>
        /// <param name="soundEffect"></param>
        /// <param name="soundCategoryType"></param>
        /// <param name="volumePitchPan"></param>
        /// <param name="isLooped"></param>
        public Cue(string id, SoundEffect soundEffect, SoundCategoryType soundCategoryType, Vector3 volumePitchPan, bool isLooped)
        {
            this.id = id.Trim();
            this.soundEffect = soundEffect;
            this.soundCategoryType = soundCategoryType;
            this.volumePitchPan = volumePitchPan;
            this.isLooped = isLooped;
        }

        #endregion Constructors

        #region Housekeeping

        public void Dispose()
        {
            ((IDisposable)SoundEffect).Dispose();
        }

        #endregion Housekeeping
    }

    /// <summary>
    /// Creates a dictionary of (string,Cue) pairs which we can use to play a sound by creating
    /// an instance of the SoundEffect stored within the Cue object. We can set Volume, Pitch, Pan and looping
    /// on the sound before or while it is playing.
    ///
    /// This class also demonstrates the use of the sealed keyword to prevent inheritance.
    /// </summary>
    /// <seealso cref="https://docs.monogame.net/api/Microsoft.Xna.Framework.Audio.SoundEffect.html"/>
    public sealed class SoundManager : PausableGameComponent, IDisposable
    {
        #region Fields

        private Dictionary<string, Cue> dictionary;
        private List<KeyValuePair<string, SoundEffectInstance>> listInstances2D;

        #endregion Fields

        #region Constructors

        public SoundManager(Game game)
            : this(game, StatusType.Updated)
        {
        }

        public SoundManager(Game game, StatusType statusType)
            : base(game, statusType)
        {
            dictionary = new Dictionary<string, Cue>();
            listInstances2D = new List<KeyValuePair<string, SoundEffectInstance>>();
        }

        #endregion Constructors

        #region Event Handling

        protected override void SubscribeToEvents()
        {
            EventDispatcher.Subscribe(EventCategoryType.Sound, HandleEvent);

            EventDispatcher.Subscribe(EventCategoryType.Player, HandlePlayerEvents);

            //if we always want the SoundManager to be available then comment this line out
            // base.SubscribeToEvents();
        }

        private void HandlePlayerEvents(EventData eventData)
        {
            if (eventData.EventActionType == EventActionType.OnPickup)
            {
                //strip out the parameters (e.g. from PickupBehaviour) and play a sound
            }
        }

        protected override void HandleEvent(EventData eventData)
        {
            switch (eventData.EventActionType)
            {
                case EventActionType.OnPlay2D:
                    Play2D(eventData.Parameters[0] as string);
                    break;

                case EventActionType.OnPlay3D:
                    Play3D(eventData.Parameters[0] as string,
                    eventData.Parameters[1] as AudioListener,
                        eventData.Parameters[2] as AudioEmitter);
                    break;

                case EventActionType.OnPause:
                    Pause(eventData.Parameters[0] as string);
                    break;

                case EventActionType.OnResume:
                    Resume(eventData.Parameters[0] as string);
                    break;

                case EventActionType.OnStop:
                    Stop(eventData.Parameters[0] as string);
                    break;

                case EventActionType.OnVolumeSet:
                    SetVolume(eventData.Parameters[0] as string,
                        (int)eventData.Parameters[1]);
                    break;

                case EventActionType.OnVolumeChange:
                    ChangeVolume(eventData.Parameters[0] as string,
                        (int)eventData.Parameters[1]);
                    break;

                case EventActionType.OnVolumeSetMaster:
                    SetMasterVolume((int)eventData.Parameters[0]);
                    break;

                default:
                    break;
                    //add more cases for each method that we want to support with events
            }

            //if (eventData.EventActionType == EventActionType.OnPlay2D)
            //{
            //    Play2D(eventData.Parameters[0] as string);
            //}
            //else if (eventData.EventActionType == EventActionType.OnPlay3D)
            //{
            //    Play3D(eventData.Parameters[0] as string,
            //        eventData.Parameters[1] as AudioListener,
            //            eventData.Parameters[2] as AudioEmitter);
            //}
            //else if (eventData.EventActionType == EventActionType.OnVolumeMaster)
            //{
            //    SetMasterVolume((int)eventData.Parameters[0]);
            //}

            //if we always want the SoundManager to be available then comment this line out
            //base.HandleEvent(eventData);
        }

        #endregion Event Handling

        #region Actions - Add, Play, Pause, Volume

        /// <summary>
        /// Adds a new cue to the managers list for later play
        /// </summary>
        /// <param name="cue"></param>
        public void Add(Cue cue)
        {
            if (!dictionary.ContainsKey(cue.ID))
            {
                dictionary.Add(cue.ID, cue);
            }
        }

        /// <summary>
        /// Sets the volume of all played sounds
        /// </summary>
        /// <param name="value"></param>
        public void SetMasterVolume(float? volume)
        {
            if (volume == null)
                return;

            SoundEffect.MasterVolume = MathHelper.Clamp(volume.Value, 0, 1);
        }

        /// <summary>
        /// Changes (i.e. apply a delta) the volume of all played sounds
        /// </summary>
        /// <param name="delta"></param>
        public void ChangeMasterVolume(float? delta)
        {
            if (delta == null)
                return;

            SoundEffect.MasterVolume = MathHelper.Clamp(SoundEffect.MasterVolume + delta.Value, 0, 1);
        }

        /// <summary>
        /// Plays a 2D sound (i.e. a sound with no 3D location in the game!)
        /// </summary>
        /// <param name="id"></param>
        public void Play2D(string id)
        {
            if (id == null)
                return;

            id = id.Trim();

            if (dictionary.ContainsKey(id))
            {
                Cue cue = dictionary[id];
                SoundEffectInstance soundEffectInstance = dictionary[id].SoundEffect.CreateInstance();

                soundEffectInstance.Volume = cue.Volume;
                soundEffectInstance.Pitch = cue.Pitch;
                soundEffectInstance.Pan = cue.Pan;
                soundEffectInstance.IsLooped = cue.IsLooped;
                soundEffectInstance.Play();

                //store in order to support later pause and stop functionality
                listInstances2D.Add(new KeyValuePair<string, SoundEffectInstance>(id, soundEffectInstance));
            }
        }

        /// <summary>
        /// Plays a 3D sound (i.e. a sound with a 3D location in the relative to the audio listener)
        /// </summary>
        /// <param name="id"></param>
        public void Play3D(string id, AudioListener listener, AudioEmitter emitter)
        {
            if (id == null)
                return;

            id = id.Trim();

            if (dictionary.ContainsKey(id))
            {
                Cue cue = dictionary[id];
                SoundEffectInstance soundEffectInstance = dictionary[id].SoundEffect.CreateInstance();

                soundEffectInstance.Volume = cue.Volume;
                soundEffectInstance.Pitch = cue.Pitch;
                soundEffectInstance.Pan = cue.Pan;
                soundEffectInstance.IsLooped = cue.IsLooped;
                soundEffectInstance.Apply3D(listener, emitter);
                soundEffectInstance.Play();

                //store in order to support later pause and stop functionality
                listInstances2D.Add(new KeyValuePair<string, SoundEffectInstance>(id, soundEffectInstance));
            }
        }

        /// <summary>
        /// Pause by unique ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Pause(string id)
        {
            if (id == null)
                return false;

            id = id.Trim();
            foreach (KeyValuePair<string, SoundEffectInstance> pair in listInstances2D)
            {
                if (pair.Key.Equals(id))
                {
                    SoundEffectInstance instance = pair.Value;
                    if (instance.State == SoundState.Playing)
                    {
                        instance.Pause();
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Resume by unique ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Resume(string id)
        {
            if (id == null)
                return false;

            id = id.Trim();
            foreach (KeyValuePair<string, SoundEffectInstance> pair in listInstances2D)
            {
                if (pair.Key.Equals(id))
                {
                    SoundEffectInstance instance = pair.Value;
                    if (instance.State == SoundState.Paused)
                    {
                        instance.Resume();
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Stop by unique ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Stop(string id)
        {
            if (id == null)
                return false;

            id = id.Trim();
            bool bFound = false;

            //foreach (KeyValuePair<string, SoundEffectInstance> pair in this.listInstances2D)
            for (int i = 0; i < listInstances2D.Count; i++)
            {
                KeyValuePair<string, SoundEffectInstance> pair = listInstances2D[i];
                if (pair.Key.Equals(id))
                {
                    SoundEffectInstance instance = pair.Value;
                    if (instance.State == SoundState.Playing)
                    {
                        instance.Stop();
                        listInstances2D.Remove(pair);
                        bFound = true;
                        break;
                    }
                }
            }
            return bFound;
        }

        /// <summary>
        /// Set volume by unique ID
        /// </summary>
        /// <param name="id"></param>
        /// <param name="volume"></param>
        /// <returns></returns>
        public bool SetVolume(string id, float volume)
        {
            id = id.Trim();
            foreach (KeyValuePair<string, SoundEffectInstance> pair in listInstances2D)
            {
                if (pair.Key.Equals(id))
                {
                    SoundEffectInstance instance = pair.Value;

                    //playable and pausable sounds can be changed in volume
                    if (instance.State == SoundState.Playing || instance.State == SoundState.Paused)
                    {
                        instance.Volume = volume;
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Change (i.e. apply a delta) volume by unique ID
        /// </summary>
        /// <param name="id"></param>
        /// <param name="volume"></param>
        /// <returns></returns>
        public bool ChangeVolume(string id, float delta)
        {
            id = id.Trim();
            foreach (KeyValuePair<string, SoundEffectInstance> pair in listInstances2D)
            {
                if (pair.Key.Equals(id))
                {
                    SoundEffectInstance instance = pair.Value;

                    //playable and pausable sounds can be changed in volume
                    if (instance.State == SoundState.Playing || instance.State == SoundState.Paused)
                    {
                        float newVolume = instance.Volume + delta;
                        if (newVolume >= 0 && newVolume <= 1)
                        {
                            instance.Volume = newVolume;
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        #endregion Actions - Add, Play, Pause, Volume

        #region Housekeeping

        public new void Dispose()
        {
            ///compare this method to calling Dispose on dictionary contents to ContentDictionary::Dispose()
            ///see https://robertgreiner.com/iterating-through-a-dictionary-in-csharp/
            foreach (KeyValuePair<string, Cue> pair in dictionary)
            {
                ((IDisposable)pair.Value).Dispose();
            }

            dictionary.Clear();
        }

        #endregion Housekeeping
    }
}