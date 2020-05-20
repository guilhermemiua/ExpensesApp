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
using JWT.Algorithms;
using JWT.Builder;

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

        [Route("login")]
        [HttpPost]
        public HttpResponseMessage Login([FromBody] User requestBody)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(ConfigurationManager.ConnectionStrings["DB_CONNECTION"]?.ConnectionString))
            {
                string email = requestBody.Email;
                string password = requestBody.Password;

                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { Message = "E-mail Password not provided." });
                }

                // Verify if user exists
                User user = connection.QueryFirstOrDefault<User>("SELECT * FROM users WHERE email = @Email", new { Email = email });

                if (user == null)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { Message = "E-mail or Password incorrect."});
                } 

                // Verify password is correct
                bool isPasswordCorrect = BCrypt.Net.BCrypt.Verify(password, user.Password);

                if (!isPasswordCorrect)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { Message = "E-mail or Password incorrect.", User = user });
                }

                string token = GenerateToken(user.Id);

                return Request.CreateResponse(HttpStatusCode.OK, token);
            }
        }

        private string GenerateToken(int userId)
        {
            // TODO: create secret and store securely
            string secret = "GQDstcKsx0NHjPOuXOYg5MbeJ1XT0uFiwDVvVBrk";

            // Generate token
            string token = new JwtBuilder()
              .WithAlgorithm(new HMACSHA256Algorithm()) 
              .WithSecret(secret)
              .AddClaim("exp", DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds())
              .AddClaim("userId", userId.ToString())
              .Encode();

            return token;
        }
    }

}