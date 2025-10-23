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

//public delegate void PropertiesHook(IncanRegion incanRegion, string[] array);

public class IncanRegion {
	public const string PROPERTIES = "incan_properties.txt";

	public Region region;
	public UserParams userParams = new();

	// Built-in properties
	public string overrideSSMusic = null;
	public bool overrideLightrods = false;
	public bool oneWayWarp = false;
	public bool rotImmune = false;
	public Color lightrodColor; 
	public float broken = -1.0f;

	public IncanRegion(Region region)
	{
		this.region = region;
		ReadProperties();
	}

	public void ReadProperties() {	
		string path = AssetManager.ResolveFilePath("world" + Path.DirectorySeparatorChar + region.name + Path.DirectorySeparatorChar + PROPERTIES);
		if (!File.Exists(path)) {
			return;
		}

		// Read lines
		string[] array = File.ReadAllLines(path);
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split(':');
			if (array2.Length >= 2)
			{
				userParams.Add(array2[0], array2[1].Trim());
			}
		}

		// Built-in properties
		oneWayWarp = userParams.GetFlag("oneWayWarp");
		rotImmune = userParams.GetFlag("rotImmunity");
		overrideLightrods = userParams.TryGetColor("lightrodColor", out lightrodColor);
		overrideSSMusic = userParams.GetString("overrideSSMusic");
		if (!userParams.TryGetFloat("SSBroken", out broken))
		{
			broken = -1f;
		}
	}
}