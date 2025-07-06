using System;
using System.Collections.Generic;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using Il2CppSystem.Collections.Generic;
using UnityEngine;

namespace TheFunGang.patches
{
    internal class Passive_FunGang : MonoBehaviour
    {
        [HarmonyPatch]
        public static void Setup(Harmony harmony)
        {
            ClassInjector.RegisterTypeInIl2Cpp<Passive_FunGang>();
            harmony.PatchAll(typeof(Passive_FunGang));
        }

        [HarmonyPatch(typeof(BattleObjectManager), "OnRoundStart_Model")]
        [HarmonyPostfix]
        private static void OnRoundStart_HealBasedOnVibration(BattleObjectManager __instance)
        {
            foreach (SupportUnitModel supportUnitModel in __instance.GetSupportUnitModels(UNIT_FACTION.PLAYER))
            {
                foreach (SupporterPassiveModel passive in supportUnitModel.PassiveDetail.SupportPassiveList)
                {
                    if (passive._classInfo.ID == 101230)
                    {
                        Il2CppSystem.Collections.Generic.List<BattleUnitModel> aliveUnits = __instance.GetAliveList(false, supportUnitModel.Faction);
                        if (aliveUnits.Count == 0)
                            return;

                        BattleUnitModel lowestHpUnit = aliveUnits[0];

                        foreach (BattleUnitModel unit in aliveUnits)
                        {
                            if (unit.Hp < lowestHpUnit.Hp)
                            {
                                lowestHpUnit = unit;
                            }
                        }

                        if (lowestHpUnit.HasBuff(BUFF_UNIQUE_KEYWORD.Vibration) || lowestHpUnit.HasBuff(BUFF_UNIQUE_KEYWORD.Laceration) || lowestHpUnit.HasBuff(BUFF_UNIQUE_KEYWORD.Sinking) || lowestHpUnit.HasBuff(BUFF_UNIQUE_KEYWORD.Burst) || lowestHpUnit.HasBuff(BUFF_UNIQUE_KEYWORD.Combustion))
                        {
                            int dummyVar;
                            int addedStack;
                            int addedTurn;
                            lowestHpUnit.RecoverHp(5, BATTLE_EVENT_TIMING.ON_START_ROUND, out dummyVar);
                            lowestHpUnit.AddBuff_NonGiver(BUFF_UNIQUE_KEYWORD.Protection, 1, 0, 0, ABILITY_SOURCE_TYPE.PASSIVE, BATTLE_EVENT_TIMING.ON_START_ROUND, null, out addedStack, out addedTurn);
                        }

                        return;
                    }
                }
            }
        }
    }
}
