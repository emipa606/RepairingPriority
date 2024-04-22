using System.Reflection;
using HarmonyLib;
using Verse;

namespace RepairingPriority;

[StaticConstructorOnStartup]
internal class RepairingPriorityInitialization
{
    static RepairingPriorityInitialization()
    {
        new Harmony("Mlie.RepairingPriority").PatchAll(Assembly.GetExecutingAssembly());
    }
}