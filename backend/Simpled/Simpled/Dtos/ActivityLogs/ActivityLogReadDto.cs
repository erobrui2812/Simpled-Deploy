using System;
using System.Text.Json.Serialization;

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
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ActivityType Type { get; set; }

        /// <summary>
        /// Nombre del campo modificado, si aplica.
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
        /// Nombre legible del valor anterior (en caso de IDs de usuario).
        /// </summary>
        public string? OldValueName { get; set; }

        /// <summary>
        /// Nombre legible del nuevo valor (en caso de IDs de usuario).
        /// </summary>
        public string? NewValueName { get; set; }

        /// <summary>
        /// Detalles adicionales de la actividad.
        /// </summary>
        public string Details { get; set; } = string.Empty;

        /// <summary>
        /// Marca de tiempo en que se registró la actividad (UTC con offset).
        /// </summary>
        public DateTimeOffset Timestamp { get; set; }
    }

    /// <summary>
    /// Tipos de acciones que pueden registrarse en el historial de actividad.
    /// </summary>
    public enum ActivityType
    {
        Created,
        Updated,
        StatusChanged,
        Assigned,
        DateChanged,
        Deleted,
        FileUploaded,
        SubtaskCreated,
        SubtaskUpdated,
        SubtaskDeleted,
        CommentAdded,
        CommentEdited,
        CommentDeleted,
        CommentResolved
    }
}
