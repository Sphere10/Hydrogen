namespace Sphere10.Framework;

public class HashChecksum : IItemChecksum<byte[]> {

	public int Calculate(byte[] item) => LittleEndianBitConverter.Little.ToInt32(item, 0);
}
