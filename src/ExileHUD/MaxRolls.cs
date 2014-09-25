using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

namespace ExileHUD
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
    public class MaxRolls_Mod
    {
        public readonly string name;
        public readonly string type;
        public readonly bool multi;
        public List<MaxRolls_ModLevel> modLevel;

        public MaxRolls_Mod(XElement m)
        {
            name = m.Attribute("readable").Value;
            type = m.Attribute("type").Value;
            modLevel = new List<MaxRolls_ModLevel>();
            foreach (XElement level in m.Elements())
            {
                modLevel.Add(new MaxRolls_ModLevel(level));
            }
            if (modLevel[0].min2 != null && modLevel[0].max2 != null)
                multi = true;
        }

        public class MaxRolls_ModLevel
        {
            public readonly string min;
            public readonly string max;
            public readonly string min2;
            public readonly string max2;
            public readonly int lvl;
            public readonly int ilvl;
            public MaxRolls_ModLevel(XElement ml)
            {
                this.ilvl = Convert.ToInt32(ml.Attribute("ilvl").Value);
                this.min = ml.Element("min").Value;
                this.max = ml.Element("max").Value;
                if (ml.Element("min2") != null && ml.Element("max2") != null)
                {
                    this.min2 = ml.Element("min2").Value;
                    this.max2 = ml.Element("max2").Value;
                }
                else
                {
                    this.min2 = null;
                    this.max2 = null;
                }
            }
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

        public MaxRolls_Current(string modName, int modLevel, int ilvl)
        {
            if (MaxRolls.data.ContainsKey(modName))
            {
                this.name = '[' + MaxRolls.data[modName].type + "] " + MaxRolls.data[modName].name;
                var current = MaxRolls.data[modName];
                var currentMod = current.modLevel;
                var currentLvl = currentMod[modLevel-1];
                int count = currentMod.Count - 1;
                // Color
                if (currentLvl.max == currentMod[count].max)
                    this.color = Color.Green;
                else
                    this.color = Color.White;
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