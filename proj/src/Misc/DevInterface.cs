using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security;
using System.Security.Permissions;
using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine;
using RWCustom;
using BepInEx;
using Debug = UnityEngine.Debug;
using EffExt;
using DevInterface;
using IL;
using On;
using RewiredConsts;

namespace IncanTools
{
	internal static class IncanDev {
		public static void Init()
		{
			On.DevInterface.SSLightRodRepresentation.ctor += SSLightRodRepresentation_ctor;
			On.DevInterface.SoundPage.Update += SoundPageUpdate;
		}

		public static void Update(RainWorldGame game) {
			// Reload Palettes
			if (Input.GetKeyDown("t")) {
				IncanMod.currentRegion.ReadProperties();

				if (IncanMod.roomCamera.paletteBank != null)
				{
					IncanMod.roomCamera.paletteBank.effectColorsTexture = null;
					IncanMod.roomCamera.paletteBank.LoadEffectPalette();
				}

				RoomCamera camera = game.cameras[0];
				camera.LoadPalette(camera.paletteA, ref camera.fadeTexA);
				if (camera.paletteB != -1) {
					camera.LoadPalette(camera.paletteB, ref camera.fadeTexB);
				}
				camera.terrainPalette?.Reload();
				camera.ApplyFade();
			}
			
			// Refresh objects (WIP)
			if (Input.GetKeyDown("y")) {
				IncanMod.currentRegion.ReadProperties();
				RefreshObjects(game.cameras[0].room);
			}

			// Increase Karma / Ripple
			if (Input.GetKeyDown(KeyCode.Slash))
			{
				Player player = game.RealizedPlayerOfPlayerNumber(0);
				if (player != null)
				{
					DeathPersistentSaveData saveData = (game.session as StoryGameSession).saveState.deathPersistentSaveData;

					// Karma
					if (!ModManager.Watcher || saveData.maximumRippleLevel == 0.0)
					{
						if (saveData.karma == saveData.karmaCap)
						{
							if (saveData.karmaCap == 4)
							{
								saveData.karmaCap = 6;
								saveData.karma++;
							}
							else if (saveData.karmaCap < 9)
							{
								saveData.karmaCap++;
								saveData.karma++;
							}
							else
							{
								saveData.karma = 0;
							}
						}
						else
						{
							saveData.karma++;
						}
					}
					// Ripple
					else 
					{
						if (saveData.rippleLevel < 5f) {
							saveData.rippleLevel += 0.5f;
							if (saveData.maximumRippleLevel < saveData.rippleLevel)
							{
								saveData.maximumRippleLevel = saveData.rippleLevel;
							}
						}
						else
						{
							saveData.rippleLevel = saveData.minimumRippleLevel;
						}
					}

					for (int num2 = 0; num2 < game.cameras.Length; num2++) {
						if (game.cameras[num2].hud.karmaMeter != null)
						{
							game.cameras[num2].hud.karmaMeter.UpdateGraphic();
						}
					}
				}
			}
		}

		// No threat drone vol slider
		private static void SoundPageUpdate(On.DevInterface.SoundPage.orig_Update orig, SoundPage self)
		{
			orig(self);
			if (self.owner.room.roomSettings.BkgDroneNoThreatVolume < 1f && self.owner.game.manager.musicPlayer != null && self.owner.game.manager.musicPlayer.threatTracker != null)
			{
				self.owner.game.manager.musicPlayer.threatTracker.currentThreat *= self.owner.room.roomSettings.BkgDroneNoThreatVolume;
			}
		}

		// TODO: Make this not bad
		private static void RefreshObjects(Room room) {
			for (int i = 0; i < room.updateList.Count; i++)
			{
				if (room.updateList[i] is LightFixture) {
					room.updateList[i].Destroy();
				}
				else if (room.updateList[i] is SuperStructureFuses) {
					room.updateList[i].Destroy();
				}
				else if (room.updateList[i] is SSLightRod) {
					room.updateList[i].Destroy();
				}
				else if (room.updateList[i] is LanternStick) {
					(room.updateList[i] as LanternStick).lantern.Destroy();
					room.updateList[i].Destroy();
				}
			}
			
			for (int i = 0; i < room.roomSettings.placedObjects.Count; i++) {
				PlacedObject pObj = room.roomSettings.placedObjects[i];
				if (pObj.type == PlacedObject.Type.LightFixture) {
					RefreshLightFixture(room, pObj);
				}
				else if (pObj.type == PlacedObject.Type.SuperStructureFuses) {
					room.AddObject(new SuperStructureFuses(pObj, (pObj.data as PlacedObject.GridRectObjectData).Rect, room));
				}
				else if (pObj.type == PlacedObject.Type.SSLightRod) {
					room.AddObject(new SSLightRod(pObj, room));
				}
				else if (pObj.type == PlacedObject.Type.LanternOnStick) {
					room.AddObject(new LanternStick(room, pObj));
				}
			}

			IncanMod.roomCamera.RefreshEffects(room);
			room.game.cameras[0].ApplyPalette();
		}

		private static void RefreshLightFixture(Room room, PlacedObject pObj) {
			if ((pObj.data as PlacedObject.LightFixtureData).type == PlacedObject.LightFixtureData.Type.RedLight)
			{
				room.AddObject(new Redlight(room, pObj, pObj.data as PlacedObject.LightFixtureData));
			}
			else if ((pObj.data as PlacedObject.LightFixtureData).type == PlacedObject.LightFixtureData.Type.HolyFire)
			{
				room.AddObject(new HolyFire(room, pObj, pObj.data as PlacedObject.LightFixtureData));
			}
			else if ((pObj.data as PlacedObject.LightFixtureData).type == PlacedObject.LightFixtureData.Type.ZapCoilLight)
			{
				room.AddObject(new ZapCoilLight(room, pObj, pObj.data as PlacedObject.LightFixtureData));
			}
			else if ((pObj.data as PlacedObject.LightFixtureData).type == PlacedObject.LightFixtureData.Type.DeepProcessing)
			{
				room.AddObject(new DeepProcessingLight(room, pObj, pObj.data as PlacedObject.LightFixtureData));
			}
			else if ((pObj.data as PlacedObject.LightFixtureData).type == PlacedObject.LightFixtureData.Type.SlimeMoldLight)
			{
				room.AddObject(new SlimeMoldLight(room, pObj, pObj.data as PlacedObject.LightFixtureData));
			}
			else if ((pObj.data as PlacedObject.LightFixtureData).type == PlacedObject.LightFixtureData.Type.RedSubmersible)
			{
				room.AddObject(new Redlight(room, pObj, pObj.data as PlacedObject.LightFixtureData, submersible: true));
			}
			else if ((pObj.data as PlacedObject.LightFixtureData).type == PlacedObject.LightFixtureData.Type.GlowWeedLight)
			{
				room.AddObject(new GlowWeedLight(room, pObj, pObj.data as PlacedObject.LightFixtureData));
			}
		}

		// Fix duplicating light rods
		private static void SSLightRodRepresentation_ctor(On.DevInterface.SSLightRodRepresentation.orig_ctor orig, SSLightRodRepresentation self, DevUI owner, string IDstring, DevUINode parentNode, PlacedObject pObj, string name) {
			for (int i = 0; i < owner.room.drawableObjects.Count; i++)
			{
				if (owner.room.drawableObjects[i] is SSLightRod && (owner.room.drawableObjects[i] as SSLightRod).placedObject == pObj)
				{
					self.rod = owner.room.drawableObjects[i] as SSLightRod;
					break;
				}
			}
			orig(self, owner, IDstring, parentNode, pObj, name);
		}
	}
}