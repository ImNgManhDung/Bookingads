using System.Collections.Generic;
using BookingAds.Common.Models.Message;

namespace BookingAds.Common.Repository.Message.Abstractions
{
    using BookingAds.Entities;

    internal interface IMessageRepository
    {
        IReadOnlyList<Employee> GetEmployees(string searchValue);

        IReadOnlyList<Message> GetMessages(string myUserName, string otherUserName, int limit);

        IReadOnlyList<Admin> GetAdmins(string searchValue);

        long CreateMessage(ViewCreateMessage dataDto);

        bool ReadMessage(ViewReadMessage dataDto);

        bool EditMessage(ViewEditMessage dataDto);

        bool DeleteMessage(long messageID, string senderID);
    }
}
