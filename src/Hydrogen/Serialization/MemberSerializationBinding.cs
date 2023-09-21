using Hydrogen.Mapping;

namespace Hydrogen;

internal record MemberSerializationBinding (Member Member, IAutoSizedSerializer Serializer);