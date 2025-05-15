using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Dapper;
using BookingAds.Models.ForgotPassword;
using BookingAds.Modules;
using BookingAds.Repository.ForgotPassword.Abstractions;

namespace BookingAds.Repository.ForgotPassword
{
    public class ForgotPasswordRepository : IForgotPasswordRepository
    {
        public string IsPassWord(string username)
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
                        var sqlChangePassWord = @"
                           SELECT Password 
                                FROM tbEmployees 
                                WHERE UserName = @UserName";

                        var parameters = new
                        {
                            UserName = username,
                        };

                        var commandChangePassWord = new CommandDefinition(
                            sqlChangePassWord,
                            parameters: parameters,
                            flags: CommandFlags.NoCache,
                            transaction: trans);

                        var resutl = conn.QueryFirstOrDefault<string>(commandChangePassWord);

                        if (resutl != null)
                        {
                            trans.Commit();
                            return resutl.ToString();
                        }
                        else
                        {
                            trans.Rollback();
                            return resutl = null;
                        }
                    }
                    catch (SqlException)
                    {
                        throw; // Throw exception further up the call stack for handling
                    }
                }
            }
        }

        public bool ChangePassWord(ViewChangePassword data)
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
                        var sqlChangePassWord = @"
                    UPDATE tbEmployees
                    SET Password = @Password 
                    WHERE UserName = @UserName";

                        var parameters = new
                        {
                            UserName = data.Username,
                            Password = PasswordUtils.MD5(data.Password.Trim()),
                        };

                        var commandChangePassWord = new CommandDefinition(
                            sqlChangePassWord,
                            parameters: parameters,
                            flags: CommandFlags.NoCache,
                            transaction: trans);

                        var resutl = conn.Execute(commandChangePassWord);

                        if (resutl > 0)
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
                        throw; // Throw exception further up the call stack for handling
                    }
                }
            }
        }

        public bool CheckConfirmCode(ViewConfirmCode data)
        {
            using (var conn = ConnectDB.BookingAdsDB())
            {
                int getIdEmployee = GetIdEmployee(data.Username);

                var sqlCheckConfirmCode = $@"SELECT COUNT(*) 
                                              FROM tbAuthorizationCode
                                              WHERE CodeConfirm = @CodeConfirm AND EmployeeID = @EmployeeId";

                var param = new { CodeConfirm = data.ConfirmCode.Trim(), EmployeeId = getIdEmployee };

                var commandCheckConfirmCode = new CommandDefinition(sqlCheckConfirmCode, parameters: param, flags: CommandFlags.NoCache);
                var result = conn.QuerySingleOrDefault<int>(commandCheckConfirmCode);
                if (result > 0)
                {
                    return true;
                }

                return false;
            }
        }

        public bool CheckTimeToken(string token, string username)
        {
            using (var conn = ConnectDB.BookingAdsDB())
            {
                DateTime dateNow = DateTime.Now.AddMinutes(-5);

                var employeesId = GetIdEmployee(username);
                var sqlCheckUserName = $@"SELECT COUNT(*) 
                                            FROM tbAuthorizationCode 
                                            WHERE InitTime > DATEADD(minute, 3, TimeNewToken) AND EmployeeID = {employeesId}";

                var param = new { Token = token };

                var commandCheckToken = new CommandDefinition(sqlCheckUserName, parameters: param, flags: CommandFlags.NoCache);
                var result = conn.QuerySingleOrDefault<int>(commandCheckToken);
                if (result > 0)
                {
                    return true;
                }

                return false;
            }
        }

        public bool CheckToken(string token, string username)
        {
            using (var conn = ConnectDB.BookingAdsDB())
            {
                var sqlCheckUserName = $@"SELECT COUNT(*) 
                                              FROM tbAuthorizationCode
                                              WHERE Token = @Token";

                var param = new { Token = token };

                var commandCheckToken = new CommandDefinition(sqlCheckUserName, parameters: param, flags: CommandFlags.NoCache);
                var result = conn.QuerySingleOrDefault<int>(commandCheckToken);
                if (result > 0)
                {
                    return true;
                }

                return false;
            }
        }

        public bool CheckUserName(string username)
        {
            using (var conn = ConnectDB.BookingAdsDB())
            {
                var sqlCheckUserName = $@"SELECT COUNT(*) 
                                              FROM tbEmployees
                                              WHERE UserName = @UserName";

                var param = new { UserName = username };

                var commandCheckUserName = new CommandDefinition(sqlCheckUserName, parameters: param, flags: CommandFlags.NoCache);
                var result = conn.QuerySingleOrDefault<int>(commandCheckUserName);
                if (result > 0)
                {
                    return true;
                }

                return false;
            }
        }

        public string CreateToken(string token, string username)
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
                        DateTime dateNow = DateTime.Now;
                        var employeesId = GetIdEmployee(username);
                        var sqlUpdateToken = $@"UPDATE tbAuthorizationCode
                            SET Token = @Token , TimeNewToken = @TimeNewToken 
                            WHERE EmployeeID = @EmployeeID";

                        var parameters = new
                        {
                            Token = token,
                            TimeNewToken = dateNow,
                            EmployeeID = employeesId,
                        };

                        var commandUpdateToken = new CommandDefinition(sqlUpdateToken, parameters: parameters, flags: CommandFlags.NoCache, transaction: trans);

                        conn.Execute(commandUpdateToken);

                        var sqlGetToken = $@"Select Token 
                            FROM tbAuthorizationCode                             
                            WHERE EmployeeID = @EmployeeID";

                        var parameters1 = new
                        {
                            EmployeeID = employeesId,
                        };

                        var commandsqlGetToken = new CommandDefinition(sqlGetToken, parameters: parameters1, flags: CommandFlags.NoCache, transaction: trans);

                        string result = conn.QuerySingleOrDefault<string>(commandsqlGetToken);
                        trans.Commit();
                        if (result != null)
                        {
                            return result.ToString();
                        }
                        else
                        {
                            return null;
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

        public string GetCode(ViewForgotPassword data)
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
                        var employeesId = GetIdEmployee(data.Username);
                        var sqlGetCode = $@"SELECT CodeConfirm FROM tbAuthorizationCode
                                                        WHERE EmployeeID = @EmployeeID";

                        var parameters = new
                        {
                            EmployeeID = employeesId,
                        };

                        var commandGetCode = new CommandDefinition(sqlGetCode, parameters: parameters, flags: CommandFlags.NoCache, transaction: trans);
                        string result = conn.QuerySingleOrDefault<string>(commandGetCode);
                        trans.Commit();
                        if (result != null)
                        {
                            return result.ToString();
                        }
                        else
                        {
                            return null;
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

        public string GetToken(string username)
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
                        var employeesId = GetIdEmployee(username);
                        var sqlGetToken = $@"Select Token 
                            FROM tbAuthorizationCode                             
                            WHERE EmployeeID = @EmployeeID";

                        var parameters1 = new
                        {
                            EmployeeID = employeesId,
                        };

                        var commandGetToken = new CommandDefinition(sqlGetToken, parameters: parameters1, flags: CommandFlags.NoCache, transaction: trans);

                        string result = conn.QuerySingleOrDefault<string>(commandGetToken);
                        trans.Commit();
                        if (result != null)
                        {
                            return result.ToString();
                        }
                        else
                        {
                            return null;
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

        public string GetPhone(string email)
        {
            using (var conn = ConnectDB.BookingAdsDB())
            {
                var sqlCheckConfirmCode = $@"SELECT Phone 
                                              FROM tbEmployees
                                              WHERE Username = @Username";

                var param = new { Username = email };

                var commandCheckConfirmCode = new CommandDefinition(sqlCheckConfirmCode, parameters: param, flags: CommandFlags.NoCache);
                var result = conn.QuerySingleOrDefault<string>(commandCheckConfirmCode);
                return result;
            }
        }

        public bool DeleteToken(string token, string username)
        {
            using (var conn = ConnectDB.BookingAdsDB())
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                try
                {
                    int employeesId = GetIdEmployee(username);
                    var sqlDeleteToken = $@"UPDATE tbAuthorizationCode
                            SET Token = NULL , TimeNewToken = NULL 
                            WHERE EmployeeID = {employeesId} AND Token = @Token";

                    var parameters = new
                    {
                        Token = token,
                    };

                    var commandDeleteToken = new CommandDefinition(sqlDeleteToken, parameters: parameters, flags: CommandFlags.NoCache);
                    int result = conn.Execute(commandDeleteToken);

                    if (result > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (SqlException)
                {
                    throw;
                }
            }
        }

        private int GetIdEmployee(string username)
        {
            using (var conn = ConnectDB.BookingAdsDB())
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                try
                {
                    var sqlDeleteToken = $@"SELECT EmployeeID 
                                                FROM tbEmployees
                                                WHERE UserName = @UserName";

                    var parameters = new
                    {
                        UserName = username,
                    };

                    var commandGetIdEmployee = new CommandDefinition(sqlDeleteToken, parameters: parameters, flags: CommandFlags.NoCache);
                    int result = conn.QuerySingleOrDefault<int>(commandGetIdEmployee);

                    if (result > 0)
                    {
                        return result;
                    }
                    else
                    {
                        return result = 0;
                    }
                }
                catch (SqlException)
                {
                    throw;
                }
            }
        }

    }
}
