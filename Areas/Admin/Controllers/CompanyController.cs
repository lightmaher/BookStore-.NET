using BookStore.DataAccess.Migrations;
using BookStore.Models;
using BookStore.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class CompanyController : Controller
    {

        private readonly IUnitOfWork iUnitOfWork;

        public CompanyController( IUnitOfWork iUnitOfWork)
        {
            this.iUnitOfWork = iUnitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Upsert(int? id)
        {
            Company company = new Company();
            if (id == null)
            {
                // this for create 
                return View(company);
            }
            company = iUnitOfWork.company.GetT(id.GetValueOrDefault());
            if (company == null)
            {
                return NotFound();
            }
            return View(company);
        }
        [HttpPost]
        public IActionResult Upsert( Company company)
        {
            if (ModelState.IsValid)
            {
                if (company.Id == 0)
                {
                    iUnitOfWork.company.add(company);
                    iUnitOfWork.save();
                } else
                {
                    iUnitOfWork.company.Update(company);
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
            var AllObj = iUnitOfWork.company.GetAll();
            return Ok(new { data = AllObj });
        }
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var objfromdb = iUnitOfWork.company.GetT(id);
            if (objfromdb == null)
            {
                return Json(new { success = "false", message = "Error While Deleting" });
            }
            iUnitOfWork.company.remove(objfromdb);
            iUnitOfWork.save();
            return Json(new { success = "true", message = "Deleted successfully" });
        }
        #endregion
    }
}
