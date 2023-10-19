using FullStackAuth_WebAPI.Data;
using FullStackAuth_WebAPI.DataTransferObjects;
using FullStackAuth_WebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FullStackAuth_WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FavoritesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FavoritesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/favorites
        [HttpGet, Authorize]
        public IActionResult GetAllFavorites()
        {
            try
            {
                var favorites = _context.Favorites.ToList();

                return StatusCode(200, favorites);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // GET api/<FavoritesController>/5
        //[HttpGet("{id}")]


        // POST api/favorte
        [HttpPost, Authorize]
        public IActionResult PostFavorites([FromBody] Favorite data)
        {
            try
            {

                string userId = User.FindFirstValue("id");

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                data.UserId = userId;

                _context.Favorites.Add(data);
                if (!ModelState.IsValid)
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

        // PUT api/<FavoritesController>/5



        // DELETE api/<FavoritesController>/5
        [HttpDelete("{id}"), Authorize]
        public IActionResult DeleteFavorites(int id)
        {
            try
            {
                // Find the favorite to be deleted
                Favorite favorite = _context.Favorites.FirstOrDefault(f => f.Id == id);
                if (favorite == null)
                {
                    // Return a 404 Not Found error if the favorite with the specified ID does not exist
                    return NotFound();
                }

                // Check if the authenticated user is the user of the favorite
                var userId = User.FindFirstValue("id");
                if (string.IsNullOrEmpty(userId) || favorite.UserId != userId)
                {
                    // Return a 401 Unauthorized error if the authenticated user is not the user of the favorite
                    return Unauthorized();
                }

                // Remove the favorite from the database
                _context.Favorites.Remove(favorite);
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

