using GrapeCity.Enterprise.Identity.ExternalIdentityProvider;
using System;
using System.Collections.Generic;
using System.Text;

namespace MySecurityProvider
{
    public class MyUserDescriptor: IExternalUserDescriptor
    {
        public UserInfo UserInformation;
        public string ExternalUserId
        {
            get
            {
                if (null == UserInformation) return null;
                return UserInformation.username;
            }
        }
        public string ExternalUserName
        {
            get
            {
                if (null == UserInformation) return null;
                return UserInformation.username;
            }
        }

        public string ExternalProvider
        {
            get
            {
                return Constants.ProviderName;
            }
        }

    }
}
