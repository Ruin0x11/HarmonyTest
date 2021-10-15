﻿using OpenNefia.Core.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.Rendering
{
    internal class OrthographicCoords : ICoords
    {
        public void GetSize(out int tileWidth, out int tileHeight)
        {
            tileWidth = Constants.TILE_SIZE;
            tileHeight = Constants.TILE_SIZE;
        }

        public void GetTiledSize(int screenWidth, int screenHeight, out int tiledWidth, out int tiledHeight)
        {
            tiledWidth = (screenWidth / Constants.TILE_SIZE) + 1;
            tiledHeight = (screenHeight / Constants.TILE_SIZE) + 1;
        }

        public void TileToScreen(int tileX, int tileY, out int screenX, out int screenY)
        {
            screenX = tileX * Constants.TILE_SIZE;
            screenY = tileY * Constants.TILE_SIZE;
        }

        public void ScreenToTile(int screenX, int screenY, out int tileX, out int tileY)
        {
            tileX = screenX / Constants.TILE_SIZE;
            tileY = screenY / Constants.TILE_SIZE;
        }

        public void BoundDrawPosition(int screenX, int screenY, int tiledWidth, int tiledHeight, int viewportWidth, int viewportHeight, out int drawX, out int drawY)
        {
            var tileSize = Constants.TILE_SIZE;

            var mapScreenWidth = tiledWidth * tileSize;
            var mapScreenHeight = tiledHeight * tileSize;

            var maxX = mapScreenWidth - viewportWidth;
            var maxY = mapScreenHeight - viewportHeight;

            var offsetX = Math.Max((viewportWidth - mapScreenWidth / 2), 0);
            var offsetY = Math.Max((viewportHeight - mapScreenHeight / 2), 0);

            drawX = Math.Clamp(-screenX + viewportWidth / 2, -maxX, 0) + offsetX;
            drawY = Math.Clamp(-screenY + viewportHeight / 2, -maxY, 0) + offsetY;
        }
    }
}