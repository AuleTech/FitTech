using FitTech.Domain.Aggregates.EmailAggregate;
using FitTech.Domain.Seedwork;

namespace FitTech.Domain.Repositories;

public interface IEmailRepository : IRepository<Email>
{
    Task AddAsync(Email email, CancellationToken cancellationToken);
}
