using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace GDLibrary.Components
{
    /// <summary>
    /// Used to set either a perspective or orthographic projection on the camera in the scene
    /// </summary>
    /// <see cref="GDLibrary.Components.Camera"/>
    public enum CameraProjectionType
    {
        Perspective, Orthographic
    }

    /// <summary>
    /// Stores the fields required to represent a Camera and provide frustum culling (using the BoundingFrustum)
    /// </summary>
    public class Camera : Component, IComparable
    {
        #region Statics

        private readonly float MIN_NEAR_CLIP = 1;
        private readonly float MIN_FAR_CLIP = 10;
        private readonly float DEFAULT_ASPECT_RATIO = 16 / 10.0f;
        private readonly float DEFAULT_FIELD_OF_VIEW = MathHelper.PiOver4 / 2.0f;

        /// <summary>
        /// Main camera used to render the scene in the SceneManager
        /// </summary>
        public static Camera Main { get; set; }

        #endregion Statics

        #region Fields

        private Matrix viewMatrix;
        private Matrix projectionMatrix;

        private Viewport viewPort;
        private CameraProjectionType projectionType;
        private float fieldOfView, aspectRatio, nearClipPlane, farClipPlane;
        private Vector3 up;
        private BoundingFrustum boundingFrustrum;
        private int drawDepth;
        private bool isProjectionDirty = true, isViewDirty = true, isFrustumDirty = true;

        #endregion Fields

        #region Properties

        public Matrix ViewMatrix
        {
            get
            {
                if (isViewDirty)
                {
                    var transform = gameObject.Transform;
                    var target = transform.LocalTranslation + Vector3.Transform(Vector3.Forward, transform.RotationMatrix);
                    viewMatrix = Matrix.CreateLookAt(transform.LocalTranslation, target, up);
                    isViewDirty = false;
                    isFrustumDirty = true;
                }

                return viewMatrix;
            }
        }

        public Matrix ProjectionMatrix
        {
            get
            {
                if (isProjectionDirty)
                {
                    projectionMatrix =
                        projectionType == CameraProjectionType.Perspective
                        ? Matrix.CreatePerspectiveFieldOfView(fieldOfView, aspectRatio, nearClipPlane, farClipPlane)
                        : Matrix.CreateOrthographic(viewPort.Width, viewPort.Height, nearClipPlane, farClipPlane);

                    isProjectionDirty = false;
                    isFrustumDirty = true;
                }

                return projectionMatrix;
            }
        }

        public BoundingFrustum BoundingFrustum
        {
            get
            {
                if (isFrustumDirty)
                {
                    boundingFrustrum = new BoundingFrustum(viewMatrix * projectionMatrix);
                    isFrustumDirty = false;
                }

                return boundingFrustrum;
            }
        }

        public CameraProjectionType ProjectionType
        {
            get => projectionType;
            set
            {
                if (projectionType != value)
                {
                    projectionType = value;
                    isProjectionDirty = true;
                }
            }
        }

        public float FieldOfView
        {
            get => fieldOfView;
            set
            {
                if (fieldOfView != value)
                {
                    fieldOfView = (value > 0 && value < MathHelper.Pi) ? value : DEFAULT_FIELD_OF_VIEW;
                    isProjectionDirty = true;
                }
            }
        }

        public float AspectRatio
        {
            get => aspectRatio;
            set
            {
                if (aspectRatio != value)
                {
                    aspectRatio = value > 0 ? value : DEFAULT_ASPECT_RATIO;
                    isProjectionDirty = true;
                }
            }
        }

        public float NearClipPlane
        {
            get => nearClipPlane;
            set
            {
                if (nearClipPlane != value)
                {
                    nearClipPlane = value > MIN_NEAR_CLIP ? value : MIN_NEAR_CLIP;
                    isProjectionDirty = true;
                }
            }
        }

        public float FarClipPlane
        {
            get => farClipPlane;
            set
            {
                if (farClipPlane != value)
                {
                    farClipPlane = value > MIN_FAR_CLIP ? value : MIN_FAR_CLIP;
                    isProjectionDirty = true;
                }
            }
        }

        public Viewport Viewport
        {
            get => viewPort;
            set => viewPort = value;
        }

        public Vector3 Forward => viewMatrix.Forward;
        public Vector3 Backward => viewMatrix.Backward;
        public Vector3 Right => viewMatrix.Right;
        public Vector3 Left => viewMatrix.Left;
        public Vector3 Up => viewMatrix.Up;
        public Vector3 Down => viewMatrix.Down;

        #endregion Properties

        #region Constructors

        public Camera(Viewport viewPort) : base()
        {
            this.viewPort = viewPort;
            projectionType = CameraProjectionType.Perspective;
            fieldOfView = MathHelper.ToRadians(45);
            aspectRatio = (float)viewPort.Width / viewPort.Height;
            nearClipPlane = 1.0f;
            farClipPlane = 1000.0f;
            up = Vector3.Up;
            drawDepth = 0;

            //controls update of view, projection and frustrum on paramater change
            isViewDirty = true;
            isProjectionDirty = true;
            isFrustumDirty = true;
        }

        public override void Awake(GameObject gameObject)
        {
            //we call the base first otherwise it wont set the transform variable
            base.Awake(gameObject);

            //registers event handler when translation or rotation (which affect view) change
            transform.PropertyChanged += UpdateViewMatrix;
        }

        /// <summary>
        /// Update the view matrix because either the translation and/or the rotation have changed which affect the camera target
        /// </summary>
        private void UpdateViewMatrix()
        {
            var transform = gameObject.Transform;
            var target = transform.LocalTranslation + Vector3.Transform(Vector3.Forward, transform.RotationMatrix);
            viewMatrix = Matrix.CreateLookAt(transform.LocalTranslation, target, up);
            isFrustumDirty = true;
        }

        #endregion Constructors

        #region Housekeeping

        public override int CompareTo(object obj)
        {
            var camera = obj as Camera;

            if (camera == null)
                return 1;

            if (this == camera)
                return 0;

            if (drawDepth == camera.drawDepth)
                return 0;
            else if (drawDepth > camera.drawDepth)
                return 1;
            else
                return -1;
        }

        #endregion Housekeeping
    }
}