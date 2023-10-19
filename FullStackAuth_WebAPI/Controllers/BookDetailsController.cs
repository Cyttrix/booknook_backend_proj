using FullStackAuth_WebAPI.Data;
using FullStackAuth_WebAPI.DataTransferObjects;
using FullStackAuth_WebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Bcpg;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FullStackAuth_WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookDetailsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BookDetailsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/<BookDetailsController>


        // GET api/<BookDetailsController>/5
        [HttpGet("{bookId}")]
        public IActionResult GetBookInformation(string bookId)
        {
            try
            {
                string userId = User.FindFirstValue("id");

                var details = _context.Reviews.Where(d => d.BookId.Equals(bookId));

                double avgRating = details.Select(d => d.Rating).Average();

                var favoritedBooks = _context.Favorites.Where(f => f.BookId.Equals(bookId) & f.UserId.Equals(userId));

                bool booksfavorited = false;

                if (favoritedBooks.Any())
                {
                    booksfavorited = true;

                }

                var information = details.Include(i => i.User).Select(i => new ReviewWithUserDto
                {
                    Id = i.Id,
                    BookId = bookId,
                    Text = i.Text,
                    Rating = i.Rating,
                    User = new UserForDisplayDto
                    {
                        Id = i.User.Id,
                        FirstName = i.User.FirstName,
                        LastName = i.User.LastName,
                        UserName = i.User.UserName,
                    }


                }).ToList();

                var finalResponse = new BookDetailsDto
                {
                    BookId = bookId,
                    BookReviews = information,
                    AverageRating = avgRating,
                    Favorited = booksfavorited,
                };

                return StatusCode(200, finalResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // POST api/<BookDetailsController>
       

        // PUT api/<BookDetailsController>/5
        

        // DELETE api/<BookDetailsController>/5
        
    }
}
