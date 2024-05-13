
namespace TPDP_Battle_Helper.Data
{
    internal class ElementalType
    {

        public readonly string Name;
        public readonly byte Id;
        public readonly string FilePath;
        private ElementalType[] WeakTo;
        private ElementalType[] ResistantTo;
        private ElementalType[] ImmuneTo;

        private ElementalType(string name, byte id, string filePath)
        {
            this.Name = name;
            this.Id = id;
            this.FilePath = filePath;
        }

        public static ElementalType DREAM = new ElementalType("Dream", 0, "/Resources/Types/type_dream.png");
        public static ElementalType VOID = new ElementalType("Void", 1, "/Resources/Types/type_void.png");
        public static ElementalType FIRE = new ElementalType("Fire", 2, "/Resources/Types/type_fire.png");
        public static ElementalType WATER = new ElementalType("Water", 3, "/Resources/Types/type_water.png");
        public static ElementalType NATURE = new ElementalType("Nature", 4, "/Resources/Types/type_nature.png");
        public static ElementalType EARTH = new ElementalType("Earth", 5, "/Resources/Types/type_earth.png");
        public static ElementalType STEEL = new ElementalType("Steel", 6, "/Resources/Types/type_steel.png");
        public static ElementalType WIND = new ElementalType("Wind", 7, "/Resources/Types/type_wind.png");
        public static ElementalType ELECTRIC = new ElementalType("Electric", 8, "/Resources/Types/type_electric.png");
        public static ElementalType LIGHT = new ElementalType("Light", 9, "/Resources/Types/type_light.png");
        public static ElementalType DARK = new ElementalType("Dark", 10, "/Resources/Types/type_dark.png");
        public static ElementalType NETHER = new ElementalType("Nether", 11, "/Resources/Types/type_nether.png");
        public static ElementalType POISON = new ElementalType("Poison", 12, "/Resources/Types/type_poison.png");
        public static ElementalType FIGHTING = new ElementalType("Fighting", 13, "/Resources/Types/type_fighting.png");
        public static ElementalType ILLUSION = new ElementalType("Illusion", 14, "/Resources/Types/type_illusion.png");
        public static ElementalType SOUND = new ElementalType("Sound", 15, "/Resources/Types/type_sound.png");
        public static ElementalType WARPED = new ElementalType("Warped", 16, "/Resources/Types/type_warped.png");

        public static ElementalType[] ALL = [DREAM, VOID, FIRE, WATER, NATURE, EARTH, STEEL, WIND, ELECTRIC, LIGHT, DARK, NETHER, POISON, FIGHTING, ILLUSION, SOUND, WARPED];

        public class ElementalTypeComparer : Comparer<ElementalType>
        {
            public override int Compare(ElementalType x, ElementalType y)
            {
                return x.Id.CompareTo(y.Id);
            }
        }
        
        public static void Init()
        {

            DREAM.WeakTo =          [];
            DREAM.ResistantTo =     [];
            DREAM.ImmuneTo =        [];

            VOID.WeakTo =           [DARK];
            VOID.ResistantTo =      [];
            VOID.ImmuneTo =         [ILLUSION];

            FIRE.WeakTo =           [WATER, EARTH];
            FIRE.ResistantTo =      [FIRE, NATURE, STEEL];
            FIRE.ImmuneTo =         [];

            WATER.WeakTo =          [NATURE, ELECTRIC, POISON];
            WATER.ResistantTo =     [FIRE, WATER, STEEL, LIGHT];
            WATER.ImmuneTo =        [];

            NATURE.WeakTo =         [FIRE, STEEL, POISON];
            NATURE.ResistantTo =    [WATER, NATURE, EARTH, ELECTRIC, LIGHT];
            NATURE.ImmuneTo =       [];

            EARTH.WeakTo =          [WATER, NATURE, FIGHTING];
            EARTH.ResistantTo =     [FIRE, POISON];
            EARTH.ImmuneTo =        [ELECTRIC];

            STEEL.WeakTo =          [FIRE, EARTH, FIGHTING, WARPED];
            STEEL.ResistantTo =     [STEEL, WIND, ILLUSION];
            STEEL.ImmuneTo =        [POISON];

            WIND.WeakTo =           [STEEL, ELECTRIC, WARPED];
            WIND.ResistantTo =      [NATURE, FIGHTING, SOUND];
            WIND.ImmuneTo =         [EARTH];

            ELECTRIC.WeakTo =       [EARTH];
            ELECTRIC.ResistantTo =  [WIND, ELECTRIC];
            ELECTRIC.ImmuneTo =     [];

            LIGHT.WeakTo =          [DARK, ILLUSION];
            LIGHT.ResistantTo =     [VOID, ELECTRIC, LIGHT, NETHER, SOUND];
            LIGHT.ImmuneTo =        [];

            DARK.WeakTo =           [STEEL, LIGHT, FIGHTING];
            DARK.ResistantTo =      [VOID, DARK, WARPED];
            DARK.ImmuneTo =         [];

            NETHER.WeakTo =         [LIGHT, DARK, NETHER];
            NETHER.ResistantTo =    [POISON, ILLUSION];
            NETHER.ImmuneTo =       [FIGHTING];

            POISON.WeakTo =         [EARTH, WIND];
            POISON.ResistantTo =    [NATURE, POISON, FIGHTING, WARPED];
            POISON.ImmuneTo =       [];

            FIGHTING.WeakTo =       [WIND, SOUND];
            FIGHTING.ResistantTo =  [EARTH, DARK, WARPED];
            FIGHTING.ImmuneTo =     [];

            ILLUSION.WeakTo =       [ILLUSION, SOUND, WARPED];
            ILLUSION.ResistantTo =  [LIGHT, DARK, FIGHTING];
            ILLUSION.ImmuneTo =     [VOID];

            SOUND.WeakTo =          [WIND, ELECTRIC];
            SOUND.ResistantTo =     [WATER, NETHER, SOUND];
            SOUND.ImmuneTo =        [];

            WARPED.WeakTo =         [POISON, FIGHTING, SOUND];
            WARPED.ResistantTo =    [STEEL, WARPED];
            WARPED.ImmuneTo =       [WIND];

        }

        public static ElementalType? FindByName(string name)
        {
            foreach(ElementalType type in ALL)
            {
                if (string.Equals(type.Name, name, StringComparison.OrdinalIgnoreCase)) return type;
            }
            return null;
        }

        public ElementalType[] DefensivelyIsWeakTo()
        {
            return this.WeakTo;
        }

        public ElementalType[] DefensivelyIsResistantTo()
        {
            return this.ResistantTo;
        }

        public ElementalType[] DefensivelyIsImmuneTo()
        {
            return this.ImmuneTo;
        }

        public ElementalType[] OffensivelyIsStrongAgainst()
        {
            List<ElementalType> result = []; 
            foreach(ElementalType type in ALL)
            {
                if (type.WeakTo.Contains(this))
                {
                    result.Add(type);
                }
            }
            return result.ToArray();
        }

        public ElementalType[] OffensivelyIsIneffectiveAgainst()
        {
            List<ElementalType> result = [];
            foreach (ElementalType type in ALL)
            {
                if (type.ResistantTo.Contains(this))
                {
                    result.Add(type);
                }
            }
            return result.ToArray();
        }

        public ElementalType[] OffensivelyIsUselessAgainst()
        {
            List<ElementalType> result = [];
            foreach (ElementalType type in ALL)
            {
                if (type.ImmuneTo.Contains(this))
                {
                    result.Add(type);
                }
            }
            return result.ToArray();
        }

    }
}
