namespace GDLibrary.Type
{
    /********************************************************************************************************/

    /// <summary>
    /// Vertex data used to draw primitives (where the user defines the vertex data directly) within the game/app
    /// </summary>
    /// <see cref="GDLibrary.Graphics.Mesh{T}"/>
    public enum MeshType : sbyte
    {
        #region Wireframe

        XYZ,
        Origin,
        Line,
        WireframeRectangle,
        WireframeCircle,
        WireframeCube,

        #endregion Wireframe

        #region Filled/Untextured

        FilledTriangle,
        FilledQuad,
        FilledButterfly,
        FilledDiamond,

        #endregion Filled/Untextured

        #region Textured

        TexturedQuad

        #endregion Textured
    }

    /********************************************************************************************************/
}