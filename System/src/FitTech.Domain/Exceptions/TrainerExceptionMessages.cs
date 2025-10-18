namespace FitTech.Domain.Exceptions;

internal static class TrainerExceptionMessages
{
    public const string InvitationEmailDifferent =
        "Email doesn't match. You need to use the same email where you got the invitation";

    public const string InvitationNoInProgress = "Invitation must be in progress";
    public const string UserAlreadyInTeam = "User already registered in trainer's team";
}
