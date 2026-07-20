using System;
using System.Collections;
using System.Collections.Generic;
using DewInternal;
using UnityEngine;
using HarmonyLib;

namespace MasterWu
{
    // ModBehaviour will be instantiated and attached as a component in a container game object named <Your Mod Id>
    // Multiple ModBehaviours in your mod will share the same container.
    public sealed class MasterWuMod : ModBehaviour
    {
        private readonly List<Action> _restoreLocalization = new List<Action>();

        private void Awake()
        {
            Debug.Log($"[Master Wu] Loaded {mod.metadata.id} v{mod.metadata.modVer}");

            // If you need to patch with Harmony, you can use this.harmony to access the Harmony instance for your mod.
            // It will be created with your mod's id automatically, the first time you access the property.
            harmony.PatchAll();
            ApplyLocalization();
        }

        private void OnDestroy()
        {
            for (int i = _restoreLocalization.Count - 1; i >= 0; i--)
            {
                _restoreLocalization[i]();
            }
            _restoreLocalization.Clear();

            // Make sure you clean up properly to support Live Reload.
            Debug.Log($"[Master Wu] Unloading {mod.metadata.id}");
            harmony.UnpatchAll();
        }

        private void ApplyLocalization()
        {
            foreach (KeyValuePair<string, PerLanguageLocalizationData> language in DewLocalization.buildData.dataByLanguage)
            {
                PerLanguageLocalizationData data = language.Value;

                ReplaceUi(data, "Hero_Husk_Name", "Master Wu");
                ReplaceUi(data, "Hero_Husk_Subtitle", "Disciple of the Unbroken Path");
                ReplaceUi(data, "Hero_Husk_Description",
                    "Master Wu is a swift melee Traveler who turns careful positioning and precise timing into relentless sword pressure.");

                ReplaceSkill(data, "D_TheKillingFlow", "Focused Rhythm",
                    "Attack speed is converted into offensive momentum.");
                ReplaceSkill(data, "M_FlashStep", "Wind Step",
                    "Dash rapidly in the chosen direction and reset your attack flow.");
                ReplaceSkill(data, "Q_Laceration", "Vanishing Cut",
                    "Surge through enemies in a line, then finish with a circular sword strike.");
                ReplaceSkill(data, "R_AnnihilationStance", "Unbound Tempo",
                    "Enter a fast offensive stance with increased attack power, attack speed, and mobility uptime.");
            }

            Debug.Log($"[Master Wu] Applied localization to {DewLocalization.buildData.dataByLanguage.Count} languages");
        }

        private void ReplaceUi(PerLanguageLocalizationData data, string key, string replacement)
        {
            bool existed = data.ui.TryGetValue(key, out string original);
            _restoreLocalization.Add(() =>
            {
                if (existed)
                {
                    data.ui[key] = original;
                }
                else
                {
                    data.ui.Remove(key);
                }
            });
            data.ui[key] = replacement;
        }

        private void ReplaceSkill(PerLanguageLocalizationData data, string key, string name, string shortDescription)
        {
            if (!data.skills.TryGetValue(key, out SkillData skill) || skill.configs.Count == 0)
            {
                Debug.LogWarning($"[Master Wu] Localization skill key not found: {key}");
                return;
            }

            foreach (SkillConfigData config in skill.configs)
            {
                string originalName = config.name;
                string originalShortDescription = config.shortDescription;
                _restoreLocalization.Add(() =>
                {
                    config.name = originalName;
                    config.shortDescription = originalShortDescription;
                });
                config.name = name;
                config.shortDescription = shortDescription;
            }
        }
    }
}
