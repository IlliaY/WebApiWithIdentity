using FluentValidation;
using WebApi.BLL.Models;

namespace WebApi.BLL.Validators
{
    public class UserLoginValidator : AbstractValidator<UserLoginModel>
    {
        public UserLoginValidator()
        {
            RuleFor(login => login.UserName).NotNull().WithMessage("Username is required").MinimumLength(3).MaximumLength(20);
            RuleFor(login => login.Password).NotNull().WithMessage("Password is required");
        }
    }
}