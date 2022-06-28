using GrapeCity.Enterprise.Identity.ExternalIdentityProvider;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MySecurityProvider
{
    public class MyUserContext : IExternalUserContext
    {
        public UserInfo UserInformation;

        //IEnumerable<string> IExternalUserContext.Keys => throw new NotImplementedException();
        IEnumerable<string> IExternalUserContext.Keys
        {
            get{ return new[] { "Name", "Title"}; }
        }

        Task<string> IExternalUserContext.GetValueAsync(string key)
        {
            if (null == UserInformation) return null;
            if (string.IsNullOrWhiteSpace(key)) return null;
            string value = "";
            if (key.ToUpper() == "NAME")
            {
                value = UserInformation.username;
            }
            else if (key.ToUpper() == "TITLE")
            {
                value = UserInformation.title;
            }
            else if (key.ToUpper() == "TENANT_ID")
            {
                value = UserInformation.tenant_id;
            }
            return Task.FromResult<string>(value);
        }

        Task<IEnumerable<string>> IExternalUserContext.GetValuesAsync(string key)
        {
            if (null == UserInformation) return null;
            if (string.IsNullOrWhiteSpace(key)) return null;
            string[] values = new string[] { UserInformation.username, UserInformation.title };
            return Task.FromResult<IEnumerable<string>>(values);
        }


    }
}
