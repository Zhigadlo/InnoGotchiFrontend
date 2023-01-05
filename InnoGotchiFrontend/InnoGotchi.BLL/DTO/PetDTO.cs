namespace InnoGotchi.BLL.DTO
{
    public class PetDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public AppearanceDTO Appearance { get; set; }
        public int FarmId { get; set; }
    }
}
