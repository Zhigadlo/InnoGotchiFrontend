namespace InnoGotchi.Web.BLL.DTO
{
    public class ColoborationRequestDTO
    {
        public int Id { get; set; }
        public int RequestOwnerId { get; set; }
        public int RequestReceipientId { get; set; }
        public DateTime Date { get; set; }
        public bool IsConfirmed { get; set; }
    }
}
