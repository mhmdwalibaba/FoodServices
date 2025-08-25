
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FoodServices.Model
{
	[Table("Language", Schema = "Base")]
	public class Language
	{
		public Language()
		{
		}
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Display(Name = "شناسه زبان")]
		[Required(ErrorMessage = " شناسه Language را وارد نمائيد ")]
		public string LanguageID { get; set; }
		[Display(Name = "عنوان زبان")]
		//[Required (ErrorMessage =" عنوان زبان را وارد نمائيد ")]
		public string? LanguageName { get; set; }

		[Display(Name = "توضيحات")]
		//[Required (ErrorMessage =" توضيحت را وارد نمائيد ")]
		public string? LanguageDesc { get; set; }

		[Display(Name = "سيمبول")]
		//[Required (ErrorMessage =" سيمبول را وارد نمائيد ")]
		public string? LanguageSimbol { get; set; }

		[Display(Name = "جهت")]
		//[Required (ErrorMessage =" جهت را وارد نمائيد ")]
		public string? LanguageDirection { get; set; }

		[Display(Name = "عکس")]
		//[Required (ErrorMessage =" عکس را وارد نمائيد ")]
		public string? LanguagePhoto { get; set; }

		[Display(Name = "ايجاد کننده")]
		//[Required (ErrorMessage =" ايجاد کننده را وارد نمائيد ")]
		public string? Creator { get; set; }

		[Display(Name = "تاريخ ايجاد")]
		//[Required (ErrorMessage =" تاريخ ايجاد را وارد نمائيد ")]
		//[BindNever]
		public string? Ctime { get; set; }

		[Display(Name = "ويرايش کننده")]
		//[Required (ErrorMessage =" ويرايش کننده را وارد نمائيد ")]
		public string? Modifier { get; set; }

		[Display(Name = "تاريخ ويرايش")]
		//[Required (ErrorMessage =" تاريخ ويرايش را وارد نمائيد ")]
		//[BindNever]
		public string? Mtime { get; set; }

		[Display(Name = "وضعیت")]
		//[Required (ErrorMessage =" فعال؟ را وارد نمائيد ")]
		public int IsEnabled { get; set; }

		[Display(Name = "موجودیت")]
		//[Required (ErrorMessage =" حذف شده؟ را وارد نمائيد ")]
		public int IsDeleted { get; set; }


	}
}



