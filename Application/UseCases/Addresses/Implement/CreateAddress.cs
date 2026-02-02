using Application.Dtos.Addresses;
using Application.Dtos.Addresses.Request;
using Application.Dtos.Addresses.Response;
using Application.Dtos.ViaCep;
using Application.Ports;
using Application.UseCases.Addresses.Interfaces;
using Domain.Entities;
using Domain.ValueObjects;

namespace Application.UseCases.Addresses.Implement
{
    public sealed class CreateAddress : ICreateAddress
    {
        private readonly IViaCepClient _viaCep;
        private readonly IAddressRepository _repo;

        public CreateAddress(IViaCepClient viaCep, IAddressRepository repo)
        {
            _viaCep = viaCep;
            _repo = repo;
        }

        public async Task<AddressResponse> ExecuteAsync(CreateAddressRequest request, CancellationToken ct)
        {
            if (request is null) throw new ArgumentException("Payload é obrigatório.");
            if (string.IsNullOrWhiteSpace(request.Cep)) throw new ArgumentException("CEP é obrigatório.");

            var cep = Cep.From(request.Cep);

            var existing = await _repo.GetByCepAsync(cep.Value, ct);
            if (existing is not null)
                return AddressMapper.ToResponse(existing);

            var via = await _viaCep.GetAsync(cep.Value, ct);
            if (via is null || via.Error)
                throw new InvalidOperationException("CEP Invalido.");

            var address = new Address(
                cep: cep,
                street: via.Street,
                neighborhood: via.Neighborhood,
                city: via.City,
                state: via.State
            );

            var created = await _repo.CreateAsync(address, ct);
            return AddressMapper.ToResponse(created);
        }
    }
}
