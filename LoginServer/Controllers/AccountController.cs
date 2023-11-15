using LoginManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace LoginManager.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        static public Dictionary<string, UserDto> AccountList = new Dictionary<string, UserDto>();
        private readonly IConfiguration _configuration;
        public AccountController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // GET: api/<AccountController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<AccountController>/5
        [HttpGet("GetAccount/{input}")]
        public ActionResult<User> Get(string input)
        {
            string[] inputCheck = input.Split(" ", 2, StringSplitOptions.None);
            string username = inputCheck[0].ToLower();
            string password = inputCheck[1];
            if (AccountList.ContainsKey(username) && AccountList[username].PasswordHash == password)
            {
                User account = new User()
                {
                    Username = AccountList[username].Username,
                    PasswordHash = AccountList[username].PasswordHash
                };
                string token = CreateToken(account);
                return Ok(token);
            }
            else if (AccountList.ContainsKey(username) && AccountList[username].PasswordHash != password)
                return NotFound("Wrong password.");
            else
                return NotFound("User not found.");
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
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


        // POST api/<AccountController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
            string[] inputCheck = value.Split(" ", 2, StringSplitOptions.None);
            string username = inputCheck[0].ToLower();
            string password = inputCheck[1];
            UserDto LatestAccount = new()
            {
                Username = username, //inputCheck(0),
                PasswordHash = password //inputCheck(1)
            };
            AccountList.Add(username, LatestAccount);
        }

        // PUT api/<AccountController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<AccountController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
