using System;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using UnityEngine;
using Lethe;
using Lethe.Patches;

namespace TheFunGang.patches
{
    internal class Passive_Susie : MonoBehaviour
    {
        [HarmonyPatch]
        public static void Setup(Harmony harmony)
        {
            ClassInjector.RegisterTypeInIl2Cpp<Passive_Susie>();
            harmony.PatchAll(typeof(Passive_Susie));
        }

        [HarmonyPatch(typeof(BattleObjectManager), "OnRoundStart_Model")]
        [HarmonyPostfix]
        private static void OnRoundStart_SusieSupport(BattleObjectManager __instance)
        {
            foreach (SupportUnitModel supportUnitModel in __instance.GetSupportUnitModels(UNIT_FACTION.PLAYER))
            {
                foreach (SupporterPassiveModel passive in supportUnitModel.PassiveDetail.SupportPassiveList)
                {
                    if (passive._classInfo.ID != 107231)
                        continue;

                    LetheHooks.LOG.LogInfo("Found Susie support passive. Grr! (If you see this, fuck you.)");

                    var aliveUnits = __instance.GetAliveList(false, supportUnitModel.Faction);
                    if (aliveUnits.Count == 0)
                        return;

                    BattleUnitModel lowestHPUnit = null;
                    float LowestUnitHp = 999999;
                    foreach (BattleUnitModel unit in aliveUnits)
                    {
                        var UnitHP = unit.GetHpRatio();
                        LetheHooks.LOG.LogInfo($"{unit.GetName()} HP Ratio: {UnitHP}");

                        if (UnitHP < LowestUnitHp)
                        {
                            LowestUnitHp = UnitHP;
                            lowestHPUnit = unit;
                        }
                    }

                    if (lowestHPUnit == null)
                    {
                        LetheHooks.LOG.LogWarning("No unit found to buff.");
                        return;
                    }
                    var addedBuffStack = 0;
                    var addedBuffCount = 0;
                    lowestHPUnit.AddBuff_NonGiver(BUFF_UNIQUE_KEYWORD.AttackDmgUp, 2, 1, 0, ABILITY_SOURCE_TYPE.PASSIVE, BATTLE_EVENT_TIMING.ON_START_ROUND, null, out addedBuffStack, out addedBuffCount);
                    lowestHPUnit.AddShield(15, true, ABILITY_SOURCE_TYPE.PASSIVE, BATTLE_EVENT_TIMING.ON_START_ROUND);

                    LetheHooks.LOG.LogInfo($"Applied +2 Offense and 15 Shield to {lowestHPUnit.GetName()}.");
                    return;
                }
            }
        }
    }
}
