using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Warehouse.Data;
using Warehouse.Models;

namespace Warehouse.Areas.Identity.Pages.Account
{
    [Authorize(Roles = "Admin" )]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
		private readonly ApplicationDbContext _context;
        public RegisterModel(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
			ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
			_context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
			[Required]
			[Display(Name = "Name")]
			public string Name { get; set; }
			
			[Required]
			[Display(Name = "Surname")]
			public string Surname { get; set; }

			[Required]
			[Display(Name ="Birthdate")]
			//[DisplayFormat(DataFormatString = "{yyyy-MM-dd}")]
            [DataType(DataType.Date)]
            public DateTime Birthdate { get; set; }

			[Required]
			[Display(Name = "Warehouse")]
            public string WarehouseId { get; set; }
			public WareHouse Warehouse { get; set; }

			[Required]
			[Display(Name ="Role")]
			public string Role { get; set; }

            [Phone]
            [Display(Name = "Phone")]
            [DataType(DataType.PhoneNumber)]
            [Required(ErrorMessage = "Phone Number Required!")]
            [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{3})$",
                   ErrorMessage = "Entered phone format is not valid.")]
            public string PhoneNumber { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
			ViewBag.Warehouse = new SelectList(_context.Warehouses, "Id", "Number");
			ViewBag.AspNetRoles = new SelectList(_context.Roles, "Id", "Name");
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            string role = null;

			if (await _userManager.IsInRoleAsync(await _userManager.GetUserAsync(User), "Admin"))
			{
				if (Input.Role == "Worker" || Input.Role == "Storekeeper" || Input.Role == "Report")
					role = Input.Role;
			}

			if (role == null)
				return Page();

			returnUrl = returnUrl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {
                var user = new AppUser {
                    Name = Input.Name,
                    SurName = Input.Surname,
                    BirthDate = Input.Birthdate,
                    WarehouseId = Input.WarehouseId,
                    UserName = Input.Email,
                    Email = Input.Email,
                    PhoneNumber = Input.PhoneNumber
                };
                
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { userId = user.Id, code = code },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                   // await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
			

			}

            ViewBag.Warehouse = new SelectList(_context.Warehouses, nameof(WareHouse.Id), nameof(WareHouse.Number));
            // If we got this far, something failed, redisplay form
            return Page();
            
           
        }

		private DynamicViewData _viewBag;

		public dynamic ViewBag
		{
			get
			{
				if (_viewBag == null)
				{
					_viewBag = new DynamicViewData(() => ViewData);
				}
				return _viewBag;
			}
		}

	}
}
