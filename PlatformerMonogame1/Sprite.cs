﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace PlatformerMonogame1
{
    public class Sprite
    {
        public Vector2 position = Vector2.Zero;
        public Vector2 offset = Vector2.Zero;
        public Vector2 velocity = Vector2.Zero;

        public bool canJump = false;

        Texture2D texture;

        public int width = 0;
        public int height = 0;

        //The edges of the Sprite, also edges of the hit box for collisions

        public int leftEdge = 0;
        public int rightEdge = 0;
        public int topEdge = 0;
        public int bottomEdge = 0;

        // The distance we need to offset to create a snug collision
        public int leftCollisionOffset = 0;
        public int rightCollisionOffset = 0;
        public int vertCollisionOffset = 0;

        List<AnimatedTexture> animations = new List<AnimatedTexture>();
        List<Vector2> animationOffsets = new List<Vector2>();
        int currentAnimation = 0;
        SpriteEffects effects = SpriteEffects.None;


        public Sprite()
        {
            
        }

        public void Load (ContentManager content, string asset, bool useOffset)
        {
            texture = content.Load<Texture2D>(asset);
            width = texture.Bounds.Width;
            height = texture.Bounds.Height;

            if (useOffset == true)
            {
                offset = new Vector2(leftEdge + width / 2, topEdge + height / 2);
            }

            UpdateHitBox();
        }

        public void UpdateHitBox()
        {
            leftEdge = (int)position.X - (int)offset.X + leftCollisionOffset;
            rightEdge = leftEdge + width - rightCollisionOffset;
            topEdge = (int)position.Y - (int)offset.Y + vertCollisionOffset;
            bottomEdge = topEdge + height - vertCollisionOffset;
        }

        public void Update (float deltaTime)
        {
            animations[currentAnimation].UpdateFrame(deltaTime);
        }

        public void Draw (SpriteBatch spriteBatch, Game1 game)
        {
            animations[currentAnimation].DrawFrame(spriteBatch, position + animationOffsets[currentAnimation], effects);

            if (game.debug == true)
            {
                 game.DrawRectangle(new Rectangle(leftEdge, topEdge, width - rightCollisionOffset, height - vertCollisionOffset), Color.White);
            }                     
        }

        public void AddAnimation (AnimatedTexture animation, int xOffset = 0, int yOffset = 0)
        {
            animations.Add(animation);
            animationOffsets.Add(new Vector2(xOffset, yOffset));
        }

        public void SetFlipped (bool state)
        {
            if (state == true)
            {
                effects = SpriteEffects.FlipHorizontally;
            }
            else
            {
                effects = SpriteEffects.None;
            }
        }

        public void Stop()
        {
            animations[currentAnimation].Stop();
        }

        public void Play()
        {
            animations[currentAnimation].Play();
        }
    }
}
