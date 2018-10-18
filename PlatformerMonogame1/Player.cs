using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace PlatformerMonogame1
{
    class Player
    {
        public Sprite playerSprite = new Sprite();

        Game1 game = null;
        float runSpeed = 15000f;

        Collision collision = new Collision();

        public Player()
        {

        }

        public void Load (ContentManager content, Game1 theGame)
        {
            playerSprite.Load(content, "hero", true);
            playerSprite.offset = new Vector2(24, 24);

            game = theGame;
            playerSprite.velocity = Vector2.Zero;
            playerSprite.position = new Vector2(theGame.GraphicsDevice.Viewport.Width / 2, 0);
        }

        public void Update (float deltaTime)
        {
            UpdateInput(deltaTime);
            playerSprite.Update(deltaTime);
            playerSprite.UpdateHitBox();
        }

        public void Draw (SpriteBatch spriteBatch)
        {
            playerSprite.Draw(spriteBatch);
        }

        private void UpdateInput (float deltaTime)
        {
            Vector2 localAcceleration = new Vector2(0, 0);

            if (Keyboard.GetState().IsKeyDown(Keys.Left) == true)
            {
                localAcceleration.X = -runSpeed;
            }
            if(Keyboard.GetState().IsKeyDown(Keys.Right) == true)
            {
                localAcceleration.X = runSpeed;
            }
            if(Keyboard.GetState().IsKeyDown(Keys.Up) == true)
            {
                localAcceleration.Y = -runSpeed;
            }
            if(Keyboard.GetState().IsKeyDown(Keys.Down) == true)
            {
                localAcceleration.Y = runSpeed;
            }
            
            playerSprite.velocity = localAcceleration * deltaTime;
            playerSprite.position += playerSprite.velocity * deltaTime;

            collision.game = game;
            playerSprite = collision.CollideWithPlatforms(playerSprite, deltaTime);

        }
    }
}
