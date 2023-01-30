namespace InnoGotchi.BLL.DTO
{
    public class PaginatedListDTO<T> where T : class
    {
        public int PageIndex { get; set; }
        public int TotalPages { get; set; }
        public List<T>? Items { get; set; } = null;

        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;
    }
}
