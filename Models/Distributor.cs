using FileKeyReference;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace MDB.Models
{
    public class Distributor
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Distributeur")]
        public string Name { get; set; }
        public string LogoImageKey { get; set; } = "";
        [Display(Name = "Pays")]
        public string CountryCode { get; set; }

        [JsonIgnore]
        [Display(Name = "Logo")]
        public string LogoImageData { get; set; }

        [JsonIgnore]
        public List<Movie> Movies
        {
            get
            {
                List<Distribution> distributions = DB.Distributions.ToList().Where(c => c.DistributorId == Id).ToList();
                List<Movie> movies = new List<Movie>();
                foreach (var distribution in distributions)
                {
                    movies.Add(DB.Movies.Get(distribution.MovieId));
                }
                return movies.OrderBy(c => c.Title).ToList();
            }
        }
        [JsonIgnore]
        private static ImageFileKeyReference AvatarsRepository = new ImageFileKeyReference(@"/Images_Data/Distributor_Logos/", @"No_Logo.png");

        public string GetLogoURL(bool thumbailFormat = false)
        {
            return AvatarsRepository.GetURL(LogoImageKey, thumbailFormat);
        }
        public void SaveLogo()
        {
            LogoImageKey = AvatarsRepository.Save(LogoImageData, LogoImageKey);
        }
        public void RemoveLogo()
        {
            AvatarsRepository.Remove(LogoImageKey);
        }
    }
}