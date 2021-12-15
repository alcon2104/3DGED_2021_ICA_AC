using GDLibrary.Parameters;

namespace GDLibrary.Components
{
    /// <summary>
    /// Moves the attached game object along a user defined track
    /// </summary>
    public class CurveBehaviour : Behaviour
    {
        private static readonly int PRECISION = 2;
        private Curve3D translationCurve;
        public CurveBehaviour(Curve3D translationCurve)
        {
            this.translationCurve = translationCurve;
        }
        public override void Update()
        {
            //evaluate the curve with a time variable - Time
            var nextTranslation = translationCurve.Evaluate(
                Time.Instance.TotalGameTimeMs, PRECISION);

            //use the value(s) to change transform (localTranslation)
            transform.SetTranslation(nextTranslation);

            base.Update();
        }
        protected override void OnEnabled()
        {
            base.OnEnabled();
        }
        protected override void OnDisabled()
        {
            base.OnDisabled();
        }
    }
}