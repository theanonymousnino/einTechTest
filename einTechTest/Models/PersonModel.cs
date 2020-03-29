using System;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace einTechTest.Models
{
    public class PersonModel
    {
        public int ID { get; set; }

        public string Name { get; set; }

        [Display(Name = "DateAdded")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateAdded { get; set; }

        public GroupModel Group { get; set; }

        public bool Update(PersonModel person)
        {
            this.ID = person.ID;
            this.Name = person.Name;
            this.DateAdded = person.DateAdded;
            this.Group = person.Group != null ? new GroupModel(person.Group) : new GroupModel();

            return true;
        }

        public PersonModel()
        {
            //
        }

        public PersonModel(PersonModel person)
        {
            Update(person);
        }

        public PersonModel(DataRow row)
        {
            this.ID = Convert.ToInt32(row["PersonID"]);
            this.Name = row["PersonName"].ToString();
            this.DateAdded = Convert.ToDateTime(row["DateAdded"]);
            this.Group = new GroupModel(row);
        }
    }
}