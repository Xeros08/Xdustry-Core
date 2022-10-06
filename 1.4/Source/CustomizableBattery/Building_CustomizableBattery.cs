using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HarmonyLib;
using UnityEngine;
using RimWorld;
using Verse;
using Verse.Sound;

namespace Xdustry {
    [StaticConstructorOnStartup]
    public class Building_CustomizableBattery : Building { // Modified version of the battery that allows for some customizations. This should be done with Harmony instead
        private int ticksToExplode;
        private Sustainer wickSustainer;

        private const float MinEnergyToExplode = 500f;
        private const float EnergyToLoseWhenExplode = 400f;
        private const float ExplodeChancePerDamage = 0.05f;

        private static readonly Vector2 BarSize = new Vector2(1.3f, 0.4f);
        private static Material BatteryBarFilledMat;
        private static Material BatteryBarUnfilledMat;

        public override void ExposeData() {
            base.ExposeData();
            Scribe_Values.Look(ref ticksToExplode, "ticksToExplode", 0);
        }

        public override void Draw() {
            base.Draw();

            // Get the PowerBattery Comp (modified)
            CompPower_CustomizableBattery comp = GetComp<CompPower_CustomizableBattery>();

            // Start drawing bar
            GenDraw.FillableBarRequest r = default(GenDraw.FillableBarRequest);
            r.center = DrawPos + Vector3.up * 0.1f;
            r.size = BarSize;
            r.fillPercent = comp.StoredEnergy / comp.Props.storedEnergyMax;

            // Dynamically create the materials based on the Props
            if (BatteryBarFilledMat == null) {
                BatteryBarFilledMat = SolidColorMaterials.SimpleSolidColorMaterial(comp.Props.barFillColor);
            }
            if (BatteryBarUnfilledMat == null) {
                BatteryBarUnfilledMat = SolidColorMaterials.SimpleSolidColorMaterial(comp.Props.barBackgroundColor);
            }


            r.filledMat = BatteryBarFilledMat;
            r.unfilledMat = BatteryBarUnfilledMat;

            r.margin = 0.15f;

            // Rotate the bar
            Rot4 rotation = base.Rotation;
            rotation.Rotate(RotationDirection.Clockwise);
            r.rotation = rotation;

            // Finish drawing bar
            GenDraw.DrawFillableBar(r);

            // Draw burning wick if exploding
            if (ticksToExplode > 0 && base.Spawned) {
                base.Map.overlayDrawer.DrawOverlay(this, OverlayTypes.BurningWick);
            }
        }

        public override void Tick() {
            base.Tick();
            if (ticksToExplode > 0) {
                if (wickSustainer == null) {
                    StartWickSustainer();
                } else {
                    wickSustainer.Maintain();
                }
                ticksToExplode--;
                if (ticksToExplode == 0) {
                    GenExplosion.DoExplosion(this.OccupiedRect().RandomCell, radius: Rand.Range(0.5f, 1f) * 3f, map: base.Map, damType: DamageDefOf.Flame, instigator: null);
                    GetComp<CompPowerBattery>().DrawPower(400f);
                }
            }
        }

        public override void PostApplyDamage(DamageInfo dinfo, float totalDamageDealt) {
            base.PostApplyDamage(dinfo, totalDamageDealt);
            if (!base.Destroyed && ticksToExplode == 0 && dinfo.Def == DamageDefOf.Flame && Rand.Value < 0.05f && GetComp<CompPowerBattery>().StoredEnergy > 500f) {
                ticksToExplode = Rand.Range(70, 150);
                StartWickSustainer();
            }
        }

        private void StartWickSustainer() {
            SoundInfo info = SoundInfo.InMap(this, MaintenanceType.PerTick);
            wickSustainer = SoundDefOf.HissSmall.TrySpawnSustainer(info);
        }
    }




    // Modified Battery COMP Properties
    public class CompProperties_CustomizableBattery : CompProperties_Battery {
        public Color barFillColor = new Color(0.9f, 0.85f, 0.2f);       // Customizable colors, the default vanilla colors are these
        public Color barBackgroundColor = new Color(0.3f, 0.3f, 0.3f);

        public CompProperties_CustomizableBattery() {
            compClass = typeof(CompPower_CustomizableBattery);
        }
    }




    // Modified Battery COMP
    public class CompPower_CustomizableBattery : CompPowerBattery {
        public new CompProperties_CustomizableBattery Props {
            get {
                return (CompProperties_CustomizableBattery)props;
            }
        }
    }
}
