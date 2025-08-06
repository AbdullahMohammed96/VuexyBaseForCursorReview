using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VuexyBase.Domain.Entities.General
{
    public class Region : BaseEntity
    {
        public Region()
        {
            Cities = new HashSet<City>();
        }

        public string Image { get; set; }

        // Navigation Properties :

        [InverseProperty(nameof(City.Region))]
        public virtual ICollection<City> Cities { get; set; }
    }
}