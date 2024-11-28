using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;

namespace GenshinRimpact
{
    public class AbilityPawnFlyer : PawnFlyer
    {
        public Ability ability;
        protected Vector3 position;
        public Vector3 target;
        public Rot4 direction;
        //public bool pawnCanFireAtWill = true;
        public CompProperties_AbilityDash compProps;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            direction = (startVec.x > target.ToIntVec3().x) ? Rot4.West : ((startVec.x < target.ToIntVec3().x) ? Rot4.East : ((startVec.y < target.ToIntVec3().y) ? Rot4.North : Rot4.South));
            compProps = ability.CompOfType<CompAbilityDash>().Props;
        }

        public override void Tick()
        {
            float num = ticksFlying / ticksFlightTime;
            position = Vector3.Lerp(startVec, target, num);
            base.Tick();
        }

        protected override void DrawAt(Vector3 drawLoc, bool flip = false)
        {
            FlyingPawn.Drawer.renderer.RenderPawnAt(position, direction);
        }

        protected override void RespawnPawn()
        {
            Position = target.ToIntVec3();
            base.RespawnPawn();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref ability, "ability");
            Scribe_Values.Look(ref direction, "direction");
        }
    }

    //[StaticConstructorOnStartup]
    public class DashingPawn : AbilityPawnFlyer
    {
        /*protected override void DrawAt(Vector3 drawLoc, bool flip = false)
        {
            FlyingPawn.DrawAt(GetDrawPos(), flip);
        }*/

        private Effecter effecter;
        private readonly List<Pawn> damagedPawns = [];
        private IntVec3 currentCell;

        public override void Tick()
        {
            base.Tick();

            if (compProps.damagePawnsOnPath)
            {
                var pos = position.ToIntVec3();
                if (pos != currentCell)
                {
                    currentCell = pos;
                    var thingsAtCell = currentCell.GetThingList(MapHeld);
                    for (int i = 0; i < thingsAtCell.Count; i++ )
                    { 
                        if (thingsAtCell[i] is Pawn p && !damagedPawns.Contains(p))
                        {
                            p.TakeDamage(new(compProps.damageDef, compProps.damageAmount, -1, instigator: FlyingPawn));
                            damagedPawns.Add(p);
                        }
                    } 
                }
            }
            
            if (MapHeld != null)
            {
                effecter ??= compProps.onFinishEffecter?.SpawnAttached(FlyingPawn, MapHeld);
                effecter?.EffectTick(FlyingPawn, FlyingPawn);
            }
            else
            {
                effecter?.Cleanup();
                effecter = null;
            }
        }

        /*private Vector3 GetDrawPos()
        {
            float num = ticksFlying / ticksFlightTime;
            Vector3 vector = position;

            return vector + Vector3.forward * (num - Mathf.Pow(num, 2f)) * comp.altitudeMultiplier;
        }*/

        protected override void RespawnPawn()
        {
            base.RespawnPawn();
            //compProps.endSound?.PlayOneShot(FlyingPawn);

            /*if (MapHeld != null && !comp.onFinishFlecks.NullOrEmpty())
            {
                foreach (FleckProps fleck in comp.onFinishFlecks)
                {
                    fleck.MakeFleck(MapHeld, position);//GetDrawPos());
                }
            }*/
            var targetCell = new LocalTargetInfo(target.ToIntVec3());

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
        }
    }
}
