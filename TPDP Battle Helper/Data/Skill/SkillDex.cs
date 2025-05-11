using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TPDP_Battle_Helper.Data.Dex;

namespace TPDP_Battle_Helper.Data.Skill
{
    internal class SkillDex
    {

        public static readonly Dictionary<ushort, SkillDexEntry> SkillList = [];

        public static void Init()
        {

            XmlDocument doc = new XmlDocument();
            doc.Load(".\\Config\\Skills.xml");

            XmlNode baseNode = doc.DocumentElement;

            foreach (XmlNode node in baseNode.ChildNodes)
            {
                if (node.NodeType == XmlNodeType.Element)
                {
                    ushort skillId = (ushort)Convert.ToInt16(node.Attributes["id"].Value, 16);
                    SkillDexEntry entry = new SkillDexEntry(node);
                    SkillList.Add(skillId, entry);
                }
            }

        }

        public static SkillDexEntry? FindSkill(ushort skillId)
        {
            if (!SkillList.ContainsKey(skillId)) return null;
            return SkillList[skillId];
        }

    }
}
