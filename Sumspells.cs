using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using System.Threading.Tasks;
using Color = System.Drawing.Color;
using SharpDX;

namespace Akali
{
    class Sumspells
    {
        public static void AutoIgniter()
        {
            var AutoIgnite = Program.MiscMenu["AI"].Cast<CheckBox>().CurrentValue;
            foreach (var enemy in EntityManager.Heroes.Enemies)
            {
                if (Program.Ignite != null && AutoIgnite)
                {
                    if (Program.Ignite.IsReady() &&
                        _Player.GetSummonerSpellDamage(enemy, DamageLibrary.SummonerSpells.Ignite) >=
                        enemy.TotalShieldHealth() && enemy.Health < 50 + 20 * _Player.Level - (enemy.HPRegenRate / 5 * 3))
                    {
                        Program.Ignite.Cast(enemy);
                    }
                }
            }
        }
        public static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }
        }
    }
}
