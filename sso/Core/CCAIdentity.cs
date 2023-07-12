using Microsoft.AspNetCore.Identity;
using System;

namespace Sso.Core
{
    public class CCAIdentity : IdentityUser
    {
        public string Name { get; set; }
        public DateTime CreatedUtcDatetime { get; set; }
        public DateTime LastModifiedUtcDateTime { get; set; }
    }
}
