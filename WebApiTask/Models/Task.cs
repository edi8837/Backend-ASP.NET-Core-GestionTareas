namespace WebApiTask.Models
{
    public class Task
    {
        public int Id { get; set; }

        public required string Name { get; set; }  // Nombre de la tarea
        public required string Description { get; set; }  // Descripción de la tarea
        public bool IsCompleted { get; set; }  // Indica si la tarea está completada
        public bool IsDeleted { get; set; }    // Indica si la tarea está eliminada lógicamente
        public string StartDate { get; set; }  // Fecha de inicio de la tarea como string
        public string EndDate { get; set; }    // Fecha de fin de la tarea como string
    }
}
