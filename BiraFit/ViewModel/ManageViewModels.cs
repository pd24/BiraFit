﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace BiraFit.Models
{
    public class IndexViewModel
    {
        public string Vorname { get; set; }
        public string Nachname { get; set; }
        public string Email { get; set; }
        public string ProfilBild { get; set; }
    }

    public class SetPasswordViewModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "\"{0}\" muss mindestens {2} Zeichen lang sein.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Neues Kennwort")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Neues Kennwort bestätigen")]
        [Compare("NewPassword", ErrorMessage = "Das neue Kennwort stimmt nicht mit dem Bestätigungskennwort überein.")]
        public string ConfirmPassword { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Aktuelles Kennwort")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "\"{0}\" muss mindestens {2} Zeichen lang sein.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Neues Kennwort")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Neues Kennwort bestätigen")]
        [Compare("NewPassword", ErrorMessage = "Das neue Kennwort stimmt nicht mit dem Bestätigungskennwort überein.")]
        public string ConfirmPassword { get; set; }
    }

    public class EditViewModel
    {
        [Required]
        [StringLength(20, ErrorMessage = "\"{0}\" darf maximal {2} Zeichen lang sein.")]
        [DataType(DataType.Text)]
        [Display(Name = "Nachname")]
        public string Name { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "\"{0}\" darf maximal {2} Zeichen lang sein.")]
        [DataType(DataType.Text)]
        [Display(Name = "Vorname")]
        public string Vorname { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "E-Mail")]
        public string Email { get; set; }

        public string Adresse { get; set; }

        public string ProfilBild { get; set; }

        public string Beschreibung;
    }

    public class ShowViewModel
    {
        [Required]
        public string User_Id { get; set; }

        [Required]
        public string Picture_ID { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Vorname { get; set; }

        [Required]
        public string Adresse { get; set; }

        [Required]
        public string Mail { get; set; }

        [Required]
        public string Beschreibung { get; set; }

        [Required]
        public float Bewertung { get; set; }

        [Required]
        public int AnzahlBew { get; set; }

        public bool AreConnected { get; set; }
    }
}