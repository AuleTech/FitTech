using Microsoft.AspNetCore.Identity;

namespace FitTech.Domain.Entities;

public class ResetPasswordEmail 
{
    public Guid EmailId { get; set; }  
    public string? ToEmail { get; set; }
    public string? Message { get; set; }
    
    public ResetPasswordEmail(Guid emailId, string toEmail, string? message)
    {
        EmailId = emailId;
        ToEmail = toEmail;
        Message = message;
    }

}



