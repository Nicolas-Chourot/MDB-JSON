using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDB.Models
{
    public class DistributorsRepository : Repository<Distributor>
    {
        public int Add(Distributor distributor, List<int> moviesId)
        {
            BeginTransaction();
            distributor.SaveLogo(); // must be done before base.Add() to update actor.AvatarImageKey
            int distributorId = base.Add(distributor);
            UpdateDistribution(Get(distributorId), moviesId);
            EndTransaction();
            return distributorId;
        }
        private void UpdateDistribution(Distributor distributor, List<int> moviesId)
        {
            DeleteDistributions(distributor);
            if (moviesId != null && moviesId.Count > 0)
            {
                foreach (var movieId in moviesId)
                {
                    DB.Distributions.Add(movieId, distributor.Id);
                }
            }
        }
        private void DeleteDistributions(Distributor distributor)
        {
            foreach (var movie in distributor.Movies)
            {
                DB.Distributions.Delete(movie.Id, distributor.Id);
            }
        }
        public bool Update(Distributor distributor, List<int> moviesId)
        {
            BeginTransaction();
            distributor.SaveLogo(); // must be done before base.Update() to update actor.AvatarImageKey
            base.Update(distributor);
            UpdateDistribution(distributor, moviesId);
            EndTransaction();
            return true;
        }
        public override bool Delete(int Id)
        {
            BeginTransaction();
            Distributor distributor = Get(Id);
            if (distributor != null)
            {
                distributor.RemoveLogo();
                DeleteDistributions(distributor);
                base.Delete(Id);
            }
            EndTransaction();
            return true;
        }
    }
}