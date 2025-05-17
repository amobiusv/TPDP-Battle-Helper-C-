
namespace TPDP_Battle_Helper.Data
{
    internal class SkillCategory
    {

        public readonly string Name;
        public readonly byte Id;
        public readonly bool Damaging;
        public readonly string FilePath;

        private SkillCategory(string name, byte id, bool damaging, string filePath)
        {
            this.Name = name;
            this.Id = id;
            this.Damaging = damaging;
            this.FilePath = filePath;
        }

        public static SkillCategory FOCUS = new SkillCategory("Focus", 0, true, "");
        public static SkillCategory SPREAD = new SkillCategory("Spread", 1, true, "");
        public static SkillCategory STATUS = new SkillCategory("Status", 2, false, "");

        public static SkillCategory[] ALL = [FOCUS, SPREAD, STATUS];

        public class SkillCategoryComparer : Comparer<SkillCategory>
        {
            public override int Compare(SkillCategory x, SkillCategory y)
            {
                return x.Id.CompareTo(y.Id);
            }
        }

        public static SkillCategory? FindByName(string name)
        {
            foreach(SkillCategory category in ALL)
            {
                if (string.Equals(category.Name, name, StringComparison.OrdinalIgnoreCase)) return category;
            }
            return null;
        }

    }
}
