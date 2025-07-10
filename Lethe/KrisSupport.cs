using System;
using System.Collections.Generic;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using UnityEngine;
using Lethe;
using Lethe.Patches;

namespace TheFunGang.patches
{
    internal class Passive_Kris : MonoBehaviour
    {
        [HarmonyPatch]
        public static void Setup(Harmony harmony)
        {
            ClassInjector.RegisterTypeInIl2Cpp<Passive_Kris>();
            harmony.PatchAll(typeof(Passive_Kris));
        }

        [HarmonyPatch(typeof(BattleObjectManager), "OnRoundStart_Model")]
        [HarmonyPostfix]
        private static void OnRoundStart_AddBasePowerKris(BattleObjectManager __instance)
        {
            var TP = CustomBuffs.ParseBuffUniqueKeyword("TP");
            var TheFunGangKeyword = CustomUnitKeywords.ParseUniqueUnitKeyword("TheFunGang");

            const int passiveID = 101230;
            var aliveUnits = __instance.GetAliveList(false, UNIT_FACTION.PLAYER);

            foreach (SupportUnitModel supportUnitModel in __instance.GetSupportUnitModels(UNIT_FACTION.PLAYER))
            {
                foreach (SupporterPassiveModel passive in supportUnitModel.PassiveDetail.SupportPassiveList)
                {
                    if (passive._classInfo.ID != passiveID)
                        continue;

                    LetheHooks.LOG.LogInfo("Kris passive found, yipee!");

                    BattleUnitModel topDpsUnit = null;
                    int highestDamage = 0;


                    foreach (BattleUnitModel unit in aliveUnits)
                    {
                        if (unit.GetName().Contains("Susie"))
                        {
                            unit.AddUnitKeyword(TheFunGangKeyword);
                        }
                        else if (unit.GetName().Contains("Ralsei"))
                        {
                            unit.AddUnitKeyword(TheFunGangKeyword);
                        }
                        else if (unit.GetName().Contains("Noelle"))
                        {
                            unit.AddUnitKeyword(TheFunGangKeyword);
                        }
                    }

                    foreach (BattleUnitModel unit in aliveUnits)
                    {
                        LetheHooks.LOG.LogInfo($"Checking unit: {unit.GetName()}");

                        if (!unit.HasUnitKeyword(TheFunGangKeyword))
                        {
                            LetheHooks.LOG.LogWarning($"Unit {unit.GetName()} does NOT have TheFunGang keyword.");
                            continue;
                        }

                        LetheHooks.LOG.LogInfo($"Unit {unit.GetName()} HAS TheFunGang keyword.");

                        foreach (BattleUnitModel candidateUnit in aliveUnits)
                        {
                            if (!candidateUnit.HasUnitKeyword(TheFunGangKeyword))
                            {
                                continue;
                            }
                            else
                            {
                                int lastRoundDamage = candidateUnit.GetHitAttackDamagePrevRound();
                                LetheHooks.LOG.LogInfo($"{candidateUnit.GetName()} took {lastRoundDamage} damage last round.");

                                if (lastRoundDamage > highestDamage)
                                {
                                    highestDamage = lastRoundDamage;
                                    topDpsUnit = candidateUnit;
                                }
                            }
                         
                        }

                        if (topDpsUnit == null || highestDamage < 1)
                        {
                            LetheHooks.LOG.LogWarning($"TopDpsUnit = {topDpsUnit}, highestDamage = {highestDamage}");
                            LetheHooks.LOG.LogWarning("No valid top DPS unit found.");
                            return;
                        }

                        var tpBuff = unit.GetActivatedBuf(TP);
                        int tpStack = tpBuff.GetStack();
                        tpStack = tpStack / 40;

                        if (tpStack > 2)
                        {
                            tpStack = 2;
                        }
                        else if (tpStack < 1)
                        {
                            LetheHooks.LOG.LogWarning("Unable to apply buff, invalid Base Power to be added. (Less than 1)");
                            LetheHooks.LOG.LogWarning($"Number after division: {tpStack}. Damage taken last round is {highestDamage}");
                            return;
                        }
                        var addedStack = 0;
                        var addedTurn = 0;

                        topDpsUnit.AddBuff_NonGiver(BUFF_UNIQUE_KEYWORD.SkillPowerUp, tpStack, 1, 0, ABILITY_SOURCE_TYPE.PASSIVE, BATTLE_EVENT_TIMING.ON_START_ROUND, null, out addedStack, out addedTurn);

                        return;
                    }

                }
            }

        }
    }
}