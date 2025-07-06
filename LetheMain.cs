using BepInEx;
using BepInEx.Unity.IL2CPP;
using BattleUI.Dialog;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.ProBuilder;
using TheFunGang.patches;

namespace TheFunGang;

[BepInPlugin("shane.TheFunGang", "The Fun Gang Support Passives", "1.0.0")]
public class Main : BasePlugin
{
    public override void Load()
    {
        Harmony harmony = new Harmony("The Fun Gang Support Passives");
        Passive_FunGang.Setup(harmony);
    }


}
