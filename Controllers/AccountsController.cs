﻿using MDB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Web.Mvc;

namespace MDB.Controllers
{
    public class AccountsController : Controller
    {

        #region Account creation
        [HttpPost]
        public JsonResult EmailAvailable(string email, int Id = 0)
        {
            return Json(DB.Users.EmailAvailable(email, Id));
        }

        [HttpPost]
        public JsonResult EmailExist(string email)
        {
            return Json(DB.Users.EmailExist(email));
        }

        public ActionResult Subscribe()
        {
            ViewBag.Genders = SelectListUtilities<Gender>.Convert(DB.Genders.ToList());
            User user = new User();
            user.UserTypeId = 3; // self subscribed user 
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult Subscribe(User user)
        {
            if (ModelState.IsValid)
            {
                user = DB.Users.Add(user);
                if (user != null)
                {
                    SendEmailVerification(user, user.Email);
                    return RedirectToAction("SubscribeDone/" + user.Id.ToString());
                }
                else
                    return RedirectToAction("Report", "Errors", new { message = "Échec de création de compte" });
            }
            ViewBag.Genders = SelectListUtilities<Gender>.Convert(DB.Genders.ToList());
            return View(user);
        }

        public ActionResult SubscribeDone(int id)
        {
            User newlySubscribedUser = DB.Users.Get(id);
            if (newlySubscribedUser != null)
                return View(newlySubscribedUser);
            return RedirectToAction("Login");
        }
        #endregion

        #region Account Verification
        public void SendEmailVerification(User user, string newEmail)
        {
            if (user.Id != 0)
            {
                UnverifiedEmail unverifiedEmail = DB.Users.Add_UnverifiedEmail(user.Id, newEmail);
                if (unverifiedEmail != null)
                {
                    string verificationUrl = Url.Action("VerifyUser", "Accounts", null, Request.Url.Scheme);
                    String Link = @"<br/><a href='" + verificationUrl + "?userid=" + user.Id + "&code=" + unverifiedEmail.VerificationCode + @"' > Confirmez votre inscription...</a>";

                    String suffixe = "";
                    if (user.GenderId == 2)
                    {
                        suffixe = "e";
                    }
                    string Subject = "MDB - Vérification d'inscription...";

                    string Body = "Bonjour " + user.GetFullName(true) + @",<br/><br/>";
                    Body += @"Merci de vous être inscrit" + suffixe + " au site MDB. <br/>";
                    Body += @"Pour utiliser votre compte vous devez confirmer votre inscription en cliquant sur le lien suivant : <br/>";
                    Body += Link;
                    Body += @"<br/><br/>Ce courriel a été généré automatiquement, veuillez ne pas y répondre.";
                    Body += @"<br/><br/>Si vous éprouvez des difficultés ou s'il s'agit d'une erreur, veuillez le signaler à <a href='mailto:"
                         + SMTP.OwnerEmail + "'>" + SMTP.OwnerName + "</a> (Webmestre du site MDB)";

                    SMTP.SendEmail(user.GetFullName(), unverifiedEmail.Email, Subject, Body);
                }
            }
        }
        public ActionResult VerifyDone(int id)
        {
            User newlySubscribedUser = DB.Users.Get(id);
            if (newlySubscribedUser != null)
                return View(newlySubscribedUser);
            return RedirectToAction("Login");
        }
        public ActionResult VerifyError()
        {
            return View();
        }
        public ActionResult AlreadyVerified()
        {
            return View();
        }
        public ActionResult VerifyUser(int userid, int code)
        {
            User newlySubscribedUser = DB.Users.Get(userid);
            if (newlySubscribedUser != null)
            {
                if (!DB.Users.HaveUnverifiedEmail(userid, code))
                {
                    if (DB.Users.EmailVerified(newlySubscribedUser.Email))
                        return RedirectToAction("AlreadyVerified");
                }
                if (DB.Users.Verify_User(userid, code))
                    return RedirectToAction("VerifyDone/" + userid);
            }
            return RedirectToAction("VerifyError");
        }
        #endregion

        #region EmailChange
        public ActionResult EmailChangedAlert()
        {
            OnlineUsers.RemoveSessionUser();
            return View();
        }
        public void SendEmailChangedVerification(User user, string newEmail)
        {
            if (user.Id != 0)
            {
                UnverifiedEmail unverifiedEmail = DB.Users.Add_UnverifiedEmail(user.Id, newEmail);
                if (unverifiedEmail != null)
                {
                    string verificationUrl = Url.Action("VerifyUser", "Accounts", null, Request.Url.Scheme);
                    String Link = @"<br/><a href='" + verificationUrl + "?userid=" + user.Id + "&code=" + unverifiedEmail.VerificationCode + @"' > Confirmez votre adresse...</a>";

                    string Subject = "MDB - Vérification de courriel...";

                    string Body = "Bonjour " + user.GetFullName(true) + @",<br/><br/>";
                    Body += @"Vous avez modifié votre adresse de courriel. <br/>";
                    Body += @"Pour que ce changement soit pris en compte, vous devez confirmer cette adresse en cliquant sur le lien suivant : <br/>";
                    Body += Link;
                    Body += @"<br/><br/>Ce courriel a été généré automatiquement, veuillez ne pas y répondre.";
                    Body += @"<br/><br/>Si vous éprouvez des difficultés ou s'il s'agit d'une erreur, veuillez le signaler à <a href='mailto:"
                         + SMTP.OwnerEmail + "'>" + SMTP.OwnerName + "</a> (Webmestre du site [nom de l'application])";

                    SMTP.SendEmail(user.GetFullName(), unverifiedEmail.Email, Subject, Body);
                }
            }
        }
        #endregion

        #region ResetPassword
        public ActionResult ResetPasswordCommand()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ResetPasswordCommand(string Email)
        {
            if (ModelState.IsValid)
            {
                SendResetPasswordCommandEmail(Email);
                return RedirectToAction("ResetPasswordCommandAlert");
            }
            return View(Email);
        }
        public void SendResetPasswordCommandEmail(string email)
        {
            ResetPasswordCommand resetPasswordCommand = DB.Users.Add_ResetPasswordCommand(email);
            if (resetPasswordCommand != null)
            {
                User user = DB.Users.Get(resetPasswordCommand.UserId);
                string verificationUrl = Url.Action("ResetPassword", "Accounts", null, Request.Url.Scheme);
                String Link = @"<br/><a href='" + verificationUrl + "?userid=" + user.Id + "&code=" + resetPasswordCommand.VerificationCode + @"' > Réinitialisation de mot de passe...</a>";

                string Subject = "MDB - Réinitialisaton ...";

                string Body = "Bonjour " + user.GetFullName(true) + @",<br/><br/>";
                Body += @"Vous avez demandé de réinitialiser votre mot de passe. <br/>";
                Body += @"Procédez en cliquant sur le lien suivant : <br/>";
                Body += Link;
                Body += @"<br/><br/>Ce courriel a été généré automatiquement, veuillez ne pas y répondre.";
                Body += @"<br/><br/>Si vous éprouvez des difficultés ou s'il s'agit d'une erreur, veuillez le signaler à <a href='mailto:"
                     + SMTP.OwnerEmail + "'>" + SMTP.OwnerName + "</a> (Webmestre du site [nom de l'application])";

                SMTP.SendEmail(user.GetFullName(), user.Email, Subject, Body);
            }
        }
        public ActionResult ResetPassword(int userid, int code)
        {
            ResetPasswordCommand resetPasswordCommand = DB.Users.Find_ResetPasswordCommand(userid, code);
            if (resetPasswordCommand != null)
                return View(new PasswordView() { UserId = userid });
            return RedirectToAction("ResetPasswordError");
        }
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult ResetPassword(PasswordView passwordView)
        {
            if (ModelState.IsValid)
            {
                if (DB.Users.ResetPassword(passwordView.UserId, passwordView.Password))
                    return RedirectToAction("ResetPasswordSuccess");
                else
                    return RedirectToAction("ResetPasswordError");
            }
            return View(passwordView);
        }
        public ActionResult ResetPasswordCommandAlert()
        {
            return View();
        }
        public ActionResult ResetPasswordSuccess()
        {
            return View();
        }
        public ActionResult ResetPasswordError()
        {
            return View();
        }
        #endregion

        #region Profil
        [OnlineUsers.UserAccess]
        public ActionResult Profil()
        {
            ViewBag.Genders = SelectListUtilities<Gender>.Convert(DB.Genders.ToList());
            User userToEdit = OnlineUsers.GetSessionUser().Clone();
            if (userToEdit != null)
            {
                Session["currentUserPassword"] = userToEdit.Password;
                Session["UnchangedPasswordCode"] = Guid.NewGuid().ToString().Substring(0, 12);
                userToEdit.Password = userToEdit.ConfirmPassword = (string)Session["UnchangedPasswordCode"];
                return View(userToEdit);
            }
            return null;
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult Profil(User user)
        {
            User currentUser = OnlineUsers.GetSessionUser();
            string newEmail = "";
            if (ModelState.IsValid)
            {
                if (user.Password == (string)Session["UnchangedPasswordCode"])
                    user.Password = user.ConfirmPassword = (string) Session["currentUserPassword"];

                if (user.Email != currentUser.Email)
                {
                    newEmail = user.Email;
                    user.Email = user.ConfirmEmail = currentUser.Email;
                }

                if (DB.Users.Update(user))
                {
                    if (newEmail != "")
                    {
                        SendEmailChangedVerification(user, newEmail);
                        return RedirectToAction("EmailChangedAlert");
                    }
                    else
                        return RedirectToAction("Index", "Movies");
                }
                else
                    return RedirectToAction("Report", "Errors", new { message = "Échec de modification de profil" });
            }
            ViewBag.Genders = SelectListUtilities<Gender>.Convert(DB.Genders.ToList());
            return View(currentUser);
        }
        #endregion

        #region Login and Logout
        public ActionResult Login(string message)
        {
            ViewBag.Message = message;
            return View(new LoginCredential());
        }
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult Login(LoginCredential loginCredential)
        {
            if (ModelState.IsValid)
            {
                if (DB.Users.EmailBlocked(loginCredential.Email))
                {
                    ModelState.AddModelError("Email", "Ce compte est bloqué.");
                    return View(loginCredential);
                }
                if (!DB.Users.EmailVerified(loginCredential.Email))
                {
                    ModelState.AddModelError("Email", "Ce courriel n'est pas vérifié.");
                    return View(loginCredential);
                }
                User user = DB.Users.GetUser(loginCredential);
                if (user == null)
                {
                    ModelState.AddModelError("Password", "Mot de passe incorrecte.");
                    return View(loginCredential);
                }
                if (OnlineUsers.IsOnLine(user.Id))
                {
                    ModelState.AddModelError("Email", "Cet usager est déjà connecté.");
                    return View(loginCredential);
                }
                OnlineUsers.AddSessionUser(user.Id);
                Session["currentLoginId"] = DB.Users.AddLogin(user.Id).Id;
                return RedirectToAction("Index", "Movies");
            }
            return View(loginCredential);
        }
        public ActionResult Logout()
        {
            if (Session["currentLoginId"] != null)
                DB.Users.UpdateLogout((int)Session["currentLoginId"]);
            OnlineUsers.RemoveSessionUser();
            return RedirectToAction("Login");
        }
        #endregion

        #region Administrator actions
        public JsonResult NeedUpdate()
        {
            return Json(OnlineUsers.HasChanged(), JsonRequestBehavior.AllowGet);
        }
       
        [OnlineUsers.AdminAccess]
        public JsonResult ChangeUserBlockedStatus(int userid, bool blocked)
        {
            User user = DB.Users.Get(userid);
            user.Blocked = blocked;
            return Json(DB.Users.Update(user), JsonRequestBehavior.AllowGet);
        }
        [OnlineUsers.AdminAccess]
        public JsonResult Delete(int userid)
        {
            return Json(DB.Users.Delete(userid), JsonRequestBehavior.AllowGet);
        }
        [OnlineUsers.AdminAccess]
        public ActionResult UserList()
        {
            return View();
        }
        [OnlineUsers.AdminAccess(false)] // RefreshTimout = false otherwise periodical refresh with lead to never timed out session
        public ActionResult GetUsersList(bool forceRefresh = false)
        {
            if (forceRefresh || OnlineUsers.HasChanged())
            {
                return PartialView(DB.Users.SortedUsers());
            }
            return null;
        }
        #endregion

        #region GroupEmail
        [OnlineUsers.AdminAccess]
        public ActionResult GroupEmail(string status = "")
        {
            ViewBag.SelectedUsers = new List<int>();
            ViewBag.Users = DB.Users.SortedUsers();
            ViewBag.Status = status;
            return View(new GroupEmail() { Message = "Bonjour [Nom]," });
        }
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult GroupEmail(GroupEmail groupEmail, List<int> SelectedUsers)
        {
            if (ModelState.IsValid)
            {
                groupEmail.SelectedUsers = SelectedUsers;
                groupEmail.Send();
                return RedirectToAction("GroupEmail", new { status = "Message envoyé avec succès." });
            }
            ViewBag.SelectedUsers = SelectedUsers;
            ViewBag.Users = DB.Users.SortedUsers();
            return View(groupEmail);
        }
        #endregion

        #region Login journal
        [OnlineUsers.AdminAccess]
        public ActionResult LoginsJournal()
        {
            return View();
        }
        [OnlineUsers.AdminAccess(false)] // RefreshTimout = false otherwise periodical refresh with lead to never timed out session
        public ActionResult GetLoginsList(bool forceRefresh = false)
        {
            if (forceRefresh || OnlineUsers.HasChanged())
            {
                ViewBag.LoggedUsersId = new List<int>(OnlineUsers.ConnectedUsersId);
                return PartialView(DB.Logins.ToList().OrderByDescending(l => l.LoginDate));
            }
            return null;
        }
        [OnlineUsers.AdminAccess]
        public ActionResult DeleteJournalDay(string day)
        {
            try
            {
                DateTime date = DateTime.Parse(day);
                DB.Users.DeleteLoginsJournalDay(date);
            }
            catch (Exception) { }
            return RedirectToAction("LoginsJournal");
        }
        #endregion
    }

}