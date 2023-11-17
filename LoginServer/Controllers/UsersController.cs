using LoginManager.Models;
using LoginServer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LoginServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserInteraction _context;
        private readonly IConfiguration _configuration;
        public UsersController(UserInteraction context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // TODO: Implement Basic Get
        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUserInfo()
        {
            if (_context.UserInfo == null)
            {
                return NotFound();
            }
            var userList = await _context.UserInfo.ToListAsync();
            var userDTOList = userList.Select(User => new UserDto
            {
                Username = User.Username,
                EncryptionHash = User.AccountCreation.GetHashCode().ToString()
            }).ToList();

            return userDTOList;
        }

        // TODO: Look over this
        // GET: api/Users/5
        //[HttpGet("{id}")]
        //public async Task<ActionResult<User>> GetUser(string id)
        //{
        //    if (_context.UserInfo == null)
        //    {
        //        return NotFound();
        //    }
        //    var user = await _context.UserInfo.FindAsync(id);

        //    if (user == null)
        //    {
        //        return NotFound();
        //    }

        //    return user;
        //}

        // GET: api/Users/{username}/{password}
        [HttpGet("{username}/{password}")]
        public async Task<ActionResult<User>> GetUserToken(string username, string password)
        {
            if (_context.UserInfo == null)
            {
                return NotFound();
            }
            var user = await _context.UserInfo.FindAsync(username);

            if (user == null)
                return NotFound("User not found.");
            else if (user.Username == username && user.PasswordHash == password)
            {
                string token = CreateToken(user); //CreateToken(account);
                return Ok(token);
            }
            else if (user.PasswordHash != password)
                return NotFound("Wrong password.");
            else
                return NotFound("Unspecified error.");
        }

        // TODO: Implement Put
        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(string id, User user)
        {
            if (id != user.Username)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
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

            return NoContent();
        }

        // TODO: Implement Post
        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(UserDto user)
        {
            if (_context.UserInfo == null)
            {
                return Problem("Entity set 'UserInteraction.UserInfo'  is null.");
            }
            User newUser = new()
            {
                Username = user.Username,
                PasswordHash = user.EncryptionHash
            };
            _context.UserInfo.Add(newUser);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = newUser.Username }, newUser);
        }

        // TODO: Implement Delete
        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            if (_context.UserInfo == null)
            {
                return NotFound();
            }
            var user = await _context.UserInfo.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.UserInfo.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(string id)
        {
            return (_context.UserInfo?.Any(e => e.Username == id)).GetValueOrDefault();
        }

        internal string CreateToken(User user)
        {
            List<Claim> claims = new()
            {
                new Claim(ClaimTypes.Name, user.Username)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }
    }

}
