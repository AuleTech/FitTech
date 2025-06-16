using FitTech.Domain.Entities;

namespace FitTech.Domain.Repositories;

public interface IEmailRepository
{
  Task AddAsync(Email email, CancellationToken cancellationToken);
}
