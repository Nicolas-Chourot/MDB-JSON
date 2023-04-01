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
        public string AvatarImageKey { get; set; } = "";
        [Display(Name = "Date de naissance"), Required(ErrorMessage = "La date de naissance est requise")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime BirthDate { get; set; }
        [Display(Name = "Nationalité")]
        public string CountryCode { get; set; }

        [JsonIgnore]
        [Display(Name = "Avatar")]
        public string AvatarImageData { get; set; }

        [JsonIgnore]
        public List<Movie> Movies
        {
            get
            {
                List<Casting> castings = DB.Castings.ToList().Where(c => c.ActorId == Id).ToList();
                List<Movie> movies = new List<Movie>();
                foreach (var casting in castings)
                {
                    movies.Add(DB.Movies.Get(casting.MovieId));
                }
                return movies.OrderBy(c => c.Title).ToList();
            }
        }
        [JsonIgnore]
        private static ImageFileKeyReference AvatarsRepository = new ImageFileKeyReference(@"/Images_Data/Actor_Avatars/", @"no_avatar.png");

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
    }
}