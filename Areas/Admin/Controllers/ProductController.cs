using BookStore.DataAccess.Migrations;
using BookStore.Models;
using BookStore.Models.ViewModels;
using BookStore.Repository.IRepository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class ProductController : Controller
    {

        private readonly IUnitOfWork iUnitOfWork;

        public IWebHostEnvironment HostEnvironment { get; }

        public ProductController( IUnitOfWork iUnitOfWork , IWebHostEnvironment _hostEnvironment)
        {
            this.iUnitOfWork = iUnitOfWork;
            HostEnvironment = _hostEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Upsert(int? id)
        {
            ProductVM productVM = new ProductVM()
            {
                product = new Product(),
                CategoryList = iUnitOfWork.Category.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
                CoverList = iUnitOfWork.coverType.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                })

            };
            if (id == null)
            {
                // this for create 
                return View(productVM);
            }
            productVM.product = iUnitOfWork.product.GetT(id.GetValueOrDefault());
            if (productVM.product == null)
            {
                return NotFound();
            }
            return View(productVM);
        }
        //[HttpPost]
        //public IActionResult Upsert( Product product)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        if (product.Id == 0)
        //        {
        //            iUnitOfWork.product.add(product);
        //            iUnitOfWork.save();
        //        } else
        //        {
        //            iUnitOfWork.product.Update(product);
        //            iUnitOfWork.save();
        //        }

        //        return RedirectToAction(nameof(Index));


        //    }
        //    return NotFound();
        //}


        #region Api Calls
        [HttpGet]
        public IActionResult GetAll()
        {
            var AllObj = iUnitOfWork.product.GetAll();
            return Ok(new { data = AllObj });
        }
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var objfromdb = iUnitOfWork.product.GetT(id);
            if (objfromdb == null)
            {
                return Json(new { success = "false", message = "Error While Deleting" });
            }
            iUnitOfWork.product.remove(objfromdb);
            iUnitOfWork.save();
            return Json(new { success = "true", message = "Deleted successfully" });
        }
        #endregion
    }
}
