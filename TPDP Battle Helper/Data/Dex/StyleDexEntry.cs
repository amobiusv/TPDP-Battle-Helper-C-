
using System.Xml;

namespace TPDP_Battle_Helper.Data.Dex
{
    internal class StyleDexEntry
    {

        public ElementalType Type1 { get; }
        public ElementalType? Type2 { get; }
        public Object Ability1 { get; }
        public Object? Ability2 { get; }
        public PuppetStats BaseStats { get; }

        public StyleDexEntry(XmlNode baseNode)
        {
            foreach (XmlNode node in baseNode.ChildNodes)
            {
                switch (node.Name)
                {
                    case "type1":
                        Type1 = ElementalType.FindByName(node.InnerText);
                        break;
                    case "type2":
                        Type2 = ElementalType.FindByName(node.InnerText);
                        break;
                    case "ability1":
                        Ability1 = node.InnerText;
                        break;
                    case "ability2":
                        Ability2 = node.InnerText;
                        break;
                    case "baseStats":
                        BaseStats = new PuppetStats(node);
                        break;
                }
            }
        }

    }
}
