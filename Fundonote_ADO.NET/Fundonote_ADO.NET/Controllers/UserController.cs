using BusinessLayer.Interface;
using DatabaseLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Fundonote_ADO.NET.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        IUserBL userBL;
        public UserController(IUserBL userBL)
        {
            this.userBL = userBL;
        }
        [HttpPost("Register")]
        public IActionResult AddUser(UsersModel users)
        {
            try
            {
                this.userBL.AddUser(users);
                return this.Ok(new { success = true, Message = "User Registration Sucessfull" });

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet("GetAllUser")]
        public IActionResult GetAllUser()
        {
            try
            {
                List<GetAllUserModel> listOfUser = new List<GetAllUserModel>();
                listOfUser = this.userBL.GetAllUser();
                return Ok(new { sucess = true, Message = "Data Fetched Successfully...", data = listOfUser });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost("Login")]
        public IActionResult CheckUser(LoginUserModel user)
        {
            try
            {
                string result = this.userBL.LoginUser(user);
                return Ok(new { success = true, Message = "Token Generated successfully", data = result });

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost("ForgetPassword/{email}")]
        public IActionResult UserForgetPassword(string email)
        {
            try
            {
                bool result = this.userBL.UserForgetPassword(email);
                return Ok(new { sucess = true, Message = "Password Reset Link sent Successfully..." });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
