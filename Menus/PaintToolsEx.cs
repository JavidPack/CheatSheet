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
using System.Reflection;
using Terraria.ModLoader.IO;
using System.Net;
using System.Collections.Specialized;
using Newtonsoft.Json.Linq;
using Terraria.DataStructures;
using System.Linq;

namespace CheatSheet
{
	internal class StampInfo
	{
		internal Tile[,] Tiles;
		//internal Texture2D[,] Textures;
		internal int Width; // in Pixels
		internal int Height;
		internal bool bFlipHorizontal;
		internal bool bFlipVertical;
	}

	internal static class PaintToolsEx
	{
		internal static string CSText(string key, string category = "PaintTools") => CheatSheet.CSText(category, key);
		internal static string importPath = Path.Combine(Main.SavePath, "CheatSheet_PaintTools.txt");
		internal static string exportPath = Path.Combine(Main.SavePath);

		private static Uri schematicsurl = new Uri("http://javid.ddns.net/tModLoader/jopojellymods/CheatSheet_Schematics_GetList.php");
		private static bool waiting = false;

		internal static void GetSchematicsComplete(object sender, UploadValuesCompletedEventArgs e)
		{
			if (!e.Cancelled)
			{
				string response = Encoding.UTF8.GetString(e.Result);
				JObject jsonObject = new JObject();
				try
				{
					jsonObject = JObject.Parse(response);
				}
				catch (Exception ex)
				{
					Main.NewText("Bad JSON: " + response);
				}
				string message = (string)jsonObject["message"];
				if (message != null)
				{
					Main.NewText(message);
				}
				JArray schematicslist = (JArray)jsonObject["schematics"];
				if (schematicslist != null)
				{
					List<PaintToolsSlot> list = new List<PaintToolsSlot>();
					foreach (JObject schematic in schematicslist.Children<JObject>())
					{
						int id = (int)schematic["id"];
						string name = (string)schematic["name"];
						int rating = (int)schematic["rating"];
						int vote = (int)schematic["vote"];
						string tiledata = (string)schematic["tiledata"];
						try
						{
							Tile[,] tiles = LoadTilesFromBase64(tiledata);
							if (tiles.GetLength(0) > 0)
							{
								var paintToolsSlot = new PaintToolsSlot(GetStampInfo(tiles));
								paintToolsSlot.browserID = id;
								paintToolsSlot.browserName = name;
								paintToolsSlot.rating = rating;
								paintToolsSlot.vote = vote;
								list.Add(paintToolsSlot);
							}
						}
						catch { }
					}
					if (list.Count > 0)
						CheatSheet.instance.paintToolsUI.view.Add(list.ToArray());
				}
			}
			else
			{
				Main.NewText("Schematics Server problem 2");
			}
			waiting = false;
		}

		internal static void OnlineImport(PaintToolsView paintToolsView)
		{
			if (waiting)
			{
				Main.NewText("Be patient");
				return;
			}
			if (CheatSheet.instance.paintToolsUI.view.slotList.Any(x => x.browserID > 0))
			{
				Main.NewText("You've already loaded the database");
				return;
			}
			waiting = true;
			try
			{
				using (WebClient client = new WebClient())
				{
					var steamIDMethodInfo = typeof(Main).Assembly.GetType("Terraria.ModLoader.ModLoader").GetProperty("SteamID64", BindingFlags.Static | BindingFlags.NonPublic);
					string steamid64 = (string)steamIDMethodInfo.GetValue(null, null);
					var values = new NameValueCollection
					{
						{ "version", CheatSheet.instance.Version.ToString() },
						{ "steamid64", steamid64 },
					};
					client.UploadValuesCompleted += new UploadValuesCompletedEventHandler(GetSchematicsComplete);
					client.UploadValuesAsync(schematicsurl, "POST", values);
				}
			}
			catch
			{
				Main.NewText("Schematics Server problem 1");
				waiting = false;
			}
		}

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

		static MethodInfo LoadTilesMethodInfo;
		static MethodInfo LoadWorldTilesVanillaMethodInfo;

		public static Tile[,] LoadTilesFromBase64(string data)
		{
			int oldX = Main.maxTilesX;
			int oldY = Main.maxTilesY;
			Tile[,] oldTiles = Main.tile;
			Tile[,] loadedTiles = new Tile[0, 0];
			try
			{
				TagCompound tagCompound = TagIO.FromStream(new MemoryStream(Convert.FromBase64String(data)));
				if (LoadTilesMethodInfo == null)
					LoadTilesMethodInfo = typeof(Main).Assembly.GetType("Terraria.ModLoader.IO.TileIO").GetMethod("LoadTiles", BindingFlags.Static | BindingFlags.NonPublic);
				if (LoadWorldTilesVanillaMethodInfo == null)
					LoadWorldTilesVanillaMethodInfo = typeof(Main).Assembly.GetType("Terraria.IO.WorldFile").GetMethod("LoadWorldTiles", BindingFlags.Static | BindingFlags.NonPublic);
				bool[] importance = new bool[TileID.Count];
				for (int i = 0; i < TileID.Count; i++)
					importance[i] = Main.tileFrameImportant[i];

				Point16 dimensions = tagCompound.Get<Point16>("d");
				Main.maxTilesX = dimensions.X;
				Main.maxTilesY = dimensions.Y;
				loadedTiles = new Tile[Main.maxTilesX, Main.maxTilesY];
				for (int i = 0; i < Main.maxTilesX; i++)
					for (int j = 0; j < Main.maxTilesY; j++)
						loadedTiles[i, j] = new Tile();
				Main.tile = loadedTiles;

				using (MemoryStream memoryStream = new MemoryStream(tagCompound.GetByteArray("v")))
				{
					using (BinaryReader binaryReader = new BinaryReader(memoryStream))
					{
						LoadWorldTilesVanillaMethodInfo.Invoke(null, new object[] { binaryReader, importance });
					}
				}

				if (tagCompound.ContainsKey("m"))
				{
					LoadTilesMethodInfo.Invoke(null, new object[] { tagCompound["m"] });
				}

				// Expand because TileFrame ignores edges of map.
				Main.maxTilesX = dimensions.X + 12;
				Main.maxTilesY = dimensions.Y + 12;
				Tile[,] loadedTilesExpanded = new Tile[Main.maxTilesX, Main.maxTilesY];
				for (int i = 0; i < Main.maxTilesX; i++)
					for (int j = 0; j < Main.maxTilesY; j++)
						if (i < 6 || i >= Main.maxTilesX - 6 || j < 6 || j >= Main.maxTilesY - 6)
							loadedTilesExpanded[i, j] = new Tile();
						else
							loadedTilesExpanded[i, j] = Main.tile[i - 6, j - 6];
				Main.tile = loadedTilesExpanded;

				for (int i = 0; i < Main.maxTilesX; i++)
				{
					for (int j = 0; j < Main.maxTilesY; j++)
					{
						//WorldGen.TileFrame(i, j, true, false);

						//if (i > 5 && j > 5 && i < Main.maxTilesX - 5 && j < Main.maxTilesY - 5
						// 0 needs to be 6 ,   MaxX == 5, 4 index, 
						// need tp add 6?       4(10) < 5(11) - 5

						if (Main.tile[i, j].active())
						{
							WorldGen.TileFrame(i, j, true, false);
						}
						if (Main.tile[i, j].wall > 0)
						{
							Framing.WallFrame(i, j, true);
						}
					}
				}
			}
			catch { }
			Main.maxTilesX = oldX;
			Main.maxTilesY = oldY;
			Main.tile = oldTiles;
			return loadedTiles;
		}

		static MethodInfo SaveTilesMethodInfo;
		static MethodInfo SaveWorldTilesVanillaMethodInfo;

		public static string SaveTilesToBase64(Tile[,] tiles)
		{
			int oldX = Main.maxTilesX;
			int oldY = Main.maxTilesY;
			Tile[,] oldTiles = Main.tile;
			string base64result = "";
			try
			{
				Main.maxTilesX = tiles.GetLength(0);
				Main.maxTilesY = tiles.GetLength(1);
				Main.tile = tiles;
				if (SaveTilesMethodInfo == null)
					SaveTilesMethodInfo = typeof(Main).Assembly.GetType("Terraria.ModLoader.IO.TileIO").GetMethod("SaveTiles", BindingFlags.Static | BindingFlags.NonPublic);
				if (SaveWorldTilesVanillaMethodInfo == null)
					SaveWorldTilesVanillaMethodInfo = typeof(Main).Assembly.GetType("Terraria.IO.WorldFile").GetMethod("SaveWorldTiles", BindingFlags.Static | BindingFlags.NonPublic);

				TagCompound ModTileData = (TagCompound)SaveTilesMethodInfo.Invoke(null, null);

				byte[] array = null;
				using (MemoryStream memoryStream = new MemoryStream(7000000))
				{
					using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
					{
						int rval = (int)SaveWorldTilesVanillaMethodInfo.Invoke(null, new object[] { binaryWriter });
						array = memoryStream.ToArray();
					}
				}

				TagCompound result = new TagCompound()
				{
					["d"] = new Point16(Main.maxTilesX, Main.maxTilesY),
					["v"] = array,
					["m"] = ModTileData,
				};
				MemoryStream ms = new MemoryStream();
				TagIO.ToStream(result, ms, true);
				base64result = Convert.ToBase64String(ms.ToArray());
			}
			catch { }
			Main.maxTilesX = oldX;
			Main.maxTilesY = oldY;
			Main.tile = oldTiles;
			return base64result;
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
				Main.NewText(CSText("DataExport") + importPath);
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

			//result.Textures = new Texture2D[texMaxX, texMaxY];
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
					//result.Textures[x, y] = TilesToTexture(tiles);
				}
			}

			return result;
		}

		/*
		private static Rectangle GetTileRect(Tile tile, int halfIndex = 0, Texture2D texture = null)
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

			if(texture != null)
			{
				if (result.Y > texture.Height)
				{
					result = new Rectangle(0, 0, 16, 16);
				}
			}

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

								Rectangle rect = GetTileRect(tile, texture:textureTile);

								if (textureTile.Width < rect.X + rect.Width)
								{
									int width2 = textureTile.Width - rect.X;
									if (width2 <= 0)
										continue;
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
								if(waterSize > 0)
									textureWater.GetData<Color>( 0, new Rectangle(0, 16 - waterSize, 16, waterSize), dataWater, (16 - waterSize) * 16, 256 - (16 - waterSize) * 16);
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
		*/

		public static bool canDrawColorTile(Tile tile)
		{
			return tile != null && tile.color() > 0 && (int)tile.color() < Main.numTileColors && Main.tileAltTextureDrawn[(int)tile.type, (int)tile.color()] && Main.tileAltTextureInit[(int)tile.type, (int)tile.color()];
		}
		public static bool canDrawColorWall(Tile tile)
		{
			return tile != null && tile.wallColor() > 0 && Main.wallAltTextureDrawn[tile.wall, tile.wallColor()] && Main.wallAltTextureInit[tile.wall, tile.wallColor()];
		}

		/*
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
		*/
	}
}