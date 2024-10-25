using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.Graphics.Effects;
using DeusMod.Core.Systems;

namespace DeusMod.Hooks
{
    public class RT2D : ModSystem
    {
		public static RenderTarget2D render;
		public override void Load()
		{
			On_FilterManager.EndCapture += On_FilterManager_EndCapture;
		}
		private void On_FilterManager_EndCapture(On_FilterManager.orig_EndCapture orig, FilterManager self, RenderTarget2D finalTexture, RenderTarget2D screenTarget1, RenderTarget2D screenTarget2, Color clearColor)
		{
			if (render == null)
				CreateRender();

			Save(); //保存原图

			DrawWarp(); //绘制扭曲用噪声图

			ApplyWarp();
			orig.Invoke(self, finalTexture, screenTarget1, screenTarget2, clearColor);
		}
		public void CreateRender()
		{
			render = new RenderTarget2D(Main.instance.GraphicsDevice, Main.screenWidth, Main.screenHeight);
		}
		private void Save()
		{
			Main.instance.GraphicsDevice.SetRenderTarget(render); //打开自建的render
			Main.instance.GraphicsDevice.Clear(Color.Transparent);
			Main.spriteBatch.Begin(0, BlendState.AlphaBlend);
			Main.spriteBatch.Draw(Main.screenTarget, Vector2.Zero, Color.White); //复制原本的屏幕内容
			Main.spriteBatch.End();
		}

		private void DrawWarp()
		{
			Main.instance.GraphicsDevice.SetRenderTarget(Main.screenTargetSwap); //打开swap用render
			Main.instance.GraphicsDevice.Clear(Color.Transparent);
			Main.spriteBatch.Begin(0, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
			for (int i = 0; i < Main.maxProjectiles; i++) //遍历所有的射弹
				if (Main.projectile[i].active && Main.projectile[i].ModProjectile is IDrawWarp) //射弹设置IDrawWarp接口
					(Main.projectile[i].ModProjectile as IDrawWarp).DrawWarp(); //应用接口里的绘制
			Main.spriteBatch.End();
		}

		private void ApplyWarp()
		{
			Main.instance.GraphicsDevice.SetRenderTarget(Main.screenTarget); //打开主render
			Main.instance.GraphicsDevice.Clear(Color.Transparent);
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
			//将shader应用图片设置成swap用render里已绘制的噪声图，设置参数，开启shader
			Effect effect = Filters.Scene["Warp"].GetShader().Shader;
			effect.Parameters["tex"].SetValue(Main.screenTargetSwap);
			effect.Parameters["intense"].SetValue(0.01f);
			effect.CurrentTechnique.Passes[0].Apply();
			Main.spriteBatch.Draw(render, Vector2.Zero, Color.White); //把先前保存的内容绘制
			Main.spriteBatch.End();
		}
		public override void Unload()
		{
			render = null;
		}
	}
}
