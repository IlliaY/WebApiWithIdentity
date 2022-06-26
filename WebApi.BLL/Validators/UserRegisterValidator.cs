using FluentValidation;
using WebApi.BLL.Models;

namespace WebApi.BLL.Validators
{
    public class UserRegisterValidator : AbstractValidator<UserRegisterModel>
    {
        public UserRegisterValidator()
        {
            RuleFor(register => register.UserName).NotNull().WithMessage("Username is required").MinimumLength(3).MaximumLength(20);
            RuleFor(register => register.Email).NotNull().WithMessage("Email is required").EmailAddress().WithMessage($"This email is incorrect");
            RuleFor(register => register.Password).NotNull().WithMessage("Password is required").MinimumLength(6).Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[#$^+=!*()@%&]).{8,}$").WithMessage("Should have at least one lower case \n Should have at least one upper case \n Should have at least one number \n Should have at least one special character \n Minimum 8 characters");
        }
    }
}