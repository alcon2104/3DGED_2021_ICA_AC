using JigLibX.Collision;
using JigLibX.Math;
using JigLibX.Physics;
using Microsoft.Xna.Framework;

namespace GDLibrary.Components
{
    /// <summary>
    /// Provides physics behaviour for a CHARACTER game object i.e. a player or camera
    /// Subclass this collider and override the HandleResponse() method as demonstrated in MyplayerCollider
    /// </summary>
    public class CharacterCollider : Collider
    {
        #region Fields

        private float accelerationRate;
        private float decelerationRate;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Create a character collider with response handling.
        /// </summary>
        /// <param name="isHandlingCollision"></param>
        /// <param name="isTrigger"></param>
        public CharacterCollider(float accelerationRate, float decelerationRate,
            bool isHandlingCollision = true, bool isTrigger = false)
            : base(isHandlingCollision, isTrigger)
        {
            this.accelerationRate = accelerationRate;
            this.decelerationRate = decelerationRate;
            this.isHandlingCollision = isHandlingCollision;
            this.isTrigger = isTrigger;
        }

        #endregion Constructors

        protected override void HandleResponse(GameObject parentGameObject)
        {
            base.HandleResponse(parentGameObject);
        }

        public override void Awake(GameObject gameObject)
        {
            //cache the transform
            transform = gameObject.Transform;

            //instanciate a new character body
            Body = new Character(accelerationRate, decelerationRate);

            //set the parent game object to be the attached drawn object (used when collisions occur)
            Body.Parent = gameObject;

            //instanciate a collision skin (which will have a primitive added e.g. sphere, capsule, trianglemesh)
            Collision = new CollisionSkin(Body, isTrigger);

            //set the skin as belonging to the body
            Body.CollisionSkin = Collision;

            //add collision reponse handling
            if (isHandlingCollision)
                Body.CollisionSkin.callbackFn += HandleCollision;
        }

        #region Actions - Physics setup related

        /// <summary>
        /// Must be called after we instanciate the new collider behaviour in order for the body to participate in the physics system
        /// </summary>
        public override void Enable(bool isImmovable, float mass)
        {
            //set whether the object can move
            Body.Immovable = isImmovable;
            //calculate the centre of mass
            Vector3 com = SetMass(mass);
            //adjust skin so that it corresponds to the 3D mesh as drawn on screen
            Body.MoveTo(transform.LocalTranslation, Matrix.Identity);
            //set the centre of mass
            Collision.ApplyLocalTransform(new JigLibX.Math.Transform(-com, Matrix.Identity));
            //constraining the collision surface
            Body.SetBodyInvInertia(0.0f, 0.0f, 0.0f);
            //preventing the physics engine from marking this object as velocity == 0
            Body.AllowFreezing = false;
            //enable so that any applied forces (e.g. gravity) will affect the object
            Body.EnableBody();
        }

        #endregion Actions - Physics setup related

        public override void Update()
        {
            //recalculate the world matrix in the parent using the body
            base.Update();

            //set the drawn object to be where the character collider is
            Transform.SetTranslation(Body.Transform.Position);
        }
    }

    public class Character : Body
    {
        #region Fields

        private bool isJumping, isCrouching;

        private float jumpHeight;
        public float accelerationRate { get; set; }
        public float decelerationRate { get; set; }
        public Vector3 DesiredVelocity { get; set; }

        #endregion Fields

        #region Properties

        public bool IsJumping
        {
            get
            {
                return isJumping;
            }
        }

        public bool IsCrouching
        {
            get
            {
                return isCrouching;
            }
            set
            {
                isCrouching = value;
            }
        }

        #endregion Properties

        #region Constructors

        public Character(float accelerationRate, float decelerationRate)
        : base()
        {
            this.accelerationRate = accelerationRate;
            this.decelerationRate = decelerationRate;
        }

        #endregion Constructors

        #region Actions - Jump & Add Forces

        public void DoJump(float jumpHeight = 5)
        {
            this.jumpHeight = jumpHeight;
            isJumping = true;
        }

        public override void AddExternalForces(float dt)
        {
            ClearForces();

            if (isJumping)
            {
                foreach (CollisionInfo info in CollisionSkin.Collisions)
                {
                    Vector3 N = info.DirToBody0;
                    if (this == info.SkinInfo.Skin1.Owner)
                    {
                        Vector3.Negate(ref N, out N);
                    }

                    if (Vector3.Dot(N, Orientation.Up) > 0.7f)
                    {
                        Vector3 vel = Velocity;
                        vel.Y = jumpHeight;
                        Velocity = vel;
                        break;
                    }
                }
            }

            Vector3 deltaVel = DesiredVelocity - Velocity;

            bool running = true;

            if (DesiredVelocity.LengthSquared() < JiggleMath.Epsilon)
                running = false;
            else
                deltaVel.Normalize();

            deltaVel.Y = -2.0f;

            // start fast then slow down
            if (running)
                deltaVel *= accelerationRate; //acceleration multiplier
            else
                deltaVel *= decelerationRate;  //deceleration multiplier

            float forceFactor = 500.0f;
            AddBodyForce(deltaVel * Mass * dt * forceFactor);
            isJumping = false;
            AddGravityToExternalForce();
        }
    }

    #endregion Actions - Jump & Add Forces
}