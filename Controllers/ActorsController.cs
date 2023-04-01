using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MDB.Models;

namespace MDB.Controllers
{
    public class ActorsController : Controller
    {
        [OnlineUsers.UserAccess]
        public ActionResult Index()
        {
            return View();
        }
        [OnlineUsers.UserAccess(false/* do not reset timeout*/)]
        public ActionResult Actors(bool forceRefresh = false)
        {
            if (forceRefresh || DB.Actors.HasChanged)
            {
                return PartialView(DB.Actors.ToList().OrderBy(c => c.Name));
            }
            return null;
        }
        [OnlineUsers.PowerUserAccess]
        public ActionResult Create()
        {
            ViewBag.Castings = null;
            ViewBag.Movies = SelectListUtilities<Movie>.Convert(DB.Movies.ToList(), "Title");
            return View(new Actor());
        }
        [OnlineUsers.PowerUserAccess]
        [HttpPost]
        public ActionResult Create(Actor actor, List<int> SelectedMoviesId)
        {
            if (ModelState.IsValid)
            {
                DB.Actors.Add(actor, SelectedMoviesId);
                return RedirectToAction("Index");
            }
            ViewBag.Castings = SelectListUtilities<Movie>.Convert(actor.Movies, "Title");
            ViewBag.Movies = SelectListUtilities<Movie>.Convert(DB.Movies.ToList(), "Title");
            return View();
        }
        [OnlineUsers.UserAccess]
        public ActionResult Details(int id)
        {
            Actor actor = DB.Actors.Get(id);
            if (actor != null)
            {
                return View(actor);
            }
            return RedirectToAction("Index");
        }
        [OnlineUsers.PowerUserAccess]
        public ActionResult Edit(int id)
        {
            Actor actor = DB.Actors.Get(id);
            if (actor != null)
            {
                ViewBag.Castings = SelectListUtilities<Movie>.Convert(actor.Movies, "Title");
                ViewBag.Movies = SelectListUtilities<Movie>.Convert(DB.Movies.ToList(), "Title");
                return View(actor);
            }
            return RedirectToAction("Index");
        }
        [OnlineUsers.PowerUserAccess]
        [HttpPost]
        public ActionResult Edit(Actor actor, List<int> SelectedMoviesId)
        {
            if (ModelState.IsValid)
            {
                DB.Actors.Update(actor, SelectedMoviesId);
                return RedirectToAction("Details", new {id = actor.Id });
            }
            ViewBag.Movies = SelectListUtilities<Movie>.Convert(actor.Movies, "Title");
            ViewBag.Movies = SelectListUtilities<Movie>.Convert(DB.Movies.ToList(), "Title");
            return View(actor);
        }
        [OnlineUsers.PowerUserAccess]
        public ActionResult Delete(int id)
        {
            DB.Actors.Delete(id);
            return RedirectToAction("Index");
        }
    }
}