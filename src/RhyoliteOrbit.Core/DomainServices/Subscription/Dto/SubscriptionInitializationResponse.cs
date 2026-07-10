namespace RhyoliteOrbit.DomainServices.Subscription.Dto;

/// <summary>
/// Standardized response returned by both <c>InitializeSubscriptionPayment</c>
/// and <c>InitializeRecurringSubscriptionPayment</c>.
/// </summary>
public class SubscriptionInitializationResponse
{
    /// <summary>
    /// Indicates how the payment was initiated so the client can render
    /// the appropriate next-step UI.
    /// <list type="bullet">
    ///   <item><c>WebCheckout</c>  – redirect the user to <see cref="CheckoutUrl"/>.</item>
    ///   <item><c>DirectDebit</c> – mandate already exists; charge was submitted immediately.
    ///         Listen for the callback result.</item>
    ///   <item><c>PreapprovalRequested</c> – user must approve the direct-debit mandate
    ///         (OTP / USSD prompt). Poll or wait for the preapproval callback.</item>
    /// </list>
    /// </summary>
    public string PaymentMode { get; init; } = string.Empty;

    /// <summary>Human-readable status string for logging / debugging.</summary>
    public string Status { get; init; } = string.Empty;

    /// <summary>A friendly message the client may display directly to the user.</summary>
    public string Message { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;

    // ── Web-checkout fields (PaymentMode = "WebCheckout") ───────────────────

    /// <summary>Hubtel-hosted checkout page; redirect user here for card/MoMo payment.</summary>
    public string CheckoutUrl { get; init; } = string.Empty;

    /// <summary>Client reference for the web checkout session.</summary>
    public string ClientReference { get; init; } = string.Empty;

    // ── Pre-approval fields (PaymentMode = "PreapprovalRequested") ───────────

    /// <summary>Hubtel pre-approval ID — required when verifying the OTP.</summary>
    public string HubtelPreApprovalId { get; init; } = string.Empty;

    /// <summary>Client reference returned by the pre-approval initiation.</summary>
    public string ClientReferenceId { get; init; } = string.Empty;

    /// <summary>OTP prefix shown to the user so they can identify the correct prompt.</summary>
    public string OtpPrefix { get; init; } = string.Empty;

    /// <summary>Pre-approval status as returned by Hubtel (e.g. "PENDING").</summary>
    public string PreapprovalStatus { get; init; } = string.Empty;
}
