using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public static class Texture2DExtensions
{
    /// <summary>
    /// Gets the origin of a texture where origin is texture center
    /// </summary>
    /// <param name="texture">Texture2D</param>
    /// <returns>Vector2</returns>
    public static Vector2 GetOriginAtCenter(this Texture2D target)
    {
        return new Vector2((int)(target.Width / 2.0f), (int)(target.Height / 2.0f));
    }
}