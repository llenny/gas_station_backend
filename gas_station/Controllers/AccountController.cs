using gas_station.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace gas_station.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly DBContext _context;
        // менеджер для аутентификации
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private RoleManager<IdentityRole<int>> _roleManager;
        public AccountController(DBContext context, UserManager<User> userManager, RoleManager<IdentityRole<int>> roleManager, SignInManager<User> signInManager)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;

        }
        [HttpPost("/registration")]
        public async Task<IActionResult> Register(UserInput model)
        {

            User user = new User() { Email = model.Email, UserName = model.UserName, Password = model.Password, IdentityRoleId = model.RoleId };
            // добавляем пользователя
 
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                var newUser = await _userManager.FindByNameAsync(model.UserName);
                // если добавляем сотрудника или администратора, привязываем сотрудника к пользователю
                if (model.RoleId == 1 || model.RoleId == 2)
                {
                    var employee = await _context.Employees.FindAsync(model.EmployeeId);
                    using (_context)
                    {
                        // прописываем соединение с базой
                        NpgsqlConnection conn = new("Server=127.0.0.1;Port=5432;Database=gas_station;User Id=postgres;Password=123;");
                        conn.Open();
                        //создаем новую команду для получения данных из базы (прописываем параметры и синтаксис)
                        NpgsqlCommand command = new NpgsqlCommand("call public.add_user_to_employee(@user_id, @employee_id)", conn);
                        command.CommandType = CommandType.Text;
                        // создаем именованные параметры (названия как в базе), прописываем их типы
                        command.Parameters.Add(new Npgsql.NpgsqlParameter("user_id", NpgsqlTypes.NpgsqlDbType.Integer) { Value = newUser.Id });
                        command.Parameters.Add(new Npgsql.NpgsqlParameter("employee_id", NpgsqlTypes.NpgsqlDbType.Integer) { Value = employee.Id });
                        // выполняем созданную команду и получаем курсор с данными
                        command.ExecuteNonQuery();

                        conn.Close();
                    }
                };
                return new JsonResult(result);
            }
            // если не проходит валидация пользователя, показываем ошибки
            else
            {
                return new JsonResult(result.Errors);
            }            
        }

        [HttpPost("/token")]
        public async Task<IActionResult> Token(UserInput user)
        {
            var identity = await GetIdentity(user.UserName, user.Password);
            if (identity == null)
            {
                return BadRequest(new { errorText = "Invalid username or password." });
            }
            var now = DateTime.UtcNow;
            // создаем JWT-токен
            var jwt = new JwtSecurityToken(
            issuer: AuthOptions.ISSUER,
            audience: AuthOptions.AUDIENCE,
            notBefore: now,
            claims: identity.Claims,
            expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
            signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            var userRole = await _roleManager.FindByIdAsync(identity.Claims.ToArray()[1].Value);

            var response = new
            {
                access_token = encodedJwt,
                username = identity.Name,
                role = userRole.Name
            };
            return new JsonResult(response);
        }

        [HttpPost]
        // идентификация пользователя
        private async Task<ClaimsIdentity> GetIdentity(string username, string password)
        {
            User user = _context.Users.FirstOrDefault(i => i.UserName == username && i.Password == password);

            var result = await _userManager.CheckPasswordAsync(user, password);
            if (user != null && result)
            {
                var claims = new List<Claim>
                {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.UserName),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.IdentityRoleId.ToString())
                };
                ClaimsIdentity claimsIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);
                return claimsIdentity;
            }
            // если пользователя не найдено
            return null;
        }

        [Authorize]
        [Route("getlogin")]
        public IActionResult GetLogin()
        {
            return Ok($"Ваш логин: {User.Identity.Name}");
        }

        [Authorize(Roles = "admin")]
        [Route("getrole")]
        public IActionResult GetRole()
        {
            return Ok("Ваша роль: администратор");
        }
    }
}
