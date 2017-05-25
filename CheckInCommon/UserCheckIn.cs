using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CheckInCommon
{
    public class UserCheckIn
    {
        
        [Key]
        public long ID { get; set; }
        
        [Required(ErrorMessage = "First name is required.")]
        [StringLength(20)]
        [RegularExpression(@"^[a-zA-Z]+[a-zA-Z-]+[a-zA-Z]$", ErrorMessage = "Enter a valid name.")]
        public string firstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(20)]
        [RegularExpression(@"^[a-zA-Z]+[a-zA-Z-]+[a-zA-Z]$", ErrorMessage = "Enter a valid name.")]
        public string lastName { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?[\d]{3}\)?[\s-]?[\d]{3}[\s-]?[\d]{4}$", ErrorMessage = "Invalid format. Please enter valid phone number.")]
        public string telNum { get; set; }

        
        [Required(ErrorMessage = "Email address is required.")]
        [DataType(DataType.EmailAddress)]
        public string email { get; set; }

        [Required(ErrorMessage = "Contact Email address is required.")]
        [DataType(DataType.EmailAddress)]
        public string contactEmail1 { get; set; }

        [DisplayFormat(NullDisplayText = "None")]
        [DataType(DataType.EmailAddress)]
        public string contactEmail2 { get; set; }

        [DisplayFormat(NullDisplayText = "None")]
        [DataType(DataType.EmailAddress)]
        public string contactEmail3 { get; set; }

        [DisplayFormat(NullDisplayText = "None")]
        [DataType(DataType.EmailAddress)]
        public string contactEmail4 { get; set; }

        [Required(ErrorMessage = "Location is required.")]
        [StringLength(20)]
        public string location { get; set; }

        [Required(ErrorMessage = "Return time is required.")]
        [DataType(DataType.Time)]
        public DateTime returnTime { get; set; }

        [DataType(DataType.MultilineText)]
        public string message { get; set; }

        public Boolean subscribe { get; set; }
       
        public string secString { get; set; }
    }
}