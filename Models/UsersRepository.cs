﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace MDB.Models
{
    public class UsersRepository : Repository<User>
    {
        public bool EmailAvailable(string email, int excludedId = 0)
        {
            User user = ToList().Where(u => u.Email.ToLower() == email.ToLower()).FirstOrDefault();
            if (user == null)
                return true;
            else
                if (user.Id != excludedId)
                return user.Email.ToLower() != email.ToLower();
            return true;
        }
        public bool EmailExist(string email)
        {
            return ToList().Where(u => u.Email.ToLower() == email.ToLower()).FirstOrDefault() != null;
        }
        public bool EmailBlocked(string email)
        {
            User user = ToList().Where(u => u.Email.ToLower() == email.ToLower()).FirstOrDefault();
            if (user != null)
                return user.Blocked;
            return true;
        }
        public bool EmailVerified(string email)
        {
            User user = ToList().Where(u => u.Email.ToLower() == email.ToLower()).FirstOrDefault();
            if (user != null)
                return user.Verified;
            return false;
        }
        public User GetUser(LoginCredential loginCredential)
        {
            User user = ToList().Where(u => (u.Email.ToLower() == loginCredential.Email.ToLower()) &&
                                            (u.Password == loginCredential.Password))
                                .FirstOrDefault();
            return user;
        }
        public User Add(User user)
        {
            try
            {
                user.CreationDate = DateTime.Now;
                user.SaveAvatar();
                OnlineUsers.SetHasChanged();
                user.Id = base.Add(user);
                return user;
            }
            catch (Exception ex)
            {
                user.RemoveAvatar();
                System.Diagnostics.Debug.WriteLine($"Add user failed : Message - {ex.Message}");
            }
            return null;
        }
        public override bool Update(User user)
        {
            try
            {
                user.SaveAvatar();
                OnlineUsers.SetHasChanged();
                return base.Update(user);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Update user failed : Message - {ex.Message}");
            }
            return false;
        }
        private void RemoveLogins(int userId)
        {
            List<Login> logins = DB.Logins.ToList().Where(l => l.UserId == userId).ToList();
            foreach (Login login in logins)
            {
                DB.Logins.Delete(login.Id);
            }
        }
        private void RemoveUnverifiedEmails(int userId)
        {
            List<UnverifiedEmail> UnverifiedEmails = DB.UnverifiedEmails.ToList().Where(u => u.UserId == userId).ToList();
            foreach (UnverifiedEmail UnverifiedEmail in UnverifiedEmails)
            {
                DB.UnverifiedEmails.Delete(UnverifiedEmail.Id);
            }
        }
        private void RemoveResetPasswordCommands(int userId)
        {
            List<ResetPasswordCommand> ResetPasswordCommands = DB.ResetPasswordCommands.ToList().Where(r => r.UserId == userId).ToList();
            foreach (ResetPasswordCommand ResetPasswordCommand in ResetPasswordCommands)
            {
                DB.ResetPasswordCommands.Delete(ResetPasswordCommand.Id);
            }
        }
        public override bool Delete(int userId)
        {
            try
            {
                BeginTransaction();
                User userToDelete = DB.Users.Get(userId);
                if (userToDelete != null)
                {
                    RemoveLogins(userId);
                    RemoveUnverifiedEmails(userId);
                    RemoveResetPasswordCommands(userId);
                    userToDelete.RemoveAvatar();
                    base.Delete(userId);
                    OnlineUsers.RemoveUser(userToDelete.Id);
                    OnlineUsers.SetHasChanged();
                    return true;
                }
                EndTransaction();
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Remove user failed : Message - {ex.Message}");
                EndTransaction();
                return false;
            }
        }
        public User FindUser(int id)
        {
            try
            {
                User user = DB.Users.Get(id);
                if (user != null)
                {
                    user.ConfirmEmail = user.Email;
                    user.ConfirmPassword = user.Password;
                }
                return user;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Find user failed : Message - {ex.Message}");
                return null;
            }
        }
        public IEnumerable<User> SortedUsers()
        {
            return ToList().OrderBy(u => u.FirstName).ThenBy(u => u.LastName);
        }
        public bool Verify_User(int userId, int code)
        {
            User user = Get(userId);
            if (user != null)
            {
                // take the last email verification request
                UnverifiedEmail unverifiedEmail = DB.UnverifiedEmails.ToList().Where(u => u.UserId == userId).FirstOrDefault();
                if (unverifiedEmail != null)
                {
                    if (unverifiedEmail.VerificationCode == code)
                    {
                        try
                        {
                            BeginTransaction();
                            user.Email = user.ConfirmEmail = unverifiedEmail.Email;
                            user.Verified = true;
                            base.Update(user);
                            DB.UnverifiedEmails.Delete(unverifiedEmail.Id);
                            OnlineUsers.SetHasChanged();
                            EndTransaction();
                            return true;
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Verify_User failed : Message - {ex.Message}");
                            EndTransaction();
                        }
                    }
                }
            }
            return false;
        }
        public UnverifiedEmail Add_UnverifiedEmail(int userId, string email)
        {
            try
            {
                UnverifiedEmail unverifiedEmail = new UnverifiedEmail() { UserId = userId, Email = email, VerificationCode = DateTime.Now.Millisecond };
                unverifiedEmail.Id = DB.UnverifiedEmails.Add(unverifiedEmail);
                return unverifiedEmail;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Add_UnverifiedEmail failed : Message - {ex.Message}");
                return null;
            }
        }
        public bool HaveUnverifiedEmail(int userId, int code)
        {
            return DB.UnverifiedEmails.ToList().Where(u => (u.UserId == userId && u.VerificationCode == code)).FirstOrDefault() != null;
        }
        public ResetPasswordCommand Add_ResetPasswordCommand(string email)
        {
            try
            {
                User user = DB.Users.ToList().Where(u => u.Email == email).FirstOrDefault();
                if (user != null)
                {
                    ResetPasswordCommand resetPasswordCommand =
                        new ResetPasswordCommand() { UserId = user.Id, VerificationCode = DateTime.Now.Millisecond };

                    resetPasswordCommand.Id = DB.ResetPasswordCommands.Add(resetPasswordCommand);
                    return resetPasswordCommand;
                }
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Add_ResetPasswordCommand failed : Message - {ex.Message}");
                return null;
            }
        }
        public ResetPasswordCommand Find_ResetPasswordCommand(int userid, int verificationCode)
        {
            return DB.ResetPasswordCommands.ToList().Where(r => (r.UserId == userid && r.VerificationCode == verificationCode)).FirstOrDefault();
        }
        public bool ResetPassword(int userId, string password)
        {
            User user = Get(userId);
            if (user != null)
            {
                user.Password = user.ConfirmPassword = password;
                ResetPasswordCommand resetPasswordCommand = DB.ResetPasswordCommands.ToList().Where(r => r.UserId == userId).FirstOrDefault();
                if (resetPasswordCommand != null)
                {
                    try
                    {
                        DB.ResetPasswordCommands.Delete(resetPasswordCommand.Id);
                        return base.Update(user);

                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"ResetPassword failed : Message - {ex.Message}");
                    }
                }
            }
            return false;
        }
        public Login AddLogin(int userId)
        {
            try
            {
                Login login = new Login();
                login.LoginDate = login.LogoutDate = DateTime.Now;
                login.UserId = userId;
                login.IpAddress = HttpContext.Current.Request.UserHostAddress;
                if (login.IpAddress != "::1")
                {
                    GeoLocation gl = GeoLocation.Call(login.IpAddress);
                    login.City = gl.city;
                    login.RegionName = gl.regionName;
                    login.CountryCode = gl.countryCode;
                }
                login.Id = DB.Logins.Add(login);
                return login;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"AddLogin failed : Message - {ex.Message}");
                return null;
            }
        }
        public bool UpdateLogout(int loginId)
        {
            try
            {
                Login login = DB.Logins.Get(loginId);
                if (login != null)
                {
                    login.LogoutDate = DateTime.Now;
                    return DB.Logins.Update(login);
                }
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UpdateLogout failed : Message - {ex.Message}");
                return false;
            }
        }
        public bool UpdateLogoutByUserId(int userId)
        {
            try
            {
                Login login = DB.Logins.ToList().Where(l => l.UserId == userId).OrderByDescending(l => l.LoginDate).FirstOrDefault();
                if (login != null)
                {
                    login.LogoutDate = DateTime.Now;
                    return DB.Logins.Update(login);
                }
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UpdateLogoutByUserId failed : Message - {ex.Message}");
                return false;
            }
        }
        public bool DeleteLoginsJournalDay(DateTime day)
        {
            try
            {
                BeginTransaction();
                DateTime dayAfter = day.AddDays(1);
                IEnumerable<Login> logins = DB.Logins.ToList().Where(l => l.LoginDate >= day && l.LoginDate < dayAfter);
                foreach (Login login in logins)
                {
                    DB.Logins.Delete(login.Id);
                }
                EndTransaction();
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"DeleteLoginsJournalDay failed : Message - {ex.Message}");
                EndTransaction();
                return false;
            }
        }
    }
}