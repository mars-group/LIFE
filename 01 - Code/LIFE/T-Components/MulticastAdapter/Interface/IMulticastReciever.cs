namespace MulticastAdapter.Interface {
    public interface IMulticastReciever {
        byte[] readMulticastGroupMessage();
        void CloseSocket();
    }
}