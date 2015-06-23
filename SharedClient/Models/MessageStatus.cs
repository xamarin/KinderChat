namespace KinderChat
{
    public enum MessageStatus
    {
        //for outgoing:
        Unsent = 0,
        Sent,
        //for outgoing and incoming:
        Delivered, 
        Seen
    }
}
