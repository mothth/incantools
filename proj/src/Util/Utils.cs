using System.IO;
using UnityEngine;

namespace IncanTools;

public static class IncanUtils
{
	public static bool TryLoadTexture(string path, ref Texture2D texture)
	{
		try
		{
			AssetManager.SafeWWWLoadTexture(ref texture, "file:///" + path, clampWrapMode: false, crispPixels: true);
			return true;
		}
		catch (FileLoadException)
		{
			return false;
		}
	}
	
	public static Color Color32ToColor(Color32 color)
	{
		return new Color(color.r / 255f, color.g / 255f, color.b / 255f, color.a / 255f);
	}

	public static void CleanCurrentRegionOfRot()
	{
		World world = IncanMod.overworld.incanWorld.world;
		world.regionState.sentientRotProgression.Clear();
		world.game.GetStorySession.saveState.miscWorldSaveData.regionsInfectedBySentientRot.Remove(world.name.ToLowerInvariant());
	}
}