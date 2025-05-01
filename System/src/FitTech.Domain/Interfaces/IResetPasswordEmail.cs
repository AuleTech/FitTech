using FitTech.Domain.Entities;
namespace FitTech.Domain.Interfaces;

public interface IResetPasswordEmail
{
  Task AddAsync(ResetPasswordEmail resetPasswordEmail);
}
