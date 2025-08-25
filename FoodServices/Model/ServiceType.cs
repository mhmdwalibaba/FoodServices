
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using FoodServices.Model;
using MRKHServices.Persistence.Entites;

namespace FoodServices.Model
{
    [Table("ServiceType", Schema = "Info")]
    public class ServiceType
    {
        public ServiceType()
        {
            Services = new HashSet<Service>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "شناسه ServiceType")]
        [Required(ErrorMessage = " شناسه ServiceType را وارد نمائيد ")]
        public int ServiceTypeID { get; set; }
        [Display(Name = "زبان")]
        public string ServiceTypeLanguageID { get; set; }

        [Display(Name = "عنوان نوع سرويس")]
        public string ServiceTypeName { get; set; }

        [Display(Name = "توضيحات")]
        public string? ServiceTypeDesc { get; set; }

        [Display(Name = "عکس")]
        public string? ServiceTypeImg { get; set; }

        [Display(Name = "آيکون")]
        public string? ServiceTypeIcon { get; set; }

        [Display(Name = "آدرس سرويس")]
        public string? ServiceTypeAddress { get; set; }

        [Display(Name = "پارامترهاي سرويس")]
        public string? ServiceTypeParams { get; set; }

        [Display(Name = "تعداد پذير؟")]
        public int? ServiceTypeHasCount { get; set; }

        [Display(Name = "تاريخ پذير؟")]
        public string? ServiceTypeHasDate { get; set; }

        [Display(Name = "ساعت پذير؟")]
        public string? ServiceTypeHasTime { get; set; }

        [Display(Name = "ايجاد کننده")]
        public string? Creator { get; set; }

        [Display(Name = "تاريخ ايجاد")]
        public string? Ctime { get; set; }

        [Display(Name = "ويرايش کننده")]
        public string? Modifier { get; set; }

        [Display(Name = "تاريخ ويرايش")]
        public string? Mtime { get; set; }

        [Display(Name = "فعال؟")]
        public int IsEnabled { get; set; }

        [Display(Name = "حذف شده؟")]
        public int IsDeleted { get; set; }

        [ForeignKey("ServiceTypeLanguageID")]
        [Display(Name = "ServiceTypeLanguage")]
        public virtual Language? ServiceTypeLanguage { get; set; }

        public virtual ICollection<Service> Services { get; set; }
    }
}


