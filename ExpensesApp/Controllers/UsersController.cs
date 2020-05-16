using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using ExpensesApp.Context;
using ExpensesApp.Models;
using ExpensesApp.DTOs;
using System.Linq.Expressions;

namespace ExpensesApp.Controllers
{
    [RoutePrefix("api/users")]
    public class UsersController : ApiController
    {
        private ExpensesAppContext db = new ExpensesAppContext();

        private static readonly Expression<Func<User, UserDto>> AsUserDto = x => new UserDto
        {
            Id = x.Id,
            Name = x.Name,
            Email = x.Email
        };

        [Route("")]
        public IQueryable<UserDto> GetUsers()
        {
            return db.Users.Select(AsUserDto);
        }

        [Route("{id}")]
        [ResponseType(typeof(UserDto))]
        public async Task<IHttpActionResult> GetUser(int id)
        {
            UserDto user = await db.Users
                .Where(u => u.Id == id)
                .Select(AsUserDto)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [Route("{id}")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutUser(int id, User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != user.Id)
            {
                return BadRequest();
            }

            db.Entry(user).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        [Route("")]
        [ResponseType(typeof(UserDto))]
        public async Task<IHttpActionResult> PostUser(User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Users.Add(user);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = user.Id }, user);
        }

        [Route("{id}")]
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> DeleteUser(int id)
        {
            User user = await db.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            db.Users.Remove(user);
            await db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.OK);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool UserExists(int id)
        {
            return db.Users.Count(e => e.Id == id) > 0;
        }
    }
}