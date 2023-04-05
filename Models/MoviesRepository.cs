using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDB.Models
{
    public class MoviesRepository : Repository<Movie>
    {
        public int Add(Movie movie, List<int> actorsId, List<int> distributorsId)
        {
            BeginTransaction(); 
            movie.SavePoster();
            base.Add(movie);
            movie.UpdateCastings(actorsId);
            movie.UpdateDistributions(distributorsId);
            EndTransaction();
            return movie.Id;
        }
       
        public bool Update(Movie movie, List<int> actorsId, List<int> distributorsId)
        {
            BeginTransaction();
            movie.SavePoster();
            base.Update(movie);
            movie.UpdateCastings(actorsId);
            movie.UpdateDistributions(distributorsId);
            EndTransaction();
            return true;
        }
        public override bool Delete(int Id)
        {
            BeginTransaction();
            Movie movie = Get(Id);
            if (movie != null)
            {
                movie.RemovePoster();
                movie.DeleteCastings();
                movie.DeleteDistributions();
                base.Delete(Id);
            }
            EndTransaction();
            return true;
        }
    }
}