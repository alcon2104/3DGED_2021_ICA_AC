using GDLibrary.Core;
using GDLibrary.Graphics;
using Microsoft.Xna.Framework;

namespace GDLibrary.Components
{
    public class ColorChangeBehaviour : Behaviour
    {
        private BasicMaterial material;
        private Vector3 originalColor;
        private float originalAlpha;

        public override void Awake(GameObject gameObject)
        {
            material = gameObject.GetComponent<Renderer>().Material as BasicMaterial;
            if (material != null)
            {
                originalColor = material.DiffuseColor;
                originalAlpha = material.Alpha;
            }

            EventDispatcher.Subscribe(EventCategoryType.MaterialChange, HandleEvent);
            base.Awake(gameObject);
        }

        private void HandleEvent(EventData eventData)
        {
            if (eventData.EventActionType == EventActionType.OnMouseClick)
            {
                string name = eventData.Parameters[0] as string; //"box1"

                if (name.Equals(gameObject.Name))
                    material.DiffuseColor = Color.Red.ToVector3();
            }
        }
    }
}