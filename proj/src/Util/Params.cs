using System.Runtime.InteropServices;
using System.Globalization;
using System.Collections.Generic;
using UnityEngine;

namespace IncanTools;

// Class for storing user parameters
public sealed class UserParams
{
	public enum ParamType {
		Unparsed = -1,
		Int,
		Float,
		Flag,
		Bool = Flag,
		Color,
		Colour = Color,
		String
	}

	// Is this a bit silly in C#? It works, so whatever.
	[StructLayout(LayoutKind.Explicit)]
	public struct ParamValue
	{
		[FieldOffset(0)]
		public System.UInt32 raw;

		[FieldOffset(0)]
		public System.Int32 @int;

		[FieldOffset(0)]
		public float @float;

		[FieldOffset(0)]
		public bool flag;

		[FieldOffset(0)]
		public Color32 color;

		[FieldOffset(4)]
		public ParamType type = ParamType.Unparsed;

		[FieldOffset(8)]
		public string @string; // Source string

		public ParamValue(string sourceString) { @string = sourceString; }
	}

	////

	internal Dictionary<string, ParamValue> values = new();

	public void Add(string name, string value)
	{
		values[name] = new(value);
	}

	////

	public int GetInt(string name)
	{
		TryGetInt(name, out int ret);
		return ret;
	}

	public float GetFloat(string name)
	{
		TryGetFloat(name, out float ret);
		return ret;
	}

	public bool GetFlag(string name)
	{
		TryGetFlag(name, out bool ret);
		return ret;
	}

	public Color32 GetColor(string name)
	{
		TryGetColor(name, out Color32 ret);
		return ret;
	}

	public Color GetColorF(string name)
	{
		TryGetColor(name, out Color ret);
		return ret;
	}

	public string GetString(string name)
	{
		TryGetString(name, out string ret);
		return ret;
	}
	
	////

	public bool TryGetInt(string name, out int value)
	{
		if (!values.TryGetValue(name, out ParamValue paramValue)) {
			value = 0;
			return false;
		}

		bool parsed = paramValue.type == ParamType.Int;
		if (!parsed && int.TryParse(paramValue.@string, NumberStyles.Any, CultureInfo.InvariantCulture, out paramValue.@int))
		{
			parsed = true;
			paramValue.type = ParamType.Int;
		}

		value = paramValue.@int;
		return parsed;
	}

	public bool TryGetFloat(string name, out float value)
	{
		if (!values.TryGetValue(name, out ParamValue paramValue))
		{
			value = 0f;
			return false;
		}

		bool parsed = paramValue.type == ParamType.Float;
		if (!parsed && float.TryParse(paramValue.@string, NumberStyles.Any, CultureInfo.InvariantCulture, out paramValue.@float))
		{
			parsed = true;
			paramValue.type = ParamType.Float;
		}
		
		value = paramValue.@float;
		return parsed;
	}

	public bool TryGetFlag(string name, out bool value)
	{
		if (!values.TryGetValue(name, out ParamValue paramValue))
		{
			value = false;
			return false;
		}

		bool parsed = paramValue.type == ParamType.Flag;
		if (!parsed && bool.TryParse(paramValue.@string, out paramValue.flag))
		{
			parsed = true;
			paramValue.type = ParamType.Flag;
		}
		
		value = paramValue.flag;
		return parsed;
	}

	public bool TryGetColor(string name, out Color32 value)
	{
		if (!values.TryGetValue(name, out ParamValue paramValue))
		{
			value = new Color32(255, 255, 255, 255);
			return false;
		}

		bool parsed = paramValue.type == ParamType.Color;
		if (!parsed && ParseColor(paramValue.@string, out paramValue.@color)) {
			parsed = true;
			paramValue.type = ParamType.Color;
		}
		
		value = paramValue.@color;
		return parsed;
	}

	public bool TryGetColor(string name, out Color value)
	{
		if (!values.TryGetValue(name, out ParamValue paramValue))
		{
			value = Color.white;
			return false;
		}

		bool parsed = paramValue.type == ParamType.Color;
		if (!parsed && ParseColor(paramValue.@string, out paramValue.@color)) {
			parsed = true;
			paramValue.type = ParamType.Color;
		}
		
		value = IncanUtils.Color32ToColor(paramValue.@color);
		return parsed;
	}

	public bool TryGetString(string name, out string value)
	{
		if (!values.TryGetValue(name, out ParamValue paramValue))
		{
			value = null;
			return false;
		}

		value = paramValue.@string;
		return true;
	}
	
	////
	
	private bool ParseColor(string src, out Color32 color)
	{
		string[] arr = src.Split(',');
		if (arr.Length >= 3)
		{
			color = new Color32(
				byte.Parse(arr[0], NumberStyles.Any, CultureInfo.InvariantCulture),
				byte.Parse(arr[1], NumberStyles.Any, CultureInfo.InvariantCulture),
				byte.Parse(arr[2], NumberStyles.Any, CultureInfo.InvariantCulture),
				(arr.Length >= 4) ? byte.Parse(arr[3], NumberStyles.Any, CultureInfo.InvariantCulture) : (byte)255
			);
			return true;
		}

		color = new Color32(255, 255, 255, 255);
		return false;
	}
}