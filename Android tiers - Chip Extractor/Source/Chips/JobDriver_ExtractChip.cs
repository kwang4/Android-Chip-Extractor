using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;
using Androids;
namespace Chips
{
    class JobDriver_ExtractChip:JobDriver
    {
        private bool corruptedChip = false;

        private const TargetIndex CorpseInd = TargetIndex.A;

        private const TargetIndex ItemInd = TargetIndex.B;

        private const int DurationTicks = 300;

        private Corpse Corpse => (Corpse)job.GetTarget(TargetIndex.A).Thing;

        private Thing Item => job.GetTarget(TargetIndex.B).Thing;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            if (pawn.Reserve(Corpse, job, 1, -1, null, errorOnFailed))
            {
                return pawn.Reserve(Item, job, 1, -1, null, errorOnFailed);
            }
            return false;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            yield return Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.Touch).FailOnDespawnedOrNull(TargetIndex.B).FailOnDespawnedOrNull(TargetIndex.A);
            yield return Toils_Haul.StartCarryThing(TargetIndex.B, false, false, false);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch).FailOnDespawnedOrNull(TargetIndex.A);
            Toil prepare = Toils_General.Wait(500, TargetIndex.None);
            prepare.WithProgressBarToilDelay(TargetIndex.A, false, -0.5f);
            prepare.FailOnDespawnedOrNull(TargetIndex.A);
            prepare.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
            yield return prepare;
            yield return Toils_General.Do(Extract);
            yield break;
        }
       
        private void Extract()
        {
            
            Pawn innerPawn = Corpse.InnerPawn;
            corruptedChip = failedExtract(innerPawn);
            if (innerPawn.health.hediffSet.HasHediff(MOARANDROIDS.Utils.hediffHaveVX3Chip))
            {
              HediffDef h = DefDatabase<HediffDef>.GetNamed("ATPP_HediffVX3Chip");
            if(!corruptedChip)
              GenSpawn.Spawn(ThingDefOf.ATPP_VX3Chip, innerPawn.Position, Corpse.Map);
              finExtract(innerPawn, h);
            }
            else if (innerPawn.health.hediffSet.HasHediff(MOARANDROIDS.Utils.hediffHaveVX2Chip))
            {
                HediffDef h = DefDatabase<HediffDef>.GetNamed("ATPP_HediffVX2Chip");
                if (!corruptedChip)
                    GenSpawn.Spawn(ThingDefOf.ATPP_VX2Chip, innerPawn.Position, Corpse.Map);
                finExtract(innerPawn, h);
            }
            else if(innerPawn.health.hediffSet.HasHediff(MOARANDROIDS.Utils.hediffHaveVX1Chip))
            {
                HediffDef h = DefDatabase<HediffDef>.GetNamed("ATPP_HediffVX1Chip");
                if (!corruptedChip)
                    GenSpawn.Spawn(ThingDefOf.ATPP_VX1Chip, innerPawn.Position, Corpse.Map);
                finExtract(innerPawn, h);
            }
            else if (innerPawn.health.hediffSet.HasHediff(MOARANDROIDS.Utils.hediffHaveVX0Chip))
            {
                HediffDef h = DefDatabase<HediffDef>.GetNamed("ATPP_HediffVX0Chip");
                if (!corruptedChip)
                    GenSpawn.Spawn(ThingDefOf.ATPP_VX0Chip, innerPawn.Position, Corpse.Map);
                finExtract(innerPawn, h);
            }
            else if (innerPawn.health.hediffSet.HasHediff(MOARANDROIDS.Utils.hediffHaveRXChip))
            {
                HediffDef h = DefDatabase<HediffDef>.GetNamed("ATPP_HediffRXChip");
                if (!corruptedChip)
                    GenSpawn.Spawn(ThingDefOf.ATPP_RXChip, innerPawn.Position, Corpse.Map);
                finExtract(innerPawn, h);
            }
            else
            {
                Messages.Message("No Chip to Extract", innerPawn, MessageTypeDefOf.RejectInput, true);
            }
           
           
        }
        private void finExtract(Pawn innerPawn,HediffDef h)
        {
            innerPawn.health.hediffSet.hediffs.Remove(innerPawn.health.hediffSet.GetFirstHediffOfDef(h));
            Item.SplitOff(1).Destroy();
            if (corruptedChip)
                Messages.Message("Chip was too corrupted from brain death to recover",MessageTypeDefOf.NegativeEvent);
        }
        private bool failedExtract(Pawn innerPawn)
        {
            double x = 0;
            var rand = new Random();
            string robotornot = innerPawn.kindDef.defName.ToLower();
            if(robotornot.Contains("android")||robotornot == "m7mech"||innerPawn.kindDef.label.Contains("android"))
            {
                x = 0.85;
            }
            else
            {
                x = 0.5;
            }
            if (rand.NextDouble() < x)
                return false;
            return true;
        }
    }
}
