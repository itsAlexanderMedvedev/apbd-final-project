using System.Globalization;
using Project.Exceptions;
using Project.Repositories.Abstraction;
using Project.Services.Abstraction;

namespace Project.Services;

public class PaymentService : IPaymentService
{
    private readonly IContractRepository _contractRepository;
    
    public PaymentService(IContractRepository contractRepository)
    {
        _contractRepository = contractRepository;
    }
    
    public async Task PayForContract(int idContract, double amount)
    {
        var contract = await _contractRepository.FindByIdAsync(idContract);
        if (contract == null)
        {
            throw new ContractNotFoundException("Contract not found");
        }
        
        if (contract.EndDate < DateTime.Now)
        {
            throw new ContractEndedException("The contract has already ended");
        }
        
        if (amount <= 0)
        {
            throw new InvalidPaymentAmountException("Payment amount must be positive");
        }
        
        if (contract.Paid + amount > contract.Price)
        {
            throw new InvalidPaymentAmountException("Payment amount too high");
        }
        
        var parts = amount.ToString(new CultureInfo("en-US")).Split('.');
        if (parts.Length == 2 && parts[1].Length > 2)
        {
            throw new InvalidPaymentAmountException("Payment amount must have at most 2 decimal places");
        }
        
        await _contractRepository.PayForContractAsync(contract, amount);
    }
}