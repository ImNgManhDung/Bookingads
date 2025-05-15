using System.Data;
using System.Data.SqlClient;
using Dapper;
using BookingAds.Common.Models.Account;
using BookingAds.Common.Repository.Account.Abstractions;
using BookingAds.Modules;

namespace BookingAds.Common.Repository.Account
{
    using BookingAds.Entities;
    using BookingAds.Models.Register;
    using Microsoft.AspNet.SignalR.Messaging;

    public class AccountRepository : IAccountRepository
    {
        public Admin GetAdmin(string userName)
        {
            using (var conn = ConnectDB.BookingAdsDB())
            {
                var sqlGetAdmin = @"SELECT UserName
                    , Password
                    , Avatar
                    FROM tbAdmin 
                    WHERE UserName = @UserName";

                var parameters = new
                {
                    UserName = userName,
                };

                var command = new CommandDefinition(sqlGetAdmin, parameters: parameters, flags: CommandFlags.NoCache);
                var admin = conn.QueryFirstOrDefault<Admin>(command);

                return admin;
            }
        }

        public Employee GetEmployee(string userName)
        {
            using (var conn = ConnectDB.BookingAdsDB())
            {
                var sqlGetEmployee = @"SELECT EmployeeID
                    , UserName
                    , Password
                    , FirstName
                    , LastName
                    , Gender
                    , Avatar
                    , LockedAt
                    , Coin
                    , Phone
                    FROM tbEmployees 
                    WHERE UserName = @UserName
                    AND LockedAt IS NULL";

                var parameters = new
                {
                    UserName = userName,
                };

                var command = new CommandDefinition(sqlGetEmployee, parameters: parameters, flags: CommandFlags.NoCache);
                var employee = conn.QueryFirstOrDefault<Employee>(command);

                return employee;
            }
        }

        public bool IsAdmin(string userName)
        {
            Admin admin = null;

            using (var conn = ConnectDB.BookingAdsDB())
            {
                var sqlGetAdmin = @"SELECT UserName
                    /* , Password */
                    , Avatar
                    FROM tbAdmin 
                    WHERE UserName = @UserName";

                var parameters = new
                {
                    UserName = userName,
                };

                var command = new CommandDefinition(sqlGetAdmin, parameters: parameters, flags: CommandFlags.NoCache);
                admin = conn.QueryFirstOrDefault<Admin>(command);
            }

            return admin != null;
        }

        public bool IsEmployee(string userName)
        {
            Employee employee = null;

            using (var conn = ConnectDB.BookingAdsDB())
            {
                var sqlGetEmployee = @"SELECT EmployeeID
                    , UserName
                    /* , Password */
                    , FirstName
                    , LastName
                    , Gender
                    , Avatar
                    , LockedAt
                    , Coin
                    , Phone
                    FROM tbEmployees 
                    WHERE UserName = @UserName 
                    AND LockedAt IS NULL";

                var parameters = new
                {
                    UserName = userName,
                };

                var command = new CommandDefinition(sqlGetEmployee, parameters: parameters, flags: CommandFlags.NoCache);
                employee = conn.QueryFirstOrDefault<Employee>(command);
            }

            return employee != null;
        }

        public bool IsLockedEmployee(string userName)
        {
            using (var conn = ConnectDB.BookingAdsDB())
            {
                var sqlGetEmployee = @"SELECT EmployeeID
                    FROM tbEmployees 
                    WHERE UserName = @UserName 
                    AND LockedAt IS NOT NULL";

                var parameters = new
                {
                    UserName = userName,
                };

                var command = new CommandDefinition(sqlGetEmployee, parameters: parameters, flags: CommandFlags.NoCache);
                var employee = conn.QueryFirstOrDefault<Employee>(command);
                return employee != null;
            }
        }

        public Account Login(ViewLogin dataDto)
        {
            Admin admin = null;
            Employee employee = null;

            using (IDbConnection conn = ConnectDB.BookingAdsDB())
            {
                var sqlGetAdmin = $@"SELECT UserName
                     , Password 
                    , Avatar
                    FROM tbAdmin 
                    WHERE UserName = @UserName 
                    AND Password = @Password";

                string sqlGetEmployee = $@"SELECT EmployeeID
                    , UserName
                     , Password 
                    , FirstName
                    , LastName
                    , Gender
                    , Avatar
                    , LockedAt
                    , Coin
                    , Phone
                    FROM tbEmployees 
                    WHERE UserName = @UserName 
                    AND Password = @Password 
                    AND LockedAt IS NULL";

                var parameters = new
                {
                    UserName = dataDto.UserName,
                    Password = PasswordUtils.MD5(dataDto.Password), // MD5 (default value: Admin@123)
                };

                var commandGetAdmin = new CommandDefinition(sqlGetAdmin, parameters: parameters, flags: CommandFlags.NoCache);
                admin = conn.QueryFirstOrDefault<Admin>(commandGetAdmin);

                var commandGetEmployee = new CommandDefinition(sqlGetEmployee, parameters: parameters, flags: CommandFlags.NoCache);
                employee = conn.QueryFirstOrDefault<Employee>(commandGetEmployee);
            }

            return admin != null
                    ? admin as Account
                    : employee as Account;
        }

        public bool Register(ViewRegister dataDto)
        {
            using (IDbConnection conn = ConnectDB.BookingAdsDB())
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        string sqlCreateEmployee = $@"INSERT INTO [dbo].[tbEmployees]
                                      ([UserName]
                                      ,[Password]
                                      ,[FirstName]
                                      ,[LastName]
                                      ,[Gender]
                                      ,[Avatar]
                                      ,[LockedAt]
                                      ,[Coin]
                                      ,[Phone])
                                    VALUES
                                      (@UserName,@Password,@FirstName,@LastName,@Gender,null,null,null,@Phone)";

                        var parameters = new
                        {
                            UserName = dataDto.Email,
                            Password = PasswordUtils.MD5(dataDto.Password),
                            FirstName = dataDto.FirstName,
                            LastName = dataDto.LastName,
                            Gender = dataDto.Gender,
                            Phone = dataDto.Phone,
                        };

                        var commandCreateEmployee = new CommandDefinition(
                            sqlCreateEmployee,
                            parameters: parameters,
                            transaction: trans, // Include the transaction here
                            flags: CommandFlags.NoCache
                        );

                        int rowsAffected = conn.Execute(commandCreateEmployee);

                        if (rowsAffected > 0)
                        {
                            trans.Commit();
                            return true;
                        }
                        else
                        {
                            trans.Rollback();
                            return false;
                        }
                    }
                    catch (SqlException)
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }


        public bool UpdateAvatarOfAdmin(string userName, string avatar)
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
                        var sqlUpdateAvatarOfAdmin = @"UPDATE tbAdmin
                            SET Avatar = @Avatar
                            WHERE UserName = @UserName";

                        var parameters = new
                        {
                            Avatar = avatar,
                            UserName = userName,
                        };

                        var command = new CommandDefinition(sqlUpdateAvatarOfAdmin, parameters: parameters, flags: CommandFlags.NoCache, transaction: trans);

                        var result = conn.Execute(command);

                        trans.Commit();

                        return result == 1;
                    }
                    catch (SqlException)
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }

        public bool UpdateEmployee(ViewUpdateEmployeeInfo dataDto)
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
                        var sqlUpdateEmployee = @"UPDATE tbEmployees
                            SET FirstName = @FirstName
                            , LastName = @Lastname
                            , Phone = @Phone
                            , Gender = @Gender
                            WHERE UserName = @UserName";

                        var parameters = new
                        {
                            FirstName = dataDto.FirstName,
                            Lastname = dataDto.LastName,
                            Phone = dataDto.Phone,
                            Gender = dataDto.Gender,
                            UserName = dataDto.UserName,
                        };

                        var command = new CommandDefinition(sqlUpdateEmployee, parameters: parameters, flags: CommandFlags.NoCache, transaction: trans);

                        var result = conn.Execute(command);

                        trans.Commit();

                        return result == 1;
                    }
                    catch (SqlException)
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }

        public bool UpdateNewPassword(ViewChangePassword dataDto)
        {
            int resultAdmin = 0;
            int resultEmployee = 0;

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
                        var sqlUpdatePasswordAdmin = $@"UPDATE tbAdmin
                            SET Password = @Password 
                            WHERE UserName = @UserName ";

                        var sqlUpdatePasswordEmployee = $@"UPDATE tbEmployees
                            SET Password = @Password 
                            WHERE UserName = @UserName ";

                        var parameters = new
                        {
                            UserName = dataDto.CurrentUserName,
                            Password = PasswordUtils.MD5(dataDto.ConfirmPassword), // MD5 (default value: Employee@123)
                        };

                        var commandUpdatePasswordAdmin = new CommandDefinition(sqlUpdatePasswordAdmin, parameters: parameters, flags: CommandFlags.NoCache, transaction: trans);
                        resultAdmin = conn.Execute(commandUpdatePasswordAdmin);

                        var commandUpdatePasswordEmployee = new CommandDefinition(sqlUpdatePasswordEmployee, parameters: parameters, flags: CommandFlags.NoCache, transaction: trans);
                        resultEmployee = conn.Execute(commandUpdatePasswordEmployee);

                        trans.Commit();
                    }
                    catch (SqlException)
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            return resultAdmin == 1 || resultEmployee == 1;
        }

        public bool UploadAvatarOfEmployee(string userName, string avatar)
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
                        var sqlUploadAvatarOfEmployee = @"UPDATE tbEmployees
                            SET Avatar = @Avatar
                            WHERE UserName = @UserName";

                        var parameters = new
                        {
                            Avatar = avatar,
                            UserName = userName,
                        };

                        var command = new CommandDefinition(sqlUploadAvatarOfEmployee, parameters: parameters, flags: CommandFlags.NoCache, transaction: trans);

                        var result = conn.Execute(command);

                        trans.Commit();

                        return result == 1;
                    }
                    catch (SqlException)
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}