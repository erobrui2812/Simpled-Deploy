namespace Simpled.Dtos.ActivityLogs
{
    /// <summary>
    /// DTO para lectura de registros de actividad sobre ítems.
    /// </summary>
    public class ActivityLogReadDto
    {
        /// <summary>
        /// Identificador único del registro de actividad.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Identificador del ítem al que se asocia la actividad.
        /// </summary>
        public Guid ItemId { get; set; }

        /// <summary>
        /// Identificador del usuario que realizó la acción.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Nombre del usuario que realizó la acción.
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// URL del avatar del usuario (si existe).
        /// </summary>
        public string? UserAvatarUrl { get; set; }

        /// <summary>
        /// Tipo de acción registrada.
        /// </summary>
        public ActivityType Type { get; set; }

        /// <summary>
        /// Campo modificado, si aplica.
        /// </summary>
        public string? Field { get; set; }

        /// <summary>
        /// Valor anterior del campo, si corresponde.
        /// </summary>
        public string? OldValue { get; set; }

        /// <summary>
        /// Nuevo valor del campo, si corresponde.
        /// </summary>
        public string? NewValue { get; set; }

        /// <summary>
        /// Detalles adicionales de la actividad.
        /// </summary>
        public string Details { get; set; } = string.Empty;

        /// <summary>
        /// Marca de tiempo en que se registró la actividad (UTC).
        /// </summary>
        public DateTime Timestamp { get; set; }
    }

    /// <summary>
    /// Tipos de acciones que pueden registrarse en el historial de actividad.
    /// </summary>
    public enum ActivityType
    {
        /// <summary>Se creó el ítem.</summary>
        Created,
        /// <summary>Se actualizó el ítem.</summary>
        Updated,
        /// <summary>Cambio de estado del ítem.</summary>
        StatusChanged,
        /// <summary>Asignación de usuario al ítem.</summary>
        Assigned,
        /// <summary>Cambio en fechas del ítem.</summary>
        DateChanged,
        /// <summary>El ítem fue eliminado.</summary>
        Deleted,
        /// <summary>Se subió un archivo al ítem.</summary>
        FileUploaded,
        /// <summary>Se creó una subtarea.</summary>
        SubtaskCreated,
        /// <summary>Se actualizó una subtarea.</summary>
        SubtaskUpdated,
        /// <summary>Se eliminó una subtarea.</summary>
        SubtaskDeleted,
        /// <summary>Se añadió un comentario.</summary>
        CommentAdded,
        /// <summary>Se editó un comentario.</summary>
        CommentEdited,
        /// <summary>Se eliminó un comentario.</summary>
        CommentDeleted,
        /// <summary>Se resolvió un comentario.</summary>
        CommentResolved
    }
}
