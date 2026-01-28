using Application.UseCases.Addresses;
using Application.Dtos.Services;

namespace Application.Services
{
    public class AdressService : IAddressService
    {
        private readonly ICreateAddressUseCase _create;
        private readonly IGetAddressUseCase _get;
        private readonly IUpdateAddressUseCase _update;
        private readonly IDeleteAddressUseCase _delete;

        public AddressService(
            ICreateAddressUseCase create,
            IGetAddressUseCase get,
            IUpdateAddressUseCase update,
            IDeleteAddressUseCase delete)
        {
            _create = create;
            _get = get;
            _update = update;
            _delete = delete;
        }

        public Task<AddressResponse> CreateAsync(CreateAddressRequest request, CancellationToken ct)
            => _create.ExecuteAsync(request, ct);

        public Task<AddressResponse?> GetByIdAsync(Guid id, CancellationToken ct)
            => _get.ExecuteAsync(id, ct);

        public Task<AddressResponse?> UpdateAsync(Guid id, UpdateAddressRequest request, CancellationToken ct)
            => _update.ExecuteAsync(id, request, ct);

        public Task<bool> DeleteAsync(Guid id, CancellationToken ct)
            => _delete.ExecuteAsync(id, ct);
    }
}
