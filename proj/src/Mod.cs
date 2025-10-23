﻿using System;
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
using BepInEx.Logging;
using EffExt;


namespace IncanTools;

public static class IncanMod
{
    public static IncanOverworld overworld;
    public static IncanRoomCamera roomCamera;
    public static IncanRegion currentRegion => overworld.currentRegion;

    ////

    private static void RegisterEnums() {
        // Effects should be registered before enums because IncanEnum relies on that
        EffectManager.RegisterEffects();
        IncanEnum.RegisterValues();
    }

    private static void InitHooks() {
        IncanOverworld.InitHooks();
        RegionModify.InitHooks();
        WatcherModify.InitHooks();
        IncanRoom.InitHooks();
        IncanRoomCamera.InitHooks();

        On.RainWorldGame.RawUpdate += RawUpdate;
        On.OverWorld.LoadFirstWorld += OverWorld_LoadFirstWorld;
        On.RoomCamera.ctor += RoomCamera_ctor;
    }

    // On mod enabled
    internal static void OnEnable() {
        On.RainWorld.LoadModResources += LoadResources;
    }

    // On mod initialisation
    internal static void OnInit()
    {
        IncanLogging.Init();

        RegisterEnums();
        InitHooks();
        

        if (ModManager.DevTools)
        {
            IncanDev.Init();
        }

        
    }

    ////

    private static void LoadResources(On.RainWorld.orig_LoadModResources orig, RainWorld self)
    {
        orig(self);
        PaletteManager.LoadPaletteBanks();
    }

    private static void RawUpdate(On.RainWorldGame.orig_RawUpdate orig, RainWorldGame self, float dt)
    {
        orig(self, dt);
        if (self.devToolsActive)
        {
            IncanDev.Update(self);
        }
    }
    
    // Create IncanOverworld
    private static void OverWorld_LoadFirstWorld(On.OverWorld.orig_LoadFirstWorld orig, OverWorld self)
    {
        overworld = new IncanOverworld(self);
        orig(self);
    }

    // Create RoomCamera
    private static void RoomCamera_ctor(On.RoomCamera.orig_ctor orig, RoomCamera self, RainWorldGame game, int cameraNumber)
    {
        roomCamera = new IncanRoomCamera(self);
        orig(self, game, cameraNumber);
    }
}