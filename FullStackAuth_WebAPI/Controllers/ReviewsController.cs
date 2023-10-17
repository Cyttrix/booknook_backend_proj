using FullStackAuth_WebAPI.Data;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FullStackAuth_WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        
        public ReviewsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/reviews
        [HttpGet]
        public IActionResult GetAllReviews()
        {
            try
            {
                var reviews = _context.Reviews.Select(r => new ReviewWit)
            }
        }

        // GET api/<ReviewsController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<ReviewsController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        
        }
    }
}
