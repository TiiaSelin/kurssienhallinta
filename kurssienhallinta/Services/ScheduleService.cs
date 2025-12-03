using kurssienhallinta.Models;
using kurssienhallinta.Models.ViewModels;

public class ScheduleService
{
    private static (DateTime weekStart, DateTime weekEnd) CalculateWeekRange(int offset)
    {
        var today = DateTime.Now;
        int daysOfWeek = (int)today.DayOfWeek;
        var mondayThisWeek = today.AddDays(-(daysOfWeek == 0 ? 6 : daysOfWeek - 1)).Date;
        var weekStart = mondayThisWeek.AddDays(offset * 7);
        return (weekStart, weekStart.AddDays(6));
    }
    private static string ConvertWeekday(DayOfWeek day)
    {
        switch (day)
        {
            case DayOfWeek.Monday:
                return "Maanantai";
            case DayOfWeek.Tuesday:
                return "Tiistai";
            case DayOfWeek.Wednesday:
                return "Keskiviikko";
            case DayOfWeek.Thursday:
                return "Torstai";
            case DayOfWeek.Friday:
                return "Perjantai";
            default:
                return "";
        }
    }
    private ScheduleItem ConvertScheduleItem(CourseSession session)
    {
        return new ScheduleItem
        {
            Id = session.CourseId,
            Name = session.Course.Name,
            Description = session.Course.Description,
            Start_time = session.Time_of_start,
            End_time = session.Time_of_end,
            Weekday = ConvertWeekday(session.WeekDay),
            Room = session.Course.Room,
            TeacherName = session.Course.Teacher?.FullName
        };
    }
    public RoomScheduleViewModel BuildRoomSchedule(Room room, int weekOffset)
    {
        var (weekStart, weekEnd) = CalculateWeekRange(weekOffset);

        var sessions = room.Courses
            .SelectMany(course => course.Sessions)
            .ToList();

        var scheduleItems = sessions.Select(ConvertScheduleItem).ToList();

        var weeklySchedule = scheduleItems
            .GroupBy(si => si.Weekday)
            .ToDictionary(g => g.Key, g => g.OrderBy(si => si.Start_time).ToList());

        return new RoomScheduleViewModel
        {
            Room = room,
            WeeklySchedule = weeklySchedule,
            WeekStart = weekStart,
            WeekEnd = weekEnd,
            WeekOffset = weekOffset
        };
    }


    // ====== Opiskelijoiden viikkonäkymä ======
    public StudentScheduleViewModel BuildStudentSchedule(Student student, int weekOffset)
    {
        var (weekStart, weekEnd) = CalculateWeekRange(weekOffset);

        var sessions = student.Enrollments
            .SelectMany(e => e.Course.Sessions)
            .ToList();

        var scheduleItems = sessions.Select(ConvertScheduleItem).ToList();

        var weeklySchedule = scheduleItems
            .GroupBy(si => si.Weekday)
            .ToDictionary(g => g.Key, g => g.OrderBy(si => si.Start_time).ToList());

        return new StudentScheduleViewModel
        {
            Student = student,
            WeeklySchedule = weeklySchedule,
            WeekStart = weekStart,
            WeekEnd = weekEnd,
            WeekOffset = weekOffset
        };
    }

}