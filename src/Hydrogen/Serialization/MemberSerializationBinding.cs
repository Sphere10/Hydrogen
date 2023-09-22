using Hydrogen.Mapping;

namespace Hydrogen;

public record MemberSerializationBinding (Member Member, IItemSerializer Serializer);