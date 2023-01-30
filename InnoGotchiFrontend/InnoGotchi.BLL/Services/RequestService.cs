using AutoMapper;
using Hanssens.Net;
using InnoGotchi.DAL.Managers;
using Microsoft.Extensions.Configuration;

namespace InnoGotchi.BLL.Services
{
    public class RequestService : BaseService
    {
        private RequestManager _requestManager;
        public RequestService(IMapper mapper,
                              IHttpClientFactory httpClientFactory,
                              LocalStorage localStorage,
                              IConfiguration configuration) : base(mapper)
        {
            _requestManager = new RequestManager(httpClientFactory, localStorage, configuration);
        }

        public async Task<bool> Create(int ownerId, int receiverId)
        {
            return await _requestManager.Create(ownerId, receiverId);
        }

        public async Task<bool> Confirm(int requestId)
        {
            return await _requestManager.Confirm(requestId);
        }

        public async Task<bool> Delete(int requestId)
        {
            return await _requestManager.Delete(requestId);
        }
    }
}
