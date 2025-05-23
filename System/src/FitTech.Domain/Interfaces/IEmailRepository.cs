using FitTech.Domain.Entities;
namespace FitTech.Domain.Interfaces;

public interface IEmailRepository
{
  Task AddAsync(Email email, CancellationToken cancellationToken);
}
