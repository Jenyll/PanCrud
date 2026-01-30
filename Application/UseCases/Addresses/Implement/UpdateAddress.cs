using Application.Dtos.Addresses;
using Application.Dtos.Addresses.Request;
using Application.Dtos.Addresses.Response;
using Application.Dtos.ViaCep;
using Application.Ports;
using Application.UseCases.Addresses.Interfaces;
using Domain.ValueObjects;

namespace Application.UseCases.Addresses.Implement;

public sealed class UpdateAddress : IUpdateAddress
{
    private readonly IAddressRepository _repo;
    private readonly IViaCepClient _viaCep;

    public UpdateAddress(IAddressRepository repo, IViaCepClient viaCep)
    {
        _repo = repo;
        _viaCep = viaCep;
    }

    public async Task<AddressResponse?> ExecuteAsync(Guid id, UpdateAddressRequest request, CancellationToken ct)
    {
        if (request is null) throw new ArgumentException("Payload é obrigatório.");

        var address = await _repo.GetByIdAsync(id, ct);
        if (address is null) return null;

        // Se vier CEP (mesmo ou novo), consulta ViaCEP e atualiza logradouro/bairro/cidade/UF.
        if (!string.IsNullOrWhiteSpace(request.Cep))
        {
            var newCep = Cep.From(request.Cep);

            // Se mudou o CEP, atualiza Cep + dados ViaCEP
            if (newCep.Value != address.Cep.Value)
            {
                var via = await _viaCep.GetAsync(newCep.Value, ct);
                if (via is null || via.Error)
                    throw new InvalidOperationException("CEP não encontrado no ViaCEP.");

                address.UpdateCepAndFromViaCep(newCep, via.Street, via.Neighborhood, via.City, via.State);
            }
            else
            {
                //Refresh mesmo CEP — manter "sempre atualizado"
                var via = await _viaCep.GetAsync(newCep.Value, ct);
                if (via is null || via.Error)
                    throw new InvalidOperationException("CEP não encontrado no ViaCEP.");

                address.UpdateFromViaCep(via.Street, via.Neighborhood, via.City, via.State);
            }
        }

        await _repo.UpdateAsync(address, ct);
        return AddressMapper.ToResponse(address);
    }
}
