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

        public string PosterImageKey { get; set; } = "";

        [JsonIgnore]
        [Display(Name = "Affiche")]
        public string PosterImageData { get; set; }

        [JsonIgnore]
        public List<Actor> Actors
        {
            get
            {
                List<Casting> castings = DB.Castings.ToList().Where(c => c.MovieId == Id).ToList();
                List<Actor> actors = new List<Actor>();
                foreach (var casting in castings)
                {
                    actors.Add(DB.Actors.Get(casting.ActorId));
                }
                return actors.OrderBy(c => c.Name).ToList();
            }
        }
        [JsonIgnore]
        public List<Distributor> Distributors
        {
            get
            {
                List<Distribution> distributions = DB.Distributions.ToList().Where(c => c.MovieId == Id).ToList();
                List<Distributor> distributors = new List<Distributor>();
                foreach (var casting in distributions)
                {
                    distributors.Add(DB.Distributors.Get(casting.DistributorId));
                }
                return distributors.OrderBy(c => c.Name).ToList();
            }
        }

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
    }
}