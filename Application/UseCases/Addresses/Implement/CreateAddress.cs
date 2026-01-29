using Application.Dtos.Addresses;
using Application.Dtos.ViaCep;
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
            if (string.IsNullOrWhiteSpace(request.Number)) throw new ArgumentException("Número é obrigatório.");

            var cep = Cep.From(request.Cep);

            var via = await _viaCep.GetAsync(cep.Value, ct);
            if (via is null || via.Error)
                throw new InvalidOperationException("CEP não encontrado no ViaCEP.");

            var address = new Address(
                cep: cep,
                street: via.Street,
                number: request.Number,
                complement: request.Complement,
                neighborhood: via.Neighborhood,
                city: via.City,
                state: via.State
            );

            var created = await _repo.CreateAsync(address, ct);
            return AddressMapper.ToResponse(created);
        }
    }
}
