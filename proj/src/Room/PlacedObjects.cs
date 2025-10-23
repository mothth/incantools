using System;
using System.IO;
using System.Collections.Generic;
using RWCustom;
using UnityEngine;
using DevInterface;
using Pom;
using System.Drawing.Drawing2D;

namespace IncanTools
{
	internal static class PlacedOjbectManager {
		public const string CATEGORY = "IncanTools";
        public const string PREFIX = "IT_";

		public static string GetName(string name)
        {
            return PREFIX + name;
        }

		public static void RegisterObjects()
		{
			// TODO: Rust
		}
	}
}