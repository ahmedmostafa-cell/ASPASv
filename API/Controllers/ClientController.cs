using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Dtos;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientController : ControllerBase
    {
        private readonly DataContext _dataContext;
        public ClientController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Client>>> GetClients() 
        {
            var clients = await _dataContext.Clients.ToListAsync();
            return clients;
        }

         [HttpGet("{id}")]
        public async Task<ActionResult<Client>> GetClient(int id) 
        {
            return await _dataContext.Clients.Where(a=> a.Id==id).FirstOrDefaultAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Client>> PostClient(ClientDtos client)
        {
            // Check if email already exists
            if (await _dataContext.Clients.AnyAsync(c => c.EmailAddress == client.EmailAddress))
            {
                return Conflict(new { message = "Email already exists." }); // Return 409 Conflict
            }
            Client newClient = new Client();
            newClient.EmailAddress = client.EmailAddress;
            newClient.FirstName = client.FirstName;
            newClient.LastName = client.LastName;
            newClient.PhoneNumber = client.PhoneNumber;
            _dataContext.Clients.Add(newClient);
            await _dataContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetClients), new { id = newClient.Id }, newClient); // Return 201 Created
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Client>> UpdateClient(int id, ClientDtos client)
        {
            // Check if the client exists
            var existingClient = await _dataContext.Clients.FindAsync(id);
            if (existingClient == null)
            {
                return NotFound(new { message = "Client not found." }); // Return 404 Not Found
            }

            // Check if email already exists for another client
            if (await _dataContext.Clients.AnyAsync(c => c.EmailAddress == client.EmailAddress && c.Id != id))
            {
                return Conflict(new { message = "Email already exists." }); // Return 409 Conflict
            }

            // Update the client's properties
            existingClient.FirstName = client.FirstName;
            existingClient.LastName = client.LastName;
            existingClient.EmailAddress = client.EmailAddress;
            existingClient.PhoneNumber = client.PhoneNumber;
            // Save changes to the database
            await _dataContext.SaveChangesAsync();

            return Ok(existingClient); // Return 200 OK with the updated client
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteClient(int id)
        {
            // Check if the client exists
            var existingClient = await _dataContext.Clients.FindAsync(id);
            if (existingClient == null)
            {
                return NotFound(new { message = "Client not found." }); // Return 404 Not Found
            }

            // Remove the client from the database
            _dataContext.Clients.Remove(existingClient);

            // Save changes to the database
            await _dataContext.SaveChangesAsync();

            return NoContent(); // Return 204 No Content to indicate successful deletion
        }


    }
}