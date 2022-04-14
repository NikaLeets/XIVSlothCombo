using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using Dalamud.Game.ClientState.Objects.Types;
using System.Linq;

namespace XIVSlothComboPlugin.Combos
{
    internal static class AST
    {
        public const byte JobID = 33;

        public const uint
            Benefic = 3594,
            Draw = 3590,
            Balance = 4401,
            Bole = 4404,
            Arrow = 4402,
            Spear = 4403,
            Ewer = 4405,
            Spire = 4406,
            MinorArcana = 7443,
            SleeveDraw = 7448,
            Malefic4 = 16555,
            LucidDreaming = 7562,
            Ascend = 3603,
            Swiftcast = 7561,
            CrownPlay = 25869,
            Astrodyne = 25870,
            FallMalefic = 25871,
            Malefic1 = 3596,
            Malefic2 = 3598,
            Malefic3 = 7442,
            Combust = 3599,
            Play = 17055,
            LordOfCrowns = 7444,
            LadyOfCrown = 7445,
            Divination = 16552,
            Lightspeed = 3606,

            // aoes
            Gravity = 3615,
            Gravity2 = 25872,

            // dots
            Combust3 = 16554,
            Combust2 = 3608,
            Combust1 = 3599,


            // heals
            Helios = 3600,
            AspectedHelios = 3601,
            CelestialOpposition = 16553,
            Benefic2 = 3610,
            EssentialDignity = 3614,
            CelestialIntersection = 16556,
            AspectedBenefic = 3595,
            Horoscope = 16557,
            Exaltation = 25873;

        public static class Buffs
        {
            public const ushort
            Divination = 1878,
            Swiftcast = 167,
            LordOfCrownsDrawn = 2054,
            LadyOfCrownsDrawn = 2055,
            AspectedHelios = 836,
            Balance = 913,
            Bole = 914,
            Arrow = 915,
            Spear = 916,
            Ewer = 917,
            Spire = 918,
            BalanceDamage = 1882,
            BoleDamage = 1883,
            ArrowDamage = 1884,
            SpearDamage = 1885,
            EwerDamage = 1886,
            SpireDamage = 1887,
            Horoscope = 1890,
            HoroscopeHelios = 1891,
            AspectedBenefic = 835,
            NeutralSect = 1892,
            NeutralSectShield = 1921;
        }

        public static class Debuffs
        {
            public const ushort
            Combust1 = 838,
            Combust2 = 843,
            Combust3 = 1881;
        }

        public static class Levels
        {
            public const byte
                EssentialDignity = 15,
                Benefic2 = 26,
                MinorArcana = 50,
                Draw = 30,
                AspectedBenefic = 34,
                CrownPlay = 70,
                CelestialOpposition = 60,
                CelestialIntersection = 74,
                Horoscope = 76,
                NeutralSect = 80,
                Exaltation = 86;
        }

        public static class Config
        {
            public const string
                ASTLucidDreamingFeature = "ASTLucidDreamingFeature";
            public const string
                AstroEssentialDignity = "ASTCustomEssentialDignity";
        }

        public static class MeleeCardTargets
        {
            public const string
                Monk = "monk",
                Dragoon = "dragoon",
                Ninja = "ninja",
                Reaper = "reaper",
                Samurai = "samurai",
                Pugilist = "pugilist",
                Lancer = "lancer",
                Rogue = "rogue";
        }

        public static class RangedCardTargets
        {
            public const string
                Bard = "bard",
                Machinist = "machinist",
                Dancer = "dancer",
                RedMage = "red mage",
                BlackMage = "black mage",
                Summoner = "summoner",
                BlueMage = "blue mage",
                Archer = "archer",
                Thaumaturge = "thaumaturge",
                Arcanist = "arcanist";

        }

        public static class TankCardTargets
        {
            public const string
                Paladin = "paladin",
                Warrior = "warrior",
                DarkKnight = "dark knight",
                Gunbreaker = "gunbreaker",
                Gladiator = "gladiator",
                Marauder = "marauder";
        }

        public static class HealerCardTargets
        {
            public const string
                WhiteMage = "white mage",
                Astrologian = "astrologian",
                Scholar = "scholar",
                Sage = "sage",
                Conjurer = "conjurer";
        }
    }

    internal class AstrologianCardsOnDrawFeaturelikewhat : CustomCombo
    {
        private new bool GetTarget = true;
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.AstrologianCardsOnDrawFeaturelikewhat;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == AST.Play)
            {
                var gauge = GetJobGauge<ASTGauge>();
                var haveCard = HasEffect(AST.Buffs.Balance) || HasEffect(AST.Buffs.Bole) || HasEffect(AST.Buffs.Arrow) || HasEffect(AST.Buffs.Spear) || HasEffect(AST.Buffs.Ewer) || HasEffect(AST.Buffs.Spire);

                if (!gauge.ContainsSeal(SealType.NONE) && IsEnabled(CustomComboPreset.AstrologianAstrodyneOnPlayFeature) && (gauge.DrawnCard != CardType.NONE || GetCooldown(AST.Draw).CooldownRemaining > 30))
                    return AST.Astrodyne;

                if (haveCard)
                {
                    if (IsEnabled(CustomComboPreset.AstAutoCardTarget))
                    {
                        if (GetTarget || (IsEnabled(CustomComboPreset.AstrologianTargetLock)))
                            SetTarget();
                    }

                    return OriginalHook(AST.Play);
                }

                GetTarget = true;
                return OriginalHook(AST.Draw);
            }

            return actionID;
        }

        private bool SetTarget()
        {
            var gauge = GetJobGauge<ASTGauge>();
            if (gauge.DrawnCard.Equals(CardType.NONE)) return false;
            var cardDrawn = gauge.DrawnCard;

            //Checks for trusts then normal parties
            int maxPartySize = GetPartySlot(5) == null ? 4 : 8;
            if (GetPartyMembers().Count() > 0) maxPartySize = GetPartyMembers().Count();
            if (GetPartyMembers().Count() == 0 && Service.BuddyList.Length == 0) maxPartySize = 0;

            for (int i = 2; i <= maxPartySize; i++)
            {
                GameObject? member = GetPartySlot(i);

                if (member == null) break;
                string job = "";
                if (member is BattleNpc) job = (member as BattleNpc).ClassJob.GameData.Name.ToString();
                if (member is BattleChara) job = (member as BattleChara).ClassJob.GameData.Name.ToString();

                if (FindEffectOnMember(AST.Buffs.BalanceDamage, member) is not null) continue;
                if (FindEffectOnMember(AST.Buffs.ArrowDamage, member) is not null) continue;
                if (FindEffectOnMember(AST.Buffs.BoleDamage, member) is not null) continue;
                if (FindEffectOnMember(AST.Buffs.EwerDamage, member) is not null) continue;
                if (FindEffectOnMember(AST.Buffs.SpireDamage, member) is not null) continue;
                if (FindEffectOnMember(AST.Buffs.SpearDamage, member) is not null) continue;

                if (cardDrawn is CardType.BALANCE or CardType.ARROW or CardType.SPEAR)
                {
                    if (typeof(AST.MeleeCardTargets).GetFields().Select(x => x.GetRawConstantValue().ToString()).Contains(job))
                    {
                        TargetPartyMember(member);
                        GetTarget = false;
                        return true;
                    }

                }
                if (cardDrawn is CardType.BOLE or CardType.EWER or CardType.SPIRE)
                {
                    if (typeof(AST.RangedCardTargets).GetFields().Select(x => x.GetRawConstantValue().ToString()).Contains(job))
                    {
                        TargetPartyMember(member);
                        GetTarget = false;
                        return true;
                    }
                }
            }

            if (IsEnabled(CustomComboPreset.AstrologianTargetExtraFeature))
            {
                for (int i = 2; i <= maxPartySize; i++)
                {
                    GameObject? member = GetPartySlot(i);
                    if (member == null) break;
                    string job = "";
                    if (member is BattleNpc) job = (member as BattleNpc).ClassJob.GameData.Name.ToString();
                    if (member is BattleChara) job = (member as BattleChara).ClassJob.GameData.Name.ToString();

                    if (FindEffectOnMember(AST.Buffs.BalanceDamage, member) is not null) continue;
                    if (FindEffectOnMember(AST.Buffs.ArrowDamage, member) is not null) continue;
                    if (FindEffectOnMember(AST.Buffs.BoleDamage, member) is not null) continue;
                    if (FindEffectOnMember(AST.Buffs.EwerDamage, member) is not null) continue;
                    if (FindEffectOnMember(AST.Buffs.SpireDamage, member) is not null) continue;
                    if (FindEffectOnMember(AST.Buffs.SpearDamage, member) is not null) continue;

                    if (cardDrawn is CardType.BALANCE or CardType.ARROW or CardType.SPEAR)
                    {
                        if (typeof(AST.TankCardTargets).GetFields().Select(x => x.GetRawConstantValue().ToString()).Contains(job))
                        {
                            TargetPartyMember(member);
                            GetTarget = false;
                            return true;
                        }

                    }
                    if (cardDrawn is CardType.BOLE or CardType.EWER or CardType.SPIRE)
                    {
                        if (typeof(AST.HealerCardTargets).GetFields().Select(x => x.GetRawConstantValue().ToString()).Contains(job))
                        {
                            TargetPartyMember(member);
                            GetTarget = false;
                            return true;
                        }
                    }
                }
            }

            return false;

        }
    }


    internal class AstrologianCrownPlayFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.AstrologianCrownPlayFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == AST.CrownPlay)
            {
                var gauge = GetJobGauge<ASTGauge>();
                /*var ladyofCrown = HasEffect(AST.Buffs.LadyOfCrownsDrawn);
                var lordofCrown = HasEffect(AST.Buffs.LordOfCrownsDrawn);
                var minorArcanaCD = GetCooldown(AST.MinorArcana);*/
                if (level >= AST.Levels.MinorArcana && gauge.DrawnCrownCard == CardType.NONE)
                    return AST.MinorArcana;
            }

            return actionID;
        }
    }

    internal class AstrologianBeneficFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.AstrologianBeneficFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == AST.Benefic2)
            {
                if (level < AST.Levels.Benefic2)
                    return AST.Benefic;
            }

            return actionID;
        }
    }

    internal class AstrologianAscendFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.AstrologianAscendFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == AST.Swiftcast)
            {
                if (IsEnabled(CustomComboPreset.AstrologianAscendFeature))
                {
                    if (HasEffect(AST.Buffs.Swiftcast))
                        return AST.Ascend;
                }

                return OriginalHook(AST.Swiftcast);
            }

            return actionID;
        }
    }
    internal class AstrologianAlternateAscendFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.AstrologianAlternateAscendFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == AST.Ascend)
            {
                var swiftCD = GetCooldown(AST.Swiftcast);
                if ((swiftCD.CooldownRemaining == 0)
)
                    return AST.Swiftcast;
            }
            return actionID;
        }
    }

    internal class AstrologianDpsFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.AstrologianDpsFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == AST.FallMalefic || actionID == AST.Malefic4 || actionID == AST.Malefic3 || actionID == AST.Malefic2 || actionID == AST.Malefic1)
            {

                var incombat = HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat);
                var combust3Debuff = FindTargetEffect(AST.Debuffs.Combust3);
                var combust2Debuff = FindTargetEffect(AST.Debuffs.Combust2);
                var combust1Debuff = FindTargetEffect(AST.Debuffs.Combust1);
                var gauge = GetJobGauge<ASTGauge>();
                var lucidDreaming = GetCooldown(AST.LucidDreaming);
                var fallmalefic = GetCooldown(AST.FallMalefic);
                var minorarcanaCD = GetCooldown(AST.MinorArcana);
                var drawCD = GetCooldown(AST.Draw);
                var actionIDCD = GetCooldown(actionID);

                if (IsEnabled(CustomComboPreset.AstrologianLightSpeedFeature) && level >= 6)
                {
                    var lightspeed = GetCooldown(AST.Lightspeed);
                    if (!lightspeed.IsCooldown && lastComboMove == OriginalHook(actionID) && actionIDCD.CooldownRemaining >= 0.6)
                        return AST.Lightspeed;
                }
                if (IsEnabled(CustomComboPreset.AstrologianAstrodyneFeature) && level >= 50)
                {
                    if (!gauge.ContainsSeal(SealType.NONE) && incombat && fallmalefic.CooldownRemaining >= 0.6 && level >= 50)
                        return AST.Astrodyne;
                }
                if (IsEnabled(CustomComboPreset.AstrologianAutoDrawFeature) && level >= 30)
                {
                    if (gauge.DrawnCard.Equals(CardType.NONE) && incombat && actionIDCD.CooldownRemaining >= 0.6 && drawCD.CooldownRemaining < 30 && level >= 30)
                        return AST.Draw;

                }
                if (IsEnabled(CustomComboPreset.AstrologianAutoCrownDrawFeature) && level >= 70)
                {
                    if (gauge.DrawnCrownCard == CardType.NONE && incombat && minorarcanaCD.CooldownRemaining == 0 && actionIDCD.CooldownRemaining >= 0.6 && level >= 70)
                        return AST.MinorArcana;
                }
                if (IsEnabled(CustomComboPreset.AstrologianLucidFeature) && level >= 24)
                {
                    if (!lucidDreaming.IsCooldown && LocalPlayer.CurrentMp <= 8000 && fallmalefic.CooldownRemaining > 0.2 && level >= 24)
                        return AST.LucidDreaming;
                }
                if (IsEnabled(CustomComboPreset.AstrologianLazyLordFeature) && level >= 70)
                {
                    if (gauge.DrawnCrownCard == CardType.LORD && incombat && actionIDCD.CooldownRemaining >= 0.6 && level >= 70)
                        return AST.LordOfCrowns;
                }
                if (IsEnabled(CustomComboPreset.AstrologianDpsFeature) && !IsEnabled(CustomComboPreset.DisableCombustOnDpsFeature) && level >= 72 && incombat)
                {
                    if ((combust3Debuff is null) || (combust3Debuff.RemainingTime <= 3))
                        return AST.Combust3;
                }

                if (IsEnabled(CustomComboPreset.AstrologianDpsFeature) && !IsEnabled(CustomComboPreset.DisableCombustOnDpsFeature) && level >= 46 && level <= 71 && incombat)
                {
                    if ((combust2Debuff is null) || (combust2Debuff.RemainingTime <= 3))
                        return AST.Combust2;
                }

                if (IsEnabled(CustomComboPreset.AstrologianDpsFeature) && !IsEnabled(CustomComboPreset.DisableCombustOnDpsFeature) && level >= 4 && level <= 45 && incombat)
                {
                    if ((combust1Debuff is null) || (combust1Debuff.RemainingTime <= 3))
                        return AST.Combust1;
                }
            }

            return actionID;
        }
    }

    internal class AstrologianHeliosFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.AstrologianHeliosFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == AST.AspectedHelios)
            {
                var heliosBuff = FindEffect(AST.Buffs.AspectedHelios);
                var horoscopeCD = GetCooldown(AST.Horoscope);
                var celestialOppositionCD = GetCooldown(AST.CelestialOpposition);
                var incombat = HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat);
                var gauge = GetJobGauge<ASTGauge>();

                if (IsEnabled(CustomComboPreset.AstrologianLazyLadyFeature))
                {
                    if (gauge.DrawnCrownCard == CardType.LADY && incombat && level >= AST.Levels.CrownPlay)
                        return AST.LadyOfCrown;
                }

                if (IsEnabled(CustomComboPreset.AstrologianCelestialOppositionFeature) && celestialOppositionCD.CooldownRemaining == 0 && level >= AST.Levels.CelestialOpposition)
                    return AST.CelestialOpposition;

                if (IsEnabled(CustomComboPreset.AstrologianHoroscopeFeature))
                {
                    if (horoscopeCD.CooldownRemaining == 0 && level >= AST.Levels.Horoscope)
                        return AST.Horoscope;

                    if (!HasEffect(AST.Buffs.AspectedHelios) && HasEffect(AST.Buffs.Horoscope) || HasEffect(AST.Buffs.NeutralSect) && !HasEffect(AST.Buffs.NeutralSectShield))
                        return AST.AspectedHelios;

                    if (HasEffect(AST.Buffs.HoroscopeHelios))
                        return OriginalHook(AST.Horoscope);
                }

                if (HasEffect(AST.Buffs.AspectedHelios) && heliosBuff.RemainingTime > 2)
                    return AST.Helios;
            }

            return actionID;
        }
    }
    internal class AstrologianDpsAoEFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.AstrologianDpsAoEFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == AST.Gravity || actionID == AST.Gravity2)
            {

                var incombat = HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat);
                var gauge = GetJobGauge<ASTGauge>();
                var lucidDreaming = GetCooldown(AST.LucidDreaming);
                var gravityCD = GetCooldown(AST.Gravity);
                var minorarcanaCD = GetCooldown(AST.MinorArcana);
                var drawCD = GetCooldown(AST.Draw);
                var actionIDCD = GetCooldown(actionID);

                if (IsEnabled(CustomComboPreset.AstrologianLightSpeedFeature) && level >= 6)
                {
                    var lightspeed = GetCooldown(AST.Lightspeed);
                    if (!lightspeed.IsCooldown && lastComboMove == OriginalHook(actionID) && actionIDCD.CooldownRemaining >= 0.6)
                        return AST.Lightspeed;
                }
                if (IsEnabled(CustomComboPreset.AstrologianAstrodyneFeature) && level >= 50)
                {
                    if (!gauge.ContainsSeal(SealType.NONE) && incombat && actionIDCD.CooldownRemaining >= 0.6 && level >= 50)
                        return AST.Astrodyne;
                }
                if (IsEnabled(CustomComboPreset.AstrologianAutoDrawFeature) && level >= 30)
                {
                    if (gauge.DrawnCard.Equals(CardType.NONE) && incombat && actionIDCD.CooldownRemaining >= 0.6 && drawCD.CooldownRemaining < 30 && level >= 30)
                        return AST.Draw;

                }
                if (IsEnabled(CustomComboPreset.AstrologianLazyLordFeature) && level >= 70)
                {
                    if (gauge.DrawnCrownCard == CardType.LORD && incombat && actionIDCD.CooldownRemaining >= 0.6 && level >= 70)
                        return AST.LordOfCrowns;
                }
                if (IsEnabled(CustomComboPreset.AstrologianAutoCrownDrawFeature) && level >= 70)
                {
                    if (gauge.DrawnCrownCard == CardType.NONE && incombat && minorarcanaCD.CooldownRemaining == 0 && actionIDCD.CooldownRemaining >= 0.6 && level >= 70)
                        return AST.MinorArcana;
                }
                if (IsEnabled(CustomComboPreset.AstrologianLucidFeature) && level >= 24)
                {
                    if (!lucidDreaming.IsCooldown && LocalPlayer.CurrentMp <= 8000 && actionIDCD.CooldownRemaining > 0.2 && level >= 24)
                        return AST.LucidDreaming;
                }
            }
            return actionID;
        }
    }
    internal class AstrologianAstrodyneOnPlayFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.AstrologianAstrodyneOnPlayFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == AST.Play && !IsEnabled(CustomComboPreset.AstrologianCardsOnDrawFeaturelikewhat))
            {
                var gauge = GetJobGauge<ASTGauge>();
                if (!gauge.ContainsSeal(SealType.NONE))
                    return AST.Astrodyne;
            }
            return actionID;

        }
    }
    internal class AstrologianAlternateDpsFeature : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.AstrologianAlternateDpsFeature;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == AST.Combust1 || actionID == AST.Combust2 || actionID == AST.Combust3)
            {

                var incombat = HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat);
                var combust3Debuff = FindTargetEffect(AST.Debuffs.Combust3);
                var combust2Debuff = FindTargetEffect(AST.Debuffs.Combust2);
                var combust1Debuff = FindTargetEffect(AST.Debuffs.Combust1);
                var gauge = GetJobGauge<ASTGauge>();
                var lucidDreaming = GetCooldown(AST.LucidDreaming);
                var fallmalefic = GetCooldown(AST.FallMalefic);
                var minorarcanaCD = GetCooldown(AST.MinorArcana);
                var drawCD = GetCooldown(AST.Draw);
                var actionIDCD = GetCooldown(actionID);



                if (IsEnabled(CustomComboPreset.AstrologianLightSpeedFeature) && level >= 6)
                {
                    var lightspeed = GetCooldown(AST.Lightspeed);
                    if (!lightspeed.IsCooldown && lastComboMove == OriginalHook(actionID) && actionIDCD.CooldownRemaining >= 0.6)
                        return AST.Lightspeed;
                }
                if (!HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat))
                {
                    return OriginalHook(AST.Malefic1);
                }
                if (IsEnabled(CustomComboPreset.AstrologianAstrodyneFeature) && level >= 50)
                {
                    if (!gauge.ContainsSeal(SealType.NONE) && incombat && fallmalefic.CooldownRemaining >= 0.6 && level >= 50)
                        return AST.Astrodyne;
                }
                if (IsEnabled(CustomComboPreset.AstrologianAutoDrawFeature) && level >= 30)
                {
                    if (gauge.DrawnCard.Equals(CardType.NONE) && incombat && actionIDCD.CooldownRemaining >= 0.6 && drawCD.CooldownRemaining < 30 && level >= 30)
                        return AST.Draw;

                }
                if (IsEnabled(CustomComboPreset.AstrologianAutoCrownDrawFeature) && level >= 70)
                {
                    if (gauge.DrawnCrownCard == CardType.NONE && incombat && minorarcanaCD.CooldownRemaining == 0 && actionIDCD.CooldownRemaining >= 0.6 && level >= 70)
                        return AST.MinorArcana;
                }
                if (IsEnabled(CustomComboPreset.AstrologianLucidFeature) && level >= 24)
                {
                    if (!lucidDreaming.IsCooldown && LocalPlayer.CurrentMp <= 8000 && fallmalefic.CooldownRemaining > 0.2 && level >= 24)
                        return AST.LucidDreaming;
                }
                if (IsEnabled(CustomComboPreset.AstrologianLazyLordFeature) && level >= 70)
                {
                    if (gauge.DrawnCrownCard == CardType.LORD && incombat && actionIDCD.CooldownRemaining >= 0.6 && level >= 70)
                        return AST.LordOfCrowns;
                }
                if (IsEnabled(CustomComboPreset.AstrologianAlternateDpsFeature) && level >= 72 && incombat)
                {
                    if ((combust3Debuff is null) || (combust3Debuff.RemainingTime <= 3))
                        return AST.Combust3;
                }

                if (IsEnabled(CustomComboPreset.AstrologianAlternateDpsFeature) && level >= 46 && level <= 71 && incombat)
                {
                    if ((combust2Debuff is null) || (combust2Debuff.RemainingTime <= 3))
                        return AST.Combust2;
                }

                if (IsEnabled(CustomComboPreset.AstrologianAlternateDpsFeature) && level >= 4 && level <= 45 && incombat)
                {
                    if ((combust1Debuff is null) || (combust1Debuff.RemainingTime <= 3))
                        return AST.Combust1;
                }
                return OriginalHook(AST.Malefic1);
            }
            return actionID;
        }
    }
    internal class CustomValuesTest : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.CustomValuesTest;

        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == AST.FallMalefic || actionID == AST.Malefic4 || actionID == AST.Malefic3 || actionID == AST.Malefic2 || actionID == AST.Malefic1)
            {

                var incombat = HasCondition(Dalamud.Game.ClientState.Conditions.ConditionFlag.InCombat);
                var combust3Debuff = FindTargetEffect(AST.Debuffs.Combust3);
                var combust2Debuff = FindTargetEffect(AST.Debuffs.Combust2);
                var combust1Debuff = FindTargetEffect(AST.Debuffs.Combust1);
                var gauge = GetJobGauge<ASTGauge>();
                var lucidDreaming = GetCooldown(AST.LucidDreaming);
                var fallmalefic = GetCooldown(AST.FallMalefic);
                var minorarcanaCD = GetCooldown(AST.MinorArcana);
                var drawCD = GetCooldown(AST.Draw);
                var actionIDCD = GetCooldown(actionID);
                var MaxHpValue = Service.Configuration.EnemyHealthMaxHp;
                var PercentageHpValue = Service.Configuration.EnemyHealthPercentage;
                var CurrentHpValue = Service.Configuration.EnemyCurrentHp;


                if (IsEnabled(CustomComboPreset.AstrologianLightSpeedFeature) && level >= 6)
                {
                    var lightspeed = GetCooldown(AST.Lightspeed);
                    if (!lightspeed.IsCooldown && lastComboMove == OriginalHook(actionID) && actionIDCD.CooldownRemaining >= 0.4)
                        return AST.Lightspeed;
                }
                if (IsEnabled(CustomComboPreset.AstrologianAstrodyneFeature) && level >= 50)
                {
                    if (!gauge.ContainsSeal(SealType.NONE) && lastComboMove == OriginalHook(actionID) && fallmalefic.CooldownRemaining >= 0.4 && level >= 50)
                        return AST.Astrodyne;
                }
                if (IsEnabled(CustomComboPreset.AstrologianAutoDrawFeature) && level >= 30)
                {
                    if (gauge.DrawnCard.Equals(CardType.NONE) && lastComboMove == OriginalHook(actionID) && actionIDCD.CooldownRemaining >= 0.4 && drawCD.CooldownRemaining < 30 && level >= 30)
                        return AST.Draw;

                }
                if (IsEnabled(CustomComboPreset.AstrologianAutoCrownDrawFeature) && level >= 70)
                {
                    if (gauge.DrawnCrownCard == CardType.NONE && lastComboMove == OriginalHook(actionID) && minorarcanaCD.CooldownRemaining == 0 && actionIDCD.CooldownRemaining >= 0.4 && level >= 70)
                        return AST.MinorArcana;
                }
                if (IsEnabled(CustomComboPreset.AstrologianLucidFeature) && level >= 24)
                {
                    if (!lucidDreaming.IsCooldown && LocalPlayer.CurrentMp <= 8000 && fallmalefic.CooldownRemaining > 0.4 && level >= 24)
                        return AST.LucidDreaming;
                }
                if (IsEnabled(CustomComboPreset.AstrologianLazyLordFeature) && level >= 70)
                {
                    var buff = HasEffect(AST.Buffs.Divination);
                    var buffcd = GetCooldown(AST.Divination);
                    if (gauge.DrawnCrownCard == CardType.LORD && lastComboMove == OriginalHook(actionID) && actionIDCD.CooldownRemaining >= 0.4 && level >= 70 && buff || gauge.DrawnCrownCard == CardType.LORD && incombat && actionIDCD.CooldownRemaining >= 0.4 && level >= 70 && buffcd.IsCooldown)
                        return AST.LordOfCrowns;
                }
                if (IsEnabled(CustomComboPreset.CustomValuesTest) && level >= 72 && incombat)
                {
                    if ((combust3Debuff is null) && EnemyHealthMaxHp() > MaxHpValue && EnemyHealthPercentage() > PercentageHpValue && EnemyHealthCurrentHp() > CurrentHpValue || (combust3Debuff.RemainingTime <= 3) && EnemyHealthPercentage() > PercentageHpValue && EnemyHealthCurrentHp() > CurrentHpValue)
                        return AST.Combust3;
                }

                if (IsEnabled(CustomComboPreset.CustomValuesTest) && level >= 46 && level <= 71 && incombat)
                {
                    if ((combust2Debuff is null) || (combust2Debuff.RemainingTime <= 3))
                        return AST.Combust2;
                }

                if (IsEnabled(CustomComboPreset.CustomValuesTest) && level >= 4 && level <= 45 && incombat)
                {
                    if ((combust1Debuff is null) || (combust1Debuff.RemainingTime <= 3))
                        return AST.Combust1;
                }
            }
            return actionID;
        }

    }

    internal class AstrologianSimpleSingleTargetHeal : CustomCombo
    {
        protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.AstrologianSimpleSingleTargetHeal;
        protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
        {
            if (actionID == AST.Benefic2)
            {
                var aspectedBeneficHoT = FindTargetEffect(AST.Buffs.AspectedBenefic);
                var NeutralSectBuff = FindTargetEffect(AST.Buffs.NeutralSect);
                var NeutralSectShield = FindTargetEffect(AST.Buffs.NeutralSectShield);
                var customEssentialDignity = Service.Configuration.GetCustomIntValue(AST.Config.AstroEssentialDignity);
                var exaltationCD = GetCooldown(AST.Exaltation);

                if (IsEnabled(CustomComboPreset.AspectedBeneficFeature) && ((aspectedBeneficHoT is null) || (aspectedBeneficHoT.RemainingTime <= 3) || (NeutralSectShield is null)) && (NeutralSectBuff is not null))
                    return AST.AspectedBenefic;

                if (IsEnabled(CustomComboPreset.AstroEssentialDignity) && GetCooldown(AST.EssentialDignity).RemainingCharges > 0 && level >= AST.Levels.EssentialDignity && EnemyHealthPercentage() <= customEssentialDignity)
                    return AST.EssentialDignity;

                if (IsEnabled(CustomComboPreset.ExaltationFeature) && exaltationCD.CooldownRemaining == 0 && level >= AST.Levels.Exaltation)
                    return AST.Exaltation;

                if (IsEnabled(CustomComboPreset.CelestialIntersectionFeature) && GetCooldown(AST.CelestialIntersection).RemainingCharges > 0 && level >= AST.Levels.CelestialIntersection)
                    return AST.CelestialIntersection;
            }


            return actionID;
        }

    }
}