﻿using OpenThreadDotNet.Spinel;

namespace OpenThreadDotNet
{
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
