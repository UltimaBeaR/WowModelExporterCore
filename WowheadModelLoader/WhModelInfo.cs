namespace WowheadModelLoader
{
    public class WhModelInfo
    {
        public WhType Type { get; set; }
        public string Id { get; set; }
        public WhModel Parent { get; set; }
        public int? Shoulder { get; set; }

        // Этот метод я сам добавил
        public static WhModelInfo CreateForCharacter(WhRace race, WhGender gender)
        {
            return new WhModelInfo()
            {
                Type = WhType.CHARACTER,
                Id = race.GetStringIdentifier() + gender.GetStringIdentifier()
            };
        }
    }
}
