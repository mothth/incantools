using System.Collections.Generic;
using RWCustom;
using UnityEngine;
using EffExt;

namespace IncanTools;

// Room proxy
public class IncanRoom {
	public Room room;

	public EffectExtraData paletteSettings;

	private static Dictionary<Room, IncanRoom> incanRooms;
	
	public IncanRoom(Room room) {
		this.room = room;
	}

	////

	public static bool TryGet(Room room, out IncanRoom incanRoom) => incanRooms.TryGetValue(room, out incanRoom);

	// Called on room constructor, so best not to use this yourself
	public static IncanRoom Add(Room room) {
		if (room != null)
		{
			IncanRoom incanRoom = new(room);
			incanRooms.Add(room, incanRoom);
			return incanRoom;
		}
		else return null;
	}

	public static void Remove(Room room)
	{
		if (TryGet(room, out IncanRoom incanRoom))
		{
			incanRoom.room = null;
			incanRooms.Remove(room);
		}
	}

	////

	internal static void InitHooks()
	{
		On.Room.ctor += NewRoom;
		On.AbstractRoom.Abstractize += AbstractizeRoom;
		incanRooms = new Dictionary<Room, IncanRoom>(); // Not a hook but initialising this here anyway
	}

	private static void NewRoom(On.Room.orig_ctor orig, Room self, RainWorldGame game, World world, AbstractRoom abstractRoom, bool devUI = false)
	{
		Add(self);
		orig(self, game, world, abstractRoom, devUI);
	}

	private static void AbstractizeRoom(On.AbstractRoom.orig_Abstractize orig, AbstractRoom self)
	{
		Remove(self.realizedRoom);
		orig(self);
	}
}