namespace FitTech.Domain.Entities;

public interface IResetPasswordEmail
{
  Task AddAsync(ResetPasswordEmail resetPasswordEmail);
}
