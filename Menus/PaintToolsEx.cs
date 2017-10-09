using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Terraria;
using Terraria.ID;
using Terraria.GameContent.Liquid;
using Newtonsoft.Json;
using CheatSheet.Menus;

namespace CheatSheet
{
    internal class StampInfo
    {
        internal Tile[,] Tiles;
        internal Texture2D[,] Textures;
        internal int Width;
        internal int Height;
        internal bool bFlipHorizontal;
        internal bool bFlipVertical;
    }

    internal static class PaintToolsEx
    {
        internal static string importPath = Path.Combine(Main.SavePath, "CheatSheet_PaintTools.txt");
        internal static string exportPath = Path.Combine(Main.SavePath);

        internal static void Import(PaintToolsView paintToolsView)
        {
            try
            {
                List<PaintToolsSlot> list = new List<PaintToolsSlot>();
                foreach (var line in File.ReadAllLines(importPath, Encoding.UTF8))
                {
                    Tile[,] tiles = JsonConvert.DeserializeObject<Tile[,]>(File.ReadAllText(line, Encoding.UTF8));
                    list.Add(new PaintToolsSlot(GetStampInfo(tiles)));
                }
                paintToolsView.Add(list.ToArray());
            }
            catch { }
        }
        
        public static void Export(PaintToolsView paintToolsView)
        {
            try
            {
                int index = 1;
                string path;
                List<string> list = new List<string>();
                foreach (var slot in paintToolsView.slotList)
                {
                    path = $@"{exportPath}\CheatSheet_PaintTools_{index++}.json";
                    list.Add(path);
                    File.WriteAllText(path, JsonConvert.SerializeObject(slot.stampInfo.Tiles), Encoding.UTF8);
                }
                if (0 < list.Count)
                    File.WriteAllLines(importPath, list);
                Main.NewText($"Exported data to {importPath}");
            }
            catch { }
        }

        public static StampInfo GetStampInfo(Tile[,] Tiles)
        {
            int maxTile = ModUtils.TextureMaxTile;

            StampInfo result = new StampInfo();
            result.Tiles = Tiles;

            int maxX = Tiles.GetLength(0);
            int maxY = Tiles.GetLength(1);
            int texMaxX = (int)Math.Ceiling((double)maxX / (double)maxTile);
            int texMaxY = (int)Math.Ceiling((double)maxY / (double)maxTile);
            int restX = maxX % maxTile == 0 ? maxTile : maxX % maxTile;
            int restY = maxY % maxTile == 0 ? maxTile : maxY % maxTile;

            result.Textures = new Texture2D[texMaxX, texMaxY];
            result.Width = maxX * 16;
            result.Height = maxY * 16;

            for (int x = 0; x < texMaxX; x++)
            {
                for (int y = 0; y < texMaxY; y++)
                {
                    Tile[,] tiles = new Tile[x < texMaxX - 1 ? maxTile : restX, y < texMaxY - 1 ? maxTile : restY];
                    for (int i = 0; i < tiles.GetLength(0); i++)
                    {
                        for (int j = 0; j < tiles.GetLength(1); j++)
                        {
                            tiles[i, j] = Tiles[x * maxTile + i, y * maxTile + j];
                        }
                    }
                    result.Textures[x, y] = TilesToTexture(tiles);
                }
            }

            return result;
        }

        private static Rectangle GetTileRect(Tile tile, int halfIndex = 0)
        {
            Rectangle result;

            if (tile.halfBrick())
            {
                if (halfIndex == 0)
                    result = new Rectangle(tile.frameX, tile.frameY, 16, 4);
                else
                    result = new Rectangle(tile.frameX, tile.frameY + 12, 16, 4);
            }
            else if (tile.type == TileID.MinecartTrack)
                result = Minecart.GetSourceRect(tile.frameX, Main.tileFrame[tile.type]);
            else
                result = new Rectangle(tile.frameX, tile.frameY, 16, 16);

            if (result.X < 0)
                result.X = 0;
            if (result.Y < 0)
                result.Y = 0;

            return result;
        }

        private static Texture2D TilesToTexture(Tile[,] Tiles)
        {
            Texture2D result = null;
            try
            {
                int maxTile = ModUtils.TextureMaxTile;
                int maxX = Math.Min(Tiles.GetLength(0), maxTile);
                int maxY = Math.Min(Tiles.GetLength(1), maxTile);
                int width = maxX * 16;
                int height = maxY * 16;

                Color[] data = new Color[width * height];
                Color[] dataTile = new Color[256];
                Color[] dataWall = new Color[256];
                Color[] dataWater = new Color[256];

                Texture2D textureTile;
                Texture2D textureWall;
                Texture2D textureWater;

                bool bTile;
                bool bWall;
                bool bWater;

                for (int y = 0; y < maxY; y++)
                {
                    for (int x = 0; x < maxX; x++)
                    {
                        try
                        {
                            Tile tile = Tiles[x, y];

                            textureTile = null;
                            textureWall = null;
                            textureWater = null;

                            bTile = tile.active();
                            bWall = 0 < tile.wall;
                            bWater = 0 < tile.liquid;

                            if (bTile)
                            {
                                Main.instance.LoadTiles(tile.type);
                                if (canDrawColorTile(tile))
                                    textureTile = Main.tileAltTexture[tile.type, tile.color()];
                                else
                                    textureTile = Main.tileTexture[tile.type];

                                Rectangle rect = GetTileRect(tile);

                                if (textureTile.Width < rect.X + rect.Width)
                                {
                                    int width2 = textureTile.Width - rect.X;
                                    rect.Width = width2;

                                    Color[] d = new Color[16 * width2];

                                    textureTile.GetData<Color>(0, rect, d, 0, d.Length);

                                    for (int y2 = 0; y2 < 16; y2++)
                                    {
                                        for (int x2 = 0; x2 < 16; x2++)
                                        {
                                            if (x2 < width2)
                                                dataTile[y2 * 16 + x2] = d[y2 * width2 + x2];
                                            else
                                                dataTile[y2 * 16 + x2] = Color.Transparent;
                                        }
                                    }
                                }
                                else
                                {
                                    if (tile.halfBrick())
                                        dataTile = GetHalfTile(tile, textureTile);
                                    else if (0 < tile.slope())
                                        dataTile = GetSlopeTile(tile, textureTile);
                                    else
                                        textureTile.GetData<Color>(0, rect, dataTile, 0, 256);
                                }
                            }
                            if (bWall)
                            {
                                Main.instance.LoadWall(tile.wall);
                                if (canDrawColorWall(tile) && tile.type < Main.wallAltTexture.GetLength(0) && Main.wallAltTexture[tile.type, tile.wallColor()] != null)
                                    textureWall = Main.wallAltTexture[tile.type, tile.wallColor()];
                                else
                                    textureWall = Main.wallTexture[tile.wall];
                                textureWall.GetData<Color>(0, new Rectangle(tile.wallFrameX() + 8, tile.wallFrameY() + (Main.wallFrame[tile.wall] * 180) + 8, 16, 16), dataWall, 0, 256);
                            }
                            if (bWater)
                            {
                                if (tile.honey())
                                    textureWater = LiquidRenderer.Instance._liquidTextures[11].Offset(16, 48, 16, 16);
                                else if (tile.lava())
                                    textureWater = LiquidRenderer.Instance._liquidTextures[1].Offset(16, 48, 16, 16);
                                else
                                    textureWater = LiquidRenderer.Instance._liquidTextures[0].Offset(16, 48, 16, 16);
                                int waterSize = (tile.liquid + 1) / 16;
                                dataWater = new Color[256];
                                textureWater.GetData<Color>(0, new Rectangle(0, 16 - waterSize, 16, waterSize), dataWater, (16 - waterSize) * 16, 256 - (16 - waterSize) * 16);
                            }

                            int w = x * 16;
                            if (bTile || bWall || bWater)
                            {
                                for (int y2 = 0; y2 < 16; y2++)
                                {
                                    int h = (y * 16 * width) + (y2 * width);
                                    for (int x2 = 0; x2 < 16; x2++)
                                    {
                                        if (bWall)
                                            data[h + w + x2] = dataWall[y2 * 16 + x2];
                                        if (bTile && dataTile[y2 * 16 + x2] != Color.Transparent)
                                            data[h + w + x2] = dataTile[y2 * 16 + x2];
                                        if (bWater && dataWater[y2 * 16 + x2] != Color.Transparent)
                                        {
                                            if (bWall || (bTile && dataTile[y2 * 16 + x2] != Color.Transparent))
                                                data[h + w + x2] = data[h + w + x2].MultiplyRGBA(dataWater[y2 * 16 + x2] * 0.8f);
                                            else
                                                data[h + w + x2] = dataWater[y2 * 16 + x2] * 0.8f;
                                        }
                                    }
                                }
                            }
                        }
                        catch { }
                    }
                }

                result = new Texture2D(Main.graphics.GraphicsDevice, width, height);
                result.SetData<Color>(data);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            return result;
        }

        public static bool canDrawColorTile(Tile tile)
        {
            return tile != null && tile.color() > 0 && (int)tile.color() < Main.numTileColors && Main.tileAltTextureDrawn[(int)tile.type, (int)tile.color()] && Main.tileAltTextureInit[(int)tile.type, (int)tile.color()];
        }
        public static bool canDrawColorWall(Tile tile)
        {
            return tile != null && tile.wallColor() > 0 && Main.wallAltTextureDrawn[tile.wall, tile.wallColor()] && Main.wallAltTextureInit[tile.wall, tile.wallColor()];
        }

        public static Color[] GetHalfTile(Tile tile, Texture2D textureTile)
        {
            Color[] result = new Color[256];
            var data = new Color[64];
            var r1 = GetTileRect(tile, 0);
            var r2 = GetTileRect(tile, 1);

            textureTile.GetData<Color>(0, r1, data, 0, 64);
            for (int y = 0; y < 4; y++)
            {
                for (int x = 0; x < 16; x++)
                {
                    result[(y + 8) * 16 + x] = data[y * 16 + x];
                }
            }

            textureTile.GetData<Color>(0, r2, data, 0, 64);
            for (int y = 0; y < 4; y++)
            {
                for (int x = 0; x < 16; x++)
                {
                    result[(y + 12) * 16 + x] = data[y * 16 + x];
                }
            }

            return result;
        }
        public static Color[] GetSlopeTile(Tile tile, Texture2D textureTile)
        {
            Color[] result = new Color[256];
            var data = new Color[256];
            var rect = GetTileRect(tile);
            textureTile.GetData<Color>(0, rect, data, 0, 256);

            switch (tile.slope())
            {
                case 1:
                    for (int x = 0; x < 16; x += 2)
                    {
                        for (int y = 0; y < (14 - x); y += 2)
                        {
                            result[(y + 0 + x) * 16 + (x + 0)] = data[(y + 0) * 16 + (x + 0)];
                            result[(y + 0 + x) * 16 + (x + 1)] = data[(y + 0) * 16 + (x + 1)];
                            result[(y + 1 + x) * 16 + (x + 0)] = data[(y + 1) * 16 + (x + 0)];
                            result[(y + 1 + x) * 16 + (x + 1)] = data[(y + 1) * 16 + (x + 1)];
                        }
                        result[(14) * 16 + (x + 0)] = data[(14) * 16 + (x + 0)];
                        result[(14) * 16 + (x + 1)] = data[(14) * 16 + (x + 1)];
                        result[(15) * 16 + (x + 0)] = data[(15) * 16 + (x + 0)];
                        result[(15) * 16 + (x + 1)] = data[(15) * 16 + (x + 1)];
                    }
                    break;

                case 2:
                    for (int x = 0; x < 16; x += 2)
                    {
                        for (int y = 0; y < (x + 2); y += 2)
                        {
                            result[(y + 0 + (14 - x)) * 16 + (x + 0)] = data[(y + 0) * 16 + (x + 0)];
                            result[(y + 0 + (14 - x)) * 16 + (x + 1)] = data[(y + 0) * 16 + (x + 1)];
                            result[(y + 1 + (14 - x)) * 16 + (x + 0)] = data[(y + 1) * 16 + (x + 0)];
                            result[(y + 1 + (14 - x)) * 16 + (x + 1)] = data[(y + 1) * 16 + (x + 1)];
                        }
                        result[(14) * 16 + (x + 0)] = data[(14) * 16 + (x + 0)];
                        result[(14) * 16 + (x + 1)] = data[(14) * 16 + (x + 1)];
                        result[(15) * 16 + (x + 0)] = data[(15) * 16 + (x + 0)];
                        result[(15) * 16 + (x + 1)] = data[(15) * 16 + (x + 1)];
                    }
                    break;

                case 3:
                    for (int x = 0; x < 16; x += 2)
                    {
                        result[(0) * 16 + (x + 0)] = data[(0) * 16 + (x + 0)];
                        result[(0) * 16 + (x + 1)] = data[(0) * 16 + (x + 1)];
                        result[(1) * 16 + (x + 0)] = data[(1) * 16 + (x + 0)];
                        result[(1) * 16 + (x + 1)] = data[(1) * 16 + (x + 1)];
                        for (int y = 2; y < (16 - x); y += 2)
                        {
                            result[(y + 0) * 16 + (x + 0)] = data[(y + 0 + x) * 16 + (x + 0)];
                            result[(y + 0) * 16 + (x + 1)] = data[(y + 0 + x) * 16 + (x + 1)];
                            result[(y + 1) * 16 + (x + 0)] = data[(y + 1 + x) * 16 + (x + 0)];
                            result[(y + 1) * 16 + (x + 1)] = data[(y + 1 + x) * 16 + (x + 1)];
                        }
                    }
                    break;

                case 4:
                    for (int x = 0; x < 16; x += 2)
                    {
                        result[(0) * 16 + (x + 0)] = data[(0) * 16 + (x + 0)];
                        result[(0) * 16 + (x + 1)] = data[(0) * 16 + (x + 1)];
                        result[(1) * 16 + (x + 0)] = data[(1) * 16 + (x + 0)];
                        result[(1) * 16 + (x + 1)] = data[(1) * 16 + (x + 1)];
                        for (int y = 2; y < (x + 2); y += 2)
                        {
                            result[(y + 0) * 16 + (x + 0)] = data[(y + 0 + (14 - x)) * 16 + (x + 0)];
                            result[(y + 0) * 16 + (x + 1)] = data[(y + 0 + (14 - x)) * 16 + (x + 1)];
                            result[(y + 1) * 16 + (x + 0)] = data[(y + 1 + (14 - x)) * 16 + (x + 0)];
                            result[(y + 1) * 16 + (x + 1)] = data[(y + 1 + (14 - x)) * 16 + (x + 1)];
                        }
                    }
                    break;

                default:
                    result = data;
                    break;
            }
            return result;
        }
    }
}