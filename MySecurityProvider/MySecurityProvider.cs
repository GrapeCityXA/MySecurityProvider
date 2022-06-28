using GrapeCity.Enterprise.Identity.ExternalIdentityProvider;
using GrapeCity.Enterprise.Identity.ExternalIdentityProvider.Configuration;
using GrapeCity.Enterprise.Identity.SecurityProvider;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MySecurityProvider
{
    public class MySecurityProvider : ISecurityProvider
    {
        List<ConfigurationItem> _settings;
        public MySecurityProvider(IEnumerable<ConfigurationItem> settings)
        {
            // 构造函数中，获取用户输入的配置参数值（比如数据库连接字串）
            _settings = (List<ConfigurationItem>)settings;

            foreach (var conf in _settings)
            {
                if (conf.Name == "DatabaseConnection")
                {
                    Database.SetConnectionString(conf.Text());
                    break;
                }
            }
        }

        public string ProviderName
        {
            get
            {
                return Constants.ProviderName;
            }
        }


        public Task DisposeTokenAsync(string token)
        {
            return Task.Delay(0);
        }

        public Task<string> GenerateTokenAsync(string username, string password, object customizedParam = null)
        {
            string rst = null;
            try
            {
                Database.WriteLogS("GenerateTokenAsync", "username: " + username + " password: " + password);

                var userInfo = Database.GetUserInfo(username, password);
                var token = JsonConvert.SerializeObject(userInfo);
                rst = Convert.ToBase64String(Encoding.UTF8.GetBytes(token));

                Database.WriteLogS("GenerateTokenAsync token=", rst);
                return Task.FromResult(rst);
            }
            catch (Exception e)
            {
                Database.WriteLogS("GenerateTokenAsync", e.ToString());
                return null;
            }

        }

        public Task<IExternalUserContext> GetUserContextAsync(string token)
        {
            //return Task.Run(() => InMemoryUsers.Users[token] as IExternalUserContext);
            try
            {
                MyUserContext userContext = new MyUserContext();
                userContext.UserInformation = Database.GetUserInfoFromToken(token);
                Database.WriteLogS("GetUserContextAsync userContext.UserInformation.UserName=", userContext.UserInformation.username);
                return Task.FromResult<IExternalUserContext>(userContext);
            }
            catch (Exception e)
            {
                Database.WriteLogS("GetUserContextAsync", e.ToString());
                return null;
            }
        }

        public Task<IExternalUserDescriptor> GetUserDescriptorAsync(string token)
        {
            //return null; // Task.Run(() => InMemoryUsers.Users[token] as IExternalUserDescriptor);
            MyUserDescriptor userDescriptor = new MyUserDescriptor();
            userDescriptor.UserInformation = Database.GetUserInfoFromToken(token);
            return Task.FromResult<IExternalUserDescriptor>(userDescriptor);
        }

        public Task<string[]> GetUserOrganizationsAsync(string token)
        {
            try
            {
                UserInfo userInfo = Database.GetUserInfoFromToken(token);
                if (null == userInfo) return Task.FromResult<string[]>(new string[0]);
                string depNames = userInfo.depnames;
                if (string.IsNullOrWhiteSpace(depNames)) return Task.FromResult<string[]>(new string[0]);
                string[] depNameList = depNames.Split(new char[] { ',' });
                Database.WriteLogS("GetUserOrganizationsAsync depNames=", depNames);
                return Task.FromResult<string[]>(depNameList);

            }
            catch (Exception e)
            {
                Database.WriteLogS("GetUserOrganizationsAsync", e.ToString());
                return Task.FromResult<string[]>(new string[0]);
            }
            //return null; // Task.Run(() => InMemoryUsers.Users[token].Groups?.ToArray());
        }

        public Task<string[]> GetUserRolesAsync(string token)
        {
            try
            {
                UserInfo userInfo = Database.GetUserInfoFromToken(token);
                if (null == userInfo) return Task.FromResult<string[]>(new string[0]);
                string roleNames = userInfo.rolenames;
                if (string.IsNullOrWhiteSpace(roleNames)) return Task.FromResult<string[]>(new string[0]);
                string[] roles = roleNames.Split(new char[] { ',' });
                Database.WriteLogS("GetUserRolesAsync roleNames=", roleNames);
                return Task.FromResult<string[]>(roles);

            }
            catch (Exception e)
            {
                Database.WriteLogS("GetUserRolesAsync", e.ToString());
                return Task.FromResult<string[]>(new string[0]);
            }
            //return null; // Task.Run(() => InMemoryUsers.Users[token].Roles?.ToArray());
        }

        public Task<bool> ValidateTokenAsync(string token)
        {
            try
            {
                bool isValid = false;
                UserInfo userInfo = Database.GetUserInfoFromToken(token);
                if (null != userInfo) isValid = true;
                Database.WriteLogS("ValidateTokenAsync isValid=", isValid.ToString());
                return Task.FromResult<bool>(isValid);
            }
            catch (Exception e)
            {
                Database.WriteLogS("ValidateTokenAsync", e.ToString());
                return null;
            }
            //return null;  // Task.Run(() => InMemoryUsers.Users.Keys.Contains(token));
        }
    }
}
