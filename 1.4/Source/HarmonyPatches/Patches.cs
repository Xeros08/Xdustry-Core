using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HarmonyLib;
using UnityEngine;
using RimWorld;
using Verse;

namespace Xdustry {
    [StaticConstructorOnStartup]
    public static class Xdustry {
        Constructor called when the game loads
        public const string harmonyID = "mod.xeros08.xdustry.core";
        static Xdustry() {
            Log.Message($"Xdustry - CORE: Created Harmony instance with ID: ({harmonyID})");
            var harmony = new Harmony(harmonyID);
        }
    }
}
