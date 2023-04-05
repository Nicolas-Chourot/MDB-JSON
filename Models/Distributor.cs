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
        [Display(Name = "Pays")]
        public string CountryCode { get; set; }
        [Display(Name = "Logo")]
        public string LogoImageKey { get; set; } = "";
        #region Logo management
        [JsonIgnore]
        public string LogoImageData { get; set; }
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
        #endregion
        public List<Distribution> Distributions { get { return DB.Distributions.ToList().Where(c => c.DistributorId == Id).ToList(); } }
        public void DeleteDistributions()
        {
            foreach (var distribution in Distributions)
                DB.Distributions.Delete(distribution.Id);
        }
        public bool UpdateDistributions(List<int> moviesId)
        {
            DeleteDistributions();
            if (moviesId != null)
                foreach (var movieId in moviesId)
                    DB.Distributions.Add(new Distribution { DistributorId = Id, MovieId = movieId });
            return true;
        }
        [JsonIgnore]
        public List<Movie> Movies
        {
            get
            {
                List<Movie> movies = new List<Movie>();
                foreach (var distribution in Distributions)
                    movies.Add(distribution.Movie);
                return movies.OrderBy(c => c.Title).ToList();
            }
        }

    }
}