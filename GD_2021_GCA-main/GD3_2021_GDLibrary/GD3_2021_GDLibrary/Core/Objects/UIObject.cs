using GDLibrary.Components.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace GDLibrary
{
    /// <summary>
    /// Enumeration of types of ui objects used by UI manager and menu manager
    /// </summary>
    public enum UIObjectType : sbyte
    {
        Text,
        Texture,
        Background,
        Progress,
        Button
    }

    /// <summary>
    /// Parent class for any ui element used in main ui or menu
    /// </summary>
    public abstract class UIObject : ICloneable, IDisposable
    {
        #region Statics

        private static readonly int DEFAULT_SIZE = 4;

        #endregion Statics

        #region Fields

        /// <summary>
        /// Enumerated type indicating what category ths ui object belongs to (e.g. Text, Texture, Progress)
        /// </summary>
        protected UIObjectType uiObjectType;

        /// <summary>
        /// Unique identifier for each ui object - may be used for search, sort later
        /// </summary>
        protected string id;

        /// <summary>
        /// Friendly name for the current ui object
        /// </summary>
        protected string name;

        /// <summary>
        /// Set on first update of the component in UISceneManager::Update
        /// </summary>
        protected bool isRunning;

        /// <summary>
        /// Set in constructor to true. By default all components are enabled on instanciation
        /// </summary>
        protected bool isEnabled;

        /// <summary>
        /// Drawn translation, rotation, and scale of ui object on screen
        /// </summary>
        protected Transform2D transform;

        /// <summary>
        /// Depth used to sort ui objects on screen (0 = front-most, 1 = back-most)
        /// </summary>
        protected float layerDepth;

        /// <summary>
        /// Used to flip the text/texture
        /// </summary>
        protected SpriteEffects spriteEffects;

        /// <summary>
        /// Blend color used for text/texture
        /// </summary>
        protected Color color;

        /// <summary>
        /// Origin of rotation for the ui object in texture space (i.e. [0,0] - [w,h])
        /// Useful to rotate textures around unusual origin points e.g. a speedometer needle
        /// </summary>
        protected Vector2 origin;

        /// <summary>
        /// List of all attached components
        /// </summary>
        protected List<UIComponent> components;

        #endregion Fields

        #region Properties

        public string ID { get => id; protected set => id = value; }
        public string Name { get => name; protected set => name = value.Trim(); }
        public bool IsRunning { get => isRunning; private set => isRunning = value; }
        public bool IsEnabled { get => isEnabled; set => isEnabled = value; }
        public Transform2D Transform { get => transform; set => transform = value; }
        public float LayerDepth { get => layerDepth; set => layerDepth = value >= 0 && value <= 1 ? value : 0; }
        public SpriteEffects SpriteEffects { get => spriteEffects; set => spriteEffects = value; }
        public Color Color { get => color; set => color = value; }
        public Vector2 Origin { get => origin; set => origin = value; }

        #endregion Properties

        #region Constructors

        protected UIObject(string name, UIObjectType uiObjectType, Transform2D transform, float layerDepth,
            Color color, SpriteEffects spriteEffects, Vector2 origin)
        {
            Transform = transform;
            LayerDepth = layerDepth;
            SpriteEffects = spriteEffects;
            Color = color;
            Origin = origin;
            components = new List<UIComponent>(DEFAULT_SIZE);

            IsEnabled = true;
            IsRunning = false;

            this.uiObjectType = uiObjectType;
            ID = "UIO-" + Guid.NewGuid();
            Name = string.IsNullOrEmpty(name) ? ID : name;
        }

        #endregion Constructors

        #region Initialization

        public virtual void Initialize()
        {
            if (!IsRunning)
            {
                IsRunning = true;

                //TODO - Add sort IComparable in each component
                //components.Sort();

                //for (int i = 0; i < components.Count; i++)
                //    components[i].Start();
            }
        }

        #endregion Initialization

        #region Actions - Update & Draw

        /// <summary>
        /// Called each update to call an update on all ui components of the ui object
        /// </summary>
        public virtual void Update()
        {
            for (int i = 0; i < components.Count; i++)
                components[i].Update();
        }

        public abstract void Draw(SpriteBatch spriteBatch);

        #endregion Actions - Update & Draw

        #region Actions - Add & Get Components

        public UIComponent AddComponent(UIComponent component)
        {
            if (component == null)
                return null;

            //set this as component's parent game object
            component.UIObject = this;
            //perform any initial wake up operations
            component.Awake();
            //add to list
            components.Add(component);

            if (isRunning && !component.IsRunning)
            {
                component.Start();
                component.IsRunning = true;
                components.Sort();
            }

            return component;
        }

        public T AddComponent<T>() where T : UIComponent, new()
        {
            var component = new T();
            return (T)AddComponent(component);
        }

        public T GetComponent<T>() where T : UIComponent
        {
            for (int i = 0; i < components.Count; i++)
            {
                if (components[i] is T)
                    return components[i] as T;
            }

            return null;
        }

        public T GetComponent<T>(Predicate<UIComponent> pred) where T : UIComponent
        {
            return components.Find(pred) as T;
        }

        public T[] GetComponents<T>() where T : UIComponent
        {
            List<T> componentList = new List<T>();

            for (int i = 0; i < components.Count; i++)
            {
                if (components[i] is T)
                    componentList.Add(components[i] as T);
            }

            return componentList.ToArray();
        }

        public T[] GetComponents<T>(Predicate<UIComponent> pred) where T : UIComponent
        {
            return components.FindAll(pred).ToArray() as T[];
        }

        #endregion Actions - Add & Get Components

        #region Actions - Housekeeping

        public virtual void Dispose()
        {
            //TODO - check dispose
            foreach (UIComponent uiComponent in components)
                uiComponent.Dispose();
        }

        public virtual object Clone()
        {
            return null;
        }

        #endregion Actions - Housekeeping
    }

    /// <summary>
    /// Draws a texture on screen
    /// </summary>
    public class UITextObject : UIObject
    {
        #region Fields

        /// <summary>
        /// Font used to render text for this object
        /// </summary>
        protected SpriteFont spriteFont;

        /// <summary>
        /// Text rendered for this object
        /// </summary>
        protected string text;

        #endregion Fields

        #region Properties

        public SpriteFont SpriteFont { get => spriteFont; set => spriteFont = value; }
        public string Text { get => text; set => text = value.Trim(); }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Use this constructor draw WHITE BLEND, UNROTATED, ZERO-ORIGIN text
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="depth"></param>
        /// <param name="activeTexture"></param>
        public UITextObject(string name, UIObjectType uiObjectType, Transform2D transform, float layerDepth,
                   SpriteFont spriteFont, string text)
                   : this(name, uiObjectType, transform, layerDepth,
                   Color.White, SpriteEffects.None, Vector2.Zero,
                   spriteFont, text)
        {
        }

        public UITextObject(string name, UIObjectType uiObjectType, Transform2D transform, float layerDepth,
            Color color, SpriteEffects spriteEffects, Vector2 origin,
            SpriteFont spriteFont, string text)
        : base(name, uiObjectType, transform, layerDepth, color, spriteEffects, origin)
        {
            SpriteFont = spriteFont;
            Text = text;
        }

        #endregion Constructors

        #region Actions - Draw

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(spriteFont,
                text,
                Transform.LocalTranslation,
                color,
                Transform.RotationInDegrees,
                Origin,
                Transform.LocalScale,
                SpriteEffects,
                LayerDepth);
        }

        #endregion Actions - Draw

        #region Actions - Housekeeping

        public override object Clone()
        {
            var clone = new UITextObject($"Clone - {name}",
                     uiObjectType,
                     transform.Clone() as Transform2D, //why do we explicitly call Clone()? (Hint: do we need shallow or deep copy?)
                     layerDepth,
                     color, spriteEffects, origin,
                     spriteFont, text);

            clone.ID = "UIO-" + Guid.NewGuid();

            UIComponent clonedComponent = null;
            foreach (UIComponent component in components)
            {
                clonedComponent = clone.AddComponent((UIComponent)component.Clone());
                clonedComponent.uiObject = clone;
            }

            return clone;
        }

        #endregion Actions - Housekeeping
    }

    /// <summary>
    /// Draws a texture on screen
    /// </summary>
    public class UITextureObject : UIObject
    {
        #region Fields

        /// <summary>
        /// Default texture shown for this object
        /// </summary>
        protected Texture2D defaultTexture;

        /// <summary>
        /// Alternate image may be used for hover/mouse click effects
        /// </summary>
        protected Texture2D alternateTexture;

        /// <summary>
        /// Used to control how much of the source image we draw (e.g. for a portion of an image as in a progress bar)
        /// </summary>
        protected Rectangle sourceRectangle;

        /// <summary>
        /// Sets current to be either active or alternate (e.g. used for hover over texture change)
        /// </summary>
        protected Texture2D currentTexture;

        /// <summary>
        /// Stores the original dimensions of the source rectangle for this texture
        /// </summary>
        protected Rectangle originalSourceRectangle;

        /// <summary>
        /// Collision bounding box for button
        /// </summary>
        private Rectangle bounds;

        #endregion Fields

        #region Properties

        public Texture2D DefaultTexture { get => defaultTexture; set => defaultTexture = value; }
        public Texture2D AlternateTexture { get => alternateTexture; set => alternateTexture = value; }
        public Texture2D CurrentTexture { get => currentTexture; protected set => currentTexture = value; }
        public Rectangle SourceRectangle { get => sourceRectangle; set => sourceRectangle = value; }
        public int SourceRectangleWidth { get => sourceRectangle.Width; set => sourceRectangle.Width = value; }
        public int SourceRectangleHeight { get => sourceRectangle.Height; set => sourceRectangle.Height = value; }

        public Rectangle OriginalSourceRectangle
        {
            get
            {
                return originalSourceRectangle;
            }
        }

        public Rectangle Bounds
        {
            get
            {
                var originalBounds = new Rectangle(0, 0, defaultTexture.Width, defaultTexture.Height);
                var worldMatrix = Matrix.CreateTranslation(new Vector3(-origin, 0))
                       * Matrix.CreateScale(new Vector3(transform.LocalScale, 1))
                       * Matrix.CreateRotationZ(MathHelper.ToRadians(transform.RotationInDegrees))
                       * Matrix.CreateTranslation(new Vector3(transform.LocalTranslation, 0));

                bounds = originalBounds.Transform(worldMatrix);
                return bounds;
            }
        }

        #endregion Properties

        #region Constructors

        public void SetRectangle(int width, int height)
        {
            sourceRectangle = new Rectangle(0, 0, width, height);
        }

        /// <summary>
        /// Construct a ui texture object when we draw WHITE BLEND, FULL, UNROTATED, ZERO-ORIGIN textures
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="depth"></param>
        /// <param name="activeTexture"></param>
        public UITextureObject(string name, UIObjectType uiObjectType, Transform2D transform,
            float layerDepth, Texture2D defaultTexture)
            : this(name, uiObjectType, transform, layerDepth,
            Color.White, SpriteEffects.None, Vector2.Zero, defaultTexture, null,
            new Rectangle(0, 0, defaultTexture.Width, defaultTexture.Height))
        {
        }

        public UITextureObject(string name, UIObjectType uiObjectType,
            Transform2D transform, float layerDepth,
            Color color, Vector2 origin,
            Texture2D defaultTexture)
        : this(name, uiObjectType, transform, layerDepth,
            color, SpriteEffects.None, origin, defaultTexture, null,
            new Rectangle(0, 0, defaultTexture.Width, defaultTexture.Height))
        {
        }

        /// <summary>
        /// Construct a ui texture object where we want to set all draw related settings (e.g. source rectangle, color, origin)
        /// </summary>
        /// <param name="name"></param>
        /// <param name="uiObjectType"></param>
        /// <param name="transform"></param>
        /// <param name="depth"></param>
        /// <param name="color"></param>
        /// <param name="spriteEffects"></param>
        /// <param name="origin"></param>
        /// <param name="defaultTexture"></param>
        /// <param name="alternateTexture"></param>
        /// <param name="sourceRectangle"></param>
        public UITextureObject(string name, UIObjectType uiObjectType, Transform2D transform, float layerDepth,
        Color color, SpriteEffects spriteEffects, Vector2 origin,
        Texture2D defaultTexture, Texture2D alternateTexture,
        Rectangle sourceRectangle)
    : base(name, uiObjectType, transform, layerDepth, color, spriteEffects, origin)
        {
            DefaultTexture = defaultTexture;
            AlternateTexture = alternateTexture;
            //store the original source rectangle in case we change the source rectangle (i.e. UIProgressBarController)
            originalSourceRectangle = SourceRectangle = sourceRectangle;

            //sets the texture used by default in the Draw() below
            CurrentTexture = defaultTexture;

            //TODO - check bounding box
            bounds = new Rectangle(
                (int)transform.LocalTranslation.X,
                (int)transform.LocalTranslation.Y,
               (int)(defaultTexture.Width * transform.LocalScale.X),
                (int)(defaultTexture.Height * transform.LocalScale.Y));
        }

        #endregion Constructors

        #region Actions - Draw

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(defaultTexture,
                Transform.LocalTranslation,
                sourceRectangle,
                color,
                Transform.RotationInDegrees,
                Origin,
                Transform.LocalScale,
                SpriteEffects,
                LayerDepth);
        }

        #endregion Actions - Draw

        #region Actions - Housekeeping

        public override object Clone()
        {
            var clone = new UITextureObject($"Clone - {name}",
                uiObjectType,
                transform.Clone() as Transform2D, //why do we explicitly call Clone()? (Hint: do we need shallow or deep copy?)
                layerDepth,
                 color, spriteEffects, origin,
                defaultTexture, alternateTexture,
                sourceRectangle);
            clone.ID = "UIO-" + Guid.NewGuid();

            UIComponent clonedComponent = null;
            foreach (UIComponent component in components)
            {
                clonedComponent = clone.AddComponent((UIComponent)component.Clone());
                clonedComponent.uiObject = clone;
            }

            return clone;
        }

        #endregion Actions - Housekeeping
    }

    /// <summary>
    /// Draws a button (i.e. text and texture) on screen
    /// </summary>
    public class UIButtonObject : UITextureObject
    {
        #region Statics

        /// <summary>
        /// Used to ensure text layer is always 105% of texture layer i.e. closer to 0
        /// </summary>
        private static float TEXT_LAYER_DEPTH_MULTIPLIER = 1.05f;

        #endregion Statics

        #region Fields

        /// <summary>
        /// Text for the button
        /// </summary>
        private string text;

        /// <summary>
        /// Origin for the text dependent on string entered and font used
        /// </summary>
        private Vector2 textOrigin;

        /// <summary>
        /// Font for button text
        /// </summary>
        private SpriteFont font;

        private Color textColor;
        private Vector2 textOffset;

        #endregion Fields

        #region Properties

        public string Text
        {
            get
            {
                return text;
            }

            set
            {
                value = value.Trim();

                text = (value.Length >= 0) ? value : "Default";

                textOrigin = font.MeasureString(text) / 2.0f;
            }
        }

        public SpriteFont Font { get => font; set => font = value; }

        #endregion Properties

        #region Constructors

        public UIButtonObject(string name, UIObjectType uiObjectType, Transform2D transform, float layerDepth,
          Color color, Vector2 origin, Texture2D defaultTexture,
          string text, SpriteFont font, Color textColor)
      : this(name, uiObjectType, transform, layerDepth,
           color, SpriteEffects.None, origin,
           defaultTexture, null,
           new Rectangle(0, 0, defaultTexture.Width, defaultTexture.Height),
            text, font,
           textColor, Vector2.Zero)
        {
        }

        public UIButtonObject(string name, UIObjectType uiObjectType, Transform2D transform, float layerDepth,
           Color color, SpriteEffects spriteEffects, Vector2 origin,
           Texture2D defaultTexture, Texture2D alternateTexture,
           Rectangle sourceRectangle, string text, SpriteFont font,
           Color textColor, Vector2 textOffset)
       : base(name, uiObjectType, transform, layerDepth,
           color, spriteEffects, origin,
           defaultTexture, alternateTexture,
           sourceRectangle)
        {
            this.textColor = textColor;
            this.textOffset = textOffset;
            Font = font;
            Text = text;
        }

        #endregion Constructors

        #region Actions - Draw

        public override void Draw(SpriteBatch spriteBatch)
        {
            //draw texture
            base.Draw(spriteBatch);

            //draw text
            spriteBatch.DrawString(font,
               text,
               Transform.LocalTranslation,
               textColor,
               Transform.RotationInDegrees,
               textOrigin,
               Transform.LocalScale,
               SpriteEffects,
               Math.Max(1, LayerDepth * TEXT_LAYER_DEPTH_MULTIPLIER)); //ensures text is in front of texture (remember sorts from 1 (back) to 0 (front))
        }

        #endregion Actions - Draw

        #region Actions - Housekeeping

        public override object Clone()
        {
            var clone = new UIButtonObject($"Clone - {name}",
                       uiObjectType,
                       transform.Clone() as Transform2D,  //why do we explicitly call Clone()? (Hint: do we need shallow or deep copy?)
                       layerDepth,
                       color, spriteEffects, origin,
                       defaultTexture, alternateTexture,
                       sourceRectangle, text, font,
                       textColor, textOffset);

            clone.ID = "UIO-" + Guid.NewGuid();

            UIComponent clonedComponent = null;
            foreach (UIComponent component in components)
            {
                clonedComponent = clone.AddComponent((UIComponent)component.Clone());
                clonedComponent.uiObject = clone;
            }

            return clone;
        }

        #endregion Actions - Housekeeping
    }
}