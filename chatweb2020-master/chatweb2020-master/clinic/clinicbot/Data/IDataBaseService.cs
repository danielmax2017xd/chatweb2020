using clinicbot.Common.Models.MedicalAppointment;
using clinicbot.Common.Models.Qualificacion;
using clinicbot.Common.Models.User;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace clinicbot.Data
{
    public interface IDataBaseService
    {
        DbSet<UserModel> User { get; set; }
        DbSet<QualificationModel> Qualification { get; set; }
        DbSet<MedicalAppointmentModel> MedicalAppointment { get; set; }
        Task<bool> SaveAsync();
    }
}
