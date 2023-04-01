using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Web.Hosting;
using System.Web.UI.WebControls;

namespace MDB.Models
{
    public class DB
    {
        private static readonly DB instance = new DB();
        public static Repository<Gender> Genders { get; set; }
        public static Repository<UserType> UserTypes { get; set; }
        public static Repository<UnverifiedEmail> UnverifiedEmails { get; set; }
        public static Repository<ResetPasswordCommand> ResetPasswordCommands { get; set; }  
        public static Repository<Login> Logins { get; set; }
        public static UsersRepository Users { get; set; }
        public static MoviesRepository Movies { get; set; }
        public static ActorsRepository Actors { get; set; }
        public static DistributorsRepository Distributors { get; set; }
        public static CastingsRepository Castings { get; set; }
        public static DistributionsRepository Distributions { get; set; }
        public DB()
        {
            Genders = new Repository<Gender>();
            UserTypes = new Repository<UserType>();
            UnverifiedEmails = new Repository<UnverifiedEmail>();
            ResetPasswordCommands = new Repository<ResetPasswordCommand>(); 
            Logins = new Repository<Login>();
            Users = new UsersRepository();
            Movies = new MoviesRepository();
            Actors = new ActorsRepository();
            Distributors = new DistributorsRepository();
            Castings = new CastingsRepository();
            Distributions = new DistributionsRepository();
            InitRepositories(this);
        }
        public static DB Instance
        {
            get { return instance; }
        }
        public static void InitRepositories(DB db)
        {
            string serverPath = HostingEnvironment.MapPath(@"~/App_Data/");
            PropertyInfo[] myPropertyInfo = db.GetType().GetProperties();
            foreach(PropertyInfo propertyInfo in myPropertyInfo)
            {
                if (propertyInfo.Name != "Instance")
                {
                    MethodInfo method = propertyInfo.PropertyType.GetMethod("Init");
                    method.Invoke(propertyInfo.GetValue(db), new object[] { serverPath + propertyInfo.Name + ".json" });
                }
            }
        }
    }
}