using GDLibrary.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDLibrary.Components.UI
{
    public class UIReticuleBehaviour : UIBehaviour
    {
        private Texture2D originalDefaultTexture;
        private float reticuleRotation;

        public override void Awake()
        {
            originalDefaultTexture = (uiObject as UITextureObject).DefaultTexture;

            Input.Mouse.SetMouseVisible(false);

            EventDispatcher.Subscribe(EventCategoryType.Picking, HandlePickedObject);

            base.Awake();
        }

        private void HandlePickedObject(EventData eventData)
        {
            var uiTextureObject = uiObject as UITextureObject;

            switch (eventData.EventActionType)
            {
                case EventActionType.OnObjectPicked:
                    uiObject.Color = Color.Red;
                    uiTextureObject.DefaultTexture = uiTextureObject.AlternateTexture;
                    uiObject.Transform.RotationInDegrees = reticuleRotation;
                    break;

                case EventActionType.OnNoObjectPicked:
                    uiObject.Color = Color.White;
                    uiTextureObject.DefaultTexture = originalDefaultTexture;
                    uiObject.Transform.RotationInDegrees = 0;
                    break;

                default:
                    break;
            }

            if (eventData.Parameters != null)
            {
                GameObject picked = eventData?.Parameters[0] as GameObject;
                var dist = Vector3.Distance(Camera.Main.Transform.LocalTranslation, picked.Transform.LocalTranslation);

                //dont forget to remove debug messages as they are CPU hungry
                //System.Diagnostics.Debug.WriteLine(dist);
            }
        }

        protected override void OnDisabled()
        {
            Input.Mouse.SetMouseVisible(true);
            base.OnDisabled();
        }

        public override void Update()
        {
            uiObject.Transform.LocalTranslation = Input.Mouse.Position;

            reticuleRotation += 1 / 60.0f;

            base.Update();
        }
    }
}