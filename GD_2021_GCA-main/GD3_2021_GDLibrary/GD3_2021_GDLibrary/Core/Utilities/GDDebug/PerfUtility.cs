using GDLibrary.Components;
using GDLibrary.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDLibrary.Utilities.GDDebug
{
    public class PerfUtility : PausableDrawableGameComponent
    {
        #region Fields

        private SpriteBatch spriteBatch;
        private SpriteFont spriteFont;
        private Vector2 fpsTextPosition;
        private Color fpsTextColor;

        private float totalTimeSinceLastFPSUpdate;
        private int fpsCountToShow;
        private int fpsCountSinceLastRefresh;

        #endregion Fields

        #region Constructors

        public PerfUtility(Game game,
        SpriteBatch spriteBatch, SpriteFont spriteFont,
        Vector2 fpsTextPosition, Color fpsTextColor)
        : base(game)
        {
            this.spriteBatch = spriteBatch;
            this.spriteFont = spriteFont;

            this.fpsTextPosition = fpsTextPosition;
            this.fpsTextColor = fpsTextColor;
        }

        #endregion Constructors

        #region Update & Draw

        public override void Update(GameTime gameTime)
        {
            if (IsUpdated)
            {
                //accumulate time until next text update
                totalTimeSinceLastFPSUpdate += Time.Instance.UnscaledDeltaTimeMs;

                //count the frames
                fpsCountSinceLastRefresh++;

                //every 500ms send the 2x frame count to fpsCountToShow
                if (totalTimeSinceLastFPSUpdate >= 500)
                {
                    //reset time until next count update
                    totalTimeSinceLastFPSUpdate = 0;

                    //store value to show in Draw()
                    fpsCountToShow = 2 * fpsCountSinceLastRefresh;

                    //reset frame count
                    fpsCountSinceLastRefresh = 0;
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            if (IsDrawn)
            {
                var translation = Camera.Main.Transform.LocalTranslation;
                translation.Round(1);

                spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null);
                spriteBatch.DrawString(spriteFont, $"FPS [{fpsCountToShow}]", fpsTextPosition, fpsTextColor);
                spriteBatch.DrawString(spriteFont, $"Camera [{Camera.Main.GameObject.Name}, {translation}]", fpsTextPosition + new Vector2(0, 20), fpsTextColor);
                spriteBatch.DrawString(spriteFont, $"Draw Calls [{Application.SceneManager.ActiveScene.Renderers.Count}]", fpsTextPosition + new Vector2(0, 40), fpsTextColor);
                //
                //
                //

                spriteBatch.End();
            }
        }

        #endregion Update & Draw
    }
}