using BookStore.Models;
using BookStore.Models.ViewModels;
using BookStore.Repository.IRepository;
using BookStore.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace BookStore.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly UserManager<IdentityUser> userManager;
        private readonly IEmailSender emailSender;

        public CartController( IUnitOfWork unitOfWork , UserManager<IdentityUser> userManager , IEmailSender emailSender)
        {
            this.unitOfWork = unitOfWork;
            this.userManager = userManager;
            this.emailSender = emailSender;
        }
        public IActionResult Index()
        {
            var userclaims = (ClaimsIdentity)User.Identity;
            var claim = userclaims.FindFirst(ClaimTypes.NameIdentifier);
            ShoppingCartVM shoppingCartVM = new ShoppingCartVM
            {
                shoppingCarts = unitOfWork.shoppingCart.GetAll(x => x.ApplicationUserId == claim.Value, includeproprites: "Product").ToList(),
                orderHeader = new OrderHeader()
            };
            shoppingCartVM.orderHeader.OrderTotal = 0;
            shoppingCartVM.orderHeader.ApplicationUser = unitOfWork.ApplicationUser.GetFirstOrDefault(x => x.Id == claim.Value , includeproprites:"Company");
            foreach ( var list in shoppingCartVM.shoppingCarts)
            {
                list.Price = Sd.GetPriceBasedOnQuantity(list.Count, list.Price, list.Product.Price50, list.Product.Price100);
                shoppingCartVM.orderHeader.OrderTotal += list.Product.Price * list.Count;
                list.Product.Description = Sd.ConvertToRawHtml(list.Product.Description);
                if (list.Product.Description.Length > 100)
                {
                    list.Product.Description = list.Product.Description.Substring(0, 99) + "...";
                }
            }
            return View(shoppingCartVM);
        }
        [HttpPost]
        [ActionName("Index")]
        public async Task<IActionResult> IndexPOST()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var user = unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == claim.Value);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Verification email is empty!");
            }

            var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { area = "Identity", userId = user.Id, code = code },
                protocol: Request.Scheme);

            await emailSender.SendEmailAsync(user.Email, "Confirm your email",
                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

            ModelState.AddModelError(string.Empty, "Verification email sent. Please check your email.");
            return RedirectToAction("Index");

        }


        public IActionResult Plus(int cartId)
        {
            var cart = unitOfWork.shoppingCart.GetFirstOrDefault
                            (c => c.Id == cartId, includeproprites: "Product");
            cart.Count += 1;
            cart.Price = Sd.GetPriceBasedOnQuantity(cart.Count, cart.Product.Price,
                                    cart.Product.Price50, cart.Product.Price100);
            unitOfWork.save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Minus(int cartId)
        {
            var cart = unitOfWork.shoppingCart.GetFirstOrDefault
                            (c => c.Id == cartId, includeproprites: "Product");

            if (cart.Count == 1)
            {
                var cnt = unitOfWork.shoppingCart.GetAll(u => u.ApplicationUserId == cart.ApplicationUserId).ToList().Count;
                unitOfWork.shoppingCart.remove(cart);
                unitOfWork.save();
                HttpContext.Session.SetInt32(Sd.ssShoppingCart, cnt - 1);
            }
            else
            {
                cart.Count -= 1;
                cart.Price = Sd.GetPriceBasedOnQuantity(cart.Count, cart.Product.Price,
                                    cart.Product.Price50, cart.Product.Price100);
               unitOfWork.save();
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Remove(int cartId)
        {
            var cart = unitOfWork.shoppingCart.GetFirstOrDefault
                            (c => c.Id == cartId, includeproprites: "Product");

            var cnt = unitOfWork.shoppingCart.GetAll(u => u.ApplicationUserId == cart.ApplicationUserId).ToList().Count;
           unitOfWork.shoppingCart.remove(cart);
            unitOfWork.save();
            HttpContext.Session.SetInt32(Sd.ssShoppingCart, cnt - 1);


            return RedirectToAction(nameof(Index));
        }
        public IActionResult summary()
        {
            var userclaims = (ClaimsIdentity)User.Identity;
            var claim = userclaims.FindFirst(ClaimTypes.NameIdentifier);
            ShoppingCartVM shoppingCartVM = new ShoppingCartVM
            {
                shoppingCarts = unitOfWork.shoppingCart.GetAll(x => x.ApplicationUserId == claim.Value, includeproprites: "Product").ToList(),
                orderHeader = new OrderHeader()
            };
            shoppingCartVM.orderHeader.ApplicationUser = unitOfWork.ApplicationUser.GetFirstOrDefault(x => x.Id == claim.Value, includeproprites: "Company");
            foreach (var list in shoppingCartVM.shoppingCarts)
            {
                list.Price = Sd.GetPriceBasedOnQuantity(list.Count, list.Price, list.Product.Price50, list.Product.Price100);
                shoppingCartVM.orderHeader.OrderTotal += list.Product.Price * list.Count;              
            }
            shoppingCartVM.orderHeader.Name = shoppingCartVM.orderHeader.ApplicationUser.Name;
            shoppingCartVM.orderHeader.PhoneNumber = shoppingCartVM.orderHeader.ApplicationUser.PhoneNumber;
            shoppingCartVM.orderHeader.StreetAddress = shoppingCartVM.orderHeader.ApplicationUser.StreetAdress;
            shoppingCartVM.orderHeader.City = shoppingCartVM.orderHeader.ApplicationUser.City;
            shoppingCartVM.orderHeader.State = shoppingCartVM.orderHeader.ApplicationUser.State;
            shoppingCartVM.orderHeader.PostalCode = shoppingCartVM.orderHeader.ApplicationUser.PostalCode;

            return View(shoppingCartVM);
        }
    }
}
