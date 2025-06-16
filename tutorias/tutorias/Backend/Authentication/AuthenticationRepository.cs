using Dapper;
using System.Data;
using System.Data.SqlClient;
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
        private readonly IDbConnection sqlConnection;

        public AuthenticationRepository(IDbConnection sqlConnection)
        {
            this.sqlConnection = sqlConnection;
        }

        public UserModel? login(UserModel user)
        {
            return sqlConnection.QuerySingleOrDefault<UserModel>("SELECT Id, [Name], Email, UserType FROM Users WHERE Email = @Email AND [Password] = @Password", user);
        }

        public int registerUser(UserModel user)
        {
            var emailExists = sqlConnection.QueryFirstOrDefault<int>(
                "SELECT COUNT(1) FROM Users WHERE Email = @Email", new { user.Email });
            if (emailExists > 0)
            {
                throw new InvalidOperationException($"El correo {user.Email} ya está asociado a un usuario en el sistema, debe usar otro");
            }

            var sql = @"INSERT INTO Users ([Name], Email, [Password], UserType) 
                        VALUES (@Name, @Email, @Password, @UserType);
                        SELECT CAST(SCOPE_IDENTITY() as int);";
            if (sqlConnection.GetType().Name.Contains("Sqlite"))
            {
                sql = @"INSERT INTO Users (Name, Email, Password, UserType)
                VALUES (@Name, @Email, @Password, @UserType);
                SELECT last_insert_rowid();";
            }
            return sqlConnection.QuerySingle<int>(sql, user);
        }
    }
}
