using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace SimpleGame
{
    public class FadeOut : Microsoft.Xna.Framework.DrawableGameComponent
    {
        private Texture2D fadeTexture;
        private float fadeAmount;
        private double fadeStartTime;
        private Game1 simpleGame;
        public Color Color;
        private SpriteBatch spriteBatch;
        public FadeOut(Game game)
            : base(game)
        {
            simpleGame = (Game1)game;
            this.Enabled = false;
            this.Visible = false;
            DrawOrder = 999;
        }
        public void Load(SpriteBatch spriteBatch)
        {
            this.spriteBatch = spriteBatch;
        }
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            if (fadeStartTime == 0)
            {
                fadeStartTime = gameTime.TotalGameTime.TotalMilliseconds;
                Visible = true;
            }
            fadeAmount += (.25f * (float)gameTime.ElapsedGameTime.TotalSeconds);
            if (gameTime.TotalGameTime.TotalMilliseconds > fadeStartTime + 4000)
            {
                fadeAmount = 0;
                fadeStartTime = 0;
                Visible = Enabled = false;
                simpleGame.ActivateStartMenu();
            }
            base.Update(gameTime);
        }
        protected override void LoadContent()
        {
            fadeTexture = CreateFadeTexture(
            GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
        }
        public override void Draw(GameTime gameTime)
        {
            Vector4 color = Color.ToVector4();
            color.W = fadeAmount; //set transparency
            spriteBatch.Draw(fadeTexture, Vector2.Zero, new Color(color));
            base.Draw(gameTime);
        }
        private Texture2D CreateFadeTexture(int width, int height)
        {
            Texture2D texture = new Texture2D(GraphicsDevice, width, height, 1,
            TextureUsage.None, SurfaceFormat.Color);
            int pixelCount = width * height;
            Color[] pixelData = new Color[pixelCount];
            Random rnd = new Random();
            for (int i = 0; i < pixelCount; i++)
            {
                pixelData[i] = Color.White;
            }
            texture.SetData(pixelData);
            return (texture);
        }
    }
}