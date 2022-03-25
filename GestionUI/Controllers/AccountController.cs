using DataLibrary.BusinessLogic;
using GestionUI.Models;
using log4net;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Configuration;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace GestionUI.Controllers
{

    [System.Web.Mvc.Authorize]
    public class AccountController : Controller
    {
        private static readonly ILog log = LogManager.GetLogger("AccountController");
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }




        public ActionResult Edit(string id, int rol)
        {
            if (RoleManager.AutorizedRole(User.Identity.GetUserId(), rol.ToString()) || User.IsInRole("Admin"))
            {
                ApplicationUser user = new ApplicationUser() { UserName = id };
                return View(user);
            }
            else
            {
                return RedirectToAction("/");
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string user, int[] roles)
        {
            if (!RoleManager.IsUserManager(User.Identity.GetUserId()))
                return Json(new
                {
                    status = "error",
                    success = false,
                    error = "No está autorizado para hacer cambios en los grupos de los usuarios."
                });
            try
            {
                var i = RoleManager.CargarRoles();
                int x = 0;
                foreach (var role in i)
                {
                    if (RoleManager.AutorizedRole(User.Identity.GetUserId(), role.id.ToString()))
                    {
                        if (RoleManager.IsAprobador(role.id))
                        {
                            x = UserProcessor.EliminarUserRol(role.id, UserManager.FindByName(user).Id);
                        }
                        else
                        {
                            UserProcessor.EliminarUserRol(role.id, UserManager.FindByName(user).Id);
                        }
                    }

                }
                if (roles != null)
                    foreach (int item in roles)
                    {
                        try
                        {
                            UserProcessor.AsignarRoles(UserManager.FindByName(user).Id, item);
                        }
                        catch (Exception)
                        {

                        }
                        
                        if (RoleManager.IsAprobador(item))
                        {
                            if (x == 1)
                                x = 0;
                            else
                                x = 2;
                            switch (x)
                            {
                                case 0:
                                    break;
                                case 2:
                                    var context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
                                    context.Clients.User(user).broadcastMessage("Ha sido agregado en el grupo que aprueba las TRD");
                                    List<DataLibrary.Models.NotificacionesModel> notificaciones = new List<DataLibrary.Models.NotificacionesModel>();
                                    NotificacionesLogic.Crearnotificacion(UserManager.FindByName(user).Id, "Ha sido agregado en el grupo que aprueba las TRD", DateTime.Now, false, "../");
                                    context.Clients.User(user).contadorNotificaciones(1);
                                    int version = TRDLogic.GetVersion();
                                    bool modificacion = true;
                                    if (version == 0)
                                    {
                                        modificacion = false;
                                        version = 1;
                                    }
                                    else
                                    {
                                        if (!TRDLogic.IsModificacion(version))
                                            modificacion = false;
                                    }

                                    if (modificacion)
                                        TRDLogic.CrearAprobacion(version, UserManager.FindByName(user).Id);
                                    break;
                            }

                        }
                    }
                if (x == 1)
                {
                    var context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
                    context.Clients.User(user).broadcastMessage("Ha sido expulsado del grupo que aprueba las TRD");
                    List<DataLibrary.Models.NotificacionesModel> notificaciones = new List<DataLibrary.Models.NotificacionesModel>();
                    NotificacionesLogic.Crearnotificacion(UserManager.FindByName(user).Id, "Ha sido expulsado del grupo que aprueba las TRD", DateTime.Now, false, "../");
                    context.Clients.User(user).contadorNotificaciones(1);
                    int version = TRDLogic.GetVersion();
                    bool modificacion = true;
                    if (version == 0)
                    {
                        modificacion = false;
                        version = 1;
                    }
                    else
                    {
                        if (!TRDLogic.IsModificacion(version))
                            modificacion = false;
                    }

                    if (modificacion)
                        TRDLogic.EliminarAprobacion(version, UserManager.FindByName(user).Id);
                }
                log.Info("El usuario " + User.Identity.Name + " ha cambiado al usuario de rol a "+user);
                return Json(new
                {
                    status = "success",
                    success = true
                });

            }
            catch (Exception e)
            {
                log.Error(e);
                return Json(new
                {
                    status = "error",
                    success = false,
                    error = " No se ha podido editar usuario, por favor, vuélvalo a intentar"
                });
            }

        }

        public ActionResult Index()
        {
            if (RoleManager.IsUserManager(User.Identity.GetUserId()))
                return View();
            else
                return RedirectToRoute(new { controller = "Home", action = "Index" });
        }
        [HttpPost]
        public ActionResult LockOut(string id)
        {
            if (!UserManager.IsInRole(UserManager.FindByName(id).Id, "Admin"))
            {
                UserManager.SetLockoutEndDate(UserManager.FindByName(id).Id, new DateTime(9999, 12, 30));
                log.Info("El usuario " + User.Identity.Name + " ha deshabilitado al usuario " + id);
                return Json(new
                {
                    status = "success",
                    success = true
                });
            }
            else
                return Json(new
                {
                    status = "error",
                    success = false,
                    error = " No deshabilitar usuario porque está en el grupo administrador"
                });

        }

        [HttpPost]
        public ActionResult Habilitar(string id)
        {
            if (!UserManager.IsInRole(UserManager.FindByName(id).Id, "Admin"))
            {
                UserManager.SetLockoutEndDateAsync(UserManager.FindByName(id).Id, DateTime.UtcNow);

                log.Info("El usuario " + User.Identity.Name + " ha habilitado al usuario " + id);
                return Json(new
                {
                    status = "success",
                    success = true
                });
            }
            else
                return Json(new
                {
                    status = "error",
                    success = false,
                    error = " No deshabilitar usuario porque está en el grupo administrador"
                });

        }


        //
        // GET: /Account/Login
        //
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            ApplicationUser UserID = UserManager.FindByName(model.Email);

            if (UserID is null)
            {
                UserID = UserManager.FindByEmail(model.Email);
                if (UserID != null)
                    model.Email = UserID.UserName;
            }



            // No cuenta los errores de inicio de sesión para el bloqueo de la cuenta
            // Para permitir que los errores de contraseña desencadenen el bloqueo de la cuenta, cambie a shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: true);
            switch (result)
            {

                case SignInStatus.Success:
                    if (UserID != null && !UserManager.IsEmailConfirmed(UserID.Id))
                    {
                        AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                        ModelState.AddModelError("", "La cuenta no ha confirmado el correo, por favor confirme el correo antes de entrar");
                        return View(model);
                    }
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    ModelState.AddModelError("", "La cuenta ha sido bloqueada, intente más tarde");
                    return View(model);
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Intento de inicio de sesión no válido.");
                    return View(model);
            }
        }

        //
        // GET: /Account/VerifyCode
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Requerir que el usuario haya iniciado sesión con nombre de usuario y contraseña o inicio de sesión externo
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // El código siguiente protege de los ataques por fuerza bruta a los códigos de dos factores. 
            // Si un usuario introduce códigos incorrectos durante un intervalo especificado de tiempo, la cuenta del usuario 
            // se bloqueará durante un período de tiempo especificado. 
            // Puede configurar el bloqueo de la cuenta en IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Código no válido.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model, int[] Roles)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user, "ContraseñaTemporal45678912318597561952184!+*.5.48.648994");

                if (result.Succeeded)
                {
                    UserProcessor.CrearUsuario(user.Id, model.nombre, model.apellido, model.identification, Roles);

                    // Para obtener más información sobre cómo habilitar la confirmación de cuentas y el restablecimiento de contraseña, visite https://go.microsoft.com/fwlink/?LinkID=320771
                    // Enviar correo electrónico con este vínculo
                    string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    await UserManager.SendEmailAsync(user.Id, "Confirmar cuenta", "Para confirmar la cuenta, haga clic <a href=\"" + callbackUrl + "\">aquí</a>");
                    log.Info("El usuario " + User.Identity.Name + " ha agregado al usuario " + user.Email);
                    return Content("<script language='javascript' type='text/javascript'>alert('Se ha ingresado usuario correctamente'); window.location.replace('/');</script>");
                }
                AddErrors(result);
            }

            // Si llegamos a este punto, es que se ha producido un error y volvemos a mostrar el formulario
            return View(model);
        }

        public ActionResult EditarUsuario()
        {
            var user = UserProcessor.CargarUsuario(User.Identity.GetUserId());
            var model = new UserModel()
            {
                id = user.id,
                apellido = user.apellido,
                email = user.email,
                identificacion = user.identificacion,
                nombre = user.nombre,
                UserName = user.UserName
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditarUsuario(UserModel model)
        {
            if (ModelState.IsValid)
            {
                var Original = UserProcessor.CargarUsuario(User.Identity.GetUserId());
                var UserModel = Original;
                ApplicationUser user = UserManager.FindById(UserModel.id);
                if (UserModel.email != model.email)
                {
                    user.Email = model.email;
                    user.EmailConfirmed = false;
                }
                if (UserModel.UserName != model.UserName)
                {
                    user.UserName = model.UserName;
                }

                if (UserModel.nombre != model.nombre)
                {
                    UserModel.nombre = model.nombre;
                }

                if (UserModel.apellido != model.apellido)
                {
                    UserModel.apellido = model.apellido;
                }
                if (UserModel.identificacion != model.identificacion)
                {
                    UserModel.identificacion = model.identificacion;
                }


                UserProcessor.UpdateUsuario(UserModel);
                var result = await UserManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    if (Original.UserName != model.UserName)
                    {
                        log.Info("El usuario " + Original.UserName + " ha cambiado su UserName de " + Original.UserName + " a " + model.UserName + ".");
                    }

                    if (Original.nombre != model.nombre)
                    {
                        log.Info("El usuario " + Original.UserName + " ha cambiado su Nombre de " + Original.nombre + " a " + model.nombre + ".");
                    }

                    if (Original.apellido != model.apellido)
                    {
                        log.Info("El usuario " + Original.UserName + " ha cambiado su Apellido de " + Original.apellido + " a " + model.apellido + ".");
                    }
                    if (Original.identificacion != model.identificacion)
                    {
                        log.Info("El usuario " + Original.UserName + " ha cambiado su Identificacion de " + Original.identificacion + " a " + model.identificacion + ".");
                    }
                    if (!user.EmailConfirmed)
                    {
                        log.Info("El usuario " + UserModel.UserName + " ha cambiado su correo de " + UserModel.email + " a " + user.Email + ".");
                        string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                        var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                        await UserManager.SendEmailAsync(user.Id, "Confirmar cuenta", "Para confirmar la cuenta, haga clic <a href=\"" + callbackUrl + "\">aquí</a>");
                    }

                    AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                    return Content("<script language='javascript' type='text/javascript'>alert('Se ha Editado el usuario correctamente'); window.location.replace('/');</script>");

                }
                AddErrors(result);
            }
            return View(model);
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByEmailAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    if (user != null)
                    {
                        string code1 = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                        var callbackUrl1 = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code1 }, protocol: Request.Url.Scheme);
                        await UserManager.SendEmailAsync(user.Id, "Confirmar cuenta", "No se ha podido reestablecer contraseña " +
                            "debido a que no se ha confirmado el correo para confirmar la cuenta, haga clic <a href=\"" + callbackUrl1 + "\">aquí</a>");
                    }
                    // No revelar que el usuario no existe o que no está confirmado
                    return View("ForgotPasswordConfirmation");
                }

                // Para obtener más información sobre cómo habilitar la confirmación de cuentas y el restablecimiento de contraseña, visite https://go.microsoft.com/fwlink/?LinkID=320771
                // Enviar correo electrónico con este vínculo
                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                await UserManager.SendEmailAsync(user.Id, "Restablecer contraseña", "Para restablecer la contraseña, haga clic <a href=\"" + callbackUrl + "\">aquí</a>");
                return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // Si llegamos a este punto, es que se ha producido un error y volvemos a mostrar el formulario
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // No revelar que el usuario no existe
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                UserManager.UpdateSecurityStamp(user.Id);
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin


        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generar el token y enviarlo
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Si el usuario ya tiene un inicio de sesión, iniciar sesión del usuario con este proveedor de inicio de sesión externo
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // Si el usuario no tiene ninguna cuenta, solicitar que cree una
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Obtener datos del usuario del proveedor de inicio de sesión externo
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Aplicaciones auxiliares
        // Se usa para la protección XSRF al agregar inicios de sesión externos
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }


            public Task SendAsync(IdentityMessage message, HttpPostedFileBase file)
            {
                var smtp = new SmtpClient();
                var mail = new MailMessage();
                var smtpSection = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
                string username = smtpSection.Network.UserName;

                mail.IsBodyHtml = true;
                mail.From = new MailAddress(username);
                mail.To.Add(message.Destination);
                mail.Subject = message.Subject;
                mail.Body = message.Body;
                if (file.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(file.FileName);
                    mail.Attachments.Add(new Attachment(file.InputStream, fileName));
                }
                smtp.Timeout = 1000;

                var t = Task.Run(() => smtp.SendAsync(mail, null));

                return t;
            }

        }
        #endregion


    }
}