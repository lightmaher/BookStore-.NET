using BookStore.DataAccess.Migrations;
using BookStore.Models;
using BookStore.Repository.IRepository;
using BookStore.Utility;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Sd.Role_Admin)]

    public class CoverTypeController : Controller
    {

        private readonly IUnitOfWork iUnitOfWork;

        public CoverTypeController( IUnitOfWork iUnitOfWork)
        {
            this.iUnitOfWork = iUnitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Upsert(int? id)
        {

            CoverType coverType = new CoverType();
            if (id == null)
            {
                //this is for create
                return View(coverType);
            }
            //this is for edit
            var parameter = new DynamicParameters();
            parameter.Add("@Id", id);
            coverType = iUnitOfWork.SP_Call.OneRecord<CoverType>(Sd.Proc_CoverType_Get, parameter);
            if (coverType == null)
            {
                return NotFound();
            }
            return View(coverType);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(CoverType coverType)
        {
            if (ModelState.IsValid)
            {
                var parameter = new DynamicParameters();
                parameter.Add("@Name", coverType.Name);

                if (coverType.Id == 0)
                {
                    iUnitOfWork.SP_Call.Execute(Sd.Proc_CoverType_Create, parameter);

                }
                else
                {
                    parameter.Add("@Id", coverType.Id);
                    iUnitOfWork.SP_Call.Execute(Sd.Proc_CoverType_Update, parameter);
                }
                iUnitOfWork.save();
                return RedirectToAction(nameof(Index));
            }
            return View(coverType);
        }


        #region Api Calls
        [HttpGet]
        public IActionResult GetAll()
        {
            var allObj = iUnitOfWork.SP_Call.List<CoverType>(Sd.Proc_CoverType_GetAll, null);
            return Json(new { data = allObj });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var parameter = new DynamicParameters();
            parameter.Add("@Id", id);
            var objFromDb = iUnitOfWork.SP_Call.OneRecord<CoverType>(Sd.Proc_CoverType_Get, parameter);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            iUnitOfWork.SP_Call.Execute(Sd.Proc_CoverType_Delete, parameter);
            iUnitOfWork.save();
            return Json(new { success = true, message = "Delete Successful" });

        }

        #endregion
    }
}
