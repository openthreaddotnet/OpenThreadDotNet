using nanoFramework.OpenThread.Spinel;

namespace nanoFramework.OpenThread.NCP
{
    /// <summary>
    /// 
    /// </summary>
    public delegate void SerialDataReceivedEventHandler();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="frameData"></param>
    internal delegate void FrameReceivedEventHandler(FrameData frameData);
}
