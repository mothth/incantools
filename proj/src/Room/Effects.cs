using System;
using System.IO;
using System.Collections.Generic;
using RWCustom;
using UnityEngine;
using DevInterface;
using EffExt;
using System.Drawing.Drawing2D;

namespace IncanTools
{
    internal static class EffectManager {
        public const string CATEGORY = "IncanTools";
        public const string PREFIX = "IT_";

        public static string GetName(string name)
        {
            return PREFIX + name;
        }

        public static void RegisterEffects()
        {
            /*
            void AddEffect(string name)
            {
                EffectDefinitionBuilder builder = new EffectDefinitionBuilder(EFFECT_PREFIX + name);
                builder.SetCategory(EFFECT_CAT);
                builder.Register();
            }

            void AddEffectFactory(string name, UADFactory factory)
            {
                EffectDefinitionBuilder builder = new EffectDefinitionBuilder(EFFECT_PREFIX + name);
                builder.SetCategory(EFFECT_CAT);
                builder.SetUADFactory(factory);
                builder.Register();
            }
            */

            // Effects

            EffectDefinitionBuilder builder = new EffectDefinitionBuilder(PREFIX + "PaletteSettings");
            builder.AddStringField("bank", "default", "Bank");
            builder.AddBoolField("effect_b", false, "Effect B");
            builder.AddBoolField("effect_a", false, "Effect A");
            builder.AddBoolField("use_fade", true, "Use Fade");
            builder.SetEffectInitializer(PaletteSettingsInit);
            builder.SetCategory(CATEGORY);
            builder.Register();
        }

        private static void PaletteSettingsInit(Room room, EffectExtraData data, bool firstTimeRealized)
        {
            if (IncanRoom.TryGet(room, out IncanRoom incanRoom))
            {
                incanRoom.paletteSettings = data;
            }
        }
    }
}