using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using einTechTest.Models;

namespace einTechTest.Controllers.Core
{
    public class SystemController
    {
        internal DataLayerController DataLayerController { get; set; }
        internal List<PersonModel> Persons { get; set; }
        internal List<GroupModel> Groups { get; set; }

        private static SystemController instance;

        private SystemController()
        {
            DataLayerController = new DataLayerController();
            Persons = new List<PersonModel>();
            Groups = new List<GroupModel>();
        }

        public static SystemController Instance
        {
            get
            {
                if (instance == null) instance = new SystemController();
                return instance;
            }
        }

        public void LoadData()
        {
            Persons = DataLayerController.GetPersons();
            Groups = DataLayerController.GetGroups();
        }
    }
}