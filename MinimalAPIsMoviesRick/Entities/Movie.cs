﻿namespace MinimalAPIsMoviesRick.Entities
{
    public class Movie
    { 
        public int Id { get; set; }
        public string Titile { get; set; } = null;
        public bool InTheater {  get; set; }
        public DateTime ReleaseDate {  get; set; }

        public string? Poster {  get; set; }
        public List<Comment> Comments  { get; set;} = new List<Comment>();

        public List<GenreMovie> GenresMovies { get; set; }= new List<GenreMovie>();

        public List<ActorMovie> ActorMoviess { get; set; }= new List<ActorMovie>();
    }
}
