using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HarmonyLib;
using UnityEngine;
using RimWorld;
using Verse;

namespace Xdustry {
    public class IngestionOutcomeDoer_RemoveHediffs : IngestionOutcomeDoer {
        public List<HediffDef> hediffDefs; //* Here go all hediffs to be removed if present

        protected override void DoIngestionOutcomeSpecial(Pawn pawn, Thing ingested) {
#if DEBUG
            Log.Message($"Removing Hediffs...");
#endif

            foreach (HediffDef hdef in hediffDefs) { // For each Hediff to remove
                if (!pawn.health.hediffSet.HasHediff(hdef)) { //* Skip non present Hediffs
#if DEBUG
                    Log.Message($"Found no Hediffs of type: \"{hdef}\"");
#endif
                    continue;
                }

                //! REFLECTION
                // As .NET doesn't allow you to use variables as generic parameters, gotta use reflection
                //                MethodInfo method = typeof(HediffSet).GetMethod("GetHediffs", BindingFlags.Public | BindingFlags.Instance);
                //                method = method.MakeGenericMethod(hdef.hediffClass); // Build a generic method with the type needed

                //                var matchingHediffs = (IEnumerable<Hediff>)method.Invoke(pawn.health.hediffSet, null);
                //                foreach (var hediff in matchingHediffs) {
                //                    if (hediff.def != hdef) continue; //* Skip hediffs not matching the def
                //#if DEBUG
                //                    Log.Message($"Removing matching Hediff: \"{hediff}\"");
                //#endif
                //                    pawn.health.RemoveHediff(hediff);
                //                }

                //! NORMAL WAY
                for (int i = 0; i < pawn.health.hediffSet.hediffs.Count; i++) { // For all Hediffs matching
                    if (pawn.health.hediffSet.hediffs[i].def == hdef) {
#if DEBUG
                        Log.Message($"Removing matching Hediff: \"{pawn.health.hediffSet.hediffs[i]}\"");
#endif
                        pawn.health.RemoveHediff(pawn.health.hediffSet.hediffs[i]);
                    }
                }
            }
        }
    }
}
