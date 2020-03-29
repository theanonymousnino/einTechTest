using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using einTechTest.Models;
using einTechTest.Controllers.Core;

namespace einTechTest.Controllers
{
    public class HomeController : Controller
    {
        private SystemController data = SystemController.Instance;

        public ActionResult Index()
        {
            ViewBag.Groups = data.Groups;
            return View(data.Persons);
        }

        [HttpPost]
        public ActionResult Create(PersonModel personModel)
        {
            if (personModel.Group == null)
                return Json(null);
            personModel.DateAdded = DateTime.Now;
            personModel.ID = data.DataLayerController.AddPerson(personModel);
            data.Persons.Add(personModel);
            return Json(personModel);
        }

        [HttpPost]
        public ActionResult Edit(PersonModel personModel)
        {
            PersonModel tpersonModel = data.Persons.Find(x => x.ID == personModel.ID);
            if ((personModel.Group == null) || (tpersonModel == null))
                return Json(null);

            int error = tpersonModel != null ? data.DataLayerController.EditPerson(personModel) : 0;
            if (error == 1)
            {
                tpersonModel.Update(personModel);
            }
            return Json(tpersonModel);
        }

        [HttpPost]
        public ActionResult Delete(PersonModel dpersonModel)
        {
            PersonModel personModel = data.Persons.Find(x => x.ID == dpersonModel.ID);
            if (personModel == null)
                return Json(null);

            int error = personModel != null ? data.DataLayerController.DeletePerson(personModel.ID) : 0;
            if (error == 1)
            {
                data.Persons.Remove(personModel);
            }
            return Json(personModel.ID);
        }
    }
}