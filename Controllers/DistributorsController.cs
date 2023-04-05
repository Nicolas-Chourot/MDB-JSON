﻿using Antlr.Runtime.Misc;
using MDB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MDB.Controllers
{
    public class DistributorsController : Controller
    {
        [OnlineUsers.UserAccess]
        public ActionResult Index()
        {
            return View();
        }
        [OnlineUsers.UserAccess(false/* do not reset timeout*/)]
        public PartialViewResult Distributors(bool forceRefresh = false)
        {
            if (forceRefresh || DB.Distributors.HasChanged)
            {
                return PartialView(DB.Distributors.ToList().OrderBy(c => c.Name));
            }
            return null;
        }
        [OnlineUsers.PowerUserAccess]
        public ActionResult Create()
        {
            ViewBag.Distributions = null;
            ViewBag.Movies = SelectListUtilities<Movie>.Convert(DB.Movies.ToList(), "Title");
            return View(new Distributor());
        }
        [OnlineUsers.PowerUserAccess]
        [HttpPost]
        public ActionResult Create(Distributor distributor, List<int> SelectedMoviesId)
        {
            if (ModelState.IsValid)
            {
                DB.Distributors.Add(distributor, SelectedMoviesId);
                return RedirectToAction("Index");
            }
            ViewBag.Distributions = SelectListUtilities<Movie>.Convert(distributor.Movies, "Title");
            ViewBag.Movies = SelectListUtilities<Movie>.Convert(DB.Movies.ToList(), "Title");
            return View();
        }
        [OnlineUsers.UserAccess]
        public ActionResult Details(int id)
        {
            Distributor distributor = DB.Distributors.Get(id);
            if (distributor != null)
            {
                return View(distributor);
            }
            return RedirectToAction("Index");
        }
        [OnlineUsers.PowerUserAccess]
        public ActionResult Edit(int id)
        {
            Distributor distributor = DB.Distributors.Get(id);
            if (distributor != null)
            {
                ViewBag.Distributions = SelectListUtilities<Movie>.Convert(distributor.Movies, "Title");
                ViewBag.Movies = SelectListUtilities<Movie>.Convert(DB.Movies.ToList(), "Title");
                return View(distributor);
            }
            return RedirectToAction("Index");
        }
        [OnlineUsers.PowerUserAccess]
        [HttpPost]
        public ActionResult Edit(Distributor distributor, List<int> SelectedMoviesId)
        {
            if (ModelState.IsValid)
            {
                DB.Distributors.Update(distributor, SelectedMoviesId);
                return RedirectToAction("Details", new {id = distributor.Id});
            }
            ViewBag.Distributions = SelectListUtilities<Movie>.Convert(distributor.Movies);
            ViewBag.Movies = SelectListUtilities<Movie>.Convert(DB.Movies.ToList());
            return View(distributor);
        }
        [OnlineUsers.PowerUserAccess]
        public ActionResult Delete(int id)
        {
            DB.Distributors.Delete(id);
            return RedirectToAction("Index");
        }
    }
}