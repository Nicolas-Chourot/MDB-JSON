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
            base.Add(distributor);
            distributor.UpdateDistributions(moviesId);
            EndTransaction();
            return distributor.Id;
        }
        public bool Update(Distributor distributor, List<int> moviesId)
        {
            BeginTransaction();
            distributor.SaveLogo(); // must be done before base.Update() to update actor.AvatarImageKey
            base.Update(distributor);
            distributor.UpdateDistributions(moviesId);
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
                distributor.DeleteDistributions();
                base.Delete(Id);
            }
            EndTransaction();
            return true;
        }
    }
}