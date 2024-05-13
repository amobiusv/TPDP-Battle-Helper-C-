
using System.Xml;

namespace TPDP_Battle_Helper.Data.Dex
{
    internal class StyleDexEntry
    {

        public byte StyleId {  get; }
        public ElementalType Type1 { get; }
        public ElementalType? Type2 { get; }
        public Object Ability1 { get; }
        public Object? Ability2 { get; }
        public PuppetStats BaseStats { get; }

        public StyleDexEntry(XmlNode baseNode, byte styleId)
        {
            this.StyleId = styleId;
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

        public ElementalType[] IsSuperWeakTo()
        {
            List<ElementalType> result = [];

            ElementalType[] types1 = Type1.DefensivelyIsWeakTo();
            ElementalType[] types2 = [];
            if (Type2 != null)
                types2 = Type2.DefensivelyIsWeakTo();

            foreach (ElementalType type in types1)
            {
                if (types2.Contains(type))
                {
                    result.Add(type);
                }
            }

            result.Sort(new ElementalType.ElementalTypeComparer());
            return result.ToArray();
        }

        public ElementalType[] IsWeakTo()
        {
            List<ElementalType> result = [];

            ElementalType[] typesWeak1 = Type1.DefensivelyIsWeakTo();
            ElementalType[] typesWeak2 = [];
            if (Type2 != null)
                typesWeak2 = Type2.DefensivelyIsWeakTo();
            ElementalType[] typesResistant1 = Type1.DefensivelyIsResistantTo();
            ElementalType[] typesResistant2 = [];
            if (Type2 != null)
                typesResistant2 = Type2.DefensivelyIsResistantTo();

            foreach (ElementalType type in typesWeak1)
            {
                if (!typesWeak2.Contains(type) && !typesResistant2.Contains(type))
                {
                    result.Add(type);
                }
            }

            foreach (ElementalType type in typesWeak2)
            {
                if (!typesWeak1.Contains(type) && !typesResistant1.Contains(type) && !result.Contains(type))
                {
                    result.Add(type);
                }
            }

            result.Sort(new ElementalType.ElementalTypeComparer());
            return result.ToArray();
        }

        public ElementalType[] IsResistantTo()
        {
            List<ElementalType> result = [];

            ElementalType[] typesWeak1 = Type1.DefensivelyIsWeakTo();
            ElementalType[] typesWeak2 = [];
            if (Type2 != null)
                typesWeak2 = Type2.DefensivelyIsWeakTo();
            ElementalType[] typesResistant1 = Type1.DefensivelyIsResistantTo();
            ElementalType[] typesResistant2 = [];
            if (Type2 != null)
                typesResistant2 = Type2.DefensivelyIsResistantTo();

            foreach (ElementalType type in typesResistant1)
            {
                if (!typesWeak2.Contains(type) && !typesResistant2.Contains(type))
                {
                    result.Add(type);
                }
            }

            foreach (ElementalType type in typesResistant2)
            {
                if (!typesWeak1.Contains(type) && !typesResistant1.Contains(type) && !result.Contains(type))
                {
                    result.Add(type);
                }
            }

            result.Sort(new ElementalType.ElementalTypeComparer());
            return result.ToArray();
        }

        public ElementalType[] IsSuperResistantTo()
        {
            List<ElementalType> result = [];

            ElementalType[] types1 = Type1.DefensivelyIsResistantTo();
            ElementalType[] types2 = [];
            if (Type2 != null)
                types2 = Type2.DefensivelyIsResistantTo();

            foreach (ElementalType type in types1)
            {
                if (types2.Contains(type))
                {
                    result.Add(type);
                }
            }

            result.Sort(new ElementalType.ElementalTypeComparer());
            return result.ToArray();
        }

        public ElementalType[] IsImmuneTo()
        {

            ElementalType[] types1 = Type1.DefensivelyIsImmuneTo();
            ElementalType[] types2 = [];
            if (Type2 != null)
                types2 = Type2.DefensivelyIsImmuneTo();

            List<ElementalType> result = new List<ElementalType>(types1);
            foreach (ElementalType type in types2)
            {
                if (!result.Contains(type))
                {
                    result.Add(type);
                }
            }

            result.Sort(new ElementalType.ElementalTypeComparer());
            return result.ToArray();
        }

    }
}
