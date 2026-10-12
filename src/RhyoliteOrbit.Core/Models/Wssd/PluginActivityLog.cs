using System;
using System.ComponentModel.DataAnnotations;

namespace RhyoliteOrbit.Models.Wssd;

public class PluginActivityLog: Abp.Domain.Entities.Auditing.FullAuditedEntity<Guid>, Abp.Domain.Entities.IMustHaveTenant
{
    
    public int TenantId { get; set; }

        public Guid PluginId { get; set; }

        /// <summary>
        /// Version or Git SHA of the DSL script/rules payload executed
        /// </summary>
        [MaxLength(32)]
        public string PluginVersion { get; set; }

        /// <summary>
        /// Category of activity: e.g. "DSL_COMPILE", "DSL_EXECUTE", "INSTALL", "CONFIG_CHANGE"
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string ActivityType { get; set; }

        /// <summary>
        /// Severity Level: "Info", "Warning", "Error", "Debug", "Critical"
        /// </summary>

        [Required]
        [MaxLength(20)]
        public string Level { get; set; }

        [Required]
        [MaxLength(500)]
        public string Message { get; set; }

        /// <summary>
        /// Correlates all logs generated during a single DSL engine evaluation session/request
        /// </summary>
        [MaxLength(100)]
        public string CorrelationId { get; set; }

        /// <summary>
        /// Specific DSL Rule, AST Node ID, or Step Name that generated the log
        /// </summary>
        [MaxLength(100)]
        public string StepOrRuleName { get; set; }

        /// <summary>
        /// Component or Engine Module: e.g., "SapoDslParser", "SapoRuntimeEngine", "PluginHost"
        /// </summary>

        [MaxLength(100)]
        public string Source { get; set; }

        /// <summary>
        /// JSON-formatted runtime variables, input context, or DSL evaluation outcome
        /// </summary>

        public string Details { get; set; } // Avoid MaxLength constraint (uses nvarchar(max) or text)

        /// <summary>
        /// Exception details or DSL evaluation stack trace
        /// </summary>

        public string StackTrace { get; set; } // Increased capacity for complex AST/exception traces

        /// <summary>
        /// Execution duration in milliseconds
        /// </summary>

        public int ExecutionTimeMs { get; set; }
}