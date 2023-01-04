﻿using AutoMapper;
using InnoGotchi.DAL.Models;
using InnoGotchi.Web.BLL.DTO;

namespace InnoGotchi.Web.BLL.Services
{
    public class RequestService : BaseService
    {
        public RequestService(IMapper mapper) : base(mapper)
        {
        }

        public ColoborationRequestDTO? GetColoborationRequestDTO(ColoborationRequest request)
        {
            return _mapper.Map<ColoborationRequestDTO>(request);
        }

        public IEnumerable<ColoborationRequestDTO>? GetColoborationRequestsDTO(IEnumerable<ColoborationRequest> requests)
        {
            return _mapper.Map<IEnumerable<ColoborationRequestDTO>>(requests);
        }
    }
}