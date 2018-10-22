using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PlatformerMonogame1
{
    public class Enemy
    {
        float walkSpeed = 7500f;
        public Sprite enemySprite = new Sprite();
        Collision collision = new Collision();
        Game1 game = null;

        int horColOffset = 32;
        int vertColOffset = 32;

        int frameCount = 4;

        public void Load(ContentManager content, Game1 game)
        {
            this.game = game;

            enemySprite.Load(content, "zombie", false);
            enemySprite.width = enemySprite.width / frameCount;

            AnimatedTexture animation = new AnimatedTexture(Vector2.Zero, 0, 1, 1);
            animation.Load(content, "zombie", frameCount, 5);

            enemySprite.AddAnimation(animation, 0, 0);
            //enemySprite.width = 64 - horColOffset;
            //enemySprite.height = 64 - vertColOffset;
            enemySprite.offset = new Vector2(2, 2);
        }

        public void Update(float deltaTime)
        {
            // Move the enemy 
            enemySprite.velocity = new Vector2(walkSpeed, 0) * deltaTime;
            enemySprite.position += enemySprite.velocity * deltaTime;

            // Check for collisions
            collision.game = game;
            enemySprite = collision.CollideWithPlatforms(enemySprite, deltaTime);

            // If enemy hits a wall (ie velocity = 0), change direction
            if (enemySprite.velocity.X == 0)
            {
                walkSpeed *= -1;
            }

            enemySprite.UpdateHitBox();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            enemySprite.Draw(spriteBatch, game);
        }
    }   
}
