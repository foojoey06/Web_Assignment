using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;


namespace Web_Assignment.Controllers;

public class AccountController(DB db, IWebHostEnvironment en, Helper hp) : Controller
{

        // GET: Account/Login
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginVM vm, string? returnURL)
        {
            //Get User record based on email
            var user = db.Staffs.FirstOrDefault(user => user.Email == vm.Email);

            //Verify Password
            if(user == null || !hp.VerifyPassword(user.Password, vm.Password))
            {
                ModelState.AddModelError("", "Incorrect Login Credentials.");
            }

            if (ModelState.IsValid)
            {
                TempData["Info"] = "Login Successfully.";

                //Sign in
                hp.SignIn(user!.Email, user.Role, vm.RememberMe);

                //Handle Return URL
                if (string.IsNullOrEmpty(returnURL))
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            return View(vm);
        }


        // GET: Account/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: Account/Register
        [HttpPost]
        public IActionResult Register(RegisterVM vm)
        {
            //Validate Photo Only
            if (ModelState.IsValid("Email") &&
            db.Staffs.Any(u => u.Email == vm.Email))
            {
                ModelState.AddModelError("Email", "Duplicated Email.");
            }

            if (ModelState.IsValid("Photo"))
            {
                var err = hp.ValidatePhoto(vm.Photo);
                if (err != "") ModelState.AddModelError("Photo", err);
            }

            if (ModelState.IsValid)
            {
                db.Staffs.Add(new()
                {
                    Name = vm.Name,
                    Password = hp.HashPassword(vm.Password),
                    Role = "Cashier",
                    Status = "Active",
                    Email = vm.Email,
                    Path = hp.SavePhoto(vm.Photo, "member"),
                });
                db.SaveChanges();
                TempData["Info"] = $"{vm.Name} inserted.";
                return RedirectToAction("Login");
            }
            return View(vm);
        }

        // GET: Account/Logout
        public IActionResult Logout(string? returnURL)
        {
            TempData["Info"] = "Logout successfully.";

            // Sign out
            hp.SignOut();

            return RedirectToAction("Index", "Home");
        }

        // GET: Account/AccessDenied
        public IActionResult AccessDenied(string? returnURL)
        {
            return View();
        }

        // GET: Account/CheckEmail
        public bool CheckEmail(string email)
        {
            return !db.Staffs.Any(u => u.Email == email);
        }

        // GET: Account/UpdatePassword
        [Authorize]
        public IActionResult UpdatePassword()
        {
            return View();
        }

        // POST: Account/UpdatePassword
        [Authorize]
        [HttpPost]
        public IActionResult UpdatePassword(UpdatePasswordVM vm)
        {
            var email = User.Identity!.Name;
            var u = db.Staffs.FirstOrDefault(x => x.Email == email);
            if (u == null) return RedirectToAction("Index", "Home");

            if (!hp.VerifyPassword(u.Password, vm.Current))
            {
                ModelState.AddModelError("Current", "Current Password not matched.");
            }

            if (ModelState.IsValid)
            {
                u.Password = hp.HashPassword(vm.New);
                db.SaveChanges();

                TempData["Info"] = "Password updated.";
                return RedirectToAction();
            }

            return View();
        }

        [Authorize(Roles = "Cashier")]
        public IActionResult UpdateProfile()
        {
            var email = User.Identity!.Name;
            var m = db.Staffs.FirstOrDefault(s => s.Email == email);
            if (m == null) return RedirectToAction("Index", "Home");

            var vm = new UpdateProfileVM
            {
                Email = m.Email,
                Name = m.Name,
                PhotoURL = m.Path,
            };

            return View(vm);
        }


        [HttpPost]
        public IActionResult UpdateProfile(UpdateProfileVM vm)
        {
            var email = User.Identity!.Name;
            var staff = db.Staffs.FirstOrDefault(s => s.Email == email);
            if (staff == null) return RedirectToAction("Index", "Home");

            if (vm.Photo != null)
            {
                var err = hp.ValidatePhoto(vm.Photo);
                if (!string.IsNullOrEmpty(err))
                    ModelState.AddModelError("Photo", err);
            }

            if (ModelState.IsValid)
            {
                staff.Name = vm.Name;

                if (vm.Photo != null)
                {
                    hp.DeletePhoto(staff.Path, "member"); // delete old photo
                    staff.Path = hp.SavePhoto(vm.Photo, "member"); // save new photo
                }

                db.SaveChanges();

                TempData["Info"] = "Profile updated.";
                return RedirectToAction();
            }

            vm.Email = staff.Email;
            vm.PhotoURL = staff.Path;
            return View(vm);
        }

    public IActionResult ForgetPassword()
    {
        return View();
    }

    [HttpPost]
    public IActionResult ForgetPassword(ForgetPasswordVM vm)
    {
        var u = db.Staffs.FirstOrDefault(s => s.Email == vm.Email);

        if (u == null)
        {
            ModelState.AddModelError("Email", "Email not found.");
        }

        if (ModelState.IsValid)
        {
            string password = hp.RandomPassword();

            //generate random otp
            Random random = new Random();
            int otp = random.Next(10000, 99999);

            //u!.Password = hp.HashPassword(password);
            u.Otp = otp;
            db.SaveChanges();

            // Send reset password email
            SendResetPasswordEmail(u, password);

            TempData["Info"] = $"Password reset. Check your email.";
        }
        return RedirectToAction();
    }

    private void SendResetPasswordEmail(Staff u, string password)
    {
        var mail = new MailMessage();
        mail.To.Add(new MailAddress(u.Email, u.Name));
        mail.Subject = "Reset Password";
        mail.IsBodyHtml = true;

        var url = Url.Action("Login", "Account", null, "https");

        var path = u switch
        {
            Admin a => Path.Combine(en.WebRootPath, "photos", a.Path),
            Cashier m => Path.Combine(en.WebRootPath, "photos", m.Path),
            _ => "",
        };

        var att = new Attachment(path);
        mail.Attachments.Add(att);
        att.ContentId = "photo";

        mail.Body = $@"
            <img src='cid:photo' style='width: 200px; height: 200px;
                                        border: 1px solid #333'>
            <p>Dear {u.Name},<p>
            <p>Your password has been reset to:</p>
            <h1 style='color: red'>{u.Otp}</h1>
            <p>
                Please <a href='{url}'>login</a>
                with your new password.
            </p>
            <p>From, 🐱 Super Admin</p>
        ";

        hp.SendEmail(mail);
    }

}

