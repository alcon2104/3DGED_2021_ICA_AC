using Microsoft.Xna.Framework;
using System.Runtime.Serialization;

namespace GDLibrary.Core.Demo
{
    [DataContract]
    public class DemoSaveLoad
    {
        private Vector3 localTranslation;
        private Vector3 localRotation;
        private Vector3 localScale;

        [DataMember]
        public Vector3 LocalTranslation
        {
            get { return localTranslation; }
            set { localTranslation = value; }
        }

        [DataMember]
        public Vector3 LocalRotation
        {
            get { return localRotation; }
            set { localRotation = value; }
        }

        [DataMember]
        public Vector3 LocalScale
        {
            get { return localScale; }
            set { localScale = value; }
        }

        public DemoSaveLoad() : this(Vector3.Zero, Vector3.Zero, Vector3.One)
        {
        }

        public DemoSaveLoad(Vector3 translation, Vector3 rotation, Vector3 scale)
        {
            localTranslation = translation;
            localRotation = rotation;
            localScale = scale;
        }
    }
}