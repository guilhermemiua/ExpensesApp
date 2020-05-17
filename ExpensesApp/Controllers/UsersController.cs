using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using ExpensesApp.Models;
using System.Linq.Expressions;
using System.Data.SqlClient;
using System.Configuration;
using Dapper;
using Npgsql;
using System.Web.Services.Description;
using BCrypt.Net;

namespace ExpensesApp.Controllers
{
    [RoutePrefix("api/users")]
    public class UsersController : ApiController
    {
        [Route("")]
        [HttpGet]
        public HttpResponseMessage GetUsers()
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["DB_CONNECTION"]?.ConnectionString))
            {
                IEnumerable<User> users = connection.Query<User>("SELECT name, email FROM users");

                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, users);

                return response;
            }
        }

        [Route("")]
        [HttpPost]
        public HttpResponseMessage Register([FromBody] User user)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["DB_CONNECTION"]?.ConnectionString))
            {
                HttpResponseMessage response;

                if (string.IsNullOrEmpty(user.Name) || string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.Password))
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new { Message = "E-mail, Name or Password not provided." });

                    return response;
                }

                // Verify if email is already registered
                User registeredUser = connection.QueryFirstOrDefault<User>("SELECT * FROM users WHERE email = @Email", new { Email = user.Email });

                if (registeredUser != null)
                {
                    response = Request.CreateResponse(HttpStatusCode.BadRequest, new { Message = "E-mail already registered." });

                    return response;
                }

                // Hashing password
                string passwordHash = BCrypt.Net.BCrypt.HashPassword(user.Password);

                connection.Execute("INSERT INTO users (name, email, password) VALUES (@Name, @Email, @Password);", new { Name = user.Name, Email = user.Email, Password = passwordHash });

                response = Request.CreateResponse(HttpStatusCode.Created);

                return response;
            }
        }
    }

}