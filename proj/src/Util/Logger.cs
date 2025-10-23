using BepInEx;
using BepInEx.Logging;

namespace IncanTools;

internal static class IncanLogging
{
	public static BepInEx.Logging.ManualLogSource source;

	public static void Init()
	{
		source = BepInEx.Logging.Logger.CreateLogSource("IncanTools");
	}

	public static void LogError(string message)
	{
		source.LogError(message);
	}
}