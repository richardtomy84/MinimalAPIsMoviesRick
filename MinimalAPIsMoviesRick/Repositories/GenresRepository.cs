using Dapper;
using Microsoft.Data.SqlClient;
using MinimalAPIsMoviesRick.Entities;
using System.Data;

namespace MinimalAPIsMoviesRick.Repositories
{
    public class GenresRepository : IGenresRepository
    {
        private readonly string connectionString;

        public GenresRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection")!;
        
        }

        public async Task<int> Create(Genre genre)
        {
            using (var connection = new SqlConnection(connectionString))
            {

                //  var query = connection.Query("select 1").FirstOrDefault(); 
                var query = @"insert into Geners (Name) Values (@Name);
                            select SCOPE_IDENTITY();";

                var id = await connection.QuerySingleAsync<int>(query,genre);
                genre.Id = id;
                return id;
            }
           // return Task.FromResult(0);
        }

        public async Task Delete(int id)
        {
            using(var connection = new SqlConnection(connectionString))
            {
                var query = @"delete from Geners where Id=@id ";
                     await connection.ExecuteAsync(query,new { id });
            }
        }

        public async Task<bool> Exists(int id)
        {
            using(var connection = new SqlConnection(connectionString))
            {
                var exists = await connection.QuerySingleAsync<bool>(@"if exists(select 1 from Geners where Id=1) 
                                                                        select 1 ;	
                                                                      else 
                                                                        select 0; ", new {id});
                return exists;
            }
        }

        public async Task<List<Genre>> GetAll()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var genres = await connection.QueryAsync<Genre>(@"Genres_GetAll",commandType:CommandType.StoredProcedure);
                return genres.ToList();
            }

        }
         
        public async Task<Genre?> GetById(int id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var genre = await connection.QueryFirstOrDefaultAsync<Genre>(@"SELECT Id,Name 
                                             from Geners where 
                                             id = @Id",  new {id});
                return genre;
            }

        }

        public async Task Update(Genre genre)
        {
            using( var connection = new SqlConnection(connectionString))
            {
                await connection.ExecuteAsync(@"update Geners set Name=@Name where id=@Id",genre);
            }

        }
    }
}
