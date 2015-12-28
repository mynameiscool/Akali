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
    class Modes
    {
        public static void Combo()
        {
            var Q = Program.Q;
            var E = Program.E;
            var R = Program.R;
            var useQ = Program.SkillMenu["QCombo"].Cast<CheckBox>().CurrentValue;
            var useR = Program.SkillMenu["RCombo"].Cast<CheckBox>().CurrentValue;
            var useE = Program.SkillMenu["ECombo"].Cast<CheckBox>().CurrentValue;
            var targetE = TargetSelector.GetTarget(Program.E.Range, DamageType.Magical);
            var x = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            if (x != null && ObjectManager.Player.Distance(x) <= Program.hextech.Range)
            {
                Program.hextech.Cast(x);
            }

            if (x != null && ObjectManager.Player.Distance(x) <= Program.cutlass.Range)
            {
                Program.cutlass.Cast(x);
            }

            if (Program.Q.IsReady() && useQ)
            {
                var target = TargetSelector.GetTarget(Program.Q.Range, DamageType.Magical);
                if (target.IsValidTarget(Program.Q.Range) && !target.IsZombie)
                {
                    Program.Q.Cast(target);
                }
            }
            if (Program.R.IsReady() && useR)
            {
                var target = TargetSelector.GetTarget(Program.R.Range, DamageType.Magical);
                if (target.IsValidTarget(Program.R.Range) && !target.IsZombie)
                {

                    Program.R.Cast(target);
                }
            }
            if (E.IsReady() && useE)
            {
                var xe = TargetSelector.GetTarget((int)_Player.GetAutoAttackRange() - 10, DamageType.Physical);
                if (!targetE.IsValidTarget(Player.Instance.GetAutoAttackRange(targetE)) && !targetE.IsZombie)
                {
                    if (targetE.IsValidTarget(Program.E.Range)) { Program.E.Cast(); }
                }
            }
        }

        public static void Harass()
        {
            var Q = Program.Q;
            var E = Program.E;
            var R = Program.R;
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            if (target == null) return;
            var useQ = Program.SkillMenu["QHarass"].Cast<CheckBox>().CurrentValue;
            if (Q.IsReady() && useQ && target.IsValidTarget(Q.Range) && !target.IsZombie)
            {
                Q.Cast(target);
            }
        }
        public static void JungleFarm()
        {
            /*var useE = Program. FarmingMenu["JCE"].Cast<CheckBox>().CurrentValue;
            var useQ = Program.FarmingMenu["JCQ"].Cast<CheckBox>().CurrentValue;
            var Q = Program.Q;
            var E = Program.E;
            var R = Program.R;
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

                E.Cast();
            }*/
            return;
        }

        public static void LaneClear()
        {
            var useE = Program.FarmingMenu["ELC"].Cast<CheckBox>().CurrentValue;
            var Q = Program.Q;
            var E = Program.E;
            var R = Program.R;
            //var ElEnergy = FarmingMenu["ELCEnergy"].Cast<Slider>().CurrentValue;
            var target = EntityManager.MinionsAndMonsters.GetLaneMinions().OrderByDescending(x => x.MaxHealth).FirstOrDefault(x => x.IsValidTarget(E.Range));
            //var enemy = (Obj_AI_Minion)GetEnemy(Program.Q.Range, GameObjectType.obj_AI_Minion);
            var target2 = EntityManager.MinionsAndMonsters.Monsters.OrderByDescending(a => a.MaxHealth).FirstOrDefault(a => a.Distance(Player.Instance) < Player.Instance.GetAutoAttackRange(a));

            if (target != null)
            {
                Program.E.Cast();
            }
            if (target2 != null)
            {
                Program.Q.Cast(target2);
                Program.E.Cast();
            }
        }

        public static void LastHit()
        {
            var Q = Program.Q;
            var E = Program.E;
            var R = Program.R;
            var useW = Program.FarmingMenu["QLH"].Cast<CheckBox>().CurrentValue;
            var minions = ObjectManager.Get<Obj_AI_Base>().OrderBy(m => m.Health).Where(m => m.IsMinion && m.IsEnemy && !m.IsDead);
            foreach (var minion in minions)
            {
                var qdmg = _Player.GetSpellDamage(minion, SpellSlot.Q) + 20;
                if (useW && Q.IsReady() && minion.Health <= qdmg)
                {
                    Q.Cast(minion);
                }
            }
        }
        public static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }
        }
    }
}
