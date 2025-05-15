using BookingAds.Common.Models.Account;

namespace BookingAds.Common.Repository.Account.Abstractions
{
    using BookingAds.Entities;
    using BookingAds.Models.Register;

    public interface IAccountRepository
    {
        bool IsAdmin(string userName);

        bool IsEmployee(string userName);

        bool IsLockedEmployee(string userName);

        Account Login(ViewLogin dataDto);

        bool Register(ViewRegister dataDto);

        bool UpdateNewPassword(ViewChangePassword dataDto);

        bool UpdateAvatarOfAdmin(string userName, string avatar);

        Admin GetAdmin(string userName);

        Employee GetEmployee(string userName);

        bool UpdateEmployee(ViewUpdateEmployeeInfo dataDto);

        bool UploadAvatarOfEmployee(string userName, string avatar);
    }
}
