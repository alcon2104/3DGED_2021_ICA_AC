using Microsoft.Xna.Framework;

namespace GDLibrary.Components.UI
{
    public class UITimeColorFlipBehaviour : UIBehaviour
    {
        private Color startColor;
        private Color endColor;
        private float flipIntervalTimeMs;

        //internal
        private float totalElapsedTimeMs;

        private bool flipColor;

        public UITimeColorFlipBehaviour(Color startColor, Color endColor, float flipIntervalTimeMs)
        {
            this.startColor = startColor;
            this.endColor = endColor;
            this.flipIntervalTimeMs = flipIntervalTimeMs;
        }

        public override void Update()
        {
            totalElapsedTimeMs += Time.Instance.DeltaTimeMs;

            if (totalElapsedTimeMs > flipIntervalTimeMs)
            {
                //reset elapsed time
                totalElapsedTimeMs -= flipIntervalTimeMs;

                //flip the control bool
                flipColor = !flipColor;

                //flips between both colors every timeInMs
                uiObject.Color = flipColor ? endColor : startColor;
            }

            //base does nothing
            // base.Update();
        }
    }
}