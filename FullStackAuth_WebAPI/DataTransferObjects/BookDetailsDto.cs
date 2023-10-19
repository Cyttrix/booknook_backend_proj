namespace FullStackAuth_WebAPI.DataTransferObjects
{
    public class BookDetailsDto
    {
        public string BookId { get; set; }
        public List<ReviewWithUserDto> BookReviews { get; set; }
        public double AverageRating {  get; set; }
        public bool Favorited {  get; set; }

    }
}
