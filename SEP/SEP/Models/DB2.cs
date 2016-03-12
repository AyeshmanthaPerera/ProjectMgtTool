using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using SEP.Models;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace SEP.Models
{
    public class DB2 : DbContext
    {
        public DB2() : base("DB2")
        {

        }

        public DbSet<RequestNew> RequestsNew { get; set; }
        public DbSet<NotificationNew> NotificationsNew { get; set; }

        public DbSet<Lecturer> Lecturers { get; set; }
        public DbSet<Student> Students { get; set; }

        public DbSet<StudentStatus> Stu_Statuses { get; set; }

        public DbSet<LectureStatus> Lec_Statuses { get; set; }

        public DbSet<Module> Modules { get; set; }

        public DbSet<Project> Projects { get; set; }

        public DbSet<Client> Clients { get; set; }

        public DbSet<StudentGroupeList> StudentGroupeLists { get; set; }

        public DbSet<LanguageProficiency> LanguageProficiencies { get; set; }
        public DbSet<Event> Event { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
        public System.Data.Entity.DbSet<SEP.Models.Group> Groups { get; set; }
        public System.Data.Entity.DbSet<SEP.Models.AllocatedLecturers> AllocatedLecturers { get; set; }

    }




}