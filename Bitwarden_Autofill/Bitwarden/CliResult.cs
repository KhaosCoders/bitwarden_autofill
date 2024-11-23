namespace Bitwarden_Autofill.Bitwarden;

/// <summary>
/// Represents the result of a CLI command execution.
/// </summary>
/// <param name="Success">Indicates whether the command executed successfully.</param>
/// <param name="Output">The standard output from the command execution.</param>
/// <param name="Error">The standard error output from the command execution.</param>
/// <param name="ExitCode">The exit code returned by the command execution.</param>
public record CliResult(bool Success, string Output, string Error, int ExitCode);
