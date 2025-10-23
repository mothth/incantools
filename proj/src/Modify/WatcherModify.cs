// Hooks into watcher warp and rot infection behaviour
using System.Collections.Generic;
using RWCustom;
using UnityEngine;
using Watcher;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace IncanTools;

// Modification to Watcher stuff
internal static class WatcherModify
{
	public static void InitHooks()
	{
		On.Region.HasSentientRotResistance += RegionHasSentientRotResistance;
		//IL.Player.SpawnDynamicWarpPoint += PlayerSpawnDynamicWarpPoint;
		//On.Watcher.WarpPoint.CreateOverrideData += WarpPointCreateOverrideData;
	}

	private static bool RegionHasSentientRotResistance(On.Region.orig_HasSentientRotResistance orig, string name)
	{
		IncanRegion incanRegion = IncanMod.overworld.GetIncanRegion(name);
		return (incanRegion != null) ? incanRegion.rotImmune : orig(name);
	}

	/*
	private static void WarpPointCreateOverrideData(On.Watcher.WarpPoint.orig_CreateOverrideData orig, World world, string oldRoom, string chosenRoom, Vector2? chosenDestPosition, bool limitedUse, bool playerCreated)
	{
		if (IncanMod.overworld.incanWorld.incanRegion.oneWayWarp)
		{
			limitedUse = true;
		}
		orig(world, oldRoom, chosenRoom, chosenDestPosition, limitedUse, playerCreated);
	}
	*/

	// Not bothered to fix this rn
	private static void PlayerSpawnDynamicWarpPoint(ILContext il)
	{
		ILCursor c = new ILCursor(il);
		if (c.TryGotoNext(MoveType.After,
			// PlacedObject placedObject = new PlacedObject(PlacedObject.Type.WarpPoint, WarpPoint.CreateOverrideData(room.abstractRoom, text, flag2 <= insert here, playerCreated: true));
			x => x.MatchLdsfld(typeof(PlacedObject.Type), "WarpPoint"),
			x => x.MatchLdarg(0),
			x => x.MatchLdfld(typeof(UpdatableAndDeletable), "room"),
			x => x.MatchCallvirt(typeof(Room), "get_abstractRoom"),
			x => x.MatchLdloc(8),
			x => x.MatchLdloc(1)))
		{
			// Unsafe but oh well, there *should* be a incanWorld if there is a world
			c.Emit(OpCodes.Ldarg_0);
			c.Emit(OpCodes.Ldfld, typeof(IncanMod).GetField("overworld"));
			c.Emit(OpCodes.Ldfld, typeof(IncanOverworld).GetField("incanWorld"));
			c.Emit(OpCodes.Ldfld, typeof(IncanWorld).GetField("incanRegion"));
			c.Emit(OpCodes.Ldfld, typeof(IncanRegion).GetField("oneWayWarp"));
			c.Emit(OpCodes.Or);
		}
		else
		{
			IncanLogging.LogError("Failed to IL Hook Player.SpawnDynamicWarpPoint!! Oh no!!");
		}
	}
}