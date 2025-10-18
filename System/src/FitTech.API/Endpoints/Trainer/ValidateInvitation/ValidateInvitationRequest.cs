using FastEndpoints;

namespace FitTech.API.Endpoints.Trainer.ValidateInvitation;

public class ValidateInvitationRequest
{
    [QueryParam, BindFrom("code")]
    public int Code { get; set; }

    [QueryParam, BindFrom("email")] 
    public string Email { get; set; } = null!;
}
