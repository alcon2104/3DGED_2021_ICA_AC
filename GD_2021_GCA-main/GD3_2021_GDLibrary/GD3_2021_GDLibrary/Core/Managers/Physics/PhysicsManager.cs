using GDLibrary.Core;
using JigLibX.Collision;
using JigLibX.Physics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace GDLibrary.Managers
{
    /// <summary>
    /// Sums all forces and torques for all collidable bodies in the world
    /// </summary>
    public class PhysicsController : Controller
    {
        #region Properties

        public enum CoordinateSystem
        {
            WorldCoordinates = 0,
            LocalCoordinates = 1
        }

        public struct Force
        {
            public CoordinateSystem coordinateSystem;
            public Vector3 force;
            public Vector3 position;
            public Body body;
        }

        public struct Torque
        {
            public CoordinateSystem coordinateSystem;
            public Vector3 torque;
            public Body body;
        }

        public Queue<Force> forces = new Queue<Force>();

        internal Queue<Force> Forces
        { get { return forces; } }

        public Queue<Torque> torques = new Queue<Torque>();

        internal Queue<Torque> Torques
        { get { return torques; } }

        #endregion Properties

        #region Actions - Update

        public override void UpdateController(float elapsedTime)
        {
            // Apply pending forces
            while (forces.Count > 0)
            {
                Force force = forces.Dequeue();
                switch (force.coordinateSystem)
                {
                    case CoordinateSystem.LocalCoordinates:
                        {
                            force.body.AddBodyForce(force.force, force.position);
                        }
                        break;

                    case CoordinateSystem.WorldCoordinates:
                        {
                            force.body.AddWorldForce(force.force, force.position);
                        }
                        break;
                }
            }

            // Apply pending torques
            while (torques.Count > 0)
            {
                Torque torque = torques.Dequeue();
                switch (torque.coordinateSystem)
                {
                    case CoordinateSystem.LocalCoordinates:
                        {
                            torque.body.AddBodyTorque(torque.torque);
                        }
                        break;

                    case CoordinateSystem.WorldCoordinates:
                        {
                            torque.body.AddWorldTorque(torque.torque);
                        }
                        break;
                }
            }
        }

        #endregion Actions - Update
    }

    public class PhysicsManager : PausableGameComponent
    {
        #region Statics

        private static readonly Vector3 GRAVITY = new Vector3(0, -9.81f, 0);

        #endregion Statics

        #region Fields

        private PhysicsSystem physicSystem;
        private PhysicsController physCont;
        private float timeStep = 0;

        #endregion Fields

        #region Properties

        public PhysicsSystem PhysicsSystem
        {
            get
            {
                return physicSystem;
            }
        }

        public PhysicsController PhysicsController
        {
            get
            {
                return physCont;
            }
        }

        #endregion Properties

        #region Constructors

        public PhysicsManager(Game game)
          : this(game, StatusType.Updated, GRAVITY)
        {
        }

        //user-defined gravity
        public PhysicsManager(Game game, StatusType statusType, Vector3 gravity)
            : base(game, statusType)
        {
            physicSystem = new PhysicsSystem();

            //add cd/cr system
            physicSystem.CollisionSystem = new CollisionSystemSAP();

            //allows us to define the direction and magnitude of gravity - default is (0, -9.8f, 0)
            physicSystem.Gravity = gravity;

            //prevents bug where objects would show correct CDCR response when velocity == Vector3.Zero
            physicSystem.EnableFreezing = true;

            physicSystem.SolverType = PhysicsSystem.Solver.Normal;
            physicSystem.CollisionSystem.UseSweepTests = true;

            //affect accuracy and the overhead == time required
            physicSystem.NumCollisionIterations = 4;
            physicSystem.NumContactIterations = 8;
            physicSystem.NumPenetrationRelaxationTimesteps = 15;

            #region SETTING_COLLISION_ACCURACY

            //affect accuracy of the collision detection
            physicSystem.AllowedPenetration = 0.00025f;
            physicSystem.CollisionTollerance = 0.0005f;

            #endregion SETTING_COLLISION_ACCURACY

            physCont = new PhysicsController();
            physicSystem.AddController(physCont);
        }

        #endregion Constructors

        #region Event Handling

        protected override void SubscribeToEvents()
        {
            //remove
            EventDispatcher.Subscribe(EventCategoryType.GameObject, HandleEvent);

            base.SubscribeToEvents();
        }

        protected override void HandleEvent(EventData eventData)
        {
            if (eventData.EventCategoryType == EventCategoryType.GameObject)
            {
                if (eventData.EventActionType == EventActionType.OnRemoveObject)
                {
                    //TODO - add remove
                    // CollidableObject collidableObject = eventData.Parameters[0] as CollidableObject;
                    // PhysicsSystem.RemoveBody(collidableObject.Body);
                }
            }

            base.HandleEvent(eventData);
        }

        #endregion Event Handling

        #region Actions - Update

        public override void Update(GameTime gameTime)
        {
            if (IsUpdated)
            {
                //TODO - change to Time.Instance
                timeStep = (float)gameTime.ElapsedGameTime.Ticks / System.TimeSpan.TicksPerSecond;

                //if the time between updates indicates a FPS of close to 60 fps or less then update CD/CR engine
                if (timeStep < 1.0f / 60.0f)
                {
                    physicSystem.Integrate(timeStep);
                }
                else
                {
                    //else fix at 60 updates per second
                    physicSystem.Integrate(1.0f / 60.0f);
                }
            }
        }

        #endregion Actions - Update
    }
}