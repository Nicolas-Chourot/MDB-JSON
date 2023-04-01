using MDB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MDB.Controllers
{
    public class MoviesController : Controller
    {
        [OnlineUsers.UserAccess]
        public ActionResult Index()
        {
            return View();
        }
        [OnlineUsers.UserAccess(false/* do not reset timeout*/)]
        public ActionResult Movies(bool forceRefresh = false)
        {
            if (forceRefresh || DB.Movies.HasChanged)
            {
                return PartialView(DB.Movies.ToList().OrderBy(c => c.Title));
            }
            return null;
        }
        [OnlineUsers.PowerUserAccess]
        public ActionResult Create()
        {
            ViewBag.Castings = null;
            ViewBag.Actors = SelectListUtilities<Actor>.Convert(DB.Actors.ToList());
            ViewBag.Distributions = null;
            ViewBag.Distributors = SelectListUtilities<Distributor>.Convert(DB.Distributors.ToList());
            return View(new Movie());
        }
        [OnlineUsers.PowerUserAccess]
        [HttpPost]
        public ActionResult Create(Movie movie, List<int> SelectedActorsId, List<int> SelectedDistributorsId)
        {
            if (ModelState.IsValid)
            {
                DB.Movies.Add(movie, SelectedActorsId, SelectedDistributorsId);
                return RedirectToAction("Index");
            }
            ViewBag.Castings = null;
            ViewBag.Actors = SelectListUtilities<Actor>.Convert(DB.Actors.ToList());
            ViewBag.Distributions = null;
            ViewBag.Distributors = SelectListUtilities<Distributor>.Convert(DB.Distributors.ToList());
            return View();
        }
        [OnlineUsers.UserAccess]
        public ActionResult Details(int id)
        {
            Movie movie = DB.Movies.Get(id);
            if (movie != null)
            {
                return View(movie);
            }
            return RedirectToAction("Index");
        }
        [OnlineUsers.PowerUserAccess]
        public ActionResult Edit(int id)
        {
            Movie movie = DB.Movies.Get(id);
            if (movie != null)
            {
                ViewBag.Castings = SelectListUtilities<Actor>.Convert(movie.Actors);
                ViewBag.Actors = SelectListUtilities<Actor>.Convert(DB.Actors.ToList());
                ViewBag.Distributions = SelectListUtilities<Distributor>.Convert(movie.Distributors);
                ViewBag.Distributors = SelectListUtilities<Distributor>.Convert(DB.Distributors.ToList());
                return View(movie);
            }
            return RedirectToAction("Index");
        }
        [OnlineUsers.PowerUserAccess]
        [HttpPost]
        public ActionResult Edit(Movie movie, List<int> SelectedActorsId, List<int> SelectedDistributorsId)
        {
            if (ModelState.IsValid)
            {
                DB.Movies.Update(movie, SelectedActorsId, SelectedDistributorsId);
                return RedirectToAction("Details", new { id = movie.Id });
            }
            ViewBag.Castings = SelectListUtilities<Actor>.Convert(movie.Actors);
            ViewBag.Actors = SelectListUtilities<Actor>.Convert(DB.Actors.ToList());
            ViewBag.Distributions = SelectListUtilities<Distributor>.Convert(movie.Distributors);
            ViewBag.Distributors = SelectListUtilities<Distributor>.Convert(DB.Distributors.ToList());
            return View();
        }
        [OnlineUsers.PowerUserAccess]
        public ActionResult Delete(int id)
        {
            DB.Movies.Delete(id);
            return RedirectToAction("Index");
        }
    }
}