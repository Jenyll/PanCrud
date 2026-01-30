using Application.Dtos.Persons.Request;
using Application.Dtos.Persons.Response;
using Application.Dtos.Persons;
using Application.Dtos.Addresses.Request;
using Application.Ports;

namespace Application.Services;

public sealed class PersonService : IPersonService
{
    private readonly IIndividualRepository _individualRepo;
    private readonly ICompanyRepository _companyRepo;
    private readonly IAddressRepository _addressRepo;
    private readonly IAddressService _addressService;

    public PersonService(
        IIndividualRepository individualRepo,
        ICompanyRepository companyRepo,
        IAddressRepository addressRepo,
        IAddressService addressService)
    {
        _individualRepo = individualRepo;
        _companyRepo = companyRepo;
        _addressRepo = addressRepo;
        _addressService = addressService;
    }

    public async Task<PersonResponse> CreateAsync(CreatePersonRequest request, CancellationToken ct)
    {
        if (request is null) throw new ArgumentException("Payload é obrigatório.");

        var addressId = await ResolveOrCreateAddress(request.AddressId, request.Address, ct);

        if (request.Type == PersonType.Individual)
        {
            if (string.IsNullOrWhiteSpace(request.Name)) throw new ArgumentException("Nome é obrigatório para PF.");
            if (string.IsNullOrWhiteSpace(request.Cpf)) throw new ArgumentException("CPF é obrigatório para PF.");

            var cpf = Domain.ValueObjects.Cpf.From(request.Cpf!);
            var individual = new Domain.Entities.Individual(request.Name!, cpf, addressId);
            var created = await _individualRepo.CreateAsync(individual, ct);
            var reloaded = await _individualRepo.GetByIdAsync(created.Id, ct) ?? created;
            return PersonMapper.FromIndividual(reloaded);
        }
        else
        {
            if (string.IsNullOrWhiteSpace(request.LegalName)) throw new ArgumentException("Razão social é obrigatória para PJ.");
            if (string.IsNullOrWhiteSpace(request.Cnpj)) throw new ArgumentException("CNPJ é obrigatório para PJ.");

            var cnpj = Domain.ValueObjects.Cnpj.From(request.Cnpj!);
            var company = new Domain.Entities.Company(request.LegalName!, cnpj, addressId);
            var created = await _companyRepo.CreateAsync(company, ct);
            var reloaded = await _companyRepo.GetByIdAsync(created.Id, ct) ?? created;
            return PersonMapper.FromCompany(reloaded);
        }
    }

    public async Task<PersonResponse?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var individual = await _individualRepo.GetByIdAsync(id, ct);
        if (individual is not null) return PersonMapper.FromIndividual(individual);

        var company = await _companyRepo.GetByIdAsync(id, ct);
        if (company is not null) return PersonMapper.FromCompany(company);

        return null;
    }

    public async Task<PersonResponse?> UpdateAsync(Guid id, UpdatePersonRequest request, CancellationToken ct)
    {
        var individual = await _individualRepo.GetByIdAsync(id, ct);
        if (individual is not null)
        {
            if (!string.IsNullOrWhiteSpace(request.Name)) individual.Rename(request.Name!);
            if (!string.IsNullOrWhiteSpace(request.Cpf)) individual.UpdateCpf(Domain.ValueObjects.Cpf.From(request.Cpf!));

            if (request.AddressId.HasValue || request.Address is not null)
            {
                var newAddressId = await ResolveOrCreateAddress(request.AddressId, request.Address, ct);
                individual.UpdateAddress(newAddressId);
            }

            await _individualRepo.UpdateAsync(individual, ct);
            var reloaded = await _individualRepo.GetByIdAsync(individual.Id, ct) ?? individual;
            return PersonMapper.FromIndividual(reloaded);
        }

        var company = await _companyRepo.GetByIdAsync(id, ct);
        if (company is not null)
        {
            if (!string.IsNullOrWhiteSpace(request.LegalName)) company.Rename(request.LegalName!);
            if (!string.IsNullOrWhiteSpace(request.Cnpj)) company.UpdateCnpj(Domain.ValueObjects.Cnpj.From(request.Cnpj!));

            if (request.AddressId.HasValue || request.Address is not null)
            {
                var newAddressId = await ResolveOrCreateAddress(request.AddressId, request.Address, ct);
                company.UpdateAddress(newAddressId);
            }

            await _companyRepo.UpdateAsync(company, ct);
            var reloaded = await _companyRepo.GetByIdAsync(company.Id, ct) ?? company;
            return PersonMapper.FromCompany(reloaded);
        }

        return null;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct)
    {
        var deletedIndividual = await _individualRepo.DeleteAsync(id, ct);
        if (deletedIndividual) return true;

        var deletedCompany = await _companyRepo.DeleteAsync(id, ct);
        return deletedCompany;
    }

    // resolve AddressId: usa o fornecido; senão procura por CEP no DB; se não encontrar chama AddressService (ViaCEP)
    private async Task<Guid> ResolveOrCreateAddress(Guid? addressId, CreateAddressRequest? addr, CancellationToken ct)
    {
        if (addressId.HasValue && addressId.Value != Guid.Empty) return addressId.Value;

        if (addr is null) throw new ArgumentException("Informações de endereço são obrigatórias quando AddressId não é fornecido.");

        var cepValue = Domain.ValueObjects.Cep.From(addr.Cep).Value;

        var existing = await _addressRepo.GetByCepAsync(cepValue, ct);
        if (existing is not null) return existing.Id;

        var created = await _addressService.CreateAsync(new CreateAddressRequest
        {
            Cep = addr.Cep,
            Number = addr.Number,
            Complement = addr.Complement
        }, ct);

        return created.Id;
    }
    public async Task<PersonResponse?> GetByDocumentAsync(string number, PersonType type, CancellationToken ct)
    {
        var digits = new string(number.Where(char.IsDigit).ToArray());
        if (string.IsNullOrWhiteSpace(digits))
            throw new ArgumentException("Número do documento inválido.");

        if (type == PersonType.Individual)
        {
            var cpf = Domain.ValueObjects.Cpf.From(digits); // valida e normaliza
            var individual = await _individualRepo.GetByCpfAsync(cpf.Value, ct);
            return individual is null ? null : PersonMapper.FromIndividual(individual);
        }

        if (type == PersonType.Company)
        {
            var cnpj = Domain.ValueObjects.Cnpj.From(digits);
            var company = await _companyRepo.GetByCnpjAsync(cnpj.Value, ct);
            return company is null ? null : PersonMapper.FromCompany(company);
        }

        throw new ArgumentException("Tipo de pessoa inválido.");
    }
}