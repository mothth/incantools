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

public class IncanWorld {
	public IncanOverworld overworld;
	public World world;
	public IncanRegion incanRegion;

	public IncanWorld(IncanOverworld overworld, World world, Region region) {
		this.world = world;
		if (region != null)
		{
			incanRegion = overworld.GetIncanRegion(region.name);
		}
	}
}