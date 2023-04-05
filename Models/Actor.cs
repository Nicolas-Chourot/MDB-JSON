using FileKeyReference;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MDB.Models
{
    public class Actor
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Nom")]
        public string Name { get; set; }
        [Display(Name = "Date de naissance"), Required(ErrorMessage = "La date de naissance est requise")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime BirthDate { get; set; }
        [Display(Name = "Nationalité")]
        public string CountryCode { get; set; }
        [Display(Name = "Avatar")]
        public string AvatarImageKey { get; set; } = "";
        #region Avatar management
        [JsonIgnore]
        private static ImageFileKeyReference AvatarsRepository = new ImageFileKeyReference(@"/Images_Data/Actor_Avatars/", @"no_avatar.png");
        [JsonIgnore]
        public string AvatarImageData { get; set; }
        public string GetAvatarURL(bool thumbailFormat = false)
        {
            return AvatarsRepository.GetURL(AvatarImageKey, thumbailFormat);
        }
        public void SaveAvatar()
        {
            AvatarImageKey = AvatarsRepository.Save(AvatarImageData, AvatarImageKey);
        }
        public void RemoveAvatar()
        {
            AvatarsRepository.Remove(AvatarImageKey);
        }
        #endregion

        [JsonIgnore]
        public List<Casting> Castings { get { return DB.Castings.ToList().Where(c => c.ActorId == Id).ToList(); } }
        public void DeleteCastings()
        {
            foreach (var casting in Castings)
                DB.Castings.Delete(casting.Id);
        }
        public bool UpdateCastings(List<int> moviesId)
        {
            DeleteCastings();
            if (moviesId != null)
                foreach (var movieId in moviesId)
                    DB.Castings.Add(new Casting { ActorId = Id, MovieId = movieId });
            return true;
        }
        [JsonIgnore]
        public List<Movie> Movies
        {
            get
            {
                List<Movie> movies = new List<Movie>();
                foreach (var casting in Castings)
                    movies.Add(casting.Movie);
                return movies.OrderBy(c => c.Title).ToList();
            }
        }
    }
}