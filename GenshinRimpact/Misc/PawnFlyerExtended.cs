using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Rimpact
{
    [HotSwap.HotSwappable]
    public class PawnFlyerExtended : PawnFlyer // use vanilla pawnflyer with OnJumpCompleted instead
    {
        private Ability ability;
        private CompProperties_AbilityFlyer compProps;

        private IntVec3 currentCell;
        private HashSet<Pawn> damagedPawns = [];

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            compProps = ability?.CompOfType<CompAbilityFlyer>()?.Props;
        }

        public override void Tick()
        {
            base.Tick();
            if (compProps == null) return;

            if (compProps.damagePawnsOnPath)
            {
                if (Position != currentCell)
                {
                    currentCell = Position;
                    var thingsAtCell = currentCell.GetThingList(MapHeld);
                    for (int i = 0; i < thingsAtCell.Count; i++)
                    {
                        if (thingsAtCell[i] is Pawn p && !damagedPawns.Contains(p))
                        {
                            p.TakeDamage(new(compProps.damageDef, compProps.damageAmount, -1, instigator: FlyingPawn));
                            damagedPawns.Add(p);
                        }
                    }
                }
            }
        }

        public override void DrawGUIOverlay()
        {
            Vector2 pos = LabelDrawPosFor(this, FlyingPawn, -0.6f);
            GenMapUI.DrawPawnLabel(FlyingPawn, pos);
        }

        public static Vector2 LabelDrawPosFor(Thing thing, Pawn heldPawn, float worldOffsetZ)
        {
            Vector3 drawPos = thing.DrawPos;
            drawPos.z += worldOffsetZ;
            Vector2 result = Find.Camera.WorldToScreenPoint(drawPos) / Prefs.UIScale;
            result.y = UI.screenHeight - result.y;

            if (!heldPawn.RaceProps.Humanlike)
                result.y -= 4f;
            else if (heldPawn.DevelopmentalStage.Baby())
                result.y -= 8f;

            return result;
        }

        protected override void RespawnPawn()
        {
            base.RespawnPawn();
            damagedPawns.Clear();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref ability, "ability");
            Scribe_Collections.Look(ref damagedPawns, "damagedPawns", LookMode.Deep);
        }

        // OLD DashingPawn CODE BELOW. TODO: DELETE LATER
        /*
        //protected Vector3 position;
        //public Vector3 target;
        //public Rot4 direction;
        //public bool pawnCanFireAtWill = true;

        private Effecter effecter;


        protected override void RespawnPawn()
        {
            base.RespawnPawn();
            if (compProps == null) return;
            //compProps.endSound?.PlayOneShot(FlyingPawn);
            var targetCell = new LocalTargetInfo(DestinationPos.ToIntVec3());

            if (targetCell.HasThing)
            {
                targetCell.TryGetPawn(out Pawn targetPawn);

                if (compProps.attackOnFinish) FlyingPawn.meleeVerbs.TryMeleeAttack(targetPawn ?? targetCell.Thing, surpriseAttack: true);
                if (compProps.subAbilityOnFinish != null)
                {
                    Utils.TryDoAbility(FlyingPawn, compProps.subAbilityOnFinish, targetCell);
                }
            }
            damagedPawns.Clear();
        }*/
    }
}
