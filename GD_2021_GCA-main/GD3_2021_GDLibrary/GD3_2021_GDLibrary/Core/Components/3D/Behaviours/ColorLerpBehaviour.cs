using GDLibrary.Graphics;
using Microsoft.Xna.Framework;
using System;

namespace GDLibrary.Components
{
    /// <summary>
    /// Allows us to slowly (based on lerpSpeed) lerp between two colors on a game object
    /// </summary>
    public class ColorLerpBehaviour : Behaviour
    {
        #region Fields

        private Color startColor;
        private Color endColor;
        private float lerpSpeed;
        private BasicMaterial material;

        #endregion Fields

        #region Properties

        public Color StartColor { get => startColor; set => startColor = value; }
        public Color EndColor { get => endColor; set => endColor = value; }
        public float LerpSpeed { get => lerpSpeed; set => lerpSpeed = value > 0 ? value : 1; }

        #endregion Properties

        #region Constructors

        public ColorLerpBehaviour(Color startColor, Color endColor, float lerpSpeed = 1)
        {
            StartColor = startColor;
            EndColor = endColor;
            LerpSpeed = lerpSpeed;
        }

        #endregion Constructors

        #region Actions - Awake & Update

        public override void Awake(GameObject gameObject)
        {
            material = gameObject.GetComponent<Renderer>().Material as BasicMaterial;

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

                //set color
                material.DiffuseColor = startColor.Lerp(endColor, lerpFactor, 255).ToVector3();
            }
        }

        #endregion Actions - Awake & Update
    }
}