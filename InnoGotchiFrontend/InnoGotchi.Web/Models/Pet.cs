namespace InnoGotchi.Web.Models
{
    public class Pet
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Appearance Appearance { get; set; }
        public int FarmId { get; set; }
    }
}
