using ExamenNet.Models;
using ExamenNet.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExamenNet.Validators
{
    public interface IRegisterValidator
    {
        ErrorsCollection Validate(RegisterPostModel registerPostModel, ExamDbContext context);
    }
    public class RegisterValidator : IRegisterValidator
    {
        public ErrorsCollection Validate(RegisterPostModel registerPostModel, ExamDbContext context)
        {
            ErrorsCollection errorCollection = new ErrorsCollection { Entity = nameof(RegisterPostModel) };
            User existing = context.Users.FirstOrDefault(u => u.Username == registerPostModel.Username);

            if (existing != null)
            {
                errorCollection.ErrorMessages.Add($"The username {registerPostModel.Username} is already taken!");
            }
            if (registerPostModel.Password.Length < 6)
            {
                errorCollection.ErrorMessages.Add("The password has to be longer than 6 characters");
            }
            //parola sa contina 2 cifre
            int numberOfDigits = 0;
            foreach (char c in registerPostModel.Password)
            {
                if (c >= '0' && c <= '9')
                {
                    numberOfDigits++;
                }
            }
            if (numberOfDigits < 2)
            {
                errorCollection.ErrorMessages.Add("The password must contains at least 2 digits");
            }
            if (errorCollection.ErrorMessages.Count > 0)
            {
                return errorCollection;
            }
            return null;
        }
    }
}

