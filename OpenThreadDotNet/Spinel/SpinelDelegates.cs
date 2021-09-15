#if (NANOFRAMEWORK_1_0)
namespace nanoFramework.OpenThread.Spinel
{
#else
namespace dotNETCore.OpenThread.Spinel 
{ 
#endif   
    public delegate void SpinelPropertyChangedHandler(uint PropertyId, object PropertyValue);
}