using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TPDP_Battle_Helper.Data.Skill
{
    internal class SkillDexEntry
    {

        public short InternalId { get; }
        public string SkillName { get; }
        public string Description { get; }
        public ElementalType SkillType { get; }
        public SkillCategory SkillCategory { get; }
        public int Sp { get; }
        public int Accuracy { get; }
        public int Power { get; }
        public short Priority { get; }

        public SkillDexEntry(XmlNode baseNode)
        {
            foreach (XmlNode node in baseNode.ChildNodes)
            {
                switch (node.Name)
                {
                    case "internalId":
                        InternalId = Convert.ToInt16(node.InnerText, 16);
                        break;
                    case "name":
                        SkillName = node.InnerText;
                        break;
                    case "description":
                        Description = node.InnerText;
                        break;
                    case "type":
                        SkillType = ElementalType.FindByName(node.InnerText);
                        break;
                    case "category":
                        SkillCategory = SkillCategory.FindByName(node.InnerText);
                        break;
                    case "sp":
                        Sp = int.Parse(node.InnerText);
                        break;
                    case "accuracy":
                        Accuracy = int.Parse(node.InnerText);
                        break;
                    case "power":
                        Power = int.Parse(node.InnerText);
                        break;
                    case "priority":
                        Priority = short.Parse(node.InnerText);
                        break;
                }
            }
        }

    }
}
