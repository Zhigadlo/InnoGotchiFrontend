namespace InnoGotchi.DAL.Models
{
    public class PaginatedList<T>
    {
        public int PageIndex { get; set; }
        public int TotalPages { get; set; }
        public List<T>? Items { get; set; } = null;
    }
}
