namespace InnoGotchi.Web.BLL.DTO
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public IFormFile FormFile { get; set; }
        public byte[]? Avatar { get; set; }
    }
}
