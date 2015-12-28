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
    class drawings
    {
        public static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }
        }
        public static void OnDraw(EventArgs args)
        {
            if (Program.R.IsReady() && Program.MiscMenu["drawR"].Cast<CheckBox>().CurrentValue)

            {
                Drawing.DrawCircle(_Player.Position, Program.R.Range, Color.Red);
            }

            if (Program.Q.IsReady() && Program.MiscMenu["drawQ"].Cast<CheckBox>().CurrentValue)
            {
                Drawing.DrawCircle(_Player.Position, Program.Q.Range, Color.DarkGreen);
            }

            if (Program.E.IsReady() && Program.MiscMenu["drawE"].Cast<CheckBox>().CurrentValue)
            {
                Drawing.DrawCircle(_Player.Position, Program.E.Range, Color.DarkGray);
            }
        }

    }
}
