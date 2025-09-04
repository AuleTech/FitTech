using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace FitTech.Domain.Entities;

public class Trainer
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    
    [NotMapped] 
    public string Password { get; set; } = null!;
    
    public void UpdateData(Guid id, string name, string email, string password, CancellationToken cancellationToken)
    {
        Id = id;
        Name = name;
        Email = email;
        Password = password; 
    }
}
