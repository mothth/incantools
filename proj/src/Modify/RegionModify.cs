using System.Collections.Generic;
using RWCustom;
using UnityEngine;
using EffExt;

namespace IncanTools;

// Modification to stuff from incan region parameters
internal static class RegionModify
{
	public static void InitHooks()
	{
		On.Music.MusicPlayer.RequestSSSong += RequestSSSong;
		On.SuperStructureFuses.ctor += SSFuses_ctor;
		On.SSLightRod.ctor += SSLightRod_ctor;
	}

	private static void SSFuses_ctor(On.SuperStructureFuses.orig_ctor orig, SuperStructureFuses self, PlacedObject placedObject, IntRect rect, Room room) {
		orig(self, placedObject, rect, room);
		if (IncanMod.currentRegion != null && IncanMod.currentRegion.broken != -1f)
		{
			self.broken = IncanMod.currentRegion.broken;
		}
	}

	private static void SSLightRod_ctor(On.SSLightRod.orig_ctor orig, SSLightRod self, PlacedObject placedObject, Room room) {
		if (IncanMod.currentRegion != null && IncanMod.currentRegion.overrideLightrods) {
			self.room = room;
			self.placedObject = placedObject;
			self.color = IncanMod.currentRegion.lightrodColor;
			self.lights = new List<SSLightRod.LightVessel>();
			self.UpdateLightAmount();
		}
		else orig(self, placedObject, room);
	}

	private static void RequestSSSong(On.Music.MusicPlayer.orig_RequestSSSong orig, Music.MusicPlayer self) {
		if (IncanMod.currentRegion != null && IncanMod.currentRegion.overrideSSMusic != null && (self.song == null || !(self.song is Music.SSSong)) && (self.nextSong == null || !(self.nextSong is Music.SSSong)) && self.manager.rainWorld.setup.playMusic)
		{
			Music.Song song = new Music.SSSong(self, IncanMod.currentRegion.overrideSSMusic);
			if (self.song == null)
			{
				self.song = song;
				self.song.playWhenReady = true;
			}
			else
			{
				self.nextSong = song;
				self.nextSong.playWhenReady = false;
			}
		}
		else orig(self);
	}
}