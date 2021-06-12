using BookStore.DataAccess.Migrations;
using BookStore.Models;
using BookStore.Repository.IRepository;
using BookStore.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize (Roles = Sd.Role_Admin)]

    public class CategoryController : Controller
    {

        private readonly IUnitOfWork iUnitOfWork;

        public CategoryController( IUnitOfWork iUnitOfWork)
        {
            this.iUnitOfWork = iUnitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Upsert(int? id)
        {
            Category category = new Category();
            if (id == null)
            {
                // this for create 
                return View(category);
            }
            category = iUnitOfWork.Category.GetT(id.GetValueOrDefault());
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }
        [HttpPost]
        public IActionResult Upsert( Category category)
        {
            if (ModelState.IsValid)
            {
                if (category.Id == 0)
                {
                    iUnitOfWork.Category.add(category);
                    iUnitOfWork.save();
                } else
                {
                    iUnitOfWork.Category.Update(category);
                    iUnitOfWork.save();
                }

                return RedirectToAction(nameof(Index));


            }
            return NotFound();
        }


        #region Api Calls
        [HttpGet]
        public IActionResult GetAll()
        {
            var AllObj = iUnitOfWork.Category.GetAll();
            return Ok(new { data = AllObj });
        }
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var objfromdb = iUnitOfWork.Category.GetT(id);
            if (objfromdb == null)
            {
                return Json(new { success = "false", message = "Error While Deleting" });
            }
            iUnitOfWork.Category.remove(objfromdb);
            iUnitOfWork.save();
            return Json(new { success = "true", message = "Deleted successfully" });
        }
        #endregion
    }
}
