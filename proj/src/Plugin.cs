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

#pragma warning disable CS0618
[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618

namespace IncanTools;

[BepInPlugin(GUID, "IncanTools", VERSION)]
internal class IncanToolsPlugin : BaseUnityPlugin
{
    public const string VERSION = "1.0.0";
    public const string GUID = "of-incandescence.incantools";
    private bool IsInit = false;

    private void OnEnable()
    {
        On.RainWorld.OnModsInit += OnModsInit;
        IncanMod.OnEnable();
    }

    private void OnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self)
    {
        orig(self);
        try
        {
            if (IsInit) return;
            IncanMod.OnInit();
            IsInit = true;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex);
            throw;
        }
    }

    /*
    private void OnModsEnabled(On.RainWorld.orig_OnModsEnabled orig, RainWorld self, ModManager.Mod[] newlyEnabledMods)
    {
        orig(self, newlyEnabledMods);
        IncanEnum.RegisterValues();
    }

    private void OnModsDisabled(On.RainWorld.orig_OnModsDisabled orig, RainWorld self, ModManager.Mod[] newlyDisabledMods)
    {
        orig(self, newlyDisabledMods);
        IncanEnum.UnregisterValues();
    }
    */
}