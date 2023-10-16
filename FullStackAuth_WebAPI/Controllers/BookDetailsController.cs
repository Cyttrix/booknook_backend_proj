using FullStackAuth_WebAPI.Data;
using FullStackAuth_WebAPI.DataTransferObjects;
using FullStackAuth_WebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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

        // GET: api/cars
        [HttpGet]
        public IActionResult GetAllReviews()
        {
            try
            {
                //Includes entire Owner object--insecure!
                //var cars = _context.Cars.Include(c => c.Owner).ToList();

                //Retrieve all cars from the database, using Dtos
                var reviews = _context.Reviews.Select(c => new ReviewWithUserDto
                {
                    Id = c.Id,
                    BookId = c.BookId,
                    Text = c.Text,
                    Rating = c.Rating,
                    User = new UserForDisplayDto
                    {
                        Id = c.User.Id,
                        FirstName = c.User.FirstName,
                        LastName = c.User.LastName,
                        UserName = c.User.UserName,
                    }
                }).ToList();

                // Return the list of cars as a 200 OK response
                return StatusCode(200, reviews);
            }
            catch (Exception ex)
            {
                // If an error occurs, return a 500 internal server error with the error message
                return StatusCode(500, ex.Message);
            }
        }

        // GET: api/cars/myCars
        [HttpGet("myCars"), Authorize]
        public IActionResult GetUsersCars()
        {
            try
            {
                // Retrieve the authenticated user's ID from the JWT token
                string userId = User.FindFirstValue("id");

                // Retrieve all cars that belong to the authenticated user, including the owner object
                var reviews = _context.Reviews.Where(c => c.UserId.Equals(userId));

                // Return the list of cars as a 200 OK response
                return StatusCode(200, reviews);
            }
            catch (Exception ex)
            {
                // If an error occurs, return a 500 internal server error with the error message
                return StatusCode(500, ex.Message);
            }
        }

        // GET api/cars/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                // Retrieve the car with the specified ID, including the owner object
                var review = _context.Reviews.Include(c => c.User).FirstOrDefault(c => c.Id == id);

                // If the car does not exist, return a 404 not found response
                if (review == null)
                {
                    return NotFound();
                }

                // Return the car as a 200 OK response
                return StatusCode(200, review);
            }
            catch (Exception ex)
            {
                // If an error occurs, return a 500 internal server error with the error message
                return StatusCode(500, ex.Message);
            }
        }

        // POST api/cars
        [HttpPost, Authorize]
        public IActionResult Post([FromBody] Review data)
        {
            try
            {
                // Retrieve the authenticated user's ID from the JWT token
                string userId = User.FindFirstValue("id");

                // If the user ID is null or empty, the user is not authenticated, so return a 401 unauthorized response
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                // Set the car's owner ID  the authenticated user's ID we found earlier
                data.UserId = userId;

                // Add the car to the database and save changes
                _context.Reviews.Add(data);
                if (!ModelState.IsValid)
                {
                    // If the car model state is invalid, return a 400 bad request response with the model state errors
                    return BadRequest(ModelState);
                }
                _context.SaveChanges();

                // Return the newly created car as a 201 created response
                return StatusCode(201, data);
            }
            catch (Exception ex)
            {
                // If an error occurs, return a 500 internal server error with the error message
                return StatusCode(500, ex.Message);
            }
        }

        // PUT api/cars/5
        [HttpPut("{id}"), Authorize]
        public IActionResult Put(int id, [FromBody] Review data)
        {
            try
            {
                // Find the car to be updated
                Review review = _context.Reviews.Include(c => c.User).FirstOrDefault(c => c.Id == id);

                if (review == null)
                {
                    // Return a 404 Not Found error if the car with the specified ID does not exist
                    return NotFound();
                }

                // Check if the authenticated user is the owner of the car
                var userId = User.FindFirstValue("id");
                if (string.IsNullOrEmpty(userId) || review.UserId != userId)
                {
                    // Return a 401 Unauthorized error if the authenticated user is not the owner of the car
                    return Unauthorized();
                }

                // Update the review properties
                review.UserId = userId;
                review.User = _context.Users.Find(userId);
                review.Text = data.Text;
                review.Rating = data.Rating;
                review.BookId = data.BookId;
                review.Id = data.Id;
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

        // DELETE api/cars/5
        [HttpDelete("{id}"), Authorize]
        public IActionResult Delete(int id)
        {
            try
            {
                // Find the car to be deleted
                Review review = _context.Reviews.FirstOrDefault(c => c.Id == id);
                if (review == null)
                {
                    // Return a 404 Not Found error if the review with the specified ID does not exist
                    return NotFound();
                }

                // Check if the authenticated user is the owner of the review
                var userId = User.FindFirstValue("id");
                if (string.IsNullOrEmpty(userId) || review.UserId != userId)
                {
                    // Return a 401 Unauthorized error if the authenticated user is not the owner of the review
                    return Unauthorized();
                }

                // Remove the car from the database
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
