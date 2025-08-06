using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VuexyBase.Domain.Entities.General
{
    public class City : BaseEntity
    {
        public City()
        {
            // Add any collections here if needed in the future
        }

        public int RegionId { get; set; }

        // Navigation Properties :

        [ForeignKey(nameof(RegionId))]
        public virtual Region Region { get; set; }
    }
}