using AutoMapper;
using BlazorSozluk.Api.Application.Interfaces.Repositories;
using BlazorSozluk.Common;
using BlazorSozluk.Common.Events.User;
using BlazorSozluk.Common.Infrastructure.Extensions;
using BlazorSozluk.Common.ViewModels.RequestModels;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorSozluk.Api.Application.Features.Commands.User.Create
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Guid>
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;

        public CreateUserCommandHandler(IMapper mapper, IUserRepository userRepository)
        {
            _mapper = mapper;
            _userRepository = userRepository;
        }

        public async Task<Guid> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var existsUSer = await _userRepository.GetSingleAsync(i => i.EmailAddress == request.EmailAddress);

            if (existsUSer is not null)
                throw new DataBaseValidationException("User already exists!");

            var dbUser = _mapper.Map<Domain.Models.User>(request);

            var rows = await _userRepository.AddAsync(dbUser);

            if (rows > 0)
            {
                var @event = new UserEmailChangedEvent()
                {
                    OldEmailAdress=null,
                    NewEmailAdress=dbUser.EmailAddress
                };

                QueueFactory.SendMassageToExchange(exchangeName:SozlukConstants.UserExchangeName,exchangeType:SozlukConstants.DefaultExchangeType,queueName:SozlukConstants.UserEmailChangedQueueName,obj: @event);
            }
            return dbUser.Id;
        }

        
    }
}
