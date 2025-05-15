using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using BookingAds.Common.Models.Message;
using BookingAds.Common.Repository.Message.Abstractions;
using BookingAds.Modules;

namespace BookingAds.Common.Repository.Employee
{
    using BookingAds.Entities;

    public class MessageRepository : IMessageRepository
    {
        public long CreateMessage(ViewCreateMessage dataDto)
        {
            using (var conn = ConnectDB.BookingAdsDB())
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        var sqlCreateMessage = @"INSERT INTO tbMessages(SenderID
                                , ReceiverID
                                , Content
                                , CreatedTime
                            ) VALUES (@SenderID
                                , @ReceiverID
                                , @Content
                                , @CreatedTime
                            )
                            SELECT CAST(SCOPE_IDENTITY() AS BIGINT)";

                        var parameters = new
                        {
                            SenderID = dataDto.SenderID,
                            ReceiverID = dataDto.ReceiverID,
                            Content = dataDto.Content,
                            CreatedTime = DateTimeUtils.ConvertToDateTimeSQL(dataDto.CreatedTime, "dd/MM/yyyy hh:mm:ss tt"),
                        };

                        var commandCreateMessage = new CommandDefinition(sqlCreateMessage, parameters: parameters, flags: CommandFlags.NoCache, transaction: trans);

                        var result = conn.ExecuteScalar<long>(commandCreateMessage);

                        trans.Commit();

                        return result;
                    }
                    catch (SqlException)
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }

        public bool DeleteMessage(long messageID, string senderID)
        {
            using (var conn = ConnectDB.BookingAdsDB())
            {
                var sqlDeleteMessage = @"DELETE FROM tbMessages
                    WHERE MessageID = @MessageID 
                    AND SenderID = @SenderID";

                var parameters = new
                {
                    MessageID = messageID,
                    SenderID = senderID,
                };

                var command = new CommandDefinition(sqlDeleteMessage, parameters: parameters, flags: CommandFlags.None);

                var result = conn.Execute(command);

                return result != 0;
            }
        }

        public bool EditMessage(ViewEditMessage dataDto)
        {
            using (var conn = ConnectDB.BookingAdsDB())
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        var sqlEditMessage = @"UPDATE tbMessages
                            SET Content = @Content
                            WHERE MessageID = @MessageID
                            AND SenderID = @SenderID";

                        var parameters = new
                        {
                            Content = dataDto.Content,
                            MessageID = dataDto.MessageID,
                            SenderID = dataDto.SenderID,
                        };

                        var command = new CommandDefinition(sqlEditMessage, parameters: parameters, flags: CommandFlags.None, transaction: trans);

                        var result = conn.Execute(command);

                        trans.Commit();

                        return result != 0;
                    }
                    catch (SqlException)
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }

        public IReadOnlyList<Admin> GetAdmins(string searchValue)
        {
            using (var conn = ConnectDB.BookingAdsDB())
            {
                var sqlGetAdmins = @"SELECT UserName,
                    Avatar
                    FROM tbAdmin
                    WHERE @SearchValue = N''
                    OR UserName LIKE @SearchValue";

                var parameters = new
                {
                    SearchValue = $"%{searchValue}%",
                };

                var commandGetAdmins = new CommandDefinition(sqlGetAdmins, parameters: parameters, flags: CommandFlags.NoCache);

                var admins = conn.Query<Admin>(commandGetAdmins)
                    .Distinct()
                    .ToList();

                return admins;
            }
        }

        public IReadOnlyList<Employee> GetEmployees(string searchValue)
        {
            using (var conn = ConnectDB.BookingAdsDB())
            {
                var sqlGetEmployees = @"SELECT EmployeeID
                    , UserName
                    , FirstName
                    , LastName
                    , Avatar
                    FROM tbEmployees
                    WHERE @SearchValue = N''
                    OR UserName LIKE @SearchValue
                    OR FirstName LIKE @SearchValue 
                    OR LastName LIKE @SearchValue";

                var parameters = new
                {
                    SearchValue = $"%{searchValue}%",
                };

                var commandGetEmployees = new CommandDefinition(sqlGetEmployees, parameters: parameters, flags: CommandFlags.NoCache);

                var employees = conn.Query<Employee>(commandGetEmployees)
                    .Distinct()
                    .ToList();

                return employees;
            }
        }

        public IReadOnlyList<Message> GetMessages(string myUserName, string otherUserName, int limit)
        {
            using (var conn = ConnectDB.BookingAdsDB())
            {
                var sqlGetMessages = $@"SELECT TOP {limit} m.SenderID
                    , m.ReceiverID
                    , m.Content
                    , m.CreatedTime
                    , m.ReadTime
                    , m.MessageID
                    FROM tbMessages m
                    WHERE (m.SenderID = @MyUserName 
                    AND m.ReceiverID = @OtherUserName)
                    OR (m.SenderID = @OtherUserName 
                    AND m.ReceiverID = @MyUserName) 
                    ORDER BY CreatedTime DESC";

                var parameters = new
                {
                    MyUserName = myUserName,
                    OtherUserName = otherUserName,
                };

                var commandGetMessages = new CommandDefinition(sqlGetMessages, parameters: parameters, flags: CommandFlags.NoCache);

                var messages = conn.Query<Message>(commandGetMessages)
                    .Distinct()
                    .ToList();

                return messages;
            }
        }

        public bool ReadMessage(ViewReadMessage dataDto)
        {
            int result = 0;

            using (var conn = ConnectDB.BookingAdsDB())
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        var sqlReadMessage = @"UPDATE tbMessages
                            SET ReadTime = @ReadTime
                            WHERE SenderID = @SenderID 
                            AND ReceiverID = @ReceiverID
                            AND ReadTime IS NULL
                        ";

                        var parameters = new
                        {
                            ReadTime = DateTimeUtils.ConvertToDateTimeSQL(dataDto.ReadTime, "dd/MM/yyyy hh:mm:ss tt"),
                            SenderID = dataDto.SenderID,
                            ReceiverID = dataDto.ReceiverID,
                        };

                        var commandReadMessage = new CommandDefinition(sqlReadMessage, parameters: parameters, flags: CommandFlags.NoCache, transaction: trans);

                        result = conn.Execute(commandReadMessage);

                        trans.Commit();
                    }
                    catch (SqlException)
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return result > 0;
        }
    }
}