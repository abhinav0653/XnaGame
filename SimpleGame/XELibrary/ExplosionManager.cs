using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XELibrary;
namespace SimpleGame
{
    public class ExplosionManager
    : Microsoft.Xna.Framework.DrawableGameComponent
    {
        private Dictionary<int, Explosion> explosions = new Dictionary<int, Explosion>(5);
        private CelAnimationManager cam;
        private SpriteBatch spriteBatch;
        private Game1 simpleGame;
        public ExplosionManager(Game game) : this(game, 0) { }
        public ExplosionManager(Game game, int maxExplosions)
            : base(game)
        {
            simpleGame = (Game1)game;
            cam = (CelAnimationManager)game.Services.GetService(
            typeof(ICelAnimationManager));
            if (maxExplosions > 0)
                SetMaxNumberOfExplosions(maxExplosions);
        }
        public void Load(SpriteBatch spriteBatch)
        {
            this.spriteBatch = spriteBatch;
        }
        public override void Update(GameTime gameTime)
        {
            if (simpleGame.State == Game1.GameState.Scene)
            {
                //Check For Explosions
                int markForDeletion = -1;
                foreach (KeyValuePair<int, Explosion> explosion in explosions)
                {
                    //have we been playing our explosion for over a second?
                    if (gameTime.TotalGameTime.TotalMilliseconds >
                    explosion.Value.TimeCreated + 100)
                    {
                        markForDeletion = explosion.Key;
                        break;
                    }
                }
                if (explosions.ContainsKey(markForDeletion))
                    explosions.Remove(markForDeletion);
            }
            base.Update(gameTime);
        }
        protected override void LoadContent()
        {
            //add our explosions
            cam.AddAnimation("explosion", "explode_1", new CelCount(4, 4), 16);
        }
        public override void Draw(GameTime gameTime)
        {
            switch (simpleGame.State)
            {
                case Game1.GameState.Scene:
                    {
                        foreach (Explosion explosion in explosions.Values)
                        {
                            cam.Draw(gameTime, "explosion", spriteBatch,
                            explosion.Position);
                        }
                        break;
                    }
                case Game1.GameState.StartMenu:
                    {
                        //we can add our explosions to make our title page pop
                        cam.Draw(gameTime, "explosion", spriteBatch,
                        new Vector2(32, 32));
                        break;
                    }
            }
            base.Draw(gameTime);
        }
        public void StartExplosion(int explosionKey, Vector2 position,
        double time)
        {
            explosions.Add(explosionKey, new Explosion(position, time));
        }
        public void SetMaxNumberOfExplosions(int maxExplosions)
        {
            explosions = new Dictionary<int, Explosion>(maxExplosions);
        }
    }
    public class Explosion
    {
        public double TimeCreated;
        public Vector2 Position;
        public Explosion(Vector2 position, double timeCreated)
        {
            Position = position;
            TimeCreated = timeCreated;
        }
    }
}