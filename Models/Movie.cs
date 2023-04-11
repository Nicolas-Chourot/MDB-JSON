using FileKeyReference;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MDB.Models
{
    public class Movie
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Le titre est requis")]
        [Display(Name = "Titre")]
        public string Title { get; set; }

        [Required(ErrorMessage = "L'année de sortie est requise")]
        [Display(Name = "Année de sortie")]
        [Range(1930, 2099, ErrorMessage = "Valeur pour {0} doit être entre {1} et {2}.")]
        public int ReleaseYear { get; set; }

        [Display(Name = "Pays")]
        public string CountryCode { get; set; }

        [Required(ErrorMessage = "Le synopsis est requis")]
        [Display(Name = "Synopsis")]
        [DataType(DataType.MultilineText)]
        public string Synopsis { get; set; }

        [Display(Name = "Affiche")]
        public string PosterImageKey { get; set; } = "";
        #region Poster management
        [JsonIgnore]
        public string PosterImageData { get; set; }
        [JsonIgnore]
        private static ImageFileKeyReference PostersRepository = new ImageFileKeyReference(@"/Images_Data/Movie_Posters/", @"no_poster.png");
        public string GetPosterURL(bool thumbailFormat = false)
        {
            return PostersRepository.GetURL(PosterImageKey, thumbailFormat);
        }
        public void SavePoster()
        {
            PosterImageKey = PostersRepository.Save(PosterImageData, PosterImageKey);
        }
        public void RemovePoster()
        {
            PostersRepository.Remove(PosterImageKey);
        }
        #endregion

        [JsonIgnore]
        public List<Casting> Castings { get { return DB.Castings.ToList().Where(c => c.MovieId == Id).ToList(); } }
        [JsonIgnore]
        public List<Distributor> Distributors
        {
            get
            {
                List<Distributor> distributors = new List<Distributor>();
                foreach (var distribution in Distributions)
                    distributors.Add(distribution.Distributor);
                return distributors.OrderBy(c => c.Name).ToList();
            }
        }
        [JsonIgnore]
        public List<Actor> Actors
        {
            get
            {
                List<Actor> actors = new List<Actor>();
                foreach (var casting in Castings)
                    actors.Add(casting.Actor);
                return actors.OrderBy(c => c.Name).ToList();
            }
        }
        [JsonIgnore]
        public List<Distribution> Distributions { get { return DB.Distributions.ToList().Where(c => c.MovieId == Id).ToList(); } }
        public void DeleteDistributions()
        {
            foreach (var distribution in Distributions)
                DB.Distributions.Delete(distribution.Id);
        }
        public bool UpdateDistributions(List<int> distributorsId)
        {
            DeleteDistributions();
            if (distributorsId != null)
                foreach (var distributorId in distributorsId)
                    DB.Distributions.Add(new Distribution { DistributorId = distributorId, MovieId = Id });
            return true;
        }
        public void DeleteCastings()
        {
            foreach (var casting in Castings)
                DB.Castings.Delete(casting.Id);
        }
        public bool UpdateCastings(List<int> actorsId)
        {
            DeleteCastings();
            if (actorsId != null)
                foreach (var actorId in actorsId)
                    DB.Castings.Add(new Casting { ActorId = actorId, MovieId = Id });
            return true;
        }
     }
}