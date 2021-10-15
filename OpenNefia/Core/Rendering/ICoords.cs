﻿namespace OpenNefia.Core.Rendering
{
    public interface ICoords
    {
        void GetSize(out int width, out int height);
        void GetTiledSize(int screenWidth, int screenHeight, out int tileWidth, out int tileHeight);
        void TileToScreen(int tileX, int tileY, out int screenX, out int screenY);
        void ScreenToTile(int screenX, int screenY, out int tileX, out int tileY);
        void BoundDrawPosition(int screenX, int screenY, int tiledWidth, int tiledHeight, int viewportWidth, int viewportHeight, out int drawX, out int drawY);
    }
}