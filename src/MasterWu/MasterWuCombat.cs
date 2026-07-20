using System;
using HarmonyLib;
using UnityEngine;

namespace MasterWu
{
    [HarmonyPatch(typeof(HeroSkill), nameof(HeroSkill.OnLateStartServer))]
    internal static class MasterWuLoadoutPatch
    {
        private static void Postfix(HeroSkill __instance)
        {
            if (!(__instance.hero is Hero_Husk hero))
            {
                return;
            }

            SkillTrigger chainTemplate = DewResources.GetByShortTypeName<SkillTrigger>("St_E_ChainLightning");
            if (chainTemplate == null)
            {
                Debug.LogError("[Master Wu] Could not find St_E_ChainLightning for Vanishing Cut");
                return;
            }

            SkillTrigger oldQ = __instance.Q;
            if (oldQ != null)
            {
                hero.Ability.RemoveAbility((int)HeroSkillLocation.Q);
                oldQ.Destroy();
            }

            SkillTrigger vanishingCut = Dew.CreateSkillTrigger(chainTemplate, hero.position, 1);
            vanishingCut.configs[0].cooldownTime = 10f;
            __instance.EquipSkill(HeroSkillLocation.Q, vanishingCut, true);

            if (hero.GetComponent<MasterWuCombatController>() == null)
            {
                hero.gameObject.AddComponent<MasterWuCombatController>();
            }

            Debug.Log("[Master Wu] Installed Double Strike, Wuju on-hit, and Vanishing Cut");
        }
    }

    internal sealed class MasterWuCombatController : MonoBehaviour
    {
        private const float WujuBaseDamage = 8f;
        private const float WujuAttackDamageRatio = 0.15f;
        private const float DoubleStrikeDamageRatio = 0.5f;

        private Hero_Husk _hero;
        private int _attackCount;
        private bool _dispatchingDoubleStrike;

        private void Awake()
        {
            _hero = GetComponent<Hero_Husk>();
            if (_hero == null || !_hero.isServer)
            {
                enabled = false;
                return;
            }

            _hero.EntityEvent_OnAttackHit += new Action<EventInfoAttackHit>(OnAttackHit);
        }

        private void OnDestroy()
        {
            if (_hero != null)
            {
                _hero.EntityEvent_OnAttackHit -= new Action<EventInfoAttackHit>(OnAttackHit);
            }
        }

        private void OnAttackHit(EventInfoAttackHit hit)
        {
            if (hit.attacker != _hero || hit.victim == null || hit.strength < 0.999f)
            {
                return;
            }

            ApplyWujuDamage(hit.victim);

            if (_dispatchingDoubleStrike)
            {
                return;
            }

            _attackCount++;
            if (_attackCount < 4)
            {
                return;
            }

            // The bonus strike is stack one of the next cycle, matching Yi's cadence.
            _attackCount = 1;
            _dispatchingDoubleStrike = true;
            try
            {
                hit.actor.DoBasicAttackHit(
                    _hero,
                    hit.victim,
                    hit.isCrit,
                    true,
                    DoubleStrikeDamageRatio,
                    DoubleStrikeDamageRatio);
            }
            finally
            {
                _dispatchingDoubleStrike = false;
            }
        }

        private void ApplyWujuDamage(Entity victim)
        {
            float amount = WujuBaseDamage + _hero.Status.attackDamage * WujuAttackDamageRatio;
            new DamageData(DamageData.SourceType.Pure, amount, 0f)
                .SetActor(_hero)
                .Dispatch(victim);
        }
    }

    [HarmonyPatch(typeof(Se_R_AnnihilationStance), "OnCreate")]
    internal static class MasterWuHighlanderSetupPatch
    {
        private static void Prefix(Se_R_AnnihilationStance __instance)
        {
            if (__instance.victim is Hero_Husk)
            {
                __instance.dontLockSkills = true;
                __instance.disableShockwave = true;
            }
        }
    }

    [HarmonyPatch(typeof(Se_R_AnnihilationStance), "FinalStatProcessor")]
    internal static class MasterWuHighlanderSpeedPatch
    {
        private static void Postfix(Se_R_AnnihilationStance __instance, ref FinalStats data)
        {
            if (__instance.victim is Hero_Husk)
            {
                data.movementSpeedMultiplier += 0.55f;
            }
        }
    }
}
