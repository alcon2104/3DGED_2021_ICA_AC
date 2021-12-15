using Microsoft.Xna.Framework.Graphics;

namespace GDLibrary.Core.Graphics.States
{
    /// <summary>
    /// Holds the graphics device states used during different draw modes e.g. drawing opaque versus transparent objects
    /// </summary>
    public sealed class GraphicsDeviceState
    {
        private readonly GraphicsDevice graphicsDevice;
        private readonly BlendState blendState;
        private readonly DepthStencilState depthStencilState;
        private readonly RasterizerState rasterizerState;
        private readonly SamplerState samplerState;

        public GraphicsDeviceState(GraphicsDevice device, BlendState blendState, DepthStencilState depthStencilState, RasterizerState rasterizerState, SamplerState samplerState)
        {
            graphicsDevice = device;
            this.blendState = device.BlendState;
            this.depthStencilState = device.DepthStencilState;
            this.rasterizerState = device.RasterizerState;
            this.samplerState = device.SamplerStates[0];

            device.BlendState = blendState;
            device.DepthStencilState = depthStencilState;
            device.RasterizerState = rasterizerState;
            device.SamplerStates[0] = samplerState;
        }
    }
}