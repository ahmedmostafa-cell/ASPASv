using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
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
            return await _dataContext.Clients.ToListAsync();
        }

         [HttpGet("{id}")]
        public async Task<ActionResult<Client>> GetClient(int id) 
        {
            return await _dataContext.Clients.Where(a=> a.Id==id).FirstOrDefaultAsync();
        }
    }
}