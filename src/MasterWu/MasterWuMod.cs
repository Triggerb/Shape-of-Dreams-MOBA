using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HarmonyLib;

namespace MasterWu
{
    // ModBehaviour will be instantiated and attached as a component in a container game object named <Your Mod Id>
    // Multiple ModBehaviours in your mod will share the same container.
    public sealed class MasterWuMod : ModBehaviour
    {
        private void Awake()
        {
            Debug.Log($"[Master Wu] Loaded {mod.metadata.id} v{mod.metadata.modVer}");

            // If you need to patch with Harmony, you can use this.harmony to access the Harmony instance for your mod.
            // It will be created with your mod's id automatically, the first time you access the property.
            harmony.PatchAll();
        }

        private void OnDestroy()
        {
            // Make sure you clean up properly to support Live Reload.
            Debug.Log($"[Master Wu] Unloading {mod.metadata.id}");
            harmony.UnpatchAll();
        }
    }
}
