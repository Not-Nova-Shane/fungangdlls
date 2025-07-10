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

[BepInPlugin("shane.TheFunGang", "TFG Kris and Susie", "1.0.1")]
public class Main : BasePlugin
{
    public override void Load()
    {
        Harmony harmony = new Harmony("Kris Support Passive");
        Passive_Kris.Setup(harmony);
        Harmony SusieHarmony = new Harmony("Susie Support Passive");
        Passive_Susie.Setup(SusieHarmony);
    }


}
