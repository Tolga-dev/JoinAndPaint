// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("EvWaziHoF8pVoYL3iGg9oqiGZp7leXS/f/m/iHtGpqw1MKSx4slckdgFfUd5k7I83WEtKhRVvVXEETC26/Qwnkbqprhg8XsMrGuweqBCjGjnU/RZyi6bFPiHQ7+lesyV02IjS53RWhq82dhQgVzrsczwQrdNGQuPBLGP1FJjy1u8nD84ksnxH2Lwij17QA+GldPVbVnY7CbNXgRhdYAfw2TIW4mDuAbMfqsUlRohksl2/H4bu1U3BmNsX6vMQIh4dioP6V3ViOf0RsXm9MnCze5CjEIzycXFxcHExwvpNUT4S8A/CaiReoul7L20ZuYMRsXLxPRGxc7GRsXFxAneAhauOnFaXHFfwd8tXERaa6bufSFrYTH4ktpOrbTm8r0BKcbHxcTF");
        private static int[] order = new int[] { 4,7,2,9,8,11,10,13,9,12,12,12,13,13,14 };
        private static int key = 196;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
