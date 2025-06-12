using System.Data.SqlClient;
using Dapper;
using tutorias.Models;

namespace tutorias.Backend.Authentication
{
    public interface IAuthenticationRepository
    {
        UserModel? login(UserModel user);
        int registerUser(UserModel user);
    }

    public class AuthenticationRepository : IAuthenticationRepository
    {
        private readonly SqlConnection sqlConnection;

        public AuthenticationRepository(SqlConnection sqlConnection)
        {
            this.sqlConnection = sqlConnection;
        }

        public UserModel? login(UserModel user)
        {
            return sqlConnection.QuerySingleOrDefault<UserModel>("SELECT Id, [Name], Email, UserType FROM Users WHERE Email = @Email AND [Password] = @Password", user);
        }

        public int registerUser(UserModel user)
        {
            var sql = @"INSERT INTO Users ([Name], Email, [Password], UserType) 
                        VALUES (@Name, @Email, @Password, @UserType);
                        SELECT CAST(SCOPE_IDENTITY() as int);";
            return sqlConnection.QuerySingle<int>(sql, user);
        }
    }
}
