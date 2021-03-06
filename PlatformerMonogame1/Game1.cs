﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Graphics;
using MonoGame.Extended.ViewportAdapters;
using System.Collections;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;

namespace PlatformerMonogame1
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public bool debug = false;

        Player player = new Player(); // Create an instance of our player class

        public List<Enemy> enemies = new List<Enemy>();
        public Chest goal = null;
        public List<Hazards> hazards = new List<Hazards>();
        public Key unlock = null;
        public bool chestUnlocked = false;
        public Sprite currentCheckpoint = null;

        Camera2D camera = null;
        TiledMap map = null;
        TiledMapRenderer mapRenderer = null;
        TiledMapTileLayer collisionLayer;
        public ArrayList allCollisionTiles = new ArrayList();
        public ArrayList keyTile = new ArrayList();
        public Sprite[,] levelGrid;

        public int tileHeight = 0;
        public int levelTileWidth = 0;
        public int levelTileHeight = 0;

        public Vector2 gravity = new Vector2(0, 1500);

        Song gameMusic;

        SpriteFont arialFont;
        int score = 0;
        public int lives = 3;
        Texture2D heart = null;

        public Texture2D rect; 

        public void DrawRectangle (Rectangle coords, Color color)
        {
            if (rect == null)
            {
                rect = new Texture2D(GraphicsDevice, 1, 1);
                rect.SetData(new[] { Color.White });
            }
            spriteBatch.Draw(rect, coords, color);
        }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
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
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            map = Content.Load<TiledMap>("CastleLevel1");
            mapRenderer = new TiledMapRenderer(GraphicsDevice);

            SetUpTiles();
            LoadObjects();

            player.Load(Content, this);

            arialFont = Content.Load<SpriteFont>("arial");
            heart = Content.Load<Texture2D>("Heart");

            BoxingViewportAdapter viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height);

            camera = new Camera2D(viewportAdapter);
            camera.Position = new Vector2(0, graphics.GraphicsDevice.Viewport.Height);

        
            gameMusic = Content.Load<Song>("Foreboding");
            MediaPlayer.Play(gameMusic);        
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.F1))
            {
                debug = true;
            }
            else if (Keyboard.GetState().IsKeyUp(Keys.F1))
            {
                debug = false;
            }

                float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            player.Update(deltaTime);

            foreach (Enemy enemy in enemies)
            {
                enemy.Update(deltaTime);
            }

            foreach (Hazards hazard in hazards)
            {
                hazard.Update(deltaTime);
            }

            unlock.Update(deltaTime);
            goal.Update(deltaTime);
     

            camera.Position = player.playerSprite.position - new Vector2(graphics.GraphicsDevice.Viewport.Width / 2, graphics.GraphicsDevice.Viewport.Height / 2);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.TransparentBlack);

            var viewMatrix = camera.GetViewMatrix();
            var projectionMatrix = Matrix.CreateOrthographicOffCenter(0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, 0, 0f, -1f);

            // Begin drawing
            spriteBatch.Begin(transformMatrix: viewMatrix);

            mapRenderer.Draw(map, ref viewMatrix, ref projectionMatrix);
            // Call the "Draw" function from our player class
            player.Draw(spriteBatch);
            goal.Draw(spriteBatch);
            unlock.Draw(spriteBatch);
            foreach (Enemy enemy in enemies)
            {
                enemy.Draw(spriteBatch);
            }        

            // Finish drawing
            spriteBatch.End();

            spriteBatch.Begin();
            spriteBatch.DrawString(arialFont, "SCORE: " + score.ToString(), new Vector2(20, 20), Color.White);
       

            if (debug == true)
            {
                 spriteBatch.DrawString(arialFont, "Debug = " + debug.ToString(), new Vector2(20, 40), Color.White);
            }

            int loopCount = 0;
            while (loopCount < lives)
            {
                spriteBatch.Draw(heart, new Vector2(GraphicsDevice.Viewport.Width - 60 - loopCount * 40, 20), Color.White); // Heart location
                loopCount++;
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        public void SetUpTiles()
        {
            tileHeight = map.TileHeight;
            levelTileHeight = map.Height;
            levelTileWidth = map.Width;
            levelGrid = new Sprite[levelTileWidth, levelTileHeight];

            foreach (TiledMapTileLayer layer in map.TileLayers)
            {
                if (layer.Name == "Collision") // Add in layer name for collisions
                {
                    collisionLayer = layer;
                }
            }

            int columns = 0;
            int rows = 0;
            int loopCount = 0;

            while (loopCount < collisionLayer.Tiles.Count)
            {
                if (collisionLayer.Tiles[loopCount].GlobalIdentifier != 0)
                {
                    Sprite tileSprite = new Sprite();
                    tileSprite.position.X = columns * tileHeight;
                    tileSprite.position.Y = rows * tileHeight;
                    tileSprite.width = tileHeight;
                    tileSprite.height = tileHeight;
                    tileSprite.UpdateHitBox();
                    allCollisionTiles.Add(tileSprite);
                    levelGrid[columns, rows] = tileSprite;
                }

                columns++;

                if (columns == levelTileWidth)
                {
                    columns = 0;
                    rows++;
                }

                loopCount++;
            }
        }

        void LoadObjects()
        {
            foreach (TiledMapObjectLayer layer in map.ObjectLayers)
            {
                if (layer.Name == "Respawn")
                {
                    TiledMapObject thing = layer.Objects[0];
                    if (thing != null)
                    {
                        Sprite respawn = new Sprite();
                        respawn.position = new Vector2(thing.Position.X, thing.Position.Y);
                        currentCheckpoint = respawn;
                    } 
                }

                if (layer.Name == "Enemies")
                {
                    foreach (TiledMapObject thing in layer.Objects)
                    {
                        Enemy enemy = new Enemy();
                        Vector2 tiles = new Vector2((int)(thing.Position.X / tileHeight), (int)(thing.Position.Y / tileHeight));
                        enemy.enemySprite.position = tiles * tileHeight;
                        enemy.Load(Content, this);
                        enemies.Add(enemy);
                    }
                }

                if (layer.Name == "Key")
                {
                    TiledMapObject thing = layer.Objects[0];
                    if (thing != null)
                    {
                        Key key = new Key();
                        key.keySprite.position = new Vector2(thing.Position.X, thing.Position.Y);
                        key.Load(Content, this);
                        unlock = key;
                        
                    }
                }

                if (layer.Name == "Goal")
                {
                    TiledMapObject thing = layer.Objects[0];
                    if (thing != null)
                    {
                        Chest chest = new Chest();
                        chest.chestSprite.position = new Vector2(thing.Position.X, thing.Position.Y);
                        chest.Load(Content, this);
                        goal = chest; 
                    }
                }

                if (layer.Name == "Hazards")
                {
                    foreach (TiledMapObject thing in layer.Objects)
                    {
                        Hazards hazard = new Hazards();
                        Vector2 tiles = new Vector2((int)(thing.Position.X / tileHeight), (int)(thing.Position.Y / tileHeight));
                        hazard.hazardsSprite.position = tiles * tileHeight;
                        hazard.Load(Content, this);
                        hazards.Add(hazard);
                    }
                }
            }
        }
    }
}
