using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDB.Models
{
    public class Casting
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public int ActorId { get; set; }
        [JsonIgnore]
        public Movie Movie { get { return DB.Movies.Get(MovieId); } }
        [JsonIgnore]
        public Actor Actor { get { return DB.Actors.Get(ActorId); } }
    }
}