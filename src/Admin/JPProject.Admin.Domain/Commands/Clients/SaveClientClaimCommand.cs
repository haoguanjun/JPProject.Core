using JPProject.Admin.Domain.Validations.Client;
using System.Security.Claims;

namespace JPProject.Admin.Domain.Commands.Clients
{
    public class SaveClientClaimCommand : ClientClaimCommand
    {

        public SaveClientClaimCommand(string clientId, string type, string value)
        {
            Type = type;
            ClientId = clientId;
            Value = value;
        }
        public override bool IsValid()
        {
            ValidationResult = new SaveClientClaimCommandValidation().Validate(this);
            return ValidationResult.IsValid;
        }

        public Claim ToEntity()
        {
            return new Claim(Type, Value);
        }
    }
}