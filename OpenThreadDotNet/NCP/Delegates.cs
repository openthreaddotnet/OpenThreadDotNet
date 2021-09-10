#if (NANOFRAMEWORK_1_0)
using nanoFramework.OpenThread.Spinel;

namespace nanoFramework.OpenThread.NCP
{ 

#else

using dotNETCore.OpenThread.Spinel;

namespace dotNETCore.OpenThread.NCP
{
#endif
    /// <summary>
    /// 
    /// </summary>
    public delegate void SerialDataReceivedEventHandler();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="frameData"></param>
    public delegate void FrameReceivedEventHandler(FrameData frameData);
}
