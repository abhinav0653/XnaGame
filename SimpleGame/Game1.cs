using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using XELibrary;

namespace SimpleGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //created by me
        bool paused;
        public enum GameState { StartMenu, Scene, Won, Lost }
        public GameState State = GameState.StartMenu;
        private InputHandler input;
        private Texture2D startMenu;
        private Vector2 centerVector;
        private int viewportWidth;
        private int viewportHeight;
        private int width;
        private int height;
        private Vector2 playerPosition;
        private Vector2 enemyPosition;

        private Player player;
        private EnemyManager enemies;
        private Background background;
        private ExplosionManager explosionManager;
        private ScrollingBackgroundManager sbm;
        private FadeOut fade;
        private CelAnimationManager cam;

        private void CheckForCollisions(GameTime gameTime)
        {
            int enemyIndex = enemies.CollidedWithPlayer(player.Position);
            if (enemyIndex != -1)
            {
                if (player.Attacking)
                {
                    //die robot
                    Vector2 enemyPosition = enemies.Die(enemyIndex);
                    explosionManager.StartExplosion(enemyIndex, enemyPosition, gameTime.TotalGameTime.TotalMilliseconds);
                    
                }
                else
                {
                    background.Enabled = false;
                    enemies.SetPause(true);
                    enemies.Enabled = false;
                    player.SetPause(true);
                    player.Enabled = false;
                    State = GameState.Lost;
                }
            }
        }

        private bool WasPressed(int playerIndex, Buttons button, Keys keys)
        {
            if (input.ButtonHandler.WasButtonPressed(playerIndex, button) ||
input.KeyboardState.WasKeyPressed(keys))
                return (true);
            else
                return (false);
        }

        public void ActivateStartMenu()
        {
            //stop updating scene backgrounds
            background.Visible = background.Enabled = false;
            player.Visible = player.Enabled = false;
            player.SetPause(true);

            enemies.Visible = enemies.Enabled = false; //stop updating enemies
            enemies.SetPause(true);
            State = GameState.StartMenu;

        }
        private void UpdateScene(GameTime gameTime)
        {
            if (WasPressed(0, Buttons.Start, Keys.Enter))
            {
                paused = true;
                ActivateStartMenu();
            }
            CheckForCollisions(gameTime);
            if (!enemies.EnemiesExist)
            {
                //Level over (would advance level here and on last level state won game)
                enemies.SetPause(true);
                enemies.Enabled = false;
                player.SetPause(true);
                player.Enabled = false;
                background.Enabled = false;
                State = GameState.Won;
            }

        }

        private void ActivateGame()
        {
            background.Visible = background.Enabled = true; //start updating scene
            player.Visible = player.Enabled = true;
            player.SetPause(false);
            if (!paused)
            {
                enemies.Load(spriteBatch, 5, 20, 120.0f);
                explosionManager.SetMaxNumberOfExplosions(5); //should be read by level
            }//should be read by level
            enemies.Visible = enemies.Enabled = true; // resume updating enemies
            enemies.SetPause(false);
            State = GameState.Scene;
        }
        private void UpdateStartMenu()
        {
            if (WasPressed(0, Buttons.Start, Keys.Enter))
            {
                ActivateGame();
                paused = false;

            }
        }

        public void DrawStartMenu()
        {
            spriteBatch.Draw(startMenu, centerVector, Color.White);
        }


        public Game1()
        {
            paused = false;
            playerPosition = new Vector2(64, 350);
            enemyPosition = new Vector2(70, 350);
            width = 640;
            height = 480;
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = width;
            graphics.PreferredBackBufferHeight = height;

            Content.RootDirectory = "Content";


            cam = new CelAnimationManager(this, @"Textures\");
            Components.Add(cam);

            //initializing variables created by me
            input = new InputHandler(this);
            Components.Add(input);
            viewportHeight = height;// graphics.GraphicsDevice.Viewport.Height;
            viewportWidth = width;// graphics.GraphicsDevice.Viewport.Width;

            background = new Background(this, @"Textures\");
            background.Enabled = background.Visible = false;
            Components.Add(background);

            player = new Player(this);
            player.Position = playerPosition;
            Components.Add(player);

            enemies = new EnemyManager(this);
            Components.Add(enemies);

            explosionManager = new ExplosionManager(this, 5);
            Components.Add(explosionManager);

        }


        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
            sbm = (ScrollingBackgroundManager)Services.GetService(
                typeof(IScrollingBackgroundManager));
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            startMenu = Content.Load<Texture2D>(@"Textures\startmenu");
            centerVector = new Vector2((viewportWidth - startMenu.Width) / 2,
            (viewportHeight - startMenu.Height) / 2);

            background.Load(spriteBatch);
            player.Load(spriteBatch);
            enemies.Load(spriteBatch, 5,2,3);
            explosionManager.Load(spriteBatch);
           
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here
            switch (State)
            {
                case GameState.Scene:
                    {
                        UpdateScene(gameTime);
                        break;
                    }
                case GameState.StartMenu:
                    {
                        UpdateStartMenu();
                        break;
                    }
                case GameState.Won:
                    {
                        //Game Over - You Won!
                        if (!fade.Enabled)
                        {
                            fade.Color = Color.Black;
                            fade.Enabled = true;
                        }
                        break;
                    }
                case GameState.Lost:
                    {
                        if (!fade.Enabled)
                        {
                            fade.Color = Color.Red;
                            fade.Enabled = true;
                        }
                        break;
                    }
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin(SpriteBlendMode.Additive, SpriteSortMode.Immediate,
            SaveStateMode.None);
            if (State == GameState.StartMenu)
                DrawStartMenu();
            base.Draw(gameTime);
            //Display our foreground (after game components)
            if (State == GameState.Scene)
                sbm.Draw("foreground", spriteBatch);
            spriteBatch.End();
        }
    }
}
