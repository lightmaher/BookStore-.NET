using BookStore.Models;
using BookStore.Models.Models;
using BookStore.Models.ViewModels;
using BookStore.Repository.IRepository;
using BookStore.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BookStore.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork unitOfWork;

        public HomeController(ILogger<HomeController> logger , IUnitOfWork unitOfWork)
        {
            _logger = logger;
            this.unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var products = unitOfWork.product.GetAll(includeproprites: "Category,CoverType");
            var claimsidentity = (ClaimsIdentity)User.Identity;
            var claim = claimsidentity.FindFirst(ClaimTypes.NameIdentifier);
          if (claim != null)
            {
                var count = unitOfWork.shoppingCart.GetAll(x => x.ApplicationUserId == claim.Value).ToList().Count();
                HttpContext.Session.SetInt32(Sd.ssShoppingCart , count);
            }
         
            return View(products);
        }
        public IActionResult Details(int id)
        {
            var product = unitOfWork.product.GetFirstOrDefault(x=> x.Id == id, includeproprites: "Category,CoverType");
            var shoppingCart = new ShoppingCart()
            {
                Product = product,
                ProductId = product.Id
        };
            return View(shoppingCart);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Details(ShoppingCart cartObject)
        {
            cartObject.Id = 0;
            if (ModelState.IsValid)
            {
                var claimsidentity = (ClaimsIdentity)User.Identity;
                var claim = claimsidentity.FindFirst(ClaimTypes.NameIdentifier);
                cartObject.ApplicationUserId = claim.Value;
                ShoppingCart cartfromdb = unitOfWork.shoppingCart.GetFirstOrDefault(
                    u => u.ApplicationUserId == cartObject.ApplicationUserId && u.ProductId == cartObject.ProductId
                    );
                if (cartfromdb == null)
                {
                    unitOfWork.shoppingCart.add(cartObject);
                } else
                {
                    cartfromdb.Count += cartObject.Count;
                }
                unitOfWork.save();
                var count = unitOfWork.shoppingCart.GetAll(c => c.ApplicationUserId == cartObject.ApplicationUserId).ToList().Count;
             //  HttpContext.Session.SetObject(Sd.ssShoppingCart, count);
                HttpContext.Session.SetInt32(Sd.ssShoppingCart, count);
                return RedirectToAction("Index");
            } 
            else
            {
                var product = unitOfWork.product.GetFirstOrDefault(includeproprites: "Category,CoverType");
                var shoppingCart = new ShoppingCart()
                {
                    Product = product,
                    ProductId = product.Id
                };
                return View(shoppingCart);
            }
        
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
