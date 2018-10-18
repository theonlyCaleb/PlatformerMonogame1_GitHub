using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PlatformerMonogame1
{
    class Collision
    {
        public Game1 game;
        
        public bool IsColliding(Sprite hero, Sprite otherSprite)
        {
            // Compare the position of each rectangle edge against the other
            // It compares opposite edges of each rectangle 
            if (hero.rightEdge < otherSprite.leftEdge || hero.leftEdge > otherSprite.rightEdge || hero.bottomEdge < otherSprite.topEdge || hero.topEdge > otherSprite.bottomEdge)
            {
                return false; // These rectangles are NOT colliding
            }

            return true; // These rectangles ARE overlapping and there is a collision
        }

        public Sprite CollideWithPlatforms(Sprite hero, float deltaTime)
        {
            // Create a copy of the hero that will move to where the hero will be on the next frame,
            // to predict if the hero will overlap an obstacle
            Sprite playerPrediction = new Sprite();
            playerPrediction.position = hero.position;
            playerPrediction.width = hero.width;
            playerPrediction.height = hero.height;
            playerPrediction.offset = hero.offset;
            playerPrediction.UpdateHitBox();

            playerPrediction.position += hero.velocity * deltaTime;

            int playerColumn = (int)playerPrediction.position.X / game.tileHeight;
            int playerRow = (int)playerPrediction.position.Y / game.tileHeight;
            Vector2 playerTile = new Vector2(playerColumn, playerRow);

            Vector2 leftTile = new Vector2(playerTile.X - 1, playerTile.Y);
            Vector2 rightTile = new Vector2(playerTile.X + 1, playerTile.Y);
            Vector2 topTile = new Vector2(playerTile.X, playerTile.Y - 1);
            Vector2 bottomTile = new Vector2(playerTile.X, playerTile.Y + 1);

            Vector2 bottomLeftTile = new Vector2(playerTile.X - 1, playerTile.Y + 1);
            Vector2 bottomRightTile = new Vector2(playerTile.X + 1, playerTile.Y + 1);
            Vector2 topLeftTile = new Vector2(playerTile.X - 1, playerTile.Y - 1);
            Vector2 topRightTile = new Vector2(playerTile.X + 1, playerTile.Y - 1);

            bool leftCheck = CheckForTile(game, leftTile);
            bool rightCheck = CheckForTile(game, rightTile);
            bool bottomCheck = CheckForTile(game, bottomTile);
            bool topCheck = CheckForTile(game, topTile);

            bool bottomLeftCheck = CheckForTile(game, bottomLeftTile);
            bool bottomRightCheck = CheckForTile(game, bottomRightTile);
            bool topLeftCheck = CheckForTile(game, topLeftTile);
            bool topRightCheck = CheckForTile(game, topRightTile);

            // Check for collisions with tiles to the left, right, top and bottom
            if (leftCheck == true)
            {
                hero = CollideLeft(game, hero, leftTile, playerPrediction);
            }
            if (rightCheck == true)
            {
                hero = CollideRight(game, hero, rightTile, playerPrediction);
            }
            if (bottomCheck == true)
            {
                hero = CollideBelow(game, hero, bottomTile, playerPrediction);
            }
            if (topCheck == true)
            {
                hero = CollideAbove(game, hero, topTile, playerPrediction);
            }
            if (leftCheck == false && bottomCheck == false && bottomLeftCheck == true)
            {
                hero = CollideBottomDiagonals(hero, bottomLeftTile, playerPrediction);
            }
            if (rightCheck == false && bottomCheck == false && bottomRightCheck == true)
            {
                hero = CollideBottomDiagonals(hero, bottomRightTile, playerPrediction);
            }
            if (leftCheck == false && topCheck == false && topLeftCheck == true)
            {
                hero = CollideAboveDiagonals(hero, topLeftTile, playerPrediction);
            }
            if (rightCheck == false && topCheck == false && topRightCheck == true)
            {
                hero = CollideAboveDiagonals(hero, topRightTile, playerPrediction); 
            }
            
            return hero;
        }

        // Check if there is a tile at the specified coordinate
        bool CheckForTile (Game1 game, Vector2 coordinates)
        {
            int column = (int)coordinates.X;
            int row = (int)coordinates.Y;

            if (column < 0 || column > game.levelTileWidth - 1)
            {
                return false;
            }
            if (row < 0 || row > game.levelTileHeight - 1)
            {
                return false;
            }

            Sprite tileFound = game.levelGrid[column, row];

            if (tileFound != null)
            {
                return true;
            }

            return false;
        }

        Sprite CollideLeft(Game1 game, Sprite hero, Vector2 tileIndex, Sprite playerPrediction)
        {
            Sprite tile = game.levelGrid[(int)tileIndex.X, (int)tileIndex.Y];

            if (IsColliding(playerPrediction, tile) == true && hero.velocity.X < 0)
            {
                hero.position.X = tile.rightEdge + hero.offset.X;
                hero.velocity.X = 0;
            }

            return hero; 
        }

        Sprite CollideRight(Game1 game, Sprite hero, Vector2 tileIndex, Sprite playerPrediction)
        {
            Sprite tile = game.levelGrid[(int)tileIndex.X, (int)tileIndex.Y];

            if (IsColliding(playerPrediction, tile) == true && hero.velocity.X > 0)
            {
                hero.position.X = tile.leftEdge - hero.width + hero.offset.X;
                hero.velocity.X = 0;
            }

            return hero;
        }

        Sprite CollideAbove(Game1 game, Sprite hero, Vector2 tileIndex, Sprite playerPrediction)
        {
            Sprite tile = game.levelGrid[(int)tileIndex.X, (int)tileIndex.Y];

            if (IsColliding(playerPrediction, tile) == true && hero.velocity.Y < 0)
            {
                hero.position.Y = tile.bottomEdge + hero.offset.Y;
                hero.velocity.Y = 0;
            }

            return hero;
        }


        Sprite CollideBelow(Game1 game, Sprite hero, Vector2 tileIndex, Sprite playerPrediction)
        {
            Sprite tile = game.levelGrid[(int)tileIndex.X, (int)tileIndex.Y];

            if (IsColliding(playerPrediction, tile) == true && hero.velocity.Y > 0)
            {
                hero.position.Y = tile.topEdge - hero.height + hero.offset.Y;
                hero.velocity.Y = 0;
            }

            return hero;
        }

        Sprite CollideBottomDiagonals(Sprite hero, Vector2 tileIndex, Sprite playerPrediction)
        {
            Sprite tile = game.levelGrid[(int)tileIndex.X, (int)tileIndex.Y];

            int leftEdgeDistance = Math.Abs(tile.leftEdge - playerPrediction.rightEdge);
            int rightEdgeDistance = Math.Abs(tile.rightEdge - playerPrediction.leftEdge);
            int topEdgeDistance = Math.Abs(tile.topEdge - playerPrediction.bottomEdge);

            if (IsColliding(playerPrediction, tile) == true)
            {
                if (topEdgeDistance < rightEdgeDistance && topEdgeDistance < leftEdgeDistance)
                {
                    // If top edge is closest, collision is happening above the platform
                    hero.position.Y = tile.topEdge - hero.height + hero.offset.Y;
                    hero.velocity.Y = 0;                  
                }
                else if (rightEdgeDistance < leftEdgeDistance)
                {
                    // If right edge is closest, collision is happening to the right of the platform
                    hero.position.X = tile.rightEdge + hero.offset.X;
                    hero.velocity.X = 0;
                }
                else
                {
                    // If left edge is closest, collision is happening to the left of the platform
                    hero.position.X = tile.leftEdge - hero.width + hero.offset.X;
                    hero.velocity.X = 0;
                }  
            }
            return hero;         
        }

        Sprite CollideAboveDiagonals(Sprite hero, Vector2 tileIndex, Sprite playerPrediction)
        {
            Sprite tile = game.levelGrid[(int)tileIndex.X, (int)tileIndex.Y];

            int leftEdgeDistance = Math.Abs(tile.rightEdge - playerPrediction.leftEdge);
            int rightEdgeDistance = Math.Abs(tile.leftEdge - playerPrediction.rightEdge);
            int bottomEdgeDistance = Math.Abs(tile.bottomEdge - playerPrediction.topEdge);

            if(IsColliding(playerPrediction, tile) == true)
            {
                if (bottomEdgeDistance < leftEdgeDistance && bottomEdgeDistance < rightEdgeDistance)
                {
                    // If the bottom edge is closest and overlapping on the top edge 
                    hero.position.Y = tile.bottomEdge + hero.offset.Y;
                    hero.velocity.Y = 0;
                }
                else if (leftEdgeDistance < rightEdgeDistance)
                {
                    // Else if left edge is closest and overlapping 
                    hero.position.X = tile.rightEdge + hero.offset.X;
                    hero.velocity.X = 0;
                }
                else
                {
                    // Else if right edge is closest and overlapping 
                    hero.position.X = tile.leftEdge - hero.width + hero.offset.X;
                    hero.velocity.X = 0;
                }
            }
            return hero;
        }
    }
}
