using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ExpensesApp.Models;
using System.Configuration;
using Dapper;
using Npgsql;
using System.Web.Services.Description;
using BCrypt.Net;
using ExpensesApp.DTOs;

namespace ExpensesApp.Controllers
{
    [Authorize]
    [RoutePrefix("api/users")]
    public class UsersController : ApiController
    {
        [Route("")]
        [HttpGet]
        public HttpResponseMessage GetUsers()
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["DB_CONNECTION"]?.ConnectionString))
            {
                IEnumerable<User> users = connection.Query<User>("SELECT id, name, email FROM users");

                return Request.CreateResponse(HttpStatusCode.OK, users);
            }
        }

        [Route("")]
        [HttpPost]
        public HttpResponseMessage Register([FromBody] User user)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["DB_CONNECTION"]?.ConnectionString))
            {
                if (string.IsNullOrEmpty(user.Name) || string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.Password))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { Message = "E-mail, Name or Password not provided." });
                }

                // Verify if email is already registered
                User registeredUser = connection.QueryFirstOrDefault<User>("SELECT * FROM users WHERE email = @Email", new { Email = user.Email });

                if (registeredUser != null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { Message = "E-mail already registered." });
                }

                // Hashing password
                string passwordHash = BCrypt.Net.BCrypt.HashPassword(user.Password);

                connection.Execute("INSERT INTO users (name, email, password) VALUES (@Name, @Email, @Password);", new { Name = user.Name, Email = user.Email, Password = passwordHash });

                return Request.CreateResponse(HttpStatusCode.Created);
            }
        }
    }

}