using Mi_punto_de_venta.Data;
using Mi_punto_de_venta.DTOs;
using Mi_punto_de_venta.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;
using System.Security.Claims;

namespace Mi_punto_de_venta.Controllers
{
    [ApiController]
    [Route("api/customers")]
    public class CustomerController : Controller
    {
        private readonly PosDbContext _context;

        public CustomerController(PosDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult Create(Customer customer)
        {
            customer.IsActive = true;
            _context.Customers.Add(customer);
            _context.SaveChanges();

            return Ok(customer);
        }

        [HttpGet]
        public IActionResult GetAll(int id)
        {
            var customers = _context.Customers
            .Where(c => c.IsActive)
            .ToList();
            return Ok(customers);

        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, UpdateCustomerDto updated)
        {
            var customer = _context.Customers.Find(id);

            if (customer == null)
                return NotFound();

            customer.Name = updated.Name;
            customer.Rfc = updated.Rfc;
            customer.FiscalRegime = updated.FiscalRegime;
            customer.CfdiUse = updated.CfdiUse;
            customer.PostalCode = updated.PostalCode;
            customer.Email = updated.Email;

            _context.SaveChanges();


            return Ok(customer);
        }

        [HttpDelete("{id}")]

        public IActionResult Delete(int id)
        {
            var customer = _context.Customers.Find(id);

            if (customer == null)
                return NotFound();

            customer.IsActive = false;
            _context.SaveChanges();
            return Ok();
        }


        [Authorize]
        [HttpGet("mine")]
        public IActionResult GetMyFiscalData()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return Unauthorized("Token inválido");

            var userId = int.Parse(userIdClaim.Value);

            var customer = _context.Customers
                .FirstOrDefault(c => c.UserId == userId && c.IsActive);

            if (customer != null)
                return Ok(customer);

            // 👇 fallback: traer datos del usuario
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);

            if (user == null)
                return NotFound();

            return Ok(new
            {
                id = 0,
                name = user.FullName, // 👈 aquí llenas el nombre automáticamente
                rfc = "",
                fiscalRegime = "",
                cfdiUse = "",
                postalCode = "",
                email = user.Email
            });
        }
    }
}

