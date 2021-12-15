using GDLibrary.Core;
using Microsoft.Xna.Framework;

namespace GDLibrary.Components.UI
{
    public class UIExpandFadeBehaviour : UIBehaviour
    {
        private float alpha = 1;
        private float elapsedTimeSince;
        private float timeToLiveMs = 2000;
        public override void Update()
        {
            elapsedTimeSince += Time.Instance.DeltaTimeMs;

            if (uiObject != null)
            {
                uiObject.Transform.LocalScale += 0.05f * Vector2.One;
                alpha -= 0.1f;
                uiObject.Color = new Color(1, 1, 1, alpha);
            }

            if (elapsedTimeSince >= timeToLiveMs)
            {
                object[] parameters = { uiObject };
                EventDispatcher.Raise(new EventData(EventCategoryType.UiObject,
                    EventActionType.OnRemoveObject, parameters));
            }

            base.Update();
        }
    }
}