using Application.Ports;
using Application.UseCases.Addresses.Interfaces;

namespace Application.UseCases.Addresses.Implement
{
    public sealed class DeleteAddress : IDeleteAddress
    {
        private readonly IAddressRepository _repo;

        public DeleteAddress(IAddressRepository repo) => _repo = repo;

        public Task<bool> ExecuteAsync(Guid id, CancellationToken ct)
            => _repo.DeleteAsync(id, ct);
    }
}
