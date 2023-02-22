namespace InnoGotchi.DAL.Models
{
    /// <summary>
    /// Entity that contains all items on view page and other information about pages
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PaginatedList<T>
    {
        public int PageIndex { get; set; }
        public int TotalPages { get; set; }
        public T? Items { get; set; }
    }
}
