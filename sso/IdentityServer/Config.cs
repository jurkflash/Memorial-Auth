// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace Sso
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
                   new IdentityResource[]
                   {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                   };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope("backend"),
                new ApiScope("frontend"),
                new ApiScope("roles")
            };

        public static IEnumerable<Client> GetClients(IConfiguration configuration)
        {
            var redirectUris = configuration.GetSection("IdentityServer:RedirectUris").Get<List<string>>();
            var postLogoutRedirectUris = configuration.GetSection("IdentityServer:PostLogoutRedirectUris").Get<List<string>>();
            var allowedCorsOrigins = configuration.GetSection("IdentityServer:AllowedCorsOrigins").Get<List<string>>();

            return new[]
            {
                // m2m client credentials flow client
                new Client
                {
                    ClientId = "m2m.client",
                    ClientName = "Client Credentials Client",

                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = { new Secret("511536EF-F270-4058-80CA-1C89C192F69A".Sha256()) },

                    AllowedScopes = { "backend" }
                },

                new Client
                {
                    ClientId = "owin.implicit",
                    ClientName = "implicit",

                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,
                    RedirectUris = redirectUris,
                    PostLogoutRedirectUris = postLogoutRedirectUris,
                    AllowedCorsOrigins = allowedCorsOrigins,
                    AllowedScopes = { "openid", "profile", "email", "roles" }
                },

                // interactive client using code flow + pkce
                new Client
                {
                    ClientId = "interactive",
                    ClientSecrets = { new Secret("49C1A7E1-0C79-4A89-A3D6-A37998FB86B0".Sha256()) },

                    AllowedGrantTypes = GrantTypes.Code,

                    RedirectUris = { "https://localhost:44300/signin-oidc" },
                    FrontChannelLogoutUri = "https://localhost:44300/signout-oidc",
                    PostLogoutRedirectUris = { "https://localhost:44300/signout-callback-oidc" },

                    //AllowOfflineAccess = true,
                    AllowedScopes = { "openid", "profile", "frontend" }
                }
            };
        }
    }
}