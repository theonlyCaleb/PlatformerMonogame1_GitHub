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
    public class Hazards
    {
        public Sprite hazardsSprite = new Sprite();
        Collision collision = new Collision();
        Game1 game = null;

        public void Load(ContentManager content, Game1 theGame)
        {
            game = theGame;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            hazardsSprite.Draw(spriteBatch, game);
        }

        public void Update(float deltaTime)
        {
            collision.game = game;
            hazardsSprite.UpdateHitBox();
        }
    }
}
