using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BookingAds.Models.ForgotPassword;

namespace BookingAds.Repository.ForgotPassword.Abstractions
{
    public interface IForgotPasswordRepository
    {
        bool CheckUserName(string username);

        bool CheckConfirmCode(ViewConfirmCode data);

        bool ChangePassWord(ViewChangePassword data);

        bool CheckToken(string token, string username);

        bool CheckTimeToken(string token, string username);

        string CreateToken(string token, string username);

        string GetCode(ViewForgotPassword data);

        string GetToken(string username);

        bool DeleteToken(string token, string username);

        string IsPassWord(string username);
        string GetPhone(string email);
    }
}