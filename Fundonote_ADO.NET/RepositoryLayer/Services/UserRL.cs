using DatabaseLayer;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RepositoryLayer.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RepositoryLayer.Services
{
    public class UserRL : IUserRL
    {
        private readonly string connectionString;
        public UserRL(IConfiguration configuartion)
        {
            connectionString = configuartion.GetConnectionString("Fundoonotes");
        }

        public bool AddUser(UsersModel users)
        {
            SqlConnection connection = new SqlConnection(connectionString);

            try
            {
                using (connection)
                {
                    connection.Open();
                    //Creating a stored Procedure for adding Users into database
                    SqlCommand com = new SqlCommand("spAddUser", connection);
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.AddWithValue("@Firstname", users.FirstName);
                    com.Parameters.AddWithValue("@Lastname", users.LastName);
                    com.Parameters.AddWithValue("@Email", users.Email);
                    com.Parameters.AddWithValue("@password", users.Password);
                    var result = com.ExecuteNonQuery();
                    if (result > 0)
                    {
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<GetAllUserModel> GetAllUser()
        {
            List<GetAllUserModel> listOfUsers = new List<GetAllUserModel>();
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            try
            {
                using (sqlConnection)
                {
                    sqlConnection.Open();
                    SqlCommand cmd = new SqlCommand("spGetAllUser", sqlConnection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        GetAllUserModel getAllUser = new GetAllUserModel();
                        getAllUser.UserId = reader["UserId"]== DBNull.Value ? default : reader.GetInt32("UserId");
                        getAllUser.Firstname = Convert.ToString(reader["Firstname"]);
                        getAllUser.Lastname = Convert.ToString(reader["Lastname"]);
                        getAllUser.Email = Convert.ToString(reader["Email"]);
                        getAllUser.Password = Convert.ToString(reader.ToString());
                        getAllUser.CreateDate = Convert.ToDateTime(reader["CreateDate"]);
                        getAllUser.MoidifyDate = Convert.ToDateTime(reader["MoidifyDate"]);

                        listOfUsers.Add(getAllUser);
                    }
                    return listOfUsers;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                sqlConnection.Close();
            }
        }
    }
}
