using Microsoft.Xna.Framework;
using System;

namespace GDLibrary.Components
{
    /// <summary>
    /// Store and manage transform operations e.g. translation, rotation and scale
    /// </summary>
    public class Transform : Component, ICloneable
    {
        #region Events

        public event Action PropertyChanged = null;

        #endregion Events

        #region Fields

        /// <summary>
        /// Used to render any visible game object (i.e. with MeshRenderer or ModelRenderer)
        /// </summary>
        private Matrix worldMatrix;

        /// <summary>
        /// Used to calculate the World matrix and also to calculate the target for a Camera
        /// </summary>
        private Matrix rotationMatrix;

        /// <summary>
        /// Scale relative to the parent transform
        /// </summary>
        private Vector3 localScale;

        /// <summary>
        /// Rotation relative to the parent transform
        /// </summary>
        private Vector3 localRotation;

        /// <summary>
        /// Translation relative to the parent transform
        /// </summary>
        private Vector3 localTranslation;

        /// <summary>
        /// Set to true if the translation, rotation, or scale change which affect World matrix
        /// </summary>
        private bool isWorldDirty = true;

        /// <summary>
        /// Set to true if the rotation changes
        /// </summary>
        private bool isRotationDirty = true;

        #endregion Fields

        #region Properties

        public Vector3 LocalScale
        {
            get
            {
                return localScale;
            }
        }

        public Vector3 LocalRotation
        {
            get
            {
                return localRotation;
            }
        }

        public Vector3 LocalTranslation
        {
            get
            {
                return localTranslation;
            }
        }

        public Matrix RotationMatrix
        {
            get
            {
                if (isRotationDirty)
                    UpdateRotationMatrix();

                return rotationMatrix;
            }
        }

        public Matrix WorldMatrix
        {
            get
            {
                if (isWorldDirty || isRotationDirty)
                    UpdateWorldMatrix();

                return worldMatrix;
            }
            set
            {
                worldMatrix = value;
            }
        }

        public Vector3 Forward
        {
            get
            {
                if (isWorldDirty)
                    UpdateWorldMatrix();

                return worldMatrix.Forward;
            }
        }

        public Vector3 Backward
        {
            get
            {
                if (isWorldDirty)
                    UpdateWorldMatrix();

                return worldMatrix.Backward;
            }
        }

        public Vector3 Right
        {
            get
            {
                if (isWorldDirty)
                    UpdateWorldMatrix();

                return worldMatrix.Right;
            }
        }

        public Vector3 Left
        {
            get
            {
                if (isWorldDirty)
                    UpdateWorldMatrix();

                return worldMatrix.Left;
            }
        }

        public Vector3 Up
        {
            get
            {
                if (isWorldDirty)
                    UpdateWorldMatrix();

                return worldMatrix.Up;
            }
        }

        public Vector3 Down
        {
            get
            {
                if (isWorldDirty)
                    UpdateWorldMatrix();

                return worldMatrix.Down;
            }
        }

        #endregion Properties

        #region Constructors

        public Transform(Vector3? scale, Vector3? rotation, Vector3? translation)
        {
            localScale = scale.HasValue ? scale.Value : Vector3.One;
            localRotation = rotation.HasValue ? rotation.Value : Vector3.Zero;
            localTranslation = translation.HasValue ? translation.Value : Vector3.Zero;

            worldMatrix = Matrix.Identity;
            rotationMatrix = Matrix.Identity;

            isWorldDirty = true;
            isRotationDirty = true;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <see cref="GameObject()"/>
        public Transform() : this(null, null, null)
        {
        }

        #endregion Constructors

        #region Actions - Modify Scale, Rotation, Translation

        /// <summary>
        /// Increases/decreases scale by adding/removing individual nullable x,y,z values
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public void Scale(float? x, float? y, float? z)
        {
            localScale.Add(x, y, z);
            isWorldDirty = true;
        }

        /// <summary>
        /// Increases/decreases scale by adding/removing Vector3 passed by reference
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public void Scale(ref Vector3 delta)
        {
            localScale.Add(ref delta);
            isWorldDirty = true;
        }

        /// <summary>
        /// Increases/decreases scale by adding/removing Vector3 passed by value
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public void Scale(Vector3 delta)
        {
            Scale(ref delta);
        }

        /// <summary>
        /// Increases/decreases rotation by adding/removing individual nullable x,y,z values
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public void Rotate(float? x, float? y, float? z)
        {
            localRotation.Add(x, y, z);
            isWorldDirty = true;
            isRotationDirty = true;
            PropertyChanged?.Invoke();
        }

        /// <summary>
        /// Increases/decreases rotation by adding/removing Vector3 passed by reference
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public void Rotate(ref Vector3 delta)
        {
            localRotation.Add(ref delta);
            isWorldDirty = true;
            isRotationDirty = true;
            PropertyChanged?.Invoke();
        }

        /// <summary>
        /// Increases/decreases rotation by adding/removing Vector3 passed by value
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public void Rotate(Vector3 delta)
        {
            Rotate(ref delta);
        }

        /// <summary>
        /// Increases/decreases translation by adding/removing individual nullable x,y,z values
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public void Translate(float? x, float? y, float? z)
        {
            localTranslation.Add(x, y, z);
            isWorldDirty = true;
            PropertyChanged?.Invoke();
        }

        /// <summary>
        /// Increases/decreases translation by adding/removing Vector3 passed by reference
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public void Translate(ref Vector3 delta)
        {
            localTranslation.Add(ref delta);
            isWorldDirty = true;
            PropertyChanged?.Invoke();
        }

        /// <summary>
        /// Increases/decreases translation by adding/removing Vector3 passed by value
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public void Translate(Vector3 delta)
        {
            Translate(ref delta);
        }

        /// <summary>
        /// Sets scale using individual nullable x,y,z values
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public void SetScale(float? x, float? y, float? z)
        {
            localScale.Set(x, y, z);
            isWorldDirty = true;
        }

        /// <summary>
        /// Sets scale using Vector3 pass by reference
        /// </summary>
        /// <param name="newScale"></param>
        public void SetScale(ref Vector3 newScale)
        {
            localScale.Set(ref newScale);
            isWorldDirty = true;
        }

        /// <summary>
        /// Sets scale using Vector3 pass by value
        /// </summary>
        /// <param name="newScale"></param>
        public void SetScale(Vector3 newScale)
        {
            SetScale(ref newScale);
        }

        /// <summary>
        /// Sets rotation using individual nullable x,y,z values
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public void SetRotation(float? x, float? y, float? z)
        {
            localRotation.Set(x, y, z);
            isWorldDirty = true;
            isRotationDirty = true;
            PropertyChanged?.Invoke();
        }

        /// <summary>
        /// Sets rotation using Vector3 pass by reference
        /// </summary>
        /// <param name="newRotation"></param>
        public void SetRotation(ref Vector3 newRotation)
        {
            localRotation.Set(ref newRotation);
            isWorldDirty = true;
            isRotationDirty = true;
            PropertyChanged?.Invoke();
        }

        /// <summary>
        /// Sets rotation using Vector3 pass by value
        /// </summary>
        /// <param name="newRotation"></param>
        public void SetRotation(Vector3 newRotation)
        {
            SetRotation(ref newRotation);
        }

        /// <summary>
        /// Sets rotation using Matrix pass by reference
        /// </summary>
        /// <param name="newRotation"></param>
        public void SetRotation(ref Matrix matrix)
        {
            var rotation = Quaternion.CreateFromRotationMatrix(matrix).ToEuler();
            localRotation.Set(rotation.X, rotation.Y, rotation.Z);
            isWorldDirty = true;
            isRotationDirty = true;
            PropertyChanged?.Invoke();
        }

        /// <summary>
        /// Sets rotation using Matrix pass by value
        /// </summary>
        /// <param name="newRotation"></param>
        public void SetRotation(Matrix matrix)
        {
            SetRotation(ref matrix);
        }

        /// <summary>
        /// Sets translation using individual nullable x,y,z values
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public void SetTranslation(float? x, float? y, float? z)
        {
            localTranslation.Set(x, y, z);
            isWorldDirty = true;
            PropertyChanged?.Invoke();
        }

        /// <summary>
        /// Sets translation using Vector3 pass by reference
        /// </summary>
        /// <param name="newTranslation"></param>
        public void SetTranslation(ref Vector3 newTranslation)
        {
            localTranslation.Set(ref newTranslation);
            isWorldDirty = true;
            PropertyChanged?.Invoke();
        }

        /// <summary>
        /// Sets translation using Vector3 pass by value
        /// </summary>
        /// <param name="newTranslation"></param>
        public void SetTranslation(Vector3 newTranslation)
        {
            SetTranslation(ref newTranslation);
        }

        public override void Update()
        {
            if (isRotationDirty)
                UpdateRotationMatrix();

            if (isWorldDirty)
                UpdateWorldMatrix();
        }

        private void UpdateRotationMatrix()
        {
            rotationMatrix = Matrix.CreateRotationX(MathHelper.ToRadians(localRotation.X))
                      * Matrix.CreateRotationY(MathHelper.ToRadians(localRotation.Y))
                          * Matrix.CreateRotationZ(MathHelper.ToRadians(localRotation.Z));
            isRotationDirty = false;
        }

        private void UpdateWorldMatrix()
        {
            worldMatrix = Matrix.Identity
                      * Matrix.CreateScale(localScale)
                          * Matrix.CreateRotationX(MathHelper.ToRadians(localRotation.X))
                      * Matrix.CreateRotationY(MathHelper.ToRadians(localRotation.Y))
                          * Matrix.CreateRotationZ(MathHelper.ToRadians(localRotation.Z))
                          * Matrix.CreateTranslation(localTranslation);
            isWorldDirty = false;
        }

        #endregion Actions - Modify Scale, Rotation, Translation

        #region Actions - Housekeeping

        //TODO - Dispose

        /// <summary>
        /// Deep or shallow, or mixed?
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            //value types - deep
            //reference types - shallow
            return MemberwiseClone();
        }

        #endregion Actions - Housekeeping
    }
}