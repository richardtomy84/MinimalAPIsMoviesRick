namespace MinimalAPIsMoviesRick.DTOs
{
    public class MovieDTO
    {
        public int Id { get; set; }
        public string Titile { get; set; } = null!;
        public bool InTheater { get; set; }
        public DateTime ReleaseDate { get; set; }

        public string? Poster { get; set; }
    }
}
