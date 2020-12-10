using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using ssdcw.Models;

namespace Identity.IdentityPolicy
{
    public class CustomPasswordPolicy : PasswordValidator<User>
    {
        public override async Task<IdentityResult> ValidateAsync(UserManager<User> manager, User user, string password)
        {

            IdentityResult result = await base.ValidateAsync(manager, user, password);
            List<IdentityError> errors = result.Succeeded ? new List<IdentityError>() : result.Errors.ToList();

            List<string> commonPasswords = new List<string> { "password666#", "qwerty234!", "letmein1!",
                                                            "pa$$word1", "pa$$w0rd1", "pa$$vv04d", "pa$$w04d"};


            if (password.ToLower().Contains(user.UserName.ToLower()))
            {
                errors.Add(new IdentityError
                {
                    Description = "Password cannot contain username"
                });
            }
            if (password.ToLower().Contains(user.FirstName.ToLower()))
            {
                errors.Add(new IdentityError
                {
                    Description = "Password cannot contain your first name"
                });
            }
            if (password.ToLower().Contains(user.LastName.ToLower()))
            {
                errors.Add(new IdentityError
                {
                    Description = "Password cannot contain your last name"
                });
            }
            if (password.Contains("123"))
            {
                errors.Add(new IdentityError
                {
                    Description = "Password cannot contain 123 numeric sequence"
                });
            }
            if (commonPasswords.Contains(password.ToLower()))
            {
                errors.Add(new IdentityError
                {
                    Description = "This password is on a common passwords list, please choose something more complex"
                });
            }
            if (password.ToLower().Contains("password"))
            {
                errors.Add(new IdentityError
                {
                    Description = "Password cannot contain a word 'password'"
                });
            }

            if (password.ToLower().Contains("letmein"))
            {
                errors.Add(new IdentityError
                {
                    Description = "This password is on a common passwords list, please choose something more complex"
                });
            }
            if (password.ToLower().Contains("pass"))
            {
                errors.Add(new IdentityError
                {
                    Description = "This password is on a common passwords list, please choose something more complex"
                });
            }
            return errors.Count == 0 ? IdentityResult.Success : IdentityResult.Failed(errors.ToArray());
        }
    }
}