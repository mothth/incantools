using System.IO;
using System.Collections.Generic;
using RWCustom;
using UnityEngine;
using EffExt;

namespace IncanTools;

public class IncanRoomCamera {
	public string last_bank = "";	// Name of the last palette bank. Used for devtools convenience, see Effects.cs.
	public PaletteBank paletteBank = null;
	public bool customFade = false;
	public bool customEffectA = false;
	public bool customEffectB = false;
	public IncanRoom incanRoom;
	public RoomCamera roomCamera;

	// private FSprite fullscreen2;
	// private float fullscreen2_alpha;
	// private RoomSettings.RoomEffect.Type fullscreen2_alpha_effect;

	public IncanRoomCamera(RoomCamera roomCamera)
	{
		this.roomCamera = roomCamera;
	}

	public void RefreshEffects(Room room)
	{
		IncanRoom.TryGet(room, out incanRoom);
		paletteBank = null;
		ResetCustomPalette();
	}

	public void ResetCustomPalette()
	{
		string bank_name = (incanRoom.paletteSettings != null) ? incanRoom.paletteSettings.GetString("bank") : null;

		if (last_bank != bank_name) {
			// Force reloading of palettes
			roomCamera.paletteA = -1;
			roomCamera.paletteB = -1;
		}

		if (bank_name != null)
		{
			PaletteManager.TryGetBank(bank_name, out paletteBank);
			customFade = incanRoom.paletteSettings.GetBool("use_fade");
			customEffectA = incanRoom.paletteSettings.GetBool("effect_a");
			customEffectB = incanRoom.paletteSettings.GetBool("effect_b");
		}
		
		last_bank = bank_name;
	}

	public void ChangeRoom(Room newRoom, int cameraPosition)
	{
		RefreshEffects(newRoom);
	}

	public bool LoadCustomPalette(int pal, ref Texture2D texture)
	{
		// TODO: Disabling custom fade doesnt work the first time loading a palette for the room!!!!!!
		// An Incan from the future here - wdym? You wrote this so long ago and none of us can remember.
		if (paletteBank == null || (!customFade && ReferenceEquals(texture, roomCamera.fadeTexB)))
		{
			return false;
		}

		if (texture != null)
		{
			Object.Destroy(texture);
		}

		texture = new Texture2D(32, 16, TextureFormat.ARGB32, mipChain: false);
		string path = AssetManager.ResolveFilePath(paletteBank.path + Path.DirectorySeparatorChar + "palette" + pal + ".png");
		if (!IncanUtils.TryLoadTexture(path, ref texture))
		{
			return false;
		}

		if (roomCamera.room != null)
		{
			roomCamera.ApplyEffectColorsToPaletteTexture(ref texture, roomCamera.room.roomSettings.EffectColorA, roomCamera.room.roomSettings.EffectColorB);
		}

		texture.Apply(updateMipmaps: false);
		return true;
	}

	public bool ApplyCustomEffectColors(ref Texture2D texture, int color1, int color2)
	{
		if (paletteBank == null || paletteBank.effectColorsTexture == null || (!customEffectA && !customEffectB))
		{
			return false;
		}

		if (color1 > -1)
		{
			Texture2D effect_texture = customEffectA ? paletteBank.effectColorsTexture : RoomCamera.allEffectColorsTexture;
			texture.SetPixels(30, 4, 2, 2, effect_texture.GetPixels(color1 * 2, 0, 2, 2, 0), 0);
			texture.SetPixels(30, 12, 2, 2, effect_texture.GetPixels(color1 * 2, 2, 2, 2, 0), 0);
		}

		if (color2 > -1)
		{
			Texture2D effect_texture = customEffectB ?  paletteBank.effectColorsTexture : RoomCamera.allEffectColorsTexture;
			texture.SetPixels(30, 2, 2, 2, effect_texture.GetPixels(color2 * 2, 0, 2, 2, 0), 0);
			texture.SetPixels(30, 10, 2, 2, effect_texture.GetPixels(color2 * 2, 2, 2, 2, 0), 0);
		}

		return true;
	}

	#region Hooks

	internal static void InitHooks() {
		On.RoomCamera.ChangeRoom += RoomCameraChangeRoom;
		On.RoomCamera.LoadPalette += RoomLoadPalette;
		On.RoomCamera.ApplyEffectColorsToPaletteTexture += ApplyEffectColorsToPaletteTexture;
	}

	private static void RoomCameraChangeRoom(On.RoomCamera.orig_ChangeRoom orig, RoomCamera self, Room newRoom, int cameraPosition)
	{
		IncanMod.roomCamera.ChangeRoom(newRoom, cameraPosition);
		orig(self, newRoom, cameraPosition);
	}

	private static void RoomLoadPalette(On.RoomCamera.orig_LoadPalette orig, RoomCamera self, int pal, ref Texture2D texture)
	{
		if (!IncanMod.roomCamera.LoadCustomPalette(pal, ref texture))
		{
			orig(self, pal, ref texture);
		}
	}

	private static void ApplyEffectColorsToPaletteTexture(On.RoomCamera.orig_ApplyEffectColorsToPaletteTexture orig, RoomCamera self, ref Texture2D texture, int color1, int color2)
	{
		if (!IncanMod.roomCamera.ApplyCustomEffectColors(ref texture, color1, color2))
		{
			orig(self, ref texture, color1, color2);
		}
	}

	#endregion
}

// Old effect stacking functionality we may reimplement some day

/*
private void SetUpFullscreen2(string container, RoomCamera camera) {
	if (fullscreen2 == null)
	{
		fullscreen2 = new FSprite("Futile_White");
		fullscreen2.scaleX = owner.game.rainWorld.options.ScreenSize.x / 16f;
		fullscreen2.scaleY = 48f;
		fullscreen2.anchorX = 0f;
		fullscreen2.anchorY = 0f;
	}
	fullscreen2.RemoveFromContainer();
	camera.ReturnFContainer(container).AddChild(fullscreen2);
}

private void CameraApplyPalette(On.RoomCamera.orig_ApplyPalette orig, RoomCamera self) {
	orig(self);
	
	if (self.room == null) {
		return;
	}

	if (melt > 0.0f) {
		SetUpFullscreen2("Bloom", self);
		fullscreen2.shader = owner.assets.shaders["SimMelt"];
		fullscreen2.color = new Color(1f, 0f, 0f);
		fullscreen2_alpha = Mathf.Pow(melt, 0.5f) * 0.8f;
		fullscreen2_alpha_effect = IncanEnum.CustomEffect.Asp_Melt;
		fullscreen2.alpha = fullscreen2_alpha;
	}
	else if (bloom > 0f)
	{
		SetUpFullscreen2("Bloom", self);
		fullscreen2.shader = owner.game.rainWorld.Shaders["Bloom"];
		fullscreen2_alpha_effect = IncanEnum.CustomEffect.Asp_Bloom;
		fullscreen2_alpha = bloom;
	}
	else if (fullscreen2 != null) {
		fullscreen2.RemoveFromContainer();
		fullscreen2 = null;
	}
}

private void CameraDrawUpdate(On.RoomCamera.orig_DrawUpdate orig, RoomCamera self, float timeStacker, float timeSpeed) {
	orig(self, timeStacker, timeSpeed);

	float num4 = 1.0f;
	if (self.room.roomSettings.DangerType != RoomRain.DangerType.None)
	{
		num4 = self.room.world.rainCycle.ShaderLight;
	}

	if (fullscreen2 != null)
	{
		if (fullscreen2_alpha_effect != RoomSettings.RoomEffect.Type.None)
		{
			fullscreen2_alpha = self.room.roomSettings.GetEffectAmount(fullscreen2_alpha_effect);
		}
		if (fullscreen2_alpha > 0f && fullscreen2_alpha_effect == IncanEnum.CustomEffect.Asp_Bloom)
		{
			fullscreen2.alpha = fullscreen2_alpha * Mathf.InverseLerp(-0.7f, 0f, num4);
		}
		else
		{
			fullscreen2.alpha = fullscreen2_alpha;
		}
	}
}
*/