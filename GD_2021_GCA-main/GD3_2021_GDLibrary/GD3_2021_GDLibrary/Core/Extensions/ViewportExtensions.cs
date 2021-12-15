using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

/// <summary>
/// Extension methods for Viewport - generally used when we have multi-camera views
/// </summary>
public static class ViewportExtensions
{
    /// <summary>
    /// Deflates the viewport all around by the deflateDelta value
    /// </summary>
    /// <param name="target"></param>
    /// <param name="deflateBy">Positive or negative value to deflate/inflate viewport by</param>
    public static void Deflate(ref this Viewport target, int deflateBy)
    {
        if (deflateBy % 2 == 1)
            throw new ArgumentException("inflateDelta value must be even to evenly inflate/deflate the viewport!");

        int half = Math.Abs(deflateBy / 2);

        //make viewport smaller
        if (deflateBy < 0)
        {
            target.X -= half;
            target.Y -= half;

            target.Width += 2 * half;
            target.Height += 2 * half;
        }
        //make viewport larger
        else
        {
            target.X += half;
            target.Y += half;

            target.Width -= 2 * half;
            target.Height -= 2 * half;
        }
    }

    /// <summary>
    /// Gets the scale factor necessary to apply to a texture2D object to fit the current viewport resolution
    /// </summary>
    /// <param name="viewport">Viewport</param>
    /// <param name="texture">Texture2D</param>
    /// <param name="additionalScaleFactor">Vector2</param>
    /// <returns>Vector2</returns>
    public static Vector2 GetScaleForTexture(this Viewport target, Texture2D texture, Vector2 additionalScaleFactor)
    {
        //prevents passing Vector2.Zero
        if (additionalScaleFactor == null || additionalScaleFactor.Length() == 0)
            additionalScaleFactor = Vector2.One;

        return additionalScaleFactor * new Vector2((float)Math.Ceiling((float)target.Width / texture.Width),
            (float)Math.Ceiling((float)target.Height / texture.Height));
    }
}