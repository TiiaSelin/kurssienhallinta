namespace kurssienhallinta.Models.ViewModels
{
    public class ModifyTeacherViewModel
    {
        public Opettaja Opettaja { get; set; } = new();
        public List<Opettaja> Opettajat { get; set; } = new();
    }

}

