namespace KinderChat
{
    public class Photo
    {
        public string OriginalCloudId { get; set; }

        public byte[] Thumbnail { get; set; }

        public Photo(string originalCloudId, byte[] thumbnail)
        {
            OriginalCloudId = originalCloudId;
            Thumbnail = thumbnail;
        }
    }
}
