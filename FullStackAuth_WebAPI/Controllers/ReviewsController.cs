using FullStackAuth_WebAPI.Data;
using FullStackAuth_WebAPI.DataTransferObjects;
using FullStackAuth_WebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        //[HttpGet("{id}")]
        

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
        [HttpPut("{id}"), Authorize]
        public IActionResult Put(int id, [FromBody] Review data)
        {
            try
            {
                // Find the review to be updated
                Review review = _context.Reviews.Include(r => r.User).FirstOrDefault(r => r.Id == id);

                if (review == null)
                {
                    // Return a 404 Not Found error if the review with the specified ID does not exist
                    return NotFound();
                }

                // Check if the authenticated user is the user of the review
                var userId = User.FindFirstValue("id");
                if (string.IsNullOrEmpty(userId) || review.UserId != userId)
                {
                    // Return a 401 Unauthorized error if the authenticated user is not the user of the review
                    return Unauthorized();
                }

                // Update the review properties
                review.UserId = userId;
                review.User = _context.Users.Find(userId);
                review.Rating = data.Rating;
                review.Text = data.Text;
                review.BookId = data.BookId;
                if (!ModelState.IsValid)
                    if (!ModelState.IsValid)
                {
                    // Return a 400 Bad Request error if the request data is invalid
                    return BadRequest(ModelState);
                }
                _context.SaveChanges();

                // Return a 201 Created status code and the updated car object
                return StatusCode(201, review);
            }
            catch (Exception ex)
            {
                // Return a 500 Internal Server Error with the error message if an exception occurs
                return StatusCode(500, ex.Message);
            }
        }



        // DELETE api/<ReviewsController>/5

        [HttpDelete("{id}"), Authorize]
        public IActionResult DeleteReview(int id)
        {
            try
            {
                // Find the review to be deleted
                Review review = _context.Reviews.FirstOrDefault(r => r.Id == id);
                if (review == null)
                {
                    // Return a 404 Not Found error if the review with the specified ID does not exist
                    return NotFound();
                }

                // Check if the authenticated user is the user of the review
                var userId = User.FindFirstValue("id");
                if (string.IsNullOrEmpty(userId) || review.UserId != userId)
                {
                    // Return a 401 Unauthorized error if the authenticated user is not the user of the review
                    return Unauthorized();
                }

                // Remove the review from the database
                _context.Reviews.Remove(review);
                _context.SaveChanges();

                // Return a 204 No Content status code
                return StatusCode(204);
            }
            catch (Exception ex)
            {
                // Return a 500 Internal Server Error with the error message if an exception occurs
                return StatusCode(500, ex.Message);
            }
        }


    }
}
