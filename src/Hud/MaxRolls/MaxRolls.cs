using System;
using System.Collections.Generic;
using System.Drawing;
using System.Xml.Linq;

namespace PoeHUD.Hud.MaxRolls
{
    public static class MaxRolls
    {
        public static Dictionary<string, MaxRolls_Mod> data;

        static MaxRolls()
        {
            XDocument doc = XDocument.Load("config/rolls.xml");
            data = new Dictionary<string, MaxRolls_Mod>();
            if (doc == null || doc.Root == null)
                throw new ApplicationException("invalid data");
            foreach (XElement m in doc.Root.Elements("mod"))
            {
                MaxRolls_Mod mod = new MaxRolls_Mod(m);
                data.Add(m.Attribute("name").Value, mod);
            }
        }
        public static void Initialize() {
            return;
        }
    }

	public class MaxRolls_Current
    {
        public string name;
        public Color color;
        public string curr;
        public string max;
        public string curr2;
        public string max2;
        public int tier;

        public MaxRolls_Current(string modName, int modLevel, int ilvl)
        {
            if (MaxRolls.data.ContainsKey(modName))
            {
                var current = MaxRolls.data[modName];
                var currentMod = current.modLevel;
                tier = (current.modLevel.Count - modLevel + 1);
                this.name = '[' + MaxRolls.data[modName].type + "] " +"[T" + tier.ToString() + "] "+ MaxRolls.data[modName].name;
                var currentLvl = currentMod[modLevel-1];
                int count = currentMod.Count - 1;
                // Color
                switch (tier)
                {                
                    case 1:
                        this.color = Color.Green;
                        break;
                    case 2:
                        this.color = Color.Yellow;
                        break;
                    default:
                        this.color = Color.White;
                        break;

                }
                // Current
                if (currentLvl.min == currentLvl.max)
                    curr = currentLvl.max;
                else
                    curr = currentLvl.min + "-" + currentLvl.max;
                // Current 2
                if (current.multi) {
                    if (currentLvl.min2 == currentLvl.max2)
                        curr2 = currentLvl.max2;
                    else
                        curr2 = currentLvl.min2 + "-" + currentLvl.max2;
                } else {
                    curr2 = null;
                }
                // Maximum
                max = "---";
                max2 = null;
                if (ilvl >= currentMod[count].ilvl)
                {
                    // Special condition if max of last level < max of previous
                    if (count>0 && currentMod[count].ilvl == currentMod[count - 1].ilvl)
                    {
                        max = currentMod[count].min + "-" + currentMod[count].max;
                        if (current.multi) 
                            max2 = currentMod[count].min2 + "-" + currentMod[count].max2;
                    }
                    else
                    {
                        max = currentMod[0].min + "-" + currentMod[count].max;
                        if (current.multi)
                            max2 = currentMod[0].min2 + "-" + currentMod[count].max2;
                    }
                }
                else {
                    // If we got mod on impossible ilvl
                    if (currentMod[0].ilvl > ilvl)
                    {
                        System.Windows.Forms.MessageBox.Show("Check rolls.xml, item contains mod " + modName + " which shouldnt have it at this ilvl");
                    }
                    else
                    {
                        for (int i = 1; i <= count; i++)
                        {
                            if (currentMod[i].ilvl > ilvl)
                            {
                                max = currentMod[0].min + "-" + currentMod[i - 1].max;
                                if (current.multi)
                                    max2 = currentMod[0].min2 + "-" + currentMod[i - 1].max2;
                                if (currentLvl.max == currentMod[i - 1].max)
                                    this.color = Color.Yellow;
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                name = modName + " (" + modLevel + ")";
                color = Color.White;
                curr = "---";
                max = "---";
                curr2 = null;
                max2 = null;
            }
        }
    }
}