using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Verse;

namespace GenshinRimpact
{
    // Unused since i use PawnRenderSubWorker to hide the graphic.

    //[HarmonyPatch(typeof(HediffComp_Invisibility), "UpdateTarget")]
    public static class Patch_Invisibility_UpdateTarget_Transpiler  // whoever thought DLC checks were a good idea, please cease your existence. 
    {
        /*[HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> il = instructions.ToList();
            for (int i = 0; i < il.Count; i++)
            {
                CodeInstruction inst = il[i];
                if (il[i].opcode == OpCodes.Ldstr &&
                    il[i+1].opcode == OpCodes.Call &&
                    il[i+2].opcode == OpCodes.Brtrue_S &&
                    il[i+3].opcode == OpCodes.Ret)
                {
                    i += 3;
                    continue; // skip current inst and 3 after it.
                }
                yield return inst;
            }
        }*/
    }
}
