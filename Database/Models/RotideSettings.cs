using System;
using System.ComponentModel.DataAnnotations;

namespace webZone.Database.Models
{
    public class RotideSettings
    {
        [Key]
        public int rotideSettingsId { get; set; }

        [MaxLength(255)]
        public string projectFolder { get; set; }
    }
}
