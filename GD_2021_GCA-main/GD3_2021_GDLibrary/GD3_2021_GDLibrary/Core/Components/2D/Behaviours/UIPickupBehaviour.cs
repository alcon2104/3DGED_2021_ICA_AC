using GDLibrary.Core;

namespace GDLibrary.Components.UI
{
    public class UIPickupBehaviour : UIBehaviour
    {
        private float timeEventOccurred;
        private bool isTextUpdated;

        public UIPickupBehaviour()
        {
            EventDispatcher.Subscribe(EventCategoryType.Player,
                HandleEvents);
        }

        private void HandleEvents(EventData eventData)
        {
            if (eventData.EventActionType == EventActionType.OnPickup)
            {
                timeEventOccurred = Time.Instance.TotalGameTimeMs;
                var uiTextObject = uiObject as UITextObject;
                var item = eventData.Parameters[0] as string;
                if (uiTextObject != null)
                {
                    uiTextObject.Text = $"You picked up a {item}";
                    isTextUpdated = true;
                }
            }
        }
        public override void Update()
        {
            if (Time.Instance.TotalGameTimeMs - timeEventOccurred >= 2000
                && isTextUpdated == true)
            {
                var uiTextObject = uiObject as UITextObject;
                if (uiTextObject != null)
                {
                    uiTextObject.Text = "";
                    //      uiTextObject.Color =
                    //        new Microsoft.Xna.Framework.Color(255, 255, 255, alpha);
                }

                isTextUpdated = false;
            }
            base.Update();
        }
    }
}