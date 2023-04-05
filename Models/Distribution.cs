using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDB.Models
{
    public class Distribution
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public int DistributorId { get; set; }
        [JsonIgnore]
        public Distributor Distributor { get { return DB.Distributors.Get(DistributorId); } }
        [JsonIgnore]
        public Movie Movie { get { return DB.Movies.Get(MovieId); } }
    }
}