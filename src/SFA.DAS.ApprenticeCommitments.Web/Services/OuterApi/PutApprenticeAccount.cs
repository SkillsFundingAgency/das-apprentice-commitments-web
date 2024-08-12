namespace SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi;

public record PutApprenticeAccount
{
    public required string Email { get; set; }
    public required string GovUkIdentifier { get; set; }
}