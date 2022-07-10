using DatabaseLayer;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLayer.Interface
{
    public interface IUserBL
    {
        public void AddUser(UsersModel users);
        public List<GetAllUserModel> GetAllUser();
        public string LoginUser(LoginUserModel loginUser);
    }
}
