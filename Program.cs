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
        public static Spell.Targeted Ignite;
        public static Item hextech;
        public static Item cutlass;
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
            FarmingMenu.AddLabel("LaneClear");
            FarmingMenu.Add("ELC", new CheckBox("Use E on LaneClear"));
            FarmingMenu.Add("ELCEnergy", new Slider("Energy Manager for E", 20, 0, 100));
            FarmingMenu.AddLabel("Jungle");
            FarmingMenu.Add("JCQ", new CheckBox("Use Q"));
            FarmingMenu.Add("JCE", new CheckBox("Use E"));
            MiscMenu = Menu.AddSubMenu("Misc", "Misc");
            MiscMenu.AddGroupLabel("Misc");
            MiscMenu.AddLabel("Drawings");
            MiscMenu.Add("DrawQ", new CheckBox("Draw Q Range"));
            MiscMenu.Add("DrawE", new CheckBox("Draw E Range"));
            MiscMenu.Add("DrawR", new CheckBox("Draw R Range"));
            MiscMenu.Add("AI", new CheckBox("Auto Ignite"));
            MiscMenu.AddLabel("KillSteal");
            MiscMenu.Add("Rkill", new CheckBox("Use R to KillSteal"));
            Game.OnTick += Game_OnTick;
            Orbwalker.OnPostAttack += Orbwalker_OnPostAttack;
            Drawing.OnDraw += Akali.drawings.OnDraw;
            var slot = _Player.GetSpellSlotFromName("summonerdot");
            if (slot != SpellSlot.Unknown)
            {
                Ignite = new Spell.Targeted(slot, 600);
            }
            Chat.Print("Cool Addon Loaded -= Akali =-", System.Drawing.Color.White);
        }
        private static void Game_OnTick(EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Modes.Combo();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Modes.Harass();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear)))
            {
                Modes.LaneClear();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
            {
                Modes.LastHit();
            }
            KillSteal();
        }
        private static void KillSteal()
        {
            var target = TargetSelector.GetTarget(R.Range, DamageType.Magical);
            if (target == null) return;
            var useR = MiscMenu["Rkill"].Cast<CheckBox>().CurrentValue;

            if (R.IsReady() && useR && target.IsValidTarget(R.Range) && !target.IsZombie && target.Health <= _Player.GetSpellDamage(target, SpellSlot.R))
            {
                R.Cast(target);
            }
            Sumspells.AutoIgniter();
        }

        static void Orbwalker_OnPostAttack(AttackableUnit target, EventArgs args)
        {
            if (target != null)
            {
                if (E.IsReady() && ObjectManager.Player.Distance(target) <= E.Range - 30)
                {
                    if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                    {
                        E.Cast();
                        Orbwalker.ResetAutoAttack();
                        EloBuddy.Player.IssueOrder(GameObjectOrder.AttackTo, target);
                        return;
                    }

                    if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
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