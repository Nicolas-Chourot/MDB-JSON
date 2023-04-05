using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FileKeyReference;

namespace MDB.Models
{
    public class ActorsRepository : Repository<Actor>
    {
        public int Add(Actor actor, List<int> moviesId)
        {
            BeginTransaction();
            actor.SaveAvatar(); // must be done before base.Add() to update actor.AvatarImageKey
            base.Add(actor);
            actor.UpdateCastings(moviesId);
            EndTransaction();
            return actor.Id;
        }
      
        public bool Update(Actor actor, List<int> moviesId)
        {
            BeginTransaction(); 
            actor.SaveAvatar(); // must be done before base.Update() to update actor.AvatarImageKey
            base.Update(actor);
            actor.UpdateCastings(moviesId);
            EndTransaction();
            return true;
        }
        public override bool Delete(int Id)
        {
            BeginTransaction();
            Actor actor = Get(Id);
            if (actor != null)
            {
                actor.RemoveAvatar();
                actor.DeleteCastings();
                base.Delete(Id);
            }
            EndTransaction();
            return true;
        }
    }
}