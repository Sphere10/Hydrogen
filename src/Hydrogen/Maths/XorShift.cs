namespace Sphere10.Framework {

	public static class XorShift {

        public static uint Next(ref uint aState) {
            aState = aState ^ (aState << 13);
            aState = aState ^ (aState >> 17);
            aState = aState ^ (aState << 5);
            return aState;
        }
    }

}
