using Daftari.API.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Daftari.API.Interfaces
{
    public interface IAuthService
    {
        //DaftariResult<bool> ForgotPasswordAsync(string userName);
        //DaftariResult<bool> ChangePasswordAsync(ChangePasswordRequest changePasswordRequest);
        string GetSecurityToken(string userNames, string userId, string role);
        int GenerateRandomNo();
        //DaftariResult<bool> ConfirmRegister(string emailAddress, string phoneNumber, int token);
    }
}