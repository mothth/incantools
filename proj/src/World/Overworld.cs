using System;
using System.Runtime.InteropServices;
using System.IO;
using System.Collections.Generic;
using RWCustom;
using UnityEngine;
using DevInterface;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System.Text.RegularExpressions;
using System.Globalization;

namespace IncanTools;

public class IncanOverworld
{
	public OverWorld overworld;
	public IncanWorld incanWorld;
	public List<IncanRegion> regions;
	public Dictionary<string, int> regionIndices;

	public IncanRegion currentRegion => incanWorld.incanRegion;

	public IncanOverworld(OverWorld overworld)
	{
		regions = new();
		regionIndices = new();
		foreach (Region region in overworld.regions)
		{
			IncanRegion incanRegion = new IncanRegion(region);
			regionIndices.Add(region.name.ToLowerInvariant(), regions.Count);
			regions.Add(incanRegion);
		}
	}

	internal static void InitHooks()
	{
		On.World.ctor += World_ctor;
	}

	// Get region by name
	public IncanRegion GetIncanRegion(string name)
	{
		if (regionIndices.TryGetValue(name.ToLowerInvariant(), out int index))
		{
			return regions[index];
		}
		return null;
	}

	// Get region by index
	public IncanRegion GetIncanRegion(int index)
	{
		return (index >= 0 && index < regions.Count) ? regions[index] : null;
	}

	private static void World_ctor(On.World.orig_ctor orig, World self, RainWorldGame game, Region region, string name, bool singleRoomWorld) {
		IncanWorld incanWorld = new IncanWorld(IncanMod.overworld, self, region);
		IncanMod.overworld.incanWorld = incanWorld;
		orig(self, game, region, name, singleRoomWorld);
	}
}