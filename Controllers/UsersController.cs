using BCrypt.Net;
using Mi_punto_de_venta.Data;
using Mi_punto_de_venta.DTOs;
using Mi_punto_de_venta.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace Mi_punto_de_venta.Controllers
{
   
    [ApiController]
    [Route("api/users")]


    public class UsersController : ControllerBase
    {

        private readonly PosDbContext _context;
        private readonly IConfiguration _configuration;

        public UsersController(PosDbContext context,IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult GetUsers()
        {
            var users = _context.Users.ToList();
            return Ok(users);
        }




        [HttpPost]
        public IActionResult CreateUser(CreateUserDto dto)
        {
            if (_context.Users.Any(u => u.Email == dto.Email))
                return BadRequest("El correo ya existe");

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                PasswordHash = passwordHash,
                Role = "customer",
                IsActive = true,
                CreatedAt = DateTime.Now,
            };

            _context.Users.Add(user);
            _context.SaveChanges();


            var customer = new Customer
            {
                UserId = user.Id,
                Name = dto.FullName,
                Email = dto.Email,
                IsActive = true
            };

            customer.UserId = user.Id;
            _context.Customers.Add(customer);
            _context.SaveChanges();

            // 🔥 Generar token automático
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings["Key"])
            );

            var creds = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256
            );

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("role", user.Role)
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new { token = tokenString });
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login(LoginDto dto)
        {
            var user =_context.Users.FirstOrDefault(u => u.Email == dto.Email);

            if (user == null)
                return Unauthorized("Usuario no encontrado");

            bool validPassword= BCrypt.Net.BCrypt.Verify(dto.Password,user.PasswordHash);
            if (!validPassword)
                return Unauthorized("Contraseña incorrecta ");

            var jwtSettings = _configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings["Key"])
            );

            var creds = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256
            );

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("role", user.Role)
               };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new { token = tokenString });
        }



    }

}
