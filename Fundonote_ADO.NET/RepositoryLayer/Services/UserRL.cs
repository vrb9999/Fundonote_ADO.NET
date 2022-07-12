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
using Experimental.System.Messaging;

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
                        getAllUser.Password = Convert.ToString(reader["Password"]);
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
        public string LoginUser(LoginUserModel userLogin)
        {
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            try
            {
                using (sqlConnection)
                {
                    sqlConnection.Open();
                    SqlCommand cmd = new SqlCommand("spUserLogin", sqlConnection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Email", userLogin.Email);
                    cmd.Parameters.AddWithValue("@Password", userLogin.Password);
                    cmd.ExecuteNonQuery();

                    SqlDataReader reader = cmd.ExecuteReader();
                    GetAllUserModel response = new GetAllUserModel();
                    if (reader.Read())
                    {
                        response.UserId = reader["UserId"] == DBNull.Value ? default : reader.GetInt32("UserId");
                        response.Email = reader["Email"] == DBNull.Value ? default : reader.GetString("Email");
                        response.Password = reader["Password"] == DBNull.Value ? default : reader.GetString("Password");
                    }
                    return GenerateJWTToken(response.Email, response.UserId);
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

        //Method to Generate JWT Token for Athuntication and Athorization
        private string GenerateJWTToken(string email, int userId)
        {
            try
            {
                // generate token
                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenKey = Encoding.ASCII.GetBytes("THIS_IS_MY_KEY_TO_GENERATE_TOKEN");
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                    new Claim("Email", email),
                    new Claim("UserId",userId.ToString())
                    }),
                    Expires = DateTime.UtcNow.AddHours(2),

                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(token);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //Method to send a RestPassword Link to the User using MSMQ
        public bool UserForgetPassword(string Email)
        {
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            try
            {
                using (sqlConnection)
                {
                    sqlConnection.Open();
                    SqlCommand cmd = new SqlCommand("spUserForgetPassword", sqlConnection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("email", Email);

                    SqlDataReader reader = cmd.ExecuteReader();
                    GetAllUserModel response = new GetAllUserModel();
                    if (reader.Read())
                    {
                        response.UserId = reader["UserId"] == DBNull.Value ? default : reader.GetInt32("UserId");
                        response.Email = reader["Email"] == DBNull.Value ? default : reader.GetString("Email");
                        response.Firstname = reader["Firstname"] == DBNull.Value ? default : reader.GetString("Firstname");
                    }
                    MessageQueue messageQueue;
                    //add message to queue
                    if (MessageQueue.Exists(@".\private$\FundonoteQueue"))
                    {
                        messageQueue = new MessageQueue(@".\private$\FundonoteQueue");
                    }
                    else
                    {
                        messageQueue = MessageQueue.Create(@".\private$\FundonoteQueue");
                    }

                    Message Mymessage = new Message();
                    Mymessage.Formatter = new BinaryMessageFormatter();
                    Mymessage.Body = GenerateJWTToken(Email, response.UserId);
                    Mymessage.Label = "Forget Password Email";
                    messageQueue.Send(Mymessage);

                    Message msg = messageQueue.Receive();
                    msg.Formatter = new BinaryMessageFormatter();
                    EmailService.SendEmail(Email, msg.Body.ToString(), response.Firstname);
                    messageQueue.ReceiveCompleted += new ReceiveCompletedEventHandler(msmQueue_ReceiveCompleted);
                    messageQueue.BeginReceive();
                    messageQueue.Close();

                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void msmQueue_ReceiveCompleted(object sender, ReceiveCompletedEventArgs e)
        {
            try
            {
                MessageQueue queue = (MessageQueue)sender;
                Message msg = queue.EndReceive(e.AsyncResult);
                EmailService.SendEmail(e.Message.ToString(), GenerateToken(e.Message.ToString()), e.Message.ToString());
                queue.BeginReceive();
            }
            catch (MessageQueueException ex)
            {
                if (ex.MessageQueueErrorCode == MessageQueueErrorCode.AccessDenied)
                {
                    Console.WriteLine("Access Denied!!" + "Queue might be system queue...");
                }
            }
        }

        private string GenerateToken(string email)
        { //generate token
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenKey = Encoding.ASCII.GetBytes("THIS_IS_MY_KEY_TO_GENERATE_TOKEN");
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim("Email", email)
                    }),
                    Expires = DateTime.UtcNow.AddHours(2),

                    SigningCredentials = new SigningCredentials(
                                        new SymmetricSecurityKey(tokenKey),
                                        SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(token);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
