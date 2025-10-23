using System.IO;
using System.Collections.Generic;
using UnityEngine;
using RWCustom;

namespace IncanTools;

public class PaletteBank {
	public const string BANK_PREFIX = "bank-";

	public string name;
	public string path;
	public Texture2D effectColorsTexture = null;

	public PaletteBank(string name) {
		this.name = name;
		path = AssetManager.ResolveDirectory("palettes" + Path.DirectorySeparatorChar + BANK_PREFIX + name);
	}

	public void LoadEffectPalette() {
		string text = AssetManager.ResolveFilePath(path + Path.DirectorySeparatorChar + "effectColors.png");
		if (File.Exists(text)) {
			effectColorsTexture = new Texture2D(40, 4, TextureFormat.ARGB32, mipChain: false);
			IncanUtils.TryLoadTexture(text, ref effectColorsTexture);
		}
	}
}

internal static class PaletteManager
{
	public static Dictionary<string, PaletteBank> palette_banks = null;

	public static void TryGetBank(string name, out PaletteBank bank) {
		if (palette_banks != null)
		{
			palette_banks.TryGetValue(name, out bank);
		}
		else {
			bank = null;
		}
	}

	public static void LoadPaletteBanks()
	{
		if (palette_banks == null)
		{
			palette_banks = new Dictionary<string, PaletteBank>();
		}
		else if (palette_banks != null)
		{
			palette_banks.Clear();
		}

		string[] directories = AssetManager.ListDirectory("palettes", true, false, true);

		foreach (string directory in directories)
		{
			string name = Path.GetFileName(directory);
			if (!name.StartsWith(PaletteBank.BANK_PREFIX))
			{
				continue;
			}

			name = name.Remove(0, PaletteBank.BANK_PREFIX.Length);
			PaletteBank bank = new PaletteBank(name);
			bank.LoadEffectPalette();

			palette_banks.Add(name, bank);
		}
	}
}