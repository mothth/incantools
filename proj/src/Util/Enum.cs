using System.Collections.Generic;
using System.Globalization;
using MoreSlugcats;
using RWCustom;
using UnityEngine;

namespace IncanTools;

public static class IncanEnum
{
	public class Effect
	{
		public static RoomSettings.RoomEffect.Type PaletteSettings;

		internal static void SetValues()
		{
			PaletteSettings = new RoomSettings.RoomEffect.Type(EffectManager.GetName("PaletteSettings"));
		}
	};

	internal static void RegisterValues()
	{
		Effect.SetValues();
	}

	internal static void UnregisterValues()
	{
		
	}
}