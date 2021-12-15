using System;

namespace GDLibrary.Graphics
{
    public abstract class Material : IDisposable, ICloneable
    {
        #region Events

        public static event Action AlphaPropertyChanged = null;

        #endregion Events

        #region Fields

        protected string name;
        protected float alpha;
        protected Shader shader;

        #endregion Fields

        #region Properties

        public string Name { get => name; set => name = value.Trim(); }

        public float Alpha
        {
            get => alpha;
            set
            {
                alpha = value >= 0 && value <= 1 ? value : 1;
                AlphaPropertyChanged?.Invoke();
            }
        }

        public Shader Shader { get => shader; set => shader = value; }

        #endregion Properties

        #region Constructors

        public Material(string name, Shader shader, float alpha)
        {
            Name = name;
            Alpha = alpha;
            this.shader = shader;
        }

        #endregion Constructors

        #region Actions - Housekeeeping

        public virtual void Dispose()
        {
            //Overridden in child classes
        }

        public abstract object Clone();

        #endregion Actions - Housekeeeping
    }
}