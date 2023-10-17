using FullStackAuth_WebAPI.Data;
using FullStackAuth_WebAPI.DataTransferObjects;
using FullStackAuth_WebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
                var reviews = _context.Reviews.Select(r => new ReviewWithUserDto
                {
                    Id = r.Id,
                    BookId = r.BookId,
                    Text = r.Text,
                    Rating = r.Rating,
                    User = new UserForDisplayDto
                    {
                        Id = r.User.Id,
                        FirstName = r.User.FirstName,
                        LastName = r.User.LastName,
                        UserName = r.User.UserName,
                    }
                }).ToList();

                return StatusCode(200, reviews);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // GET api/<ReviewsController>/5
        [HttpGet("{id}")]
        

        // POST api/reviews
        [HttpPost, Authorize]
        public IActionResult Post([FromBody] Review data)
        {
            try
            {
                string userId = User.FindFirstValue("id");

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                data.UserId = userId;

                _context.Reviews.Add(data);
                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                _context.SaveChanges();

                return StatusCode(201, data);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // PUT api/<ReviewsController>/5
        

        // DELETE api/<ReviewsController>/5
       
        
    }
}
