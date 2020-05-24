using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Owin.Security.OAuth;
using System.Security.Claims;
using System.Threading.Tasks;
using Dapper;
using Npgsql;
using System.Configuration;
using ExpensesApp.Models;

namespace ExpensesApp.Providers
{
    public class AccessTokenProvider: OAuthAuthorizationServerProvider
    {
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
        }
        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["DB_CONNECTION"]?.ConnectionString))
            {
                User user = connection.QueryFirstOrDefault<User>("SELECT * FROM users WHERE name = @UserName", new { UserName = context.UserName });

                if (user != null)
                {
                    var identity = new ClaimsIdentity(context.Options.AuthenticationType);
                    identity.AddClaim(new Claim("sub", context.UserName));
                    identity.AddClaim(new Claim("role", "user"));
                    context.Validated(identity);
                }
                else
                {
                    context.SetError("Invalid Access", "");
                    return;
                }
            }
        }
    }
}