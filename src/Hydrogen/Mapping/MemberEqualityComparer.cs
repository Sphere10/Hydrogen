using System.Collections.Generic;

namespace Hydrogen.Mapping;

public class MemberEqualityComparer : IEqualityComparer<Member>
{
	public bool Equals(Member x, Member y)
	{
		return x.MemberInfo.MetadataToken.Equals(y.MemberInfo.MetadataToken) && x.MemberInfo.Module.Equals(y.MemberInfo.Module);
	}

	public int GetHashCode(Member obj)
	{
		return obj.MemberInfo.MetadataToken.GetHashCode() & obj.MemberInfo.Module.GetHashCode();
	}
}
