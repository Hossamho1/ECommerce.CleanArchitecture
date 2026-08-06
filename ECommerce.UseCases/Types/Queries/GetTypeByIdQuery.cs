using ECommerce.Application.Types.Dtos;
using ECommerce.Domain.Common;
using MediatR;

namespace ECommerce.Application.Types.Queries;

public sealed record GetTypeByIdQuery(Guid Id) : IRequest<Result<GetByIdTypeResponse>>;
