using Microsoft.AspNetCore.Http;

namespace InnoGotchi.BLL.DTO
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
        public FarmDTO Farm { get; set; }
        public List<ColoborationRequestDTO> SentRequests { get; set; }
        public List<ColoborationRequestDTO> ReceivedRequests { get; set; }

        public int GetSentRequestId(int receipientId)
        {
            var request = SentRequests.FirstOrDefault(sr => sr.RequestOwnerId == this.Id
                                                         && sr.RequestReceipientId == receipientId);

            if (request == null)
                return -1;

            return request.Id;
        }

        public int GetReceivedRequestId(int ownerId)
        {
            var request = ReceivedRequests.FirstOrDefault(sr => sr.RequestOwnerId == ownerId
                                                          && sr.RequestReceipientId == this.Id);

            if (request == null)
                return -1;

            return request.Id;
        }
    }
}
