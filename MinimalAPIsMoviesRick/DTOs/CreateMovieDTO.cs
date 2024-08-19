namespace MinimalAPIsMoviesRick.DTOs
{
    public class CreateMovieDTO
    {
        public string Titile { get; set; } = null!;
        public bool InTheater {  get; set; }
        public DateTime ReleaseDate { get; set; }

        public IFormFile? Poster { get; set; }


    }
}
