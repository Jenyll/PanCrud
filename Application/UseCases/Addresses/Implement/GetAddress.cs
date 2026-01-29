using Application.Dtos.Addresses;
using Application.UseCases.Addresses.Interfaces;

namespace Application.UseCases.Addresses.Implement
{
    public sealed class GetAddress : IGetAddress
    {
        private readonly IAddressRepository _repo;

        public GetAddress(IAddressRepository repo) => _repo = repo;

        public async Task<AddressResponse?> ExecuteAsync(Guid id, CancellationToken ct)
        {
            var address = await _repo.GetByIdAsync(id, ct);
            return address is null ? null : AddressMapper.ToResponse(address);
        }
    }
