using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using Color = System.Drawing.Color;
using SharpDX;


namespace Akali
{
    class Program
    {
        public static Spell.Targeted Q;
        public static Spell.Active E, W;
        public static Spell.Targeted R;
        static Spell.Targeted Ignite = null;
        private static Item hextech;
        private static Item cutlass;
        public static Menu Menu, SkillMenu, FarmingMenu, MiscMenu;
        public static HitChance MinimumHitChance { get; set; }

        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        public static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }
        }

        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            if (Player.Instance.ChampionName != "Akali")
                return;

            Bootstrap.Init(null);

            uint level = (uint)Player.Instance.Level;
            Q = new Spell.Targeted(SpellSlot.Q, 600);
            R = new Spell.Targeted(SpellSlot.R, 700);
            E = new Spell.Active(SpellSlot.E, 290);
            W = new Spell.Active(SpellSlot.W, 700);
            hextech = new Item(3146, 700);
            cutlass = new Item(3144, 450);
            Menu = MainMenu.AddMenu("Akali", "helloakali");
            Menu.AddSeparator();
            Menu.AddLabel("Created by MyNameIsCool");
            SkillMenu = Menu.AddSubMenu("Skills", "Skills");
            SkillMenu.AddGroupLabel("Skills");
            SkillMenu.AddLabel("Combo");
            SkillMenu.Add("QCombo", new CheckBox("Use Q in Combo"));
            SkillMenu.Add("ECombo", new CheckBox("Use E in Combo"));
            SkillMenu.Add("RCombo", new CheckBox("Use R in Combo"));
            SkillMenu.AddLabel("Harass");
            SkillMenu.Add("QHarass", new CheckBox("Use Q on Harass"));
            FarmingMenu = Menu.AddSubMenu("Farming", "Farming");
            FarmingMenu.AddGroupLabel("Farming");
            FarmingMenu.AddLabel("LastHit");
            FarmingMenu.Add("QLH", new CheckBox("Use Q to secure last hits", false));
            //FarmingMenu.Add("ELHMana", new Slider("Mana Manager for E", 60, 0, 100));
            FarmingMenu.AddLabel("LaneClear");
            FarmingMenu.Add("ELC", new CheckBox("Use E on LaneClear"));
            //FarmingMenu.Add("ELCMana", new Slider("Mana Manager for E", 50, 0, 100));
            FarmingMenu.AddLabel("Jungle");
            FarmingMenu.Add("JCQ", new CheckBox("Use Q"));
            FarmingMenu.Add("JCE", new CheckBox("Use E"));
            MiscMenu = Menu.AddSubMenu("Misc", "Misc");
            MiscMenu.AddGroupLabel("Misc");
            Menu.Add("Auto Ignite if killable", new CheckBox("Auto Ignite"));
            MiscMenu.AddLabel("KillSteal");
            MiscMenu.Add("Rkill", new CheckBox("Use R to KillSteal"));
            Game.OnTick += Game_OnTick;
            Orbwalker.OnPostAttack += Orbwalker_OnPostAttack;
            Chat.Print("Cool Addon Loaded -= Akali =-", System.Drawing.Color.White);
            SpellDataInst dot = ObjectManager.Player.Spellbook.Spells.Where(spell => spell.Name.Contains("dot")).Any() ? ObjectManager.Player.Spellbook.Spells.Where(spell => spell.Name.Contains("dot")).First() : null;
            if (dot != null)
            {
                Ignite = new Spell.Targeted(dot.Slot, 600);
            }
        }
        private static void Game_OnTick(EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Harass();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                JungleFarm();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                LaneClear();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
            {
                LastHit();
            }
            KillSteal();
        }
        private static void Combo()
        {
            var useQ = SkillMenu["QCombo"].Cast<CheckBox>().CurrentValue;
            var useR = SkillMenu["RCombo"].Cast<CheckBox>().CurrentValue;
            var useE = SkillMenu["ECombo"].Cast<CheckBox>().CurrentValue;
            var targetE = TargetSelector.GetTarget(E.Range, DamageType.Magical);
            var x = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            if (x != null && ObjectManager.Player.Distance(x) <= hextech.Range)
            {
                hextech.Cast(x);
            }

            if (x != null && ObjectManager.Player.Distance(x) <= cutlass.Range)
            {
                cutlass.Cast(x);
            }

            if (Q.IsReady() && useQ)
            {
                var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
                if (target.IsValidTarget(Q.Range) && !target.IsZombie)
                {
                    Q.Cast(target);
                }
            }
            if (R.IsReady()  && useR)
            {
                var target = TargetSelector.GetTarget(R.Range, DamageType.Magical);
                if (target.IsValidTarget(R.Range) && !target.IsZombie)
                {

                    R.Cast(target);
                }
            }
            if (ObjectManager.Player.Distance(targetE) >= ObjectManager.Player.GetAutoAttackRange() - 30 && useE)
            {
                if (targetE.IsValidTarget(E.Range) && !targetE.IsZombie)
                {
                    E.Cast();
                }
            }
        }
        private static void KillSteal()
        {
            var target = TargetSelector.GetTarget(E.Range, DamageType.Physical);
            if (target == null) return;
            var useR = MiscMenu["Rkill"].Cast<CheckBox>().CurrentValue;

            if (R.IsReady() && useR && target.IsValidTarget(R.Range) && !target.IsZombie && target.Health <= _Player.GetSpellDamage(target, SpellSlot.R))
            {
                R.Cast(target);
            }

            var useI = MiscMenu["Auto Ignite"].Cast<CheckBox>().CurrentValue;
            if (Ignite.IsReady())
            {
                var IgniteEnemy = EntityManager.Heroes.Enemies.FirstOrDefault(it => DamageLibrary.GetSummonerSpellDamage(ObjectManager.Player, it, DamageLibrary.SummonerSpells.Ignite) >= it.Health - 30);

                if (IgniteEnemy != null)
                {
                    if ((useI && IgniteEnemy.Distance(ObjectManager.Player) >= 300 || ObjectManager.Player.HealthPercent <= 40))
                    {
                        Ignite.Cast(IgniteEnemy);
                    }
                }
            }
        }
        private static void Harass()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            if (target == null) return;
            var useQ = SkillMenu["QHarass"].Cast<CheckBox>().CurrentValue;
            if (Q.IsReady() && useQ && target.IsValidTarget(Q.Range) && !target.IsZombie)
            {
                Q.Cast(target);
            }
        }

        private static void JungleFarm()
        {
            var useE = FarmingMenu["JCE"].Cast<CheckBox>().CurrentValue;
            var useQ = FarmingMenu["JCQ"].Cast<CheckBox>().CurrentValue;
            //var Wmana = FarmingMenu["ELCMana"].Cast<Slider>().CurrentValue;
            //var target = EntityManager.MinionsAndMonsters.GetJungleMonsters().OrderByDescending(x => x.MaxHealth).FirstOrDefault(x => x.IsValidTarget(Q.MaximumRange));
            if (useQ && Q.IsReady())
            {
                var target = EntityManager.MinionsAndMonsters.GetJungleMonsters().OrderByDescending(x => x.MaxHealth).FirstOrDefault(x => x.IsValidTarget(Q.Range));

                Q.Cast(target);
            }
            if (useE && E.IsReady())
            {
                var target = EntityManager.MinionsAndMonsters.GetJungleMonsters().OrderByDescending(x => x.MaxHealth).FirstOrDefault(x => x.IsValidTarget(E.Range));

                E.Cast(target);
            }
        }

        private static void LaneClear()
        {
            var useE = FarmingMenu["ELC"].Cast<CheckBox>().CurrentValue;
            //var Wmana = FarmingMenu["ELCMana"].Cast<Slider>().CurrentValue;
            var minions = ObjectManager.Get<Obj_AI_Base>().OrderBy(m => m.Health).Where(m => m.IsMinion && m.IsEnemy && !m.IsDead);
            foreach (var minion in minions)
            {
                if (useE && E.IsReady() && minion.Health <= _Player.GetSpellDamage(minion, SpellSlot.E))
                {
                    E.Cast(minion);
                }
            }
        }

        private static void LastHit()
        {
            var useW = FarmingMenu["QLH"].Cast<CheckBox>().CurrentValue;
            var minions = ObjectManager.Get<Obj_AI_Base>().OrderBy(m => m.Health).Where(m => m.IsMinion && m.IsEnemy && !m.IsDead);
            foreach (var minion in minions)
            {
                if (useW && Q.IsReady() && minion.Health <= _Player.GetSpellDamage(minion, SpellSlot.Q))
                {
                    Q.Cast(minion);
                }
            }
        }
        static void Orbwalker_OnPostAttack(AttackableUnit target, EventArgs args)
        {
            if (target != null)
            {
                if (E.IsReady() && ObjectManager.Player.Distance(target) <= E.Range - 30)
                {
                    if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))// && Menu["UseWAARCombo"].Cast<CheckBox>().CurrentValue)
                    {
                        E.Cast();
                        Orbwalker.ResetAutoAttack();
                        EloBuddy.Player.IssueOrder(GameObjectOrder.AttackTo, target);
                        return;
                    }

                    if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))// && Menu["UseWAARHarass"].Cast<CheckBox>().CurrentValue)
                    {
                        E.Cast();
                        Orbwalker.ResetAutoAttack();
                        EloBuddy.Player.IssueOrder(GameObjectOrder.AttackTo, target);
                        return;
                    }
                }
            }

            return;
        }

    }
}