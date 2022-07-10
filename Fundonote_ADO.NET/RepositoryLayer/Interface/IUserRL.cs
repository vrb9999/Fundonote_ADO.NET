using DatabaseLayer;
using System;
using System.Collections.Generic;
using System.Text;

namespace RepositoryLayer.Interface
{
    public interface IUserRL
    {
        public bool AddUser(UsersModel users);
        public List<GetAllUserModel> GetAllUser();
        public string LoginUser(LoginUserModel loginUser);
    }
}
