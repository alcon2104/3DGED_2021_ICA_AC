using GDLibrary.Graphics;
using Microsoft.Xna.Framework;
using System;

namespace GDLibrary.Components
{
    /// <summary>
    /// Allows us to slowly (based on lerpSpeed) lerp between two alpha on a game object
    /// </summary>
    public class AlphaLerpBehaviour : Behaviour
    {
        #region Fields

        private float startAlpha;
        private float endAlpha;
        private float lerpSpeed;
        private Material material;

        #endregion Fields

        #region Properties

        public float StartAlpha { get => startAlpha; set => startAlpha = value >= 0 && value <= 1 ? value : 1; }
        public float EndAlpha { get => endAlpha; set => endAlpha = value >= 0 && value <= 1 ? value : 1; }
        public float LerpSpeed { get => lerpSpeed; set => lerpSpeed = value > 0 ? value : 1; }

        #endregion Properties

        #region Constructors

        public AlphaLerpBehaviour(float startAlpha, float endAlpha, float lerpSpeed = 1)
        {
            StartAlpha = startAlpha;
            EndAlpha = endAlpha;
            LerpSpeed = lerpSpeed;
        }

        #endregion Constructors

        #region Actions - Awake & Update

        public override void Awake(GameObject gameObject)
        {
            material = gameObject.GetComponent<Renderer>().Material;
            base.Awake(gameObject);
        }

        public override void Update()
        {
            if (material != null)
            {
                //sine has ampltiude -1 to +1
                float lerpFactor = (float)Math.Sin(LerpSpeed * MathHelper.ToRadians(Time.Instance.TotalGameTimeMs));

                //for lerp we need 0 to 1
                lerpFactor = lerpFactor * 0.5f + 0.5f;

                //set alpha
                material.Alpha = MathHelper.Lerp(StartAlpha, EndAlpha, lerpFactor);
            }
        }

        #endregion Actions - Awake & Update
    }
}