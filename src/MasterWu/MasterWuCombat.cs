using System;
using System.Runtime.CompilerServices;
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

            Equip(__instance, hero, HeroSkillLocation.Q, "St_R_LightningDance", 9f);
            Equip(__instance, hero, HeroSkillLocation.W, "St_R_Tranquility", 14f);
            Equip(__instance, hero, HeroSkillLocation.E, "St_Q_IncendiaryRounds", 12f);

            if (hero.GetComponent<MasterWuCombatController>() == null)
            {
                hero.gameObject.AddComponent<MasterWuCombatController>();
            }

            Debug.Log("[Master Wu] Installed Alpha Strike, Meditation, Wuju Style, Double Strike, and Highlander");
        }

        private static void Equip(HeroSkill skills, Hero_Husk hero, HeroSkillLocation slot, string templateName, float cooldown)
        {
            SkillTrigger template = DewResources.GetByShortTypeName<SkillTrigger>(templateName);
            if (template == null)
            {
                Debug.LogError($"[Master Wu] Could not find {templateName} for {slot}");
                return;
            }

            SkillTrigger replacement = Dew.CreateSkillTrigger(template, hero.position, 1);
            foreach (TriggerConfig config in replacement.configs)
            {
                config.cooldownTime = cooldown;
            }
            skills.EquipSkill(slot, replacement, true);
        }
    }

    internal sealed class MasterWuCombatController : MonoBehaviour
    {
        private const float WujuBaseDamage = 10f;
        private const float WujuAttackDamageRatio = 0.2f;
        private const float DoubleStrikeDamageRatio = 0.5f;
        private const float MeditationHealPerSecondRatio = 0.08f;

        private Hero_Husk _hero;
        private int _attackCount;
        private bool _dispatchingDoubleStrike;
        private bool _meditating;
        private Vector3 _meditationStart;
        private float _nextMeditationHeal;

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
                if (_meditating)
                {
                    _hero.takenDamageProcessor.Remove(ReduceMeditationDamage);
                }
            }
        }

        private void Update()
        {
            if (_hero == null || !_hero.isActive)
            {
                return;
            }

            bool hasMeditation = _hero.Status.TryGetStatusEffect<Se_R_Tranquility>(out Se_R_Tranquility meditation);
            if (hasMeditation && !_meditating)
            {
                _meditating = true;
                _meditationStart = _hero.agentPosition;
                _nextMeditationHeal = Time.time;
                _hero.takenDamageProcessor.Add(ReduceMeditationDamage, 1000);
            }
            else if (!hasMeditation && _meditating)
            {
                _meditating = false;
                _hero.takenDamageProcessor.Remove(ReduceMeditationDamage);
            }

            if (!_meditating)
            {
                return;
            }

            if (Vector2.Distance(_meditationStart.ToXY(), _hero.agentPosition.ToXY()) > 0.12f)
            {
                meditation.Destroy();
                return;
            }

            if (Time.time >= _nextMeditationHeal)
            {
                _nextMeditationHeal = Time.time + 0.25f;
                float amount = _hero.Status.maxHealth * MeditationHealPerSecondRatio * 0.25f;
                new HealData(amount).SetActor(_hero).SetCanMerge().Dispatch(_hero);
            }
        }

        private void ReduceMeditationDamage(ref DamageData damage, Actor attacker, Entity victim)
        {
            damage = damage.ApplyReduction(0.9f);
        }

        private void OnAttackHit(EventInfoAttackHit hit)
        {
            if (hit.attacker != _hero || hit.victim == null || hit.strength < 0.999f)
            {
                return;
            }

            if (_hero.Status.HasStatusEffect<Se_Q_IncendiaryRounds_EmpowerAttacks>())
            {
                ApplyWujuDamage(hit.victim);
            }

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
                hit.actor.DoBasicAttackHit(_hero, hit.victim, hit.isCrit, true,
                    DoubleStrikeDamageRatio, DoubleStrikeDamageRatio);
            }
            finally
            {
                _dispatchingDoubleStrike = false;
            }
        }

        private void ApplyWujuDamage(Entity victim)
        {
            float amount = WujuBaseDamage + _hero.Status.attackDamage * WujuAttackDamageRatio;
            new DamageData(DamageData.SourceType.Pure, amount, 0f).SetActor(_hero).Dispatch(victim);
        }
    }

    [HarmonyPatch(typeof(Se_D_TheKillingFlow), "OnCreate")]
    internal static class MasterWuDisableShellPassivePatch
    {
        private static bool Prefix(Se_D_TheKillingFlow __instance)
        {
            if (!(__instance.victim is Hero_Husk))
            {
                return true;
            }

            __instance.Destroy();
            return false;
        }
    }

    [HarmonyPatch(typeof(Se_R_LightningDance), "OnCreateSequenced")]
    internal static class MasterWuAlphaStrikePatch
    {
        private static void Prefix(Se_R_LightningDance __instance)
        {
            if (__instance.victim is Hero_Husk)
            {
                __instance.bounceCount = 4;
                __instance.bounceInterval = 0.12f;
                __instance.targetRadius = 4f;
                __instance.damage = (ScalingValue)"30 0.7ad";
            }
        }
    }

    [HarmonyPatch(typeof(Se_R_Tranquility), "OnCreateSequenced")]
    internal static class MasterWuMeditationPatch
    {
        private static void Prefix(Se_R_Tranquility __instance)
        {
            if (__instance.victim is Hero_Husk)
            {
                __instance.duration = 4f;
                __instance.ticks = 16;
                __instance.movementSpeedPercentage = 0f;
                __instance.disableInvul = true;
                __instance.enableEssenceReduction = false;
            }
        }
    }

    [HarmonyPatch(typeof(Se_Q_IncendiaryRounds_EmpowerAttacks), "OnCreate")]
    internal static class MasterWuWujuStylePatch
    {
        private static void Prefix(Se_Q_IncendiaryRounds_EmpowerAttacks __instance)
        {
            if (__instance.victim is Hero_Husk)
            {
                __instance.duration = 5f;
                __instance.numOfAttacks = int.MaxValue;
                __instance.speedOnShoot = false;
                __instance.hasteAmount = (ScalingValue)"0";
            }
        }
    }

    [HarmonyPatch(typeof(Ai_Q_IncendiaryRounds_Attack), "OnCreate")]
    internal static class MasterWuDisableIncendiaryDamagePatch
    {
        private static void Prefix(Ai_Q_IncendiaryRounds_Attack __instance)
        {
            if (__instance.info.caster is Hero_Husk)
            {
                __instance.damage = (ScalingValue)"0";
                __instance.doAreaOfEffectDamage = false;
            }
        }
    }

    [HarmonyPatch(typeof(Se_R_AnnihilationStance), "OnCreate")]
    internal static class MasterWuHighlanderSetupPatch
    {
        internal static readonly ConditionalWeakTable<Se_R_AnnihilationStance, StatBonus> RangeOffsets =
            new ConditionalWeakTable<Se_R_AnnihilationStance, StatBonus>();

        private static void Prefix(Se_R_AnnihilationStance __instance)
        {
            if (__instance.victim is Hero_Husk)
            {
                __instance.dontLockSkills = true;
                __instance.disableShockwave = true;
            }
        }

        private static void Postfix(Se_R_AnnihilationStance __instance)
        {
            if (__instance.victim is Hero_Husk hero)
            {
                StatBonus offset = new StatBonus { attackRangePercentage = -50f };
                hero.Status.AddStatBonus(offset);
                RangeOffsets.Add(__instance, offset);
            }
        }
    }

    [HarmonyPatch(typeof(Se_R_AnnihilationStance), "OnDestroyActor")]
    internal static class MasterWuHighlanderCleanupPatch
    {
        private static void Prefix(Se_R_AnnihilationStance __instance)
        {
            if (__instance.victim is Hero_Husk hero &&
                MasterWuHighlanderSetupPatch.RangeOffsets.TryGetValue(__instance, out StatBonus offset))
            {
                hero.Status.RemoveStatBonus(offset);
                MasterWuHighlanderSetupPatch.RangeOffsets.Remove(__instance);
            }
        }
    }

    [HarmonyPatch(typeof(Se_R_AnnihilationStance), "ClientActorEventOnCreate")]
    internal static class MasterWuHighlanderVisualPatch
    {
        private static bool Prefix(Se_R_AnnihilationStance __instance) => !(__instance.victim is Hero_Husk);
    }

    [HarmonyPatch(typeof(Se_R_AnnihilationStance), "UpdateAnimOverride")]
    internal static class MasterWuHighlanderAnimationPatch
    {
        private static bool Prefix(Se_R_AnnihilationStance __instance) => !(__instance.victim is Hero_Husk);
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
