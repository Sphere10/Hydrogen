using System;
using System.Collections.Generic;
using System.Text;

namespace Hydrogen.Application;

public class ApplicationVersion : IComparable<ApplicationVersion>, IEquatable<ApplicationVersion>, IFormattable {

	public ApplicationVersion() {
	}


	public ApplicationVersion(int major, int minor, int build, int revision, ProductDistribution distribution) {
		Major = major;
		Minor = minor;
		Build = build;
		Revision = revision;
	}

	public int Major { get; init; } = 0;
	public int Minor { get; init; } = 0;
	public int Build { get; init; } = 0;
	public int Revision { get; init; } = 0;
	public ProductDistribution Distribution { get; set; } = ProductDistribution.ReleaseCandidate;


	public static bool TryParse(string versionString, out ApplicationVersion result) {
		result = null;

		if (string.IsNullOrWhiteSpace(versionString)) 
			return false;

		var parts = versionString.Split('.');
		if (!parts.Length.IsIn(1, 2, 4))
			return false;

		switch (parts.Length) {
			case 1:
				if (int.TryParse(parts[0], out var major)) {
					result = new ApplicationVersion(major, 0, 0, 0, ProductDistribution.ReleaseCandidate);
					return true;
				}
				break;

			case 2:
				if (int.TryParse(parts[0], out major) &&
					int.TryParse(parts[1], out var minor)) {
					result = new ApplicationVersion(major, minor, 0, 0, ProductDistribution.ReleaseCandidate);
					return true;
				}
				break;
			case 4:
				if (int.TryParse(parts[0], out major) &&
					int.TryParse(parts[1], out minor) &&
					int.TryParse(parts[2], out var build) &&
					int.TryParse(parts[3], out var revision)) {
					result = new ApplicationVersion(major, minor, build, revision, ProductDistribution.ReleaseCandidate);
					return true;
				}
				break;
			default:
				break;
		}
		return false;
	}

	public static ApplicationVersion Parse(string versionString) {
		if (!TryParse(versionString, out var result)) 
			throw new FormatException($"Unable to parse version string '{versionString}'");
		return result;
	}

	public int CompareTo(ApplicationVersion other) {
		if (other == null) 
			return 1;

		var majorComparison = Major.CompareTo(other.Major);
		if (majorComparison != 0) 
			return majorComparison;

		var minorComparison = Minor.CompareTo(other.Minor);
		if (minorComparison != 0) 
			return minorComparison;

		var buildComparison = Build.CompareTo(other.Build);
		if (buildComparison != 0) 
			return buildComparison;

		return Revision.CompareTo(other.Revision);
	}

	public bool Equals(ApplicationVersion other) {
		if (other == null)
			return false;
		return Major == other.Major && Minor == other.Minor && Build == other.Build && Revision == other.Revision;
	}

	public override bool Equals(object obj) 
		=> Equals(obj as ApplicationVersion);

	public override int GetHashCode() 
		=> HashCode.Combine(Major, Minor, Build, Revision);

	public override string ToString() 
		=> ToString("G", null);

	public string ToString(string format, IFormatProvider formatProvider) {
		if (string.IsNullOrEmpty(format))
			format = "G"; // Default format

		var distributionCode = Distribution switch {
			ProductDistribution.ReleaseCandidate => string.Empty,
			_ => $" ({Tools.Enums.GetSerializableOrientedName(Distribution)})"
		};

		switch (format.ToUpperInvariant()) {
			case "G": // General format - "Major.Minor.Build.Revision"
				return $"{Major}.{Minor}.{Build}.{Revision}";
			case "F": // Full version format - "Version Major.Minor.Build.Revision"
				return $"Version {Major}.{Minor}.{Build}.{Revision}";
			case "S": // Short format - "Major.Minor"
				return $"{Major}.{Minor}";
			case "D": // Detailed format - "Major.Minor (Build:Revision)"
				return $"Version {Major}.{Minor}.{Build}.{Revision}{distributionCode}";
			default:
				throw new FormatException($"The '{format}' format string is not supported.");
		}
	}

	public static bool operator ==(ApplicationVersion left, ApplicationVersion right) {
		if (ReferenceEquals(left, right)) 
			return true;

		if (left is null) 
			return false;

		return left.Equals(right);
	}

	public static bool operator !=(ApplicationVersion left, ApplicationVersion right) 
		=> !(left == right);

	public static bool operator <(ApplicationVersion left, ApplicationVersion right) 
		=> left is null ? right is not null : left.CompareTo(right) < 0;

	public static bool operator <=(ApplicationVersion left, ApplicationVersion right) 
		=> left is null || left.CompareTo(right) <= 0;

	public static bool operator >(ApplicationVersion left, ApplicationVersion right) 
		=> left is not null && left.CompareTo(right) > 0;

	public static bool operator >=(ApplicationVersion left, ApplicationVersion right) 
		=> left is null ? right is null : left.CompareTo(right) >= 0;
}
