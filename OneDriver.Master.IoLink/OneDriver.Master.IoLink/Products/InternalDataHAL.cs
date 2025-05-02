using OneDriver.Framework.Libs.Announcer;

namespace OneDriver.Master.IoLink.Products
{
    public class InternalDataHAL : BaseDataForAnnouncement
    {
        public InternalDataHAL(int channelNumber, ushort index, ushort subindex
            , byte[] data)
        {
            TimeStamp = DateTime.Now;
            ChannelNumber = channelNumber;
            Index = index;
            Subindex = subindex;
            Data = data;
        }

        public InternalDataHAL()
        {
            TimeStamp = DateTime.Now;
            Data = null;
            ChannelNumber = 0;
        }

        public int ChannelNumber { get; } = 0;
        public ushort Index { get; }
        public ushort Subindex { get; }
        public byte[]? Data { get; }
    }
}
