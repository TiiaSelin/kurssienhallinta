namespace kurssienhallinta.Models.ViewModels
{
    public class AddTeacherViewModel
    {
        public Opettaja Opettaja { get; set; } = new();
        public List<Opettaja> Opettajat { get; set; } = new();
    }

}

