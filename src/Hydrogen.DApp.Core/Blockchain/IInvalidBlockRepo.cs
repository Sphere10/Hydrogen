namespace Sphere10.Hydrogen.Core.Blockchain {
	public interface IInvalidBlockRepo {

		bool IsInvalid(byte[] blockHash);

		void InvalidateBlock(byte[] blockHash);

		void RevokeInvalidation(byte[] blockHash);

	}
}
