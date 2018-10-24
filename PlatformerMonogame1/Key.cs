using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace PlatformerMonogame1
{
    public class Key
    {
        public Sprite keySprite = new Sprite();
        Collision collision = new Collision();
        Game1 game = null;

        SoundEffect jumpSound;
        SoundEffectInstance jumpSoundtInstance;

        public void Load(ContentManager content, Game1 theGame)
        {
            game = theGame;

            AnimatedTexture animation = new AnimatedTexture(Vector2.Zero, 0, 1, 1);
            animation.Load(content, "KeyIcons", 1, 1);
            jumpSound = content.Load<SoundEffect>("Key Collect (edited)");
            jumpSoundtInstance = jumpSound.CreateInstance();

            keySprite.AddAnimation(animation, 0, 3);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            keySprite.Draw(spriteBatch, game);
        }

        public void Update(float deltaTime)
        {
            collision.game = game;
            keySprite.UpdateHitBox();
        }
    }
}

