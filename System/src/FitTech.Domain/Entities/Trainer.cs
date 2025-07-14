using System.Data;

namespace FitTech.Domain.Entities;

public class Trainer
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public DateTime Birthdate { get; set; }
    public string Password { get; set; } = null!;
    
    public void UpdateData(string name, string email, string password, CancellationToken cancellationToken)
    {
        Name = name;
        Email = email;
        Password = password; 
    }
}
