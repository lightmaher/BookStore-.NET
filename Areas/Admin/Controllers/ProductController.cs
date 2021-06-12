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
using System.IO;
using BookStore.Utility;
using Microsoft.AspNetCore.Authorization;

namespace BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Sd.Role_Admin )]


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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM productVM)
        {
            if (ModelState.IsValid)
            {
                string webRootPath = HostEnvironment.WebRootPath;
                var files = Request.Form.Files;
                if (files.Count > 0)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(webRootPath, @"images\products");
                    var extenstion = Path.GetExtension(files[0].FileName);

                    if (productVM.product.ImageUrl != null)
                    {
                        //this is an edit and we need to remove old image
                        var imagePath = Path.Combine(webRootPath, productVM.product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }
                    using (var filesStreams = new FileStream(Path.Combine(uploads, fileName + extenstion), FileMode.Create))
                    {
                        files[0].CopyTo(filesStreams);
                    }
                    productVM.product.ImageUrl = @"\images\products\" + fileName + extenstion;
                }
                else
                {
                    //update when they do not change the image
                    if (productVM.product.Id != 0)
                    {
                        Product objFromDb = iUnitOfWork.product.GetT(productVM.product.Id);
                        productVM.product.ImageUrl = objFromDb.ImageUrl;
                    }
                }


                if (productVM.product.Id == 0)
                {
                    iUnitOfWork.product.add(productVM.product);

                }
                else
                {
                    iUnitOfWork.product.Update(productVM.product);
                }
                iUnitOfWork.save();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                productVM.CategoryList = iUnitOfWork.Category.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                });
                productVM.CoverList = iUnitOfWork.coverType.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                });
                if (productVM.product.Id != 0)
                {
                    productVM.product = iUnitOfWork.product.GetT(productVM.product.Id);
                }
            }
            return View(productVM);
        }



        #region Api Calls
        [HttpGet]
        public IActionResult GetAll()
        {
            var AllObj = iUnitOfWork.product.GetAll(includeproprites:"Category,CoverType");
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
