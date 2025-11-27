namespace kurssienhallinta.Models.ViewModels
{
    public class ScheduleItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Weekday { get; set; }
        public DateTime Day_of_start { get; set; }
        public DateTime Day_of_end { get; set; }
        public int? TeacherId { get; set; }
        public int? RoomId { get; set; }
        public Room? Room { get; set; }
        public TimeSpan Start_time { get; set; }
        public TimeSpan End_time { get; set; }

        
    }
    public class TeacherScheduleViewModel
    {
        public Teacher Teacher { get; set; }
        public Dictionary<string, List<ScheduleItem>> WeeklySchedule { get; set; } // Melkein sama kuin ICollection mutta helpompi tulostaa kamaa for each-lauseella
    }

    public class StudentScheduleViewModel
    {
        public Student Student { get; set; }
        public Dictionary<string, List<ScheduleItem>> WeeklySchedule { get; set; }
    }

    public class RoomScheduleViewModel
    {
        public Room Room { get; set; }
        public Dictionary<string, List<ScheduleItem>> WeeklySchedule { get; set; }
    }
}