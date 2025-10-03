using MediatR;
using Microsoft.Extensions.Logging;
using SharedKernel.Messaging.RabbitMQ;
using UserModule.Application.Interfaces;
using UserModule.Application.Interfaces.Repositories;

namespace UserModule.Application.User.Commands.DeactivateUser
{
    public class DeactivateUserCommandHandler : IRequestHandler<DeactivateUserCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserUnitOfWork _unitOfWork;
        private readonly IRabbitMqService _rabbitMqService;
        private readonly ILogger<DeactivateUserCommandHandler> _logger;
        public DeactivateUserCommandHandler(IUserRepository userRepository,
            IRabbitMqService rabbitMqService,
            IUserUnitOfWork unitOfWork,
            ILogger<DeactivateUserCommandHandler> logger)
        {
            _userRepository = userRepository;
            _rabbitMqService = rabbitMqService;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Handle(DeactivateUserCommand command, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(command.UserId, cancellationToken);
            await _rabbitMqService.DeclareQueueAsync(
                queueName: "delayed_product_delete_queue",
                arguments: new Dictionary<string, object>
                {
                    { "x-message-ttl", 60000 },
                    { "x-dead-letter-exchange", "product_exchange" },
                    { "x-dead-letter-routing-key", "product.delete" }
                },
                cancellationToken: cancellationToken
            );
            await _rabbitMqService.PublishAsync("user_exchange", "user.delete", command.UserId.ToString(), cancellationToken);
            user.MarkAsDeleted();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("[User Module(DeactivateUserCommandHandler)] Profile for user with ID {userId} has been soft deleted.", command.UserId);
        }
    }
}
