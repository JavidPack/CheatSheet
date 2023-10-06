﻿using System;
using Terraria;

namespace CheatSheet
{
	public readonly record struct TileData(TileTypeData TileTypeData, WallTypeData WallTypeData, TileWallWireStateData TileWallWireStateData, LiquidData LiquidData, TileWallBrightnessInvisibilityData TileWallBrightnessInvisibilityData)
	//public record struct TileData(TileTypeData TileTypeData, WallTypeData WallTypeData, TileWallWireStateData TileWallWireStateData, LiquidData LiquidData)
	{
		public TileData(Tile tile) : this(tile.Get<TileTypeData>(), tile.Get<WallTypeData>(), tile.Get<TileWallWireStateData>(), tile.Get<LiquidData>(), tile.Get<TileWallBrightnessInvisibilityData>())
		{
			//TileTypeData = tile.Get<TileTypeData>();
			//WallTypeData = tile.Get<WallTypeData>();
			//TileWallWireStateData = tile.Get<TileWallWireStateData>();
			//LiquidData = tile.Get<LiquidData>();
		}

		internal void CopyToTile(Tile tile)
		{
			tile.Get<TileTypeData>() = TileTypeData;
			tile.Get<WallTypeData>() = WallTypeData;
			tile.Get<TileWallWireStateData>() = TileWallWireStateData;
			tile.Get<LiquidData>() = LiquidData;
			tile.Get<TileWallBrightnessInvisibilityData>() = TileWallBrightnessInvisibilityData;
		}
	}
}
