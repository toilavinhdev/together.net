using Infrastructure.SharedKernel.ValueObjects;
using MediatR;

namespace Infrastructure.SharedKernel.Mediator;

public interface IBaseRequest : IRequest<BaseResponse>;

public interface IBaseRequest<TResponse> : IRequest<BaseResponse<TResponse>>;