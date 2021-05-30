
#if (NANOFRAMEWORK_1_0)
namespace nanoFramework.OpenThread.Net.IPv6
{ 
#else
namespace dotNETCore.OpenThread.Net.IPv6
{
#endif
    public interface Icmpv6Message
    {
        /// <summary>
        /// Writes the layer to the byte array.
        /// </summary>
        /// <returns>byte array.</returns>
        byte[] ToBytes();

        /// <summary>
        ///  Builds a packet from byte array.
        /// </summary>
        /// <param name="buffer">The buffer to write the layer to.</param>
        /// <param name="offset">The offset in the buffer to start writing the layer at.</param>
        /// <returns>ILayer.</returns>
        bool FromBytes(byte[] buffer, ref int offset);

    }
}

