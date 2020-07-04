using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;
using MOARANDROIDS;
namespace Chips
{
    public class CompTargetEffect_Extract : CompTargetEffect
    {
        
        public override void DoEffectOn(Pawn user, Thing target)
        {
            if (!user.IsColonistPlayerControlled)
            {
                return;
            }
            if (!user.CanReserveAndReach(target, PathEndMode.Touch, Danger.Deadly, 1, -1, null, true))
            {
                return;
            }
            Corpse c = (Corpse)target;
            if (c.InnerPawn.VXChipPresent()||c.InnerPawn.health.hediffSet.GetFirstHediffOfDef(Utils.hediffHaveRXChip)!= null)
            {
                Job job = new Job(JobDefOf.ExtractChip, target, this.parent);
                job.count = 1;
                user.jobs.TryTakeOrderedJob(job, JobTag.Misc);
            }
            else
            {
                Messages.Message("No Chip present in the body", MessageTypeDefOf.RejectInput, true);
            }
        }
    } 
}
