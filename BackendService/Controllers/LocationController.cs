using BackendService.Data;
using BackendService.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackendService.Controllers
{
    public class LocationController : IdentityController
    {
        private readonly ShoppingDbContext _context;
        private readonly ILogger<LocationController> _logger;
        public LocationController(ILogger<LocationController> logger, ShoppingDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult MigrateDatabase()
        {
            _context.Database.Migrate();
            return Content("Migration complete!");
        }

    }
}
