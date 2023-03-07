using AutoMapper;
using Hanssens.Net;
using InnoGotchi.DAL.Managers;
using Microsoft.Extensions.Configuration;

namespace InnoGotchi.BLL.Services
{
    /// <summary>
    /// Class that have ability to get coloboration requests data from data access layer
    /// </summary>
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
        /// <summary>
        /// Creates coloboration request
        /// </summary>
        public async Task<bool> Create(int ownerId, int receiverId)
        {
            return await _requestManager.Create(ownerId, receiverId);
        }
        /// <summary>
        /// Confirms coloboration request by id
        /// </summary>
        public async Task<bool> Confirm(int requestId)
        {
            return await _requestManager.Confirm(requestId);
        }
        /// <summary>
        /// Deletes coloboration request by id
        /// </summary>
        public async Task<bool> Delete(int requestId)
        {
            return await _requestManager.Delete(requestId);
        }
    }
}
