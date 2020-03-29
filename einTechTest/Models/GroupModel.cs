using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace einTechTest.Models
{
    public class GroupModel
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public GroupModel()
        {
            //
        }

        public GroupModel(DataRow row)
        {
            this.ID = Convert.ToInt32(row["GroupID"]);
            this.Name = row["GroupName"].ToString();
        }

        public GroupModel(GroupModel group)
        {
            this.ID = group.ID;
            this.Name = group.Name;
        }
    }
}