using GDLibrary.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDLibrary.Components
{
    /// <summary>
    /// Base class for all renderers (i.e. an object to render a mesh, model, animated model) used by the engine
    /// </summary>
    public abstract class Renderer : Component
    {
        #region Fields

        protected BoundingSphere boundingSphere;
        protected BoundingBox boundingBox;
        protected Material material;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the sphere that bounds all vertices in the model/mesh that this renderer will render
        /// </summary>
        protected BoundingSphere BoundingSphere { get; }

        /// <summary>
        /// Gets the axis-aligned box bounds all vertices in the model/mesh that this renderer will render
        /// </summary>
        protected BoundingBox BoundingBox { get; }

        /// <summary>
        /// Gets/sets the material (e.g. diffuse color, alpha, and attached Effect file) of the model/mesh that this renderer will render
        /// </summary>
        public Material Material
        {
            get { return material; }
            set { material = value; }
        }

        #endregion Properties

        #region Constructors

        public Renderer(Material material) : base()
        {
            this.material = material;
            boundingSphere = new BoundingSphere();
            boundingBox = new BoundingBox();
        }

        #endregion Constructors

        #region Update

        public override void Update()
        {
            //TODO - add if() for static objects
            //update the bounding sphere if the game object which this component is attached to moves
            boundingSphere.Center = transform.LocalTranslation;
        }

        #endregion Update

        #region Actions - Bounding sphere, Draw

        /// <summary>
        /// Compute the bounding sphere of mesh used for culling objects out of the camera frustum
        /// </summary>
        public abstract void SetBoundingVolume();

        /// <summary>
        /// Draw the content of the mesh
        /// </summary>
        public abstract void Draw(GraphicsDevice device, Effect effect);

        #endregion Actions - Bounding sphere, Draw

        //TODO - Add sort by material then alpha
        //public override int CompareTo(object obj)
        //{
        //    var renderer = obj as Renderer;
        //    var material = renderer != null ? renderer.Material : null;

        //    if (renderer == null)
        //        return 1;

        //    if (material == null || Material == null)
        //        return base.CompareTo(obj);

        //    if (Material._hasAlpha == material._hasAlpha)
        //        return 0;
        //    else if (Material._hasAlpha && !material._hasAlpha)
        //        return 1;
        //    else
        //        return -1;
        //}
    }
}